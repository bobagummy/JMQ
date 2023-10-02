/*
Load
Establishes singletons and other permanent objects.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class Load : MonoBehaviour {

    //delay before loading game, in seconds
    public float loadDelay;

    //awake
    void Awake() {
		
            //load
            StartCoroutine(LoadGame());
    }

    //load
    private IEnumerator LoadGame() {
  
		//init persistent objects
		InitPersistentObjects();

        //delay
        yield return new WaitForSeconds(loadDelay);

        //load scene
        StartCoroutine(StateManager.Instance.SwitchSceneTo("Menu"));
    }
	
    //init objects that persist through scenes
    public void InitPersistentObjects() {
		
		//state manager
		if (StateManager.Instance) {};
		
		//data manager
		if (DataManager.Instance) {};
		
		//interaction system
		if (InteractionSystem.Instance) {};

        //canvas
        GameObject canvas = GameObject.FindWithTag("Canvas");
        DontDestroyOnLoad(canvas);
        canvas.GetComponent<CanvasGroup>().alpha = 0.0f;
    }
}