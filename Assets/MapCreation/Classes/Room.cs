using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public List<GameObject> connectedWalls = new List<GameObject>();
    public List<GameObject> corners = new List<GameObject>();
    public List<GameObject> connectedRooms = new List<GameObject>();
    public MeshCollider clickBox;
    public GameObject pointTester;
    public Material material;
    public int roomNumber;
    public int floor;
    public float width = 10;
    public float depth = 10;
    public int numCorners = 4;
    public void Start()
    {
        //createRoom();
    }
    public void Update()
    {
        if(connectedRooms!=null)
        {
            for(int x = 0; x < connectedRooms.Count; x++)
                connectRoom(connectedRooms[x]);
        }
        createClickBox();
    }
    public void createRoom(List<Vector2> inputCorners)
    {
        for(int i = 0; i < inputCorners.Count; i++)
            {
                GameObject corner = new GameObject();
                corner.transform.parent = this.transform;
                corner.transform.position = new Vector3(inputCorners[i].x,inputCorners[i].y,0);
                corner.name = "Corner " + i;
                corner.AddComponent<MovementCorrner>();
                corners.Add(corner);
            }

        for(int x = 0; x < numCorners; x++)
        {
            GameObject wall = new GameObject();
            wall.AddComponent<Wall>();
            wall.GetComponent<Wall>().width = width;
            wall.GetComponent<Wall>().depth = depth;
            wall.GetComponent<Wall>().lineMaterial = material;
            wall.transform.parent = this.transform;
            wall.name = "Wall " + x;
            if(x + 1 == numCorners)
            {
                wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[0].transform.position.x)  / 2,(corners[x].transform.position.y + corners[0].transform.position.y)  / 2,(corners[x].transform.position.z + corners[0].transform.position.z)  / 2);
                
                wall.GetComponent<Wall>().startPoint = corners[x].transform;
                wall.GetComponent<Wall>().endPoint = corners[0].transform;
            }
            else
            {
                wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[x + 1].transform.position.x)  / 2,(corners[x].transform.position.y + corners[x + 1].transform.position.y)  / 2,(corners[x].transform.position.z + corners[x + 1].transform.position.z)  / 2);
                
                wall.GetComponent<Wall>().startPoint = corners[x].transform;
                wall.GetComponent<Wall>().endPoint = corners[x+1].transform;
            }
            connectedWalls.Add(wall);
        }
    }
    public void createRoom(Vector3 bottomLeft = new Vector3(), Vector3 topRight = new Vector3())
    {
        GameObject corner = new GameObject();
            corner.transform.parent = this.transform;
            corner.transform.position = bottomLeft;
            corner.name = "Corner 1";
            corners.Add(corner);
            corner = new GameObject();
            corner.transform.parent = this.transform;
            corner.transform.position = new Vector3(topRight.x,bottomLeft.y,(bottomLeft.z+topRight.z)/2);
            corner.name = "Corner 2";
            corners.Add(corner);
            corner = new GameObject();
            corner.transform.parent = this.transform;
            corner.transform.position = topRight;
            corner.name = "Corner 3";
            corners.Add(corner);
            corner = new GameObject();
            corner.transform.parent = this.transform;
            corner.transform.position = new Vector3(bottomLeft.x,topRight.y,(bottomLeft.z+topRight.z)/2);
            corner.name = "Corner 4";
            corners.Add(corner);

            for(int x = 0; x < numCorners; x++)
        {
            GameObject wall = new GameObject();
            wall.AddComponent<Wall>();
            wall.GetComponent<Wall>().width = width;
            wall.GetComponent<Wall>().depth = depth;
            wall.GetComponent<Wall>().lineMaterial = material;
            wall.transform.parent = this.transform;
            wall.name = "Wall " + x;
            if(x + 1 == numCorners)
            {
                wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[0].transform.position.x)  / 2,(corners[x].transform.position.y + corners[0].transform.position.y)  / 2,(corners[x].transform.position.z + corners[0].transform.position.z)  / 2);
                
                wall.GetComponent<Wall>().startPoint = corners[x].transform;
                wall.GetComponent<Wall>().endPoint = corners[0].transform;
            }
            else
            {
                wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[x + 1].transform.position.x)  / 2,(corners[x].transform.position.y + corners[x + 1].transform.position.y)  / 2,(corners[x].transform.position.z + corners[x + 1].transform.position.z)  / 2);
                
                wall.GetComponent<Wall>().startPoint = corners[x].transform;
                wall.GetComponent<Wall>().endPoint = corners[x+1].transform;
            }
            connectedWalls.Add(wall);
        }
    }
    public void createRoom()
    {
        float pointAddition = 360 / numCorners;
        
            corners = new List<GameObject>(); 
            for(int x = 0; x < numCorners; x++)
            {
                GameObject corner = new GameObject();
                corner.transform.parent = this.transform;
                corner.transform.localPosition = new Vector3(GetPointOnCircle(5,x*pointAddition + pointAddition/2).x,0,GetPointOnCircle(5,x*pointAddition + pointAddition/2).y);
                corner.name = "Corner " + x;
                corners.Add(corner);
            }
        
        for(int x = 0; x < numCorners; x++)
        {
            GameObject wall = new GameObject();
            wall.AddComponent<Wall>();
            wall.GetComponent<Wall>().width = width;
            wall.GetComponent<Wall>().depth = depth;
            wall.GetComponent<Wall>().lineMaterial = material;
            wall.transform.parent = this.transform;
            wall.name = "Wall " + x;
            if(x + 1 == numCorners)
            {
                wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[0].transform.position.x)  / 2,(corners[x].transform.position.y + corners[0].transform.position.y)  / 2,(corners[x].transform.position.z + corners[0].transform.position.z)  / 2);
                
                wall.GetComponent<Wall>().startPoint = corners[x].transform;
                wall.GetComponent<Wall>().endPoint = corners[0].transform;
            }
            else
            {
                wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[x + 1].transform.position.x)  / 2,(corners[x].transform.position.y + corners[x + 1].transform.position.y)  / 2,(corners[x].transform.position.z + corners[x + 1].transform.position.z)  / 2);
                
                wall.GetComponent<Wall>().startPoint = corners[x].transform;
                wall.GetComponent<Wall>().endPoint = corners[x+1].transform;
            }
            connectedWalls.Add(wall);
        }
    }
    Vector2 GetPointOnCircle(float radius, float angleInDegrees)
    {
        // Convert the angle to radians
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;

        // Calculate the x and y coordinates
        float x = radius * Mathf.Cos(angleInRadians);
        float y = radius * Mathf.Sin(angleInRadians);

        return new Vector2(x, y);
    }
    public void createClickBox()
    {
        if(pointTester == null)
        {
            pointTester = new GameObject();
        }
        PolygonCollider2D clickBoxTmp = pointTester.AddComponent<PolygonCollider2D>();
        if (clickBox == null)
        {
            clickBox = gameObject.AddComponent<MeshCollider>();
        }
        Vector2[] vertArray = new Vector2[corners.Count];
        for(int x = 0; x < corners.Count; x++)
        {
            vertArray[x] = new Vector2(corners[x].transform.localPosition.x,corners[x].transform.localPosition.y);
        }
        clickBoxTmp.SetPath(0,vertArray);
        Mesh mesh = clickBoxTmp.CreateMesh(true,true);
        Destroy(clickBoxTmp);
        clickBox.sharedMesh = mesh;
    }
    public void connectRoom(GameObject room)
    {

    }
}
