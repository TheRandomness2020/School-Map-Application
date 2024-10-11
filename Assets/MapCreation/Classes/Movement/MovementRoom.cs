using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementRoom : MonoBehaviour
{
    private Vector3 mousePosition;
    public GameObject selectedObject;
    public GameObject dragOrb;
    private bool draging;
    private Vector3 offset;
    public bool isMovingRoom;
    public GlobalControlandVars gCV;
    public GameObject clicked;
    // Start is called before the first frame update
    void Start()
    {
        gCV = transform.parent.gameObject.GetComponent<GlobalControlandVars>();
        //createDot();
    }

    // Update is called once per frame
    void Update()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if(selectedObject != null)
            offset = selectedObject.transform.position - mousePosition;
        checkForRoomClick();
        //checkForDrag();
        if(isMovingRoom)
        {
            GetComponent<Room>().material = gCV.selectedMaterial;
            createDot();
            checkForDrag();
        }
        else
        {
            GetComponent<Room>().material = gCV.baseLineMaterial;
            removeDot();
        }
    }
    void createDot()
    {
        if(dragOrb == null)
        {
            dragOrb = GameObject.Instantiate(gCV.clickDotPrefab);
            dragOrb.transform.position =(GetComponent<Room>().corners[0].transform.position
                                        + GetComponent<Room>().corners[1].transform.position
                                        + GetComponent<Room>().corners[2].transform.position
                                        + GetComponent<Room>().corners[3].transform.position)/4 
                                        + new Vector3(0,0,0);
            dragOrb.transform.parent = transform;
        }
    }
    void removeDot()
    {
        if(dragOrb != null)
            Destroy(dragOrb);
    }
    void checkForRoomClick()
    {
        if(gCV.selectedObject2D == gameObject)
        {
            foreach(GameObject room in gCV.rooms)
            {
                room.GetComponent<MovementRoom>().isMovingRoom = false;
            }
            isMovingRoom = true;
        }
    }
    void checkForDrag()
    {
        // When clicked and dragged move dot
        if (gCV.selectedObject2D == dragOrb && Input.GetMouseButtonDown(0))
        {
            draging = true;
        }
        else if(Input.GetMouseButtonUp(0))
        {
            draging = false;
        }
        if(draging)
        {
            transform.position = new Vector3(gCV.pointingAtVector.x,gCV.pointingAtVector.y ,transform.position.z) - dragOrb.transform.localPosition;
        }
    }
}
