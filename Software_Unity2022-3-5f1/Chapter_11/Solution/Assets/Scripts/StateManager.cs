/*
StateManager
Manages the overall application state. 

Copyright 2015 John M. Quick
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StateManager : MonoBehaviour {

    //default size of tiles, in pixels
    public int tileSize;

    //pixels to Unity world units conversion
    public int pixelsToUnits;

    //fade script
    public Fade fade;

	//singleton instance
    private static StateManager _Instance;

    //singleton accessor
    //access StateManager.Instance from other classes
    public static StateManager Instance {
        
		//create instance via getter
		get {
            
			//check for existing instance
            //if no instance
            if (_Instance == null) {

                //create game object
                GameObject StateManagerObj = new GameObject();
                StateManagerObj.name = "StateManager";

                //create instance
                _Instance = StateManagerObj.AddComponent<StateManager>();

                //init properties
                _Instance.tileSize = 64;
                _Instance.pixelsToUnits = 100;

                //add scripts
                _Instance.fade = StateManagerObj.AddComponent<Fade>();
            }

            //return the instance
            return _Instance;
        }
    }

    //awake
    void Awake() {

        //prevent from being destroyed
        DontDestroyOnLoad(this);     
    }

    //switch scene by name
    public IEnumerator SwitchSceneTo(string theScene, float theDelay = 0.0f) {

        //save
        DataManager.Instance.SaveData();

        //delay
        yield return new WaitForSeconds(theDelay);

        //fade
        fade.ToggleFade();

        //audio 
        /*
        Note: Only fade audio if a source clip is defined
        and a different scene is loaded (i.e. continue music 
        across repeated scenes).
        */
        if (AudioManager.Instance.bgmSource.clip != null &&
            SceneManager.GetActiveScene().name != theScene) {
            AudioManager.Instance.ToggleFade();
        }

        //delay
        yield return new WaitForSeconds(fade.duration);
		
		//hide canvas
        GameObject canvas = GameObject.FindWithTag("Canvas");
        canvas.GetComponent<CanvasGroup>().alpha = 0.0f;

        //check win conditions
        //retrieve dungeon data
        Dictionary<string, bool> dungeonData = DataManager.Instance.currentSave.dungeonData;

        //check if all dungeons completed
        if (dungeonData["Red"] == true &&
            dungeonData["Green"] == true &&
            dungeonData["Blue"] == true) {

            //create dialogue
			InteractionSystem.Instance.dialogue.CreateDialogueWithText("The game is won!");

			//show dialogue
			InteractionSystem.Instance.dialogue.Show();
        }

        //otherwise
        else {

            //load next scene
			SceneManager.LoadScene(theScene);
        }

        //fade
        fade.ToggleFade();
    }

} //end class