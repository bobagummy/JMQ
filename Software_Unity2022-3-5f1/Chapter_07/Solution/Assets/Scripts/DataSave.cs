/*
DataSave
Represents a single instance of a saved game.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//mark class for saving
[Serializable]

public class DataSave {

    //heroes found
    public Dictionary<string, bool> heroData;

    //dungeons completed
    public Dictionary<string, bool> dungeonData;

    //number of collectables
    public int numCollectables;

    //custom constructor
    //create new save data
    public DataSave() {

        //define hero data
        heroData = new Dictionary<string, bool>() {
                {"Lily", false},
                {"Pink Beard", false}, 
                {"Larg", false}
        };

        //define dungeon data
        dungeonData = new Dictionary<string, bool>() {
                {"Red", false},
                {"Green", false}, 
                {"Blue", false}
        };

        //initial number of collectables
        numCollectables = 5;
    }
    
} //end class