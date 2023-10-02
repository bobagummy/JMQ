/*
CollectableInventory
Manages an inventory of collectable objects.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CollectableInventory : MonoBehaviour {

	//the number of objects stored in the inventory
    public int numObjects;

    //minimum number of objects in inventory
    public int minObjects;

    //maximum number of objects in inventory
    public int maxObjects;
	
	//inventory text
	private Text _txtObjects;

    //init
    void Start() {
        
        //retrieve text
        _txtObjects = gameObject.GetComponentInChildren<Text>();

        //update text
        _txtObjects.text = numObjects.ToString();

    } //end function
    
	//add an object to the inventory
	public void AddItem() {

        //check whether space remains in inventory
        if (numObjects < maxObjects) {

            //increment counter
            numObjects++;

            //update text
            _txtObjects.text = numObjects.ToString();

        } //end if

	} //end function

	//remove the most recent object from the inventory
	public void RemoveItem() {

		//only remove if at least one item exists
        if (numObjects > minObjects) {

            //increment counter
            numObjects--;

            //update text
            _txtObjects.text = numObjects.ToString();

		} //end if

	} //end function

} //end class