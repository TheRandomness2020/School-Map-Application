using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotControl : MonoBehaviour
{
    private Camera _mainCamera;
    private Vector3 _offset;
    private float _zCoord;

    private void Start()
    {
        _mainCamera = Camera.main;
    }

    private void OnMouseDown()
    {

        _zCoord = _mainCamera.WorldToScreenPoint(transform.position).z;

        _offset = transform.position - GetMouseWorldPosition();
    }

    private void OnMouseDrag()
    {
        transform.position = GetMouseWorldPosition() + _offset;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector3 mousePoint = Input.mousePosition;

        mousePoint.z = _zCoord;

        return _mainCamera.ScreenToWorldPoint(mousePoint);
    }
}
