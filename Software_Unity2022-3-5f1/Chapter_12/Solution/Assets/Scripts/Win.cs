/*
Win
Manages the win scene.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class Win : MonoBehaviour {

    //whether input is enabled
    private bool _isInputEnabled;

    //init
    void Start() {

        //audio
        //play music
        AudioManager.Instance.PlayClipFromSource(
			AudioManager.Instance.bgmWin, 
			AudioManager.Instance.bgmSource
		);

		//fade music
        AudioManager.Instance.ToggleFade();

        //create dialogue
        InteractionSystem.Instance.dialogue.CreateDialogueWithText(new string[] {
            "Congratulations on your successful quest!",
            "Press ESC to return to the menu.", 
            "Your coding journey continues..."
        });

        //show dialogue
        InteractionSystem.Instance.dialogue.Show();

        //hide dialogue
        StartCoroutine(InteractionSystem.Instance.dialogue.HideOnInput());

        //set flag
        _isInputEnabled = true;
    }

    //update
    void Update() {

        //check user input
        if (_isInputEnabled == true && Input.GetKeyUp(KeyCode.Escape)) {

            //toggle flag
            _isInputEnabled = false;

            //reset game
            StateManager.Instance.ResetGame();
        }
    }

} //end class