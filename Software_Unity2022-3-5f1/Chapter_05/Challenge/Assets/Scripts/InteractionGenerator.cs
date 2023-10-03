/*
InteractionGenerator
Manages the generation of interaction scenes.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class InteractionGenerator : MonoBehaviour {

    //map size, in number of rows and columns
    public int numRow;
    public int numCol;

    //store character prefabs
    public GameObject[] heroPrefabs;
    public GameObject[] opponentPrefabs;

    //init
    void Start() {

        //create background map
        CreateBgMap();

        //store all characters
        List<GameObject> allChars = new List<GameObject>();

        //add characters to interaction
        //heroes
        allChars.AddRange(heroPrefabs);

        //opponents
        allChars.AddRange(opponentPrefabs);

        //create interaction
        InteractionSystem.Instance.CreateInteraction(allChars);

        //start interaction
        StartCoroutine(InteractionSystem.Instance.StartInteraction());
    }


    //create background map
    private void CreateBgMap() {

        //get map object from scene
        RandomMap bgMap = GameObject.FindWithTag("BgMap").GetComponent<RandomMap>();

        //generate map array
        int[,] map = bgMap.CreateMapWithSize(numCol, numRow);

        //display tiles
        bgMap.DisplayMap(
            map,
            StateManager.Instance.tileSize,
            StateManager.Instance.pixelsToUnits
            );
    } 
    
} //end class