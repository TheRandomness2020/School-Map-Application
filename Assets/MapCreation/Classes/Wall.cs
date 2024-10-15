using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public Transform startPoint;  // The starting point of the line
    public Transform endPoint;    // The ending point of the line
    public float width = 0.1f;    // The width of the line
    public float depth = 0.1f;    // The depth of the line
    public Material lineMaterial; // The material to apply to the line

    public GameObject lineObject;
    private Material currentMaterial;

    void Start()
    {
        // Create the line object
        lineObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        lineObject.AddComponent<BoxCollider>();
        if(transform.parent.GetComponent<HallGenertationScript>() != true)
            gameObject.AddComponent<MovementWall>();
        lineObject.transform.parent = this.transform;
        if (lineMaterial != null)
        {
            lineObject.GetComponent<Renderer>().material = lineMaterial;
        }

        UpdateLine();
    }

    void Update()
    {
        // Continuously update the line position, rotation, and scale
        UpdateLine();
        if(transform.parent.GetComponent<HallGenertationScript>() != true)
            if (lineMaterial != transform.parent.GetComponent<Room>().material)
            {
                UpdateLineMaterial();
            }
    }

    void UpdateLine()
    {
        if (startPoint != null && endPoint != null && lineObject != null)
        {
            // Calculate the midpoint between the start and end points
            Vector3 midPoint = (startPoint.position + endPoint.position) / 2;

            // Set the position of the line object
            lineObject.transform.position = midPoint;

            // Calculate the direction from start to end
            Vector3 direction = ((endPoint.position ) - startPoint.position).normalized;

            // Set the rotation of the line object to face the end point
            lineObject.transform.rotation = Quaternion.LookRotation(direction);

            // Set the scale of the line object based on the distance between points and the specified width/depth
            float distance = Vector3.Distance(startPoint.position, endPoint.position);
            lineObject.transform.localScale = new Vector3(width, depth, distance + width);
            
        }
    }

    void UpdateLineMaterial()
    {
        if (lineMaterial != null && lineObject != null)
        {
            lineMaterial = transform.parent.GetComponent<Room>().material;
            lineObject.GetComponent<Renderer>().material = lineMaterial;
            currentMaterial = lineMaterial;  // Keep track of the current material
        }
    }
}
