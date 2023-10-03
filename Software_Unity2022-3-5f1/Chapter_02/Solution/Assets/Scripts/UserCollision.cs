/*
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

                //create dialogue text
                dialogue.CreateDialogueWithText(new string[] {"Entering the Dungeon in 3..."});

                //show
                dialogue.Show();

                //hide
                StartCoroutine(dialogue.HideAfterDelay(4.0f));

                //delay
                yield return new WaitForSeconds(1.0f);

                //update dialogue text
                dialogue.UpdateDialogueWithText("2...");

                //delay
                yield return new WaitForSeconds(1.0f);

                //update dialogue text
                dialogue.UpdateDialogueWithText("1...");

                //delay
                yield return new WaitForSeconds(1.0f);

                //update dialogue text
                dialogue.UpdateDialogueWithText("Go!");

                //delay
                yield return new WaitForSeconds(1.0f);

                //switch to dungeon scene
                StateManager.Instance.SwitchSceneTo("Dungeon");

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