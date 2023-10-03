﻿/*
UserCollision
Manages collisions for a player-controlled object.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UserCollision : MonoBehaviour {

    //check entry collisions
    private IEnumerator OnTriggerEnter2D(Collider2D theCollider) {

        //retrieve the tag for the collider's game object
        string tag = theCollider.gameObject.tag;

        //retrieve user move
        UserMove userMove = gameObject.GetComponent<UserMove>();

        //retrieve inventory
        CollectableInventory inventory = GameObject.FindWithTag("Inventory").GetComponent<CollectableInventory>();

        //check the tag
        switch (tag) {

            //stairs
            case "Stairs":

                //disable collisions
                gameObject.GetComponent<Collider2D>().enabled = false;

                //disable input
                userMove.SetUserInputEnabled(false);

                //continue to next dungeon level
                StateManager.Instance.SwitchSceneTo("Dungeon");           

                break;
				
			//collectable 
            case "Collectable":

                //inventory has space remaining
                if (inventory.numObjects < inventory.maxObjects) {

                    //add collectable to inventory
                    inventory.AddItem();
					
					//destroy
					Destroy(theCollider.gameObject);
                }

                break;
				
			//knights
            case "Knight":

                //disable input
                userMove.SetUserInputEnabled(false);

                //retrieve dialogue
                Dialogue dialogue = GameObject.FindWithTag("Dialogue").GetComponent<Dialogue>();

                //update dialogue text
                dialogue.CreateDialogueWithText(new string[] {
                    "Do you dare to enter the dungeon?", 
                    "Change your choice using the UP or DOWN ARROW keys.",
                    "Press SPACE to confirm your choice."
                });

                //show
                dialogue.Show();

                //hide on input
                StartCoroutine(dialogue.HideOnInput());

                //retrieve selection
                Selection selection = GameObject.FindWithTag("Selection").GetComponent<Selection>();

                //create selection text
                List<string> txtSelection = new List<string>() { 
                    "Yes", 
                    "No", 
                    "Cancel"
                };

                //create selection
                selection.CreateSelection(txtSelection);

                //position selection
                selection.PositionAt(gameObject);

                //show selection
                selection.Show();

                //make choice
                int choice = 2;
                yield return StartCoroutine(
                    selection.StartSelection(
                        txtSelection.Count, 
                        value => choice = value
                        )
                    );

                //check choice
                //yes
                if (choice == 0) {

                    //switch to dungeon scene
                    StateManager.Instance.SwitchSceneTo("Dungeon");
                }

                //no or cancel
                else if (choice == 1 || choice == 2) {

                    //enable input
                    userMove.SetUserInputEnabled(true);
                }

                break;

            //drake
            case "Drake":

                //disable collisions
                theCollider.enabled = false;

                //stop movement
                theCollider.gameObject.GetComponent<AIMove>().StopMove();

                //check inventory
                //inventory has items remaining
                if (inventory.numObjects > inventory.minObjects) {

                    //remove item from inventory
                    inventory.RemoveItem();
                }

                //if inventory is empty
                if (inventory.numObjects <= inventory.minObjects) {

                    //disable collisions
                    gameObject.GetComponent<Collider2D>().enabled = false;

                    //disable input
                    userMove.SetUserInputEnabled(false);
					
					//switch scene
					StateManager.Instance.SwitchSceneTo("Map");
                }

                break;

            //default
            default:
                Debug.Log("[UserCollision] Tag not recognized");
                break;
        } 
    } 

} //end class