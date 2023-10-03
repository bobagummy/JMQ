/*
AIMove
Manages the movement of a non-player-controlled object. 
The AI will chase the player while visible on screen.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AIMove : MonoBehaviour {

    //speed boundaries
    public float minSpeed;
    public float maxSpeed;

    //whether object can move
    private bool _canMove;

    //chase target object
    private GameObject _target;

    //speed object is moving
    private float _speed;

    //destination position of the object
    private Vector3 _destPos;

    //update
    void Update() {

        //check whether in camera view
        bool isVisible = IsRendererVisibleToCamera(gameObject.GetComponent<Renderer>(), Camera.main);

        //if object can move and is visible
        if (_canMove == true && isVisible == true) {

            //move object
            MoveTo(_destPos);

            //animation
            //update sprite direction based on relative player position
            //left
            if (_destPos.x < gameObject.transform.position.x) {

                //invert scale
                gameObject.transform.localScale = new Vector3(-1, 1, 1);
            }
            //right
            else {

                //normal scale
                gameObject.transform.localScale = Vector3.one;
            }
        }                       
    }

    //find destination position
    private Vector3 FindDestPos(GameObject theTarget) {

        //store position
        Vector3 destPos = gameObject.transform.position;

        //target position
        Vector3 targetPos = _target.transform.position;

        //update destination
        destPos.x = targetPos.x;
        destPos.y = targetPos.y;

        //return
        return destPos;
    }

    //move the object towards destination
    private void MoveTo(Vector3 theDestPos) {

        //store new position
        Vector3 newPos = gameObject.transform.position;

        //update movement based on speed, direction, and time
        //up
        if (newPos.y < theDestPos.y - _speed * Time.deltaTime) {

            //update y
            newPos.y += _speed * Time.deltaTime;
        }

        //down
        else if (newPos.y > theDestPos.y + _speed * Time.deltaTime) {

            //update y
            newPos.y -= _speed * Time.deltaTime;
        }

        //at destination
        else {

            //update y
            newPos.y = theDestPos.y;
        }

        //left
        if (newPos.x > theDestPos.x + _speed * Time.deltaTime) {

            //update x
            newPos.x -= _speed * Time.deltaTime;
        }

        //right
        else if (newPos.x < theDestPos.x - _speed * Time.deltaTime) {

            //update x
            newPos.x += _speed * Time.deltaTime;
        }

        //at destination
        else {

            //update x
            newPos.x = theDestPos.x;
        }

        //update object position
        gameObject.transform.position = newPos;

        //update destination position
        _destPos = FindDestPos(_target);
    }

    //start movement
    public void StartMove() {

        //generate random speed
        _speed = Random.Range(minSpeed, maxSpeed);

        //set target
        //chase the player
        _target = GameObject.FindWithTag("Player");

        //calculate destination
        _destPos = FindDestPos(_target);

        //toggle flag
        _canMove = true;
    }

    //stop movement
    public void StopMove() {

        //toggle flag
        _canMove = false;
    }

    //check visibility of a renderer to a specific camera
    public bool IsRendererVisibleToCamera(Renderer theRenderer, Camera theCamera) {

        //retrieve planes from camera view
        Plane[] camPlanes = GeometryUtility.CalculateFrustumPlanes(theCamera);

        //test renderer bounds against camera planes
        //returns true if within view
        bool isVisible = GeometryUtility.TestPlanesAABB(camPlanes, theRenderer.bounds);

        //return
        return isVisible;
    }

} //end class