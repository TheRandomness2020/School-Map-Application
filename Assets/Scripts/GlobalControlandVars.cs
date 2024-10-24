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
    public TMP_Dropdown StudentDropdown;
    public TMP_Dropdown StudentDropdownFloors;
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
    public Material StairMaterial;
    public GameObject deleteButton;
    public TMP_Dropdown roomDropDown;
    public GameObject PersonObject;
    public int numFloors = 1;
    public List<GameObject> FloorsButtons;
    public List<GameObject> Stairs;
    public List<GameObject> Maps;
    public GameObject NewFloorButton;
    public GameObject DoneButton;
    public GameObject StairCustomPanel;
    public TMP_Dropdown ManagerDropdownFloors;
    public TMP_Dropdown FloorGotoDropDown;
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
        foreach(GameObject room in rooms)
        {
            if(GlobalFloorView == room.GetComponent<Room>().floor)
            {
                room.SetActive(true);
            }
            else
            {
                room.SetActive(false);
            }
        }
        foreach(GameObject map in Maps)
        {
            if(GlobalFloorView == map.GetComponent<MapPlane>().floor)
            {
                map.SetActive(true);
            }
            else
            {
                map.SetActive(false);
            }
        }
        
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
        for(int i = 0; i < numFloors; i++)
        {
            if(StudentDropdownFloors.options.Count < numFloors)
                StudentDropdownFloors.options.Add(new TMP_Dropdown.OptionData("Floor : " + i, null));
            else if(StudentDropdownFloors.options.Count > numFloors)
                StudentDropdownFloors.options.RemoveAt(StudentDropdownFloors.options.Count - 1);
            else
                StudentDropdownFloors.options[i].text = "Floor : " + i;
        }
        for(int i = 0; i < numFloors; i++)
        {
            if(ManagerDropdownFloors.options.Count < numFloors)
                ManagerDropdownFloors.options.Add(new TMP_Dropdown.OptionData("Floor : " + i, null));
            else if(ManagerDropdownFloors.options.Count > numFloors)
                ManagerDropdownFloors.options.RemoveAt(ManagerDropdownFloors.options.Count - 1);
            else
                ManagerDropdownFloors.options[i].text = "Floor : " + i;
        }
        if(StudentMode)
            isAddingHalls = false;
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
        foreach(GameObject Stair in Stairs)
        {
            if(Stair.GetComponent<MovementRoom>().isMovingRoom)
            {
                Destroy(Stair);
                Stairs.Remove(Stair);
                roomDropDown.options.RemoveAt(Stairs.IndexOf(Stair));
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
    public void wallEdit()
    {
        isAddingHalls = !isAddingHalls;
    }
    public void SetFloorStudent()
    {
        GlobalFloorView = StudentDropdownFloors.value;
    }
    public void SetFloorManager()
    {
        GlobalFloorView = ManagerDropdownFloors.value;
    }
    public void addStairs()
    {
        GameObject roomPoint = Instantiate(GetComponent<MapProcessor>().roomPointPrefab, Vector3.zero , Quaternion.identity);
        roomPoint.AddComponent<MovementRoom>();
        Room roomComponent = roomPoint.AddComponent<Room>();
        roomComponent.material = StairMaterial;
        roomPoint.transform.parent = transform;
        roomComponent.roomNumber = -1;
        roomComponent.width = 5;
        roomComponent.depth = 5;
        List<Vector2> corners = new List<Vector2>();
        corners.Add(new Vector2(20,20));
        corners.Add(new Vector2(20,-20));
        corners.Add(new Vector2(-20,-20));
        corners.Add(new Vector2(-20,20));
        roomComponent.createRoom(corners);
        roomPoint.name = "Stair" + GlobalFloorView + " to ";
        roomPoint.GetComponent<Room>().Stair = true;
        roomComponent.floor = GlobalFloorView;
        Stairs.Add(roomPoint);
        StairCustomPanel.SetActive(true);
        if(GlobalFloorView > 0)
        {
            if(numFloors != GlobalFloorView + 1)
            {
                FloorGotoDropDown.options.Clear();
                FloorGotoDropDown.options.Add(new TMP_Dropdown.OptionData("N/A", null));
                FloorGotoDropDown.options.Add(new TMP_Dropdown.OptionData("" + (GlobalFloorView-1), null));
                FloorGotoDropDown.options.Add(new TMP_Dropdown.OptionData("" + (GlobalFloorView+1), null));
            }
        }
        else
        {
            FloorGotoDropDown.options.Clear();
            FloorGotoDropDown.options.Add(new TMP_Dropdown.OptionData("N/A", null));
            FloorGotoDropDown.options.Add(new TMP_Dropdown.OptionData("" + (GlobalFloorView+1), null));
        }
    }
    public void StairsDoneButton()
    {
        if(FloorGotoDropDown.value != 0)
        {
            Stairs[Stairs.Count - 1].GetComponent<Room>().StairConnectedTo = FloorGotoDropDown.value;
            StairCustomPanel.SetActive(false);
        }
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
