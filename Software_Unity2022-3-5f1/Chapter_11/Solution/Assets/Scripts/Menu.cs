/*
Menu
Manages the main menu scene.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {

    //whether input is enabled
    private bool _isInputEnabled;

    //init
    void Start() {

        //audio
		//play music
        AudioManager.Instance.PlayClipFromSource(
            AudioManager.Instance.bgmMenu, 
            AudioManager.Instance.bgmSource
            );
			
		//fade music
        AudioManager.Instance.ToggleFade();

        //set flag
        _isInputEnabled = true;
    }

    //update
    void Update() {

        //check user input
        if (_isInputEnabled && Input.anyKeyDown) {

            //toggle flag
            _isInputEnabled = false;

            //start game
            StartCoroutine(StateManager.Instance.SwitchSceneTo("Map"));
        }
    }
}