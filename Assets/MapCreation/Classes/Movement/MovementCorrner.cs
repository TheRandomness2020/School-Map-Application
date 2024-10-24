using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCorrner : MonoBehaviour
{
    private Vector3 mousePosition;
    public GameObject selectedObject;
    private GameObject dragOrb;
    private bool draging;
    private Vector3 offset;
    public bool isMovingRoom;
    public GlobalControlandVars gCV;
    public GameObject clicked;
    public GameObject sphere;
    public int cornerNumber;
    public bool dragOrbWasClickedAlready = true;
    // Start is called before the first frame update
    void Start()
    {
        gCV = transform.parent.parent.gameObject.GetComponent<GlobalControlandVars>();
        sphere = transform.parent.gameObject;
        for(int x = 0; x < sphere.GetComponent<Room>().corners.Count; x++)
        {
            if(sphere.GetComponent<Room>().corners[x] == gameObject)
            {
                cornerNumber = x;
                break;
            }
        }
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
            createDot();
            checkForDrag();
        }
        else if(gCV.isAddingHalls)
        {
            createDot();
            if (gCV.selectedObject2D == dragOrb && Input.GetMouseButtonDown(0) && dragOrbWasClickedAlready)
            {
                dragOrbWasClickedAlready = false;
                gCV.hallPoints.Add(dragOrb.transform.parent.gameObject);
            }
            else if(Input.GetMouseButtonUp(0))
            {
                dragOrbWasClickedAlready = true;
            }
        }
        else
        {
            removeDot();
        }
    }
    void createDot()
    {
        if(dragOrb == null)
        {
            dragOrb = GameObject.Instantiate(gCV.clickDotPrefab);
            dragOrb.transform.position = gameObject.transform.position + new Vector3(0,0,-5);
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
        isMovingRoom = transform.parent.GetComponent<MovementRoom>().isMovingRoom;
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
            transform.position = new Vector3(gCV.pointingAtVector.x,gCV.pointingAtVector.y ,transform.position.z);
            Transform sphere1 = sphere.GetComponent<Room>().corners[(cornerNumber == 0) ? 3 : cornerNumber - 1].transform;
            Transform sphere2 = sphere.GetComponent<Room>().corners[(cornerNumber == 3) ? 0 : cornerNumber + 1].transform;
            if(Mathf.Abs(sphere1.position.y - transform.position.y)
            >  Mathf.Abs(sphere2.position.y - transform.position.y))
            {
                sphere1.position = new Vector3(transform.position.x,sphere1.position.y,sphere1.position.z);
                sphere2.position = new Vector3(sphere2.position.x,transform.position.y,sphere2.position.z);
            }
            else
            {
                sphere1.position = new Vector3(sphere1.position.x,transform.position.y,sphere1.position.z);
                sphere2.position = new Vector3(transform.position.x,sphere2.position.y,sphere2.position.z);
            }
            transform.parent.GetComponent<MovementRoom>().dragOrb.transform.position = (sphere.GetComponent<Room>().corners[0].transform.position
                                                                                      + sphere.GetComponent<Room>().corners[1].transform.position
                                                                                      + sphere.GetComponent<Room>().corners[2].transform.position
                                                                                      + sphere.GetComponent<Room>().corners[3].transform.position)/4;
        }
    }
}
