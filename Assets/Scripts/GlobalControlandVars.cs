using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalControlandVars : MonoBehaviour
{
    public GameObject clickDotPrefab;
    public bool isAddingHalls;
    public bool isAddingStairs;
    public GameObject selectedObject2D;
    public Vector3 pointingAtVector;
    public Vector3 pointOrigin;
    public Material selectedMaterial;
    public Material baseLineMaterial;
    public GameObject deleteButton;
    // int - index
    // GameObject - point may make a vector
    // bool - continueus
    public List<hallJoint> hallJoints = new List<hallJoint>();
    public List<GameObject> hallPoints = new List<GameObject>();

    public List<GameObject> rooms = new List<GameObject>();

    public TMPro.TextMeshProUGUI buttonText;


    private Vector3 mousePosition;
    
    void OnDrawGizmos()
    {   
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(mousePosition, 100);
       
    }

    void Update()
    {
        hallJoints.Clear();
        for(int x = 0; x < hallPoints.Count; x++)
        {
            if(x >= hallJoints.Count)
                hallJoints.Add(new hallJoint(hallPoints[x],true));
            else
                hallJoints[x] = new hallJoint(hallPoints[x],true);
        }
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
        
        
        pointOrigin = ray.origin;
        if (Physics.Raycast (ray, out hit, Mathf.Infinity)) 
        {
            Debug.DrawRay(ray.origin, ray.direction * distance(pointOrigin,hit.transform.position), Color.white);
            pointingAtVector = ray.origin + ray.direction * distance(pointOrigin,hit.transform.position);
            if (Input.GetMouseButtonDown(0))
            {
                selectedObject2D = hit.transform.gameObject;
            }
        }
        if(selectedObject2D != null && selectedObject2D.GetComponent<Room>() != null)
        {
            deleteButton.transform.parent.gameObject.SetActive(true);
            buttonText = deleteButton.GetComponent<TextMeshProUGUI>();
            buttonText.text = "Delete Room " + selectedObject2D.GetComponent<Room>().roomNumber;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            deleteButton.transform.parent.gameObject.SetActive(false);
        }
        if (Input.GetMouseButtonUp(0) && selectedObject2D)
        {
            selectedObject2D = null;
        }
    }
    public void DeleteRoom()
    {
        foreach(GameObject room in rooms)
        {
            if(room.GetComponent<MovementRoom>().isMovingRoom)
            {
                Destroy(room);
                rooms.Remove(room);
                deleteButton.transform.parent.gameObject.SetActive(false);
                break;
            }
        }
    }

    float distance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v2.x - v1.x,2) + Mathf.Pow(v2.y - v1.y,2) + Mathf.Pow(v2.z - v1.z,2));
    }
}
public class hallJoint
    {
        public GameObject jointObject;
        public bool continuous;

        public hallJoint(GameObject g,bool b)
        {
            jointObject = g;
            continuous = b;
        }
    }
