using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMovement : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _offset;
    private float _zCoord;

    private Vector3 startOffset;
    private Vector3 endOffset;

    private Transform sp ;
    private Transform ep ;

    // add later for control z and back arrow
    private Vector3 lastLocation ;

    private Wall wall;

    private void Start()
    {
        wall = GetComponentInParent<Wall>();
        sp = wall.startPoint;
        ep = wall.endPoint;
        _mainCamera = Camera.main;
        startOffset = new Vector3(sp.position.x, sp.position.y, sp.position.z) - transform.position;
        endOffset = new Vector3(ep.position.x, ep.position.y, ep.position.z) - transform.position;
    }

    private void OnMouseDown()
    {

        _zCoord = _mainCamera.WorldToScreenPoint(transform.position).z;

        _offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        sp.position = GetMouseWorldPosition() + _offset + startOffset;
        ep.position = GetMouseWorldPosition() + _offset + endOffset;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = _zCoord;

        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
