using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementWall : MonoBehaviour
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
            dragOrb.transform.position = transform.GetComponent<Wall>().lineObject.transform.position + new Vector3(0,0,-5);
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
            var startOffset = new Vector3(GetComponent<Wall>().startPoint.position.x, GetComponent<Wall>().startPoint.position.y, GetComponent<Wall>().startPoint.position.z) - GetComponent<Wall>().lineObject.transform.position;
            var endOffset = new Vector3(GetComponent<Wall>().endPoint.position.x, GetComponent<Wall>().endPoint.position.y, GetComponent<Wall>().endPoint.position.z) - GetComponent<Wall>().lineObject.transform.position;

            GetComponent<Wall>().startPoint.position = new Vector3(gCV.pointingAtVector.x,gCV.pointingAtVector.y ,GetComponent<Wall>().startPoint.position.z) + startOffset;
            GetComponent<Wall>().endPoint.position = new Vector3(gCV.pointingAtVector.x,gCV.pointingAtVector.y ,GetComponent<Wall>().endPoint.position.z) + endOffset;
        }
        dragOrb.transform.position = transform.GetComponent<Wall>().lineObject.transform.position + new Vector3(0,0,-5);
    }
}
