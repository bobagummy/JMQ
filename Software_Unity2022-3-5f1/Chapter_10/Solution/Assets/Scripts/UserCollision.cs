/*
UserCollision
Manages collisions for a player-controlled object.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class UserCollision : MonoBehaviour {

    //check entry collisions
    private IEnumerator OnTriggerEnter2D(Collider2D theCollider) {

        //retrieve the tag for the collider's game object
        string tag = theCollider.gameObject.tag;

        //retrieve user move
        UserMove userMove = gameObject.GetComponent<UserMove>();
		
		//retrieve hero group
        Group group = gameObject.GetComponent<Group>();

        //retrieve inventory
        CollectableInventory inventory = GameObject.FindWithTag("Inventory").GetComponent<CollectableInventory>();

        //retrieve level generator
        LevelGenerator levelGen = GameObject.FindWithTag("LevelGenerator").GetComponent<LevelGenerator>();

        //animation
        //retrieve collision object animator
        CharacterAnimator collideAnim = theCollider.gameObject.GetComponent<CharacterAnimator>();

        //check the tag
        switch (tag) {

            //stairs
            case "Stairs":

                //disable collisions
                gameObject.GetComponent<Collider2D>().enabled = false;

                //disable input
                userMove.SetUserInputEnabled(false);

                //animation
                //group
                group.StopAnimations();

                //disable drakes and dragons
                StartCoroutine(levelGen.MoveObjectsWithTag("Drake", false));
                StartCoroutine(levelGen.MoveObjectsWithTag("Dragon", false));

                //continue to next dungeon level
                StartCoroutine(StateManager.Instance.SwitchSceneTo(SceneManager.GetActiveScene().name));           

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

                //animation
                //group
                group.StopAnimations();

                //collision object
                collideAnim.currentAnim.GoToFrameAndStop(0);

                //update dialogue text
                InteractionSystem.Instance.dialogue.CreateDialogueWithText(new string[] {
                    "Do you dare to enter the dungeon?", 
                    "Change your choice using the WASD or ARROW keys.",
                    "Press SPACE to confirm your choice."
                });

                //show
                InteractionSystem.Instance.dialogue.Show();

                //hide on input
                StartCoroutine(InteractionSystem.Instance.dialogue.HideOnInput());

                //create selection text
                List<string> txtSelection = new List<string>() { 
                    "Yes", 
                    "No", 
                    "Cancel"
                };

                //create selection
                InteractionSystem.Instance.selection.CreateSelection(txtSelection);

                //position selection
                InteractionSystem.Instance.selection.PositionAt(gameObject);

                //show selection
                InteractionSystem.Instance.selection.Show();

                //make choice
                int choice = 2;
                yield return StartCoroutine(InteractionSystem.Instance.selection.StartSelection(txtSelection.Count, value => choice = value));

                //check choice
                //yes
                if (choice == 0) {

                    //retrieve character name
                    string knightName = theCollider.gameObject.GetComponent<CharacterData>().characterName;

                    //check name
                    if (knightName == "Red Knight") {

                        //switch to dungeon scene
                        StartCoroutine(StateManager.Instance.SwitchSceneTo("DungeonRed"));
                    }
                    else if (knightName == "Green Knight") {

                        //switch to dungeon scene
                        StartCoroutine(StateManager.Instance.SwitchSceneTo("DungeonGreen"));
                    }
                    else if (knightName == "Blue Knight") {

                        //switch to dungeon scene
                        StartCoroutine(StateManager.Instance.SwitchSceneTo("DungeonBlue"));
                    }
                }

                //no
                else if (choice == 1 || choice == 2) {

                    //enable input
                    userMove.SetUserInputEnabled(true);

                    //animation
                    //collision object
                    collideAnim.currentAnim.Play();
                }

                break;
				
            //hero 
            case "Hero":

				//disable collisions
                theCollider.enabled = false;
			
                //animation
                //group
                group.StopAnimations();

                //collision object
                collideAnim.currentAnim.GoToFrameAndStop(0);

                //retrieve character name
                string heroName = theCollider.gameObject.GetComponent<CharacterData>().characterName;

                //create dialogue
                InteractionSystem.Instance.dialogue.CreateDialogueWithText(
                    new string[] {
                        heroName + " has joined your party!", 
                        "Press SPACE to dismiss."
                    });

                //show
                InteractionSystem.Instance.dialogue.Show();

                //hide
                StartCoroutine(InteractionSystem.Instance.dialogue.HideOnInput());

                //update game data
                DataManager.Instance.currentSave.heroData[heroName] = true;

                //update hero group
                group.LoadMembers();

                //destroy
                Destroy(theCollider.gameObject);

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

                    //animation
                    //group
                    group.StopAnimations();

                    //collision object
                    collideAnim.currentAnim.GoToFrameAndStop(0);
					
					//disable drakes
                    StartCoroutine(levelGen.MoveObjectsWithTag("Drake", false));

                    //return to map
					StartCoroutine(StateManager.Instance.SwitchSceneTo("Map"));
                }

                break;

            //dragon
            case "Dragon":
			
				//disable collisions
                gameObject.GetComponent<Collider2D>().enabled = false;

                //disable input
                userMove.SetUserInputEnabled(false);

                //stop movement
                theCollider.gameObject.GetComponent<AIMove>().StopMove();
				
				//animation
                //group
                group.StopAnimations();

                //collision object
                collideAnim.currentAnim.GoToFrameAndStop(0);

                //create scene name
                //replace the word "Dungeon" in the scene name with "Interaction"
                //e.g. "DungeonRed" becomes "InteractionRed"
                string interactionName = SceneManager.GetActiveScene().name.Replace("Dungeon", "Interaction");

                //load interaction scene
                StartCoroutine(StateManager.Instance.SwitchSceneTo(interactionName));

                break;

            //default
            default:
                Debug.Log("[UserCollision] Tag not recognized");
                break;
        } 
    } 

} //end class