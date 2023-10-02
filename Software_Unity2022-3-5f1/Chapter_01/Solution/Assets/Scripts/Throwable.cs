using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    Vector3 throwVector;
    Rigidbody2D _rb;
    LineRenderer _lr;
    public float sensitivity;
    //boundary object
    public GameObject boundsObj;

    //boundaries
    private Bounds _bounds;

    void Awake()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _lr = this.GetComponent<LineRenderer>();
        _bounds = FindBounds(boundsObj);
    }

    /*void Update()
    {

        //update position
        StopAtMapEdge();
    }*/

    private Bounds FindBounds(GameObject theParent)
    {

        //store bounds
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        //retrieve renderers for all children
        Renderer[] allChildren = theParent.GetComponentsInChildren<Renderer>();

        //search all children
        foreach (Renderer aChild in allChildren)
        {

            //add child bounds to total
            bounds.Encapsulate(aChild.bounds);
        }

        //return
        return bounds;
    }
    //onmouse events possible thanks to monobehaviour + collider2d
    void OnMouseDown()
    {
        CalculateThrowVector();
        SetArrow();
    }
    void OnMouseDrag()
    {
        CalculateThrowVector();
        SetArrow();
    }
    void CalculateThrowVector()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //doing vector2 math to ignore the z values in our distance.
        Vector2 distance = mousePos - this.transform.position;
        //dont normalize the ditance if you want the throw strength to vary
        float sensitivity = 50f;
        throwVector = -distance * sensitivity;
    }
    void SetArrow()
    {
        _lr.positionCount = 2;
        _lr.SetPosition(0, transform.position);
        _lr.SetPosition(1, transform.position + throwVector / 2);
        _lr.enabled = true;
    }
    void OnMouseUp()
    {
        RemoveArrow();
        Throw();
    }
    void RemoveArrow()
    {
        _lr.enabled = false;
    }
    public void Throw()
    {
        _rb.AddForce(throwVector);
    }

    public void StopAtMapEdge()
    {
        // Calculate the new position
        Vector3 newPos = gameObject.transform.position;

        // Calculate half extents for your object
        float halfObjW = gameObject.GetComponent<Renderer>().bounds.extents.x;
        float halfObjH = gameObject.GetComponent<Renderer>().bounds.extents.y;
        Debug.Log("Half Extents: (" + halfObjW + ", " + halfObjH + ")");

        // Check and clamp the object's position to the map bounds
        // x-axis
        if (newPos.x - halfObjW < _bounds.min.x)
        {
            newPos.x = _bounds.min.x + halfObjW;
        }
        else if (newPos.x + halfObjW > _bounds.max.x)
        {
            newPos.x = _bounds.max.x - halfObjW;
        }

        // y-axis
        if (newPos.y - halfObjH < _bounds.min.y)
        {
            newPos.y = _bounds.min.y + halfObjH;
        }
        else if (newPos.y + halfObjH > _bounds.max.y)
        {
            newPos.y = _bounds.max.y - halfObjH;
        }

        // Update the object's position
        gameObject.transform.position = newPos;
    }

  
}