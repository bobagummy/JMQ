/*
DataSave
Represents a single instance of a saved game.

Copyright 2015 John M. Quick
*/

//mark class for saving
[System.Serializable]

public class DataSave {

    //number of collectables
    public int numCollectables;

    //custom constructor
    //create new save data
    public DataSave() {

        //initial number of collectables
        numCollectables = 5;
    }
    
} //end class