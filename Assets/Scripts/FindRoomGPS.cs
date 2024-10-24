using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FindRoomGPS : MonoBehaviour
{
    [SerializeField]private TMP_Dropdown roomDropDown;
    public Transform target; 
    public float rayDistance = 5f;
    public float stepSize = 1f;
    public int maxDepth = 10;
    public GlobalControlandVars gCV;
    public LineRenderer lineRenderer;

    public List<Vector3> pathPoints = new List<Vector3>(); 

    void Update()
    {
        DrawPath();
    }

    public void FindPathToRoom()
    {
        if(!gCV.StudentMode)
        {
            ManagerRoomChange();
        }
        else
        {
            gCV.roomCustomPanel.SetActive(false);
            pathPoints.Clear();
            target = gCV.rooms[gCV.StudentDropdown.value].transform;
            if (FindPath(transform.position, target.position))
            {
                Debug.Log("Found");
            }
            pathPoints.RemoveAt(0);
            DrawPath();
        }
    }
    public void ManagerRoomChange()
    {
        gCV.roomCustomTextInput.text = gCV.rooms[gCV.roomNumberIndex[gCV.roomDropDown.value]].name;
        gCV.roomCustomPanel.SetActive(true);
    }
    public void ChangeRoomName()
    {
        gCV.rooms[gCV.roomNumberIndex[gCV.roomDropDown.value]].name = gCV.roomCustomTextInput.text;
    }

    private bool FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Queue<Vector3> queue = new Queue<Vector3>();
        Dictionary<Vector3, Vector3> cameFrom = new Dictionary<Vector3, Vector3>();
        queue.Enqueue(startPos);
        cameFrom[startPos] = startPos; 

        while (queue.Count > 0)
        {
            Vector3 currentPos = queue.Dequeue();

            if (Vector3.Distance(currentPos, targetPos) < stepSize)
            {
                ConstructPath(cameFrom, currentPos, targetPos);
                return true;
            }

            Vector3[] directions = {
                Vector3.left,
                Vector3.right,
                Vector3.up,
                Vector3.down
            };

            foreach (var direction in directions)
            {
                Vector3 newPos = currentPos + direction * stepSize;

                if (IsValidPosition(currentPos, newPos) && !cameFrom.ContainsKey(newPos))
                {
                    queue.Enqueue(newPos);
                    cameFrom[newPos] = currentPos;
                }
            }
        }

        Debug.LogWarning("Max recursion depth reached");
        return FindPath(startPos + new Vector3(0,-5,0), targetPos);
    }

    private void ConstructPath(Dictionary<Vector3, Vector3> cameFrom, Vector3 currentPos, Vector3 targetPos)
    {
        pathPoints.Clear(); 
        Vector3 tempPos = currentPos;

        while (tempPos != cameFrom[tempPos])
        {
            pathPoints.Add(tempPos);
            tempPos = cameFrom[tempPos];
        }
        pathPoints.Add(targetPos); 
        pathPoints.Reverse(); 
    }

    private bool IsValidPosition(Vector3 position, Vector3 targetPos)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, targetPos - position, out hit, rayDistance))
        {
            if (!hit.transform.IsChildOf(target))
                return false;
        }

        return true;
    }

    private void DrawPath()
    {
        if (pathPoints.Count != lineRenderer.positionCount)
        {
            lineRenderer.positionCount = pathPoints.Count;
        }
        for(int i = 0; i < pathPoints.Count; i++)
            lineRenderer.SetPosition(i,pathPoints[i] - lineRenderer.transform.parent.transform.position);
    }
}
