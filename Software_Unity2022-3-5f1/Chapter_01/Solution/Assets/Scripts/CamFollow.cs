/*
CamFollow
Manages the movement of the camera to follow a target.
Attach to camera object in scene.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class CamFollow : MonoBehaviour {

    //follow speed
    public float followSpeed;

    //minimum distance at which camera begins to follow
    public float minFollowDist;

    //target object to follow
    public GameObject targetObj;

    //previous position of target
    private Vector3 _targetPrevPos;

    //boundary object
    public GameObject boundsObj;

    //boundaries
    private Bounds _bounds;

    //init
    void Start() {

        //store target pos
        Vector3 targetPos = targetObj.transform.position;
        _targetPrevPos = targetPos;

        //set camera to start from target pos
        targetPos.z = gameObject.transform.position.z;
        gameObject.transform.position = targetPos;

        //retrieve bounds
        _bounds = FindBounds(boundsObj);
        //Debug.Log(_bounds);

    }

    //update
    void Update() {

        //update position
        UpdatePos();
    }

    //update position based on target movement
    private void UpdatePos() {

        //retrieve camera position
        Vector3 camPos = gameObject.transform.position;

        //retrieve target position
        Vector3 targetPos = targetObj.transform.position;

        //check distance to target
        float targetDist = Mathf.Sqrt(
            Mathf.Pow((targetPos.x - camPos.x), 2) +
            Mathf.Pow((targetPos.y - camPos.y), 2)
            );

        //if distance to target is greater than minimum
        //or target is idle
        if (targetDist > minFollowDist || targetPos == _targetPrevPos) {

            //calculate new position
            Vector3 newCamPos = camPos + (targetPos - camPos) * followSpeed * Time.deltaTime;
            newCamPos.z = camPos.z;

            //if new pos would cause camera to exceed bounds, lock to bounds
            //bounds size
            float halfBoundsW = _bounds.size.x / 2;
            float halfBoundsH = _bounds.size.y / 2;

            //camera size
            float halfCamW = Camera.main.orthographicSize * Screen.width / Screen.height;
            float halfCamH = Camera.main.orthographicSize;

            //x axis
            if (newCamPos.x < -halfBoundsW + halfCamW) {

                //lock to left bound
                newCamPos.x = -halfBoundsW + halfCamW;
            }
            else if (newCamPos.x > halfBoundsW - halfCamW) {

                //lock to right bound
                newCamPos.x = halfBoundsW - halfCamW;
            }

            //y axis
            if (newCamPos.y < -halfBoundsH + halfCamH) {

                //lock to bottom bound
                newCamPos.y = -halfBoundsH + halfCamH;
            }
            else if (newCamPos.y > halfBoundsH - halfCamH) {

                //lock to top bound
                newCamPos.y = halfBoundsH - halfCamH;
            }

            //update position
            gameObject.transform.position = newCamPos;
        }

        //update previous target position
        _targetPrevPos = targetPos;
    }

    //find bounds for object based on children
    private Bounds FindBounds(GameObject theParent) {

        //store bounds
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

        //retrieve renderers for all children
        Renderer[] allChildren = theParent.GetComponentsInChildren<Renderer>();

        //search all children
        foreach (Renderer aChild in allChildren) {

            //add child bounds to total
            bounds.Encapsulate(aChild.bounds);
        }

        //return
        return bounds;
    }

} //end class