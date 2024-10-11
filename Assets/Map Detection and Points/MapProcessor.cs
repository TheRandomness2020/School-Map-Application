using UnityEngine;
using System.Collections.Generic;

public class MapProcessor : MonoBehaviour
{
    public Texture2D mapTexture; 
    public GameObject roomPointPrefab; 
    public GameObject junctionPointPrefab;
    [Range(0f, 1f)] public float edgeThreshold = 0.3f; 
    [Range(0f, 1f)] public float junctionThreshold = 0.5f;
    public float minRoomSize = 10f; 
    public Vector3 rotationOffset = Vector3.zero;
    public float mergeThreshold = 5f;
    public float edgeFlushThreshold = 5f;
    public Material frontPlane;
    public Material wallMaterial;
    public GlobalControlandVars gCV;

    public List<RoomInfo> roomInfos = new List<RoomInfo>();

    void Start()
    {
        gCV = gameObject.GetComponent<GlobalControlandVars>();
        if (mapTexture == null)
        {
            Debug.LogError("MapTexture is not assigned.");
            return;
        }

        if (!mapTexture.isReadable)
        {
            Debug.LogError("MapTexture is not readable. Please make sure it is marked as readable in the import settings.");
            return;
        }

        ProcessMap(mapTexture);
        //this.transform.rotation = Quaternion.Euler(this.transform.rotation.x + 90, 0, 0);
    }

    void ProcessMap(Texture2D texture)
    {
        CreateMapGameObject(texture);
        Texture2D grayTexture = ConvertToGrayscale(texture);
        Texture2D edgeTexture = DetectEdges(grayTexture);
        List<List<Vector2>> rooms = DetectRooms(edgeTexture);
        List<Vector2> junctions = DetectJunctions(edgeTexture);

        roomInfos = GetRoomInfos(rooms);

        MergeCloseCorners(roomInfos);
        MergeCornersWithEdges(roomInfos);

        PlacePointsInRooms(roomInfos);
        MovePointsToGameObjectPosition();
    }
    void CreateMapGameObject(Texture2D texture)
    {
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(texture.width/10,1,texture.height/10);
        plane.transform.parent = transform;
        plane.transform.position = new Vector3(texture.width/2,texture.height/2,5);
        plane.transform.rotation = Quaternion.Euler(90, -90, 90);
        plane.layer = 2;

        frontPlane.mainTexture = texture;

        // Apply to Plane
        MeshRenderer mr = plane.GetComponent<MeshRenderer> ();
        mr.material = frontPlane;
    }

    Texture2D ConvertToGrayscale(Texture2D texture)
    {
        Texture2D grayTexture = new Texture2D(texture.width, texture.height);
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            Color color = pixels[i];
            float gray = color.grayscale;
            grayTexture.SetPixel(i % texture.width, i / texture.width, new Color(gray, gray, gray));
        }

        grayTexture.Apply();
        return grayTexture;
    }

    Texture2D DetectEdges(Texture2D texture)
    {
        Texture2D edgeTexture = new Texture2D(texture.width, texture.height);
        Color[] pixels = texture.GetPixels();

        int width = texture.width;
        int height = texture.height;

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                float gx = 0f, gy = 0f;

                float[,] sobelX = {
                    { -1, 0, 1 },
                    { -2, 0, 2 },
                    { -1, 0, 1 }
                };

                float[,] sobelY = {
                    { -1, -2, -1 },
                    { 0, 0, 0 },
                    { 1, 2, 1 }
                };

                for (int ky = -1; ky <= 1; ky++)
                {
                    for (int kx = -1; kx <= 1; kx++)
                    {
                        Color pixel = texture.GetPixel(x + kx, y + ky);
                        float gray = pixel.grayscale;

                        gx += gray * sobelX[kx + 1, ky + 1];
                        gy += gray * sobelY[kx + 1, ky + 1];
                    }
                }

                float magnitude = Mathf.Sqrt(gx * gx + gy * gy);
                Color edgeColor = magnitude > edgeThreshold ? Color.black : Color.white;
                edgeTexture.SetPixel(x, y, edgeColor);
            }
        }

        edgeTexture.Apply();
        return edgeTexture;
    }

    List<List<Vector2>> DetectRooms(Texture2D edgeTexture)
    {
        List<List<Vector2>> rooms = new List<List<Vector2>>();
        bool[,] visited = new bool[edgeTexture.width, edgeTexture.height];
        Color[] pixels = edgeTexture.GetPixels();

        int width = edgeTexture.width;
        int height = edgeTexture.height;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (!visited[x, y] && pixels[y * width + x] == Color.white)
                {
                    List<Vector2> room = FloodFill(edgeTexture, x, y, visited);
                    rooms.Add(room);
                }
            }
        }

        return rooms;
    }

    List<Vector2> FloodFill(Texture2D texture, int startX, int startY, bool[,] visited)
    {
        List<Vector2> roomPixels = new List<Vector2>();
        Queue<Vector2> pixels = new Queue<Vector2>();
        pixels.Enqueue(new Vector2(startX, startY));

        int width = texture.width;
        int height = texture.height;

        while (pixels.Count > 0)
        {
            Vector2 pixel = pixels.Dequeue();
            int x = (int)pixel.x;
            int y = (int)pixel.y;

            if (x < 0 || x >= width || y < 0 || y >= height || visited[x, y])
                continue;

            if (texture.GetPixel(x, y) != Color.white)
                continue;

            visited[x, y] = true;
            roomPixels.Add(pixel);

            pixels.Enqueue(new Vector2(x - 1, y));
            pixels.Enqueue(new Vector2(x + 1, y));
            pixels.Enqueue(new Vector2(x, y - 1));
            pixels.Enqueue(new Vector2(x, y + 1));
        }

        return roomPixels;
    }

    List<Vector2> DetectJunctions(Texture2D edgeTexture)
    {
        List<Vector2> junctions = new List<Vector2>();
        Color[] pixels = edgeTexture.GetPixels();

        int width = edgeTexture.width;
        int height = edgeTexture.height;

        for (int y = 1; y < height - 1; y++)
        {
            for (int x = 1; x < width - 1; x++)
            {
                Color pixel = edgeTexture.GetPixel(x, y);
                if (pixel == Color.black)
                {
                    if (IsJunction(edgeTexture, x, y))
                    {
                        junctions.Add(new Vector2(x, y));
                    }
                }
            }
        }

        return junctions;
    }

    bool IsJunction(Texture2D texture, int x, int y)
    {
        int width = texture.width;
        int height = texture.height;

        int blackCount = 0;

        for (int dy = -1; dy <= 1; dy++)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                if (dx == 0 && dy == 0) continue;

                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                {
                    if (texture.GetPixel(nx, ny) == Color.black)
                    {
                        blackCount++;
                    }
                }
            }
        }

        return blackCount >= 3;
    }

    List<RoomInfo> GetRoomInfos(List<List<Vector2>> rooms)
    {
        List<RoomInfo> roomInfos = new List<RoomInfo>();
        Vector2 totalCenter = Vector2.zero;
        int roomCount = 0;

        foreach (var room in rooms)
        {
            if (room.Count == 0) continue;

            Vector2 center = CalculateRoomCenter(room);
            List<Vector2> corners = GetRoomCorners(room);

            float roomSize = CalculateRoomArea(corners);

            if (roomSize >= minRoomSize)
            {
                RoomInfo roomInfo = new RoomInfo
                {
                    roomPoint = center,
                    corners = corners
                };

                roomInfos.Add(roomInfo);
                totalCenter += center;
                roomCount++;
            }
        }

        if (roomCount > 0)
        {
            Vector2 averageCenter = totalCenter / roomCount;
            Vector3 offset = transform.position - new Vector3(averageCenter.x, averageCenter.y, transform.position.z);

            MoveOffset = offset;
        }

        return roomInfos;
    }

    Vector2 CalculateRoomCenter(List<Vector2> roomPixels)
    {
        Vector2 sum = Vector2.zero;

        foreach (var pixel in roomPixels)
        {
            sum += pixel;
        }

        return sum / roomPixels.Count;
    }

    List<Vector2> GetRoomCorners(List<Vector2> roomPixels)
    {
        // For simplicity, we'll consider the outermost pixels as corners for now
        List<Vector2> corners = new List<Vector2>();
        float minX = float.MaxValue, minY = float.MaxValue;
        float maxX = float.MinValue, maxY = float.MinValue;

        foreach (var pixel in roomPixels)
        {
            if (pixel.x < minX) minX = pixel.x;
            if (pixel.x > maxX) maxX = pixel.x;
            if (pixel.y < minY) minY = pixel.y;
            if (pixel.y > maxY) maxY = pixel.y;
        }

        corners.Add(new Vector2(minX, minY));
        corners.Add(new Vector2(maxX, minY));
        corners.Add(new Vector2(maxX, maxY));
        corners.Add(new Vector2(minX, maxY));

        return corners;
    }

    float CalculateRoomArea(List<Vector2> corners)
    {
        if (corners.Count < 3) return 0;
        
        float area = 0;
        for (int i = 0; i < corners.Count; i++)
        {
            Vector2 current = corners[i];
            Vector2 next = corners[(i + 1) % corners.Count];
            area += current.x * next.y - next.x * current.y;
        }
        return Mathf.Abs(area) / 2f;
    }

    void PlacePointsInRooms(List<RoomInfo> roomInfos)
    {
        foreach (var roomInfo in roomInfos)
        {
            Vector3 centerPosition = new Vector3(roomInfo.roomPoint.x, roomInfo.roomPoint.y, 0);
            GameObject roomPoint = Instantiate(roomPointPrefab, centerPosition, Quaternion.identity);
            roomPoint.layer = 6;
            gCV.rooms.Add(roomPoint);
            roomPoint.transform.parent = transform;

            roomPoint.AddComponent<MovementRoom>();
            Room roomComponent = roomPoint.AddComponent<Room>();
            roomComponent.material = wallMaterial;
            //roomComponent.InitializeRoom(roomInfo.roomPoint, roomInfo.corners);

            // Optionally visualize the room corners
            for (int i = 0; i < roomInfo.corners.Count; i++)
            {
                Vector3 cornerPosition = new Vector3(roomInfo.corners[i].x, roomInfo.corners[i].y, 0);
                Debug.DrawLine(cornerPosition, 
                    new Vector3(roomInfo.corners[(i + 1) % roomInfo.corners.Count].x, roomInfo.corners[(i + 1) % roomInfo.corners.Count].y, 0), 
                    Color.red, 100f);
            }
            roomComponent.width = 5;
            roomComponent.depth = 5;
            roomComponent.createRoom(roomInfo.corners);
        }
    }
    void MergeCloseCorners(List<RoomInfo> roomInfos)
    {
        float threshold = mergeThreshold * mergeThreshold; // Square the threshold for efficient distance comparison

        for (int i = 0; i < roomInfos.Count; i++)
        {
            for (int j = i + 1; j < roomInfos.Count; j++)
            {
                MergeCorners(roomInfos[i].corners, roomInfos[j].corners, threshold);
            }
        }
    }

    void MergeCorners(List<Vector2> cornersA, List<Vector2> cornersB, float threshold)
    {
        for (int i = 0; i < cornersA.Count; i++)
        {
            for (int j = 0; j < cornersB.Count; j++)
            {
                if ((cornersA[i] - cornersB[j]).sqrMagnitude < threshold)
                {
                    Vector3 mergedPoint = (cornersA[i] + cornersB[j]) / 2;
                    cornersA[i] = mergedPoint;
                    cornersB[j] = mergedPoint;
                }
            }
        }
    }

    void MergeCornersWithEdges(List<RoomInfo> roomInfos)
    {
        float flushThreshold = edgeFlushThreshold;

        foreach (var roomInfo in roomInfos)
        {
            for (int i = 0; i < roomInfo.corners.Count; i++)
            {
                Vector2 corner = roomInfo.corners[i];
                
                // Calculate room boundaries (edges)
                float minX = float.MaxValue, minY = float.MaxValue;
                float maxX = float.MinValue, maxY = float.MinValue;

                foreach (var otherCorner in roomInfo.corners)
                {
                    if (otherCorner.x < minX) minX = otherCorner.x;
                    if (otherCorner.x > maxX) maxX = otherCorner.x;
                    if (otherCorner.y < minY) minY = otherCorner.y;
                    if (otherCorner.y > maxY) maxY = otherCorner.y;
                }

                // Check if the corner is near an edge (flush with the edge)
                if (Mathf.Abs(corner.x - minX) < flushThreshold) corner.x = minX;
                if (Mathf.Abs(corner.x - maxX) < flushThreshold) corner.x = maxX;
                if (Mathf.Abs(corner.y - minY) < flushThreshold) corner.y = minY;
                if (Mathf.Abs(corner.y - maxY) < flushThreshold) corner.y = maxY;

                // Update the corner with the new (snapped) position
                roomInfo.corners[i] = corner;
            }
        }
    }

    void MovePointsToGameObjectPosition()
    {
        Vector3 offset = MoveOffset;

        foreach (Transform child in transform)
        {
            child.position += offset;
        }
    }

    Vector3 MoveOffset;

    public class RoomInfo
    {
        public Vector2 roomPoint;
        public List<Vector2> corners;
    }
}
