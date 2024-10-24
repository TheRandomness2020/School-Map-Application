using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HallGenertationScript : MonoBehaviour
{
    List<GameObject> corners = new List<GameObject>();
     List<GameObject> connectedWalls = new List<GameObject>();
     List<Vector2> inputCorner = new List<Vector2>();
    public List<bool> isCorrnerContinueus = new List<bool>();
     List<bool> isCorrnerContinueusTemp = new List<bool>();
    GlobalControlandVars gVC;
    public Material material;
    public float width = 10;
    public float depth = 10;
    public bool changeWalls = false;
    // Start is called before the first frame update
    void Start()
    {
        gVC=GetComponent<GlobalControlandVars>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gVC.hallJoints.Count > 0)
        {
            inputCorner.Clear();
        
            foreach(hallJoint joint in gVC.hallJoints)
            {
                inputCorner.Add(new Vector2(joint.jointObject.transform.position.x,joint.jointObject.transform.position.y));
                
            }
            while(gVC.hallJoints.Count > isCorrnerContinueus.Count)
            {
                if(gVC.hallJoints.Count % 2 == 0)
                    isCorrnerContinueus.Add(false);
                else
                    isCorrnerContinueus.Add(true);
            }
            while(gVC.hallJoints.Count < isCorrnerContinueus.Count)
            {
                isCorrnerContinueus.RemoveAt(isCorrnerContinueus.Count-1);
            }
            createRoom(inputCorner);
        }
        
    }
    public void addPointToHall()
    {
        
    }

    public void createRoom(List<Vector2> inputCorners)
    {
        for(int i = 0; i < inputCorners.Count; i++)
            {
                if(corners.Count < inputCorners.Count)
                {
                    changeWalls = true;
                    corners.Add(gVC.hallJoints[i].jointObject);
                }
                else if(corners[i] != gVC.hallJoints[i].jointObject)
                {
                    changeWalls = true;
                    corners[i] = gVC.hallJoints[i].jointObject;
                }
                
            }
        for(int i = 0; i < isCorrnerContinueus.Count; i++)
        {
            if(isCorrnerContinueusTemp.Count !=  isCorrnerContinueus.Count)
            {
                changeWalls = true;
                while(isCorrnerContinueus.Count > isCorrnerContinueusTemp.Count)
                {
                    isCorrnerContinueusTemp.Add(isCorrnerContinueus[isCorrnerContinueusTemp.Count]);
                }
                while(isCorrnerContinueus.Count < isCorrnerContinueusTemp.Count)
                {
                    isCorrnerContinueusTemp.RemoveAt(isCorrnerContinueusTemp.Count-1);
                }
                for(int y = 0; y < isCorrnerContinueus.Count; y++)
                {
                    isCorrnerContinueusTemp[y] = isCorrnerContinueus[y];
                }
                break;
            }
            else if(isCorrnerContinueusTemp[i] != isCorrnerContinueus[i])
            {
                changeWalls = true;
                while(isCorrnerContinueus.Count > isCorrnerContinueusTemp.Count)
                {
                    isCorrnerContinueusTemp.Add(isCorrnerContinueus[isCorrnerContinueusTemp.Count]);
                }
                while(isCorrnerContinueus.Count < isCorrnerContinueusTemp.Count)
                {
                    isCorrnerContinueusTemp.RemoveAt(isCorrnerContinueusTemp.Count-1);
                }
                for(int y = 0; y < isCorrnerContinueus.Count; y++)
                {
                    isCorrnerContinueusTemp[y] = isCorrnerContinueus[y];
                }
                break;
            }
        }
        if(connectedWalls.Count != inputCorners.Count || changeWalls) 
        {
            foreach(GameObject wall in connectedWalls)
            {
                Destroy(wall);
                connectedWalls.Remove(wall);
            }
            for(int x = 0; x < inputCorners.Count; x++)
            {
                GameObject wall = new GameObject();
                wall.AddComponent<Wall>();
                wall.GetComponent<Wall>().width = width;
                wall.GetComponent<Wall>().depth = depth;
                wall.GetComponent<Wall>().lineMaterial = material;
                wall.layer = 6;
                wall.transform.parent = this.transform;
                wall.name = "Wall " + x;
                if(x + 1 == inputCorners.Count)
                {
                    /*
                    wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[0].transform.position.x)  / 2,(corners[x].transform.position.y + corners[0].transform.position.y)  / 2,(corners[x].transform.position.z + corners[0].transform.position.z)  / 2);
                    
                    wall.GetComponent<Wall>().startPoint = corners[x].transform;
                    wall.GetComponent<Wall>().endPoint = corners[0].transform;
                    */
                }
                else if (isCorrnerContinueus[x] == true)
                {
                    wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[x + 1].transform.position.x)  / 2,(corners[x].transform.position.y + corners[x + 1].transform.position.y)  / 2,(corners[x].transform.position.z + corners[x + 1].transform.position.z)  / 2);
                    
                    wall.GetComponent<Wall>().startPoint = corners[x].transform;
                    wall.GetComponent<Wall>().endPoint = corners[x+1].transform;
                }
                else
                {
                    wall.transform.localPosition = new Vector3((corners[x].transform.position.x + corners[x + 1].transform.position.x)  / 2,(corners[x].transform.position.y + corners[x + 1].transform.position.y)  / 2,(corners[x].transform.position.z + corners[x + 1].transform.position.z)  / 2);
                    
                    wall.GetComponent<Wall>().startPoint = corners[x].transform;
                    wall.GetComponent<Wall>().endPoint = corners[x].transform;
                }
                connectedWalls.Add(wall);
            }
        }
        changeWalls = false;
    }
}
