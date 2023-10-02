/*
UserMove
Manages the movement of a player-controlled object.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class UserMove : MonoBehaviour {

    //whether user input is enabled
    public bool isInputEnabled;

	//the speed at which to move the object
    public float speed; 
	
	//the updated direction
    private Vector2 _newDir; 

    //whether currently listening for keys
    private bool _isKeyListen;

    //possible key presses
    private enum Keys {
        None = 0,
        Up, 
        Down, 
        Left, 
        Right
    }

    //current key press
    private Keys _currentKey;

    //update
    void Update() {

        //if input enabled
        if (isInputEnabled == true) {

            //check user input
            CheckUserInput();

            //move object
            MoveObject();
        }
    } 

    //set user input
    public void SetUserInputEnabled(bool theIsEnabled) {

        //toggle flag
        isInputEnabled = theIsEnabled;

        //reset keys
        _isKeyListen = theIsEnabled;
        _currentKey = Keys.None;
    }

    //check user input
    private void CheckUserInput() {

        /*
        Input Notes 
        GetKey returns true while key is held
        GetKeyDown returns true only when the key is initially pressed
        GetKeyUp returns true only when the key is initially released
        */

        //key released
        if (_isKeyListen == false && 
            ((Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W)) && _currentKey == Keys.Up) || 
            ((Input.GetKeyUp(KeyCode.DownArrow) || Input.GetKeyUp(KeyCode.S)) && _currentKey == Keys.Down) || 
            ((Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A)) && _currentKey == Keys.Left) ||
            ((Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D)) && _currentKey == Keys.Right)
            ) {

            //update keys
            _isKeyListen = true;
            _currentKey = Keys.None;
        }

        //no key held
        else if (
            !Input.GetKey(KeyCode.UpArrow) && !Input.GetKey(KeyCode.W) && 
            !Input.GetKey(KeyCode.DownArrow) && !Input.GetKey(KeyCode.S) && 
            !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.A) &&
            !Input.GetKey(KeyCode.RightArrow) && !Input.GetKey(KeyCode.D)
            ) {

            //update keys
            _isKeyListen = true;
            _currentKey = Keys.None;
        }

        //movement
        //store the new movement direction based on user input
        int newDirX = 0;
        int newDirY = 0;

		//move up
		if (
            (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) && 
            (_isKeyListen == true || _currentKey == Keys.Up)) {

            //update keys
            _isKeyListen = false;
            _currentKey = Keys.Up;
			
			//move up along the Y axis
			newDirY = 1;
		}

		//move down
        else if (
            (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) && 
            (_isKeyListen == true || _currentKey == Keys.Down)
            ) {

            //update keys
            _isKeyListen = false;
            _currentKey = Keys.Down;
			
			//move down along the Y axis
			newDirY = -1;
		}

		//move left
        else if (
            (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) && 
            (_isKeyListen == true || _currentKey == Keys.Left)
            ) {

            //update keys
            _isKeyListen = false;
            _currentKey = Keys.Left;
			
			//move left along the X axis
			newDirX = -1;
		} 

		//move right
        else if (
            (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) && 
            (_isKeyListen == true || _currentKey == Keys.Right)
            ) {

            //update keys
            _isKeyListen = false;
            _currentKey = Keys.Right;
			
			//move right along the X axis
			newDirX = 1;
		} 

		//update current direction attempted
		_newDir = new Vector2(newDirX, newDirY);
    } 

    //move the object to its new position
    private void MoveObject() {

        //store the player's position
        Vector3 newPos = gameObject.transform.position;
				
        //change in movement based on user input, speed, and time
		float deltaMoveX = _newDir.x * speed * Time.deltaTime;
        float deltaMoveY = _newDir.y * speed * Time.deltaTime;
		
		//update coordinates
        newPos.x += deltaMoveX;
        newPos.y += deltaMoveY;

        //check bounds
        float halfObjW = gameObject.GetComponent<Renderer>().bounds.extents.x;
        float halfObjH = gameObject.GetComponent<Renderer>().bounds.extents.y;
        float halfCamW = Camera.main.orthographicSize * Screen.width / Screen.height;
        float halfCamH = Camera.main.orthographicSize;
        Vector3 camPos = Camera.main.transform.position;

        //x axis
        if (newPos.x < camPos.x - halfCamW + halfObjW) {

            //stop at left edge
            newPos.x = camPos.x - halfCamW + halfObjW;
        }
        else if (newPos.x > camPos.x + halfCamW - halfObjW) {

            //stop at right edge
            newPos.x = camPos.x + halfCamW - halfObjW;
        }

        //y axis
        if (newPos.y > camPos.y + halfCamH - halfObjH) {

            //stop at top edge
            newPos.y = camPos.y + halfCamH - halfObjH;
        }
        else if (newPos.y < camPos.y - halfCamH + halfObjH) {

            //stop at bottom edge
            newPos.y = camPos.y - halfCamH + halfObjH;
        }

        //update object position
		gameObject.transform.position = newPos;
    } 

} //end class