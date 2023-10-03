/*
CharacterData
Stores data about characters.
Used by the interaction system.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class CharacterData : MonoBehaviour {

    //character's name
    public string characterName;

    //whether the character is a hero
    public bool isHero;

    //whether the character is active in the current interaction
    public bool isActive;

    //currently selected action
    public int currentAction;

    //currently selected target
    public int currentTarget;

} //end class