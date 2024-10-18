using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GlobalControlandVars : MonoBehaviour
{
    public Camera MainCamera;
    [Header("Student Values")]
    public bool StudentMode;
    public GPSValue current_Location;
    [Header("Manager Values")]
    public GPSTestOne GPSScript;
    public GameObject gpsButton;
    public GameObject clickDotPrefab;
    public bool isAddingHalls;
    public bool isAddingStairs;
    public GameObject selectedObject2D;
    public Vector3 pointingAtVector;
    public Vector3 pointOrigin;
    public Material selectedMaterial;
    public Material baseLineMaterial;
    public GameObject deleteButton;
    public TMP_Dropdown roomDropDown;
    public GameObject PersonObject;
    public List<GameObject> FloorsButtons;
    public GameObject NewFloorButton;
    public GameObject DoneButton;
    public int floorNumberRooms = 0;

    public List<int> roomNumberIndex;

    public int GlobalFloorView;

    private int tempGFV;

    public GameObject roomCustomPanel;
    public TMP_InputField roomCustomTextInput;

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
        for(int i = 0; i < FloorsButtons.Count; i++)
        {
            if(!StudentMode)
            {
                FloorsButtons[i].SetActive(false);
            }
            else
            {
                FloorsButtons[i].SetActive(true);
            }
        }
        if(StudentMode)
        {
            NewFloorButton.SetActive(false);
            DoneButton.SetActive(false);
            deleteButton.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            NewFloorButton.SetActive(true);
            DoneButton.SetActive(true);
        }
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
            if(!StudentMode)
                deleteButton.transform.parent.gameObject.SetActive(true);
            else
                deleteButton.transform.parent.gameObject.SetActive(false);
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
        floorNumberRooms = 0;
        if (GlobalFloorView != tempGFV) {roomDropDown.options.Clear(); roomNumberIndex.Clear();}
        tempGFV = GlobalFloorView;
        for(int i = 0; i < rooms.Count; i++)
        {
            if(rooms[i].GetComponent<Room>().floor == GlobalFloorView) {floorNumberRooms++;}
            if (roomNumberIndex.Count < floorNumberRooms) 
                roomNumberIndex.Add(i);
        }
        for(int i = 0; i < floorNumberRooms; i++)
        {
            if(roomDropDown.options.Count < floorNumberRooms)
                roomDropDown.options.Add(new TMP_Dropdown.OptionData(rooms[roomNumberIndex[i]].name, null));
            else
                roomDropDown.options[i].text = rooms[i].name;
        }
        //GPSScript.gpsOut.text = "Location: " + current_Location.latitude + " " + current_Location.longitude + " " + current_Location.altitude;
        gpsButton.SetActive(!StudentMode);
    }
    public void DeleteRoom()
    {
        foreach(GameObject room in rooms)
        {
            if(room.GetComponent<MovementRoom>().isMovingRoom)
            {
                Destroy(room);
                rooms.Remove(room);
                roomDropDown.options.RemoveAt(rooms.IndexOf(room));
                deleteButton.transform.parent.gameObject.SetActive(false);
                break;
            }
        }
    }
    float distance(Vector3 v1, Vector3 v2)
    {
        return Mathf.Sqrt(Mathf.Pow(v2.x - v1.x,2) + Mathf.Pow(v2.y - v1.y,2) + Mathf.Pow(v2.z - v1.z,2));
    }
    public void SetGPSPivot()
    {
        CameraGotoPerson();
        //current_Location.latitude = GPSScript.latitude;
        //current_Location.longitude = GPSScript.longitude;
        //current_Location.altitude = GPSScript.altitude;
    }
    public void CameraGotoPerson()
    {
        roomCustomPanel.SetActive(false);
        MainCamera.transform.position = new Vector3(PersonObject.transform.position.x,PersonObject.transform.position.y,MainCamera.transform.position.z);
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
public class GPSValue
    {
        public double latitude;
        public double longitude;
        public double altitude;
    }
