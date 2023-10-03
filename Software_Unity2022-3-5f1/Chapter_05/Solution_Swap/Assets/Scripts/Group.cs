/*
Group
Manages the player's group of heroes.

Copyright 2015 John M. Quick.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Group : MonoBehaviour {

    //the member sprites
    public List<Sprite> members;

    //index of current member selected in group
    private int _currentMember;

    //init
    void Start() {

        //init List
        members = new List<Sprite>();

        //add player sprite
        members.Add(gameObject.GetComponent<SpriteRenderer>().sprite);

        //start at first member
        _currentMember = 0;
    }

    //update
    void Update() {

        //check for e key press
        if (Input.GetKeyDown(KeyCode.E)) {

            //toggle member
            ToggleMember();
        }
    }

    //toggle between group members
    private void ToggleMember() {

        //increment current index
        _currentMember++;

        //verify that index is within bounds
        if (_currentMember > members.Count - 1) {

            //reset
            _currentMember = 0;
        }

        //update the renderer based on the current index
        gameObject.GetComponent<SpriteRenderer>().sprite = members[_currentMember];
    }

} //end class