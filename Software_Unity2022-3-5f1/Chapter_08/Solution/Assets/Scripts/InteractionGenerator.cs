﻿/*
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
        //add main character (stored at index 0)
        allChars.Add(heroPrefabs[0]);

        //retrieve hero data
        Dictionary<string, bool> heroData = DataManager.Instance.currentSave.heroData;

        //check each member prefab
        //start at 1 to skip main character
        for (int i = 1; i < heroPrefabs.Length; i++) {

            //retrieve character name
            string charName = heroPrefabs[i].GetComponent<CharacterData>().characterName;

            //if saved data contains member
            if (heroData[charName] == true) {

                //add character
                allChars.Add(heroPrefabs[i]);
            }
        }

        //add opponents
		//retrieve number of heroes
        int numHero = allChars.Count;
		
		//spawn drakes based on number of heroes
        for (int j = 0; j < numHero - 1; j++) {

            //add drake (stored at index 1)
            allChars.Add(opponentPrefabs[1]);
        }

        //add dragon (stored at index 0)
        allChars.Add(opponentPrefabs[0]);

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