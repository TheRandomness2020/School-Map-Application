using UnityEngine;

public class ClickAndDragCamera : MonoBehaviour
{
    public float dragSpeed = 2.0f; 
    private Vector3 dragOrigin; 
    private bool isDragging = false;
    public GlobalControlandVars gCV;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }

        if (isDragging && (gCV.selectedObject2D == null || gCV.selectedObject2D.GetComponent<Room>() != null))
        {
            Vector3 difference = Input.mousePosition - dragOrigin; 
            Vector3 movement = new Vector3(-difference.x, -difference.y, 0) * dragSpeed * Time.deltaTime;

            transform.Translate(movement, Space.World); 
            
            dragOrigin = Input.mousePosition;  
        }
    }
}