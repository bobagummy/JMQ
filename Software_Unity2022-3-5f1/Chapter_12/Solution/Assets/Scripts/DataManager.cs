/*
DataManager
Manages data throughout the application. Uses a singleton instance.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//save/load imports
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class DataManager:MonoBehaviour {

    //current save data
    public DataSave currentSave;

	//singleton instance
    private static DataManager _Instance;

    //singleton accessor
    //access StateManager.Instance from other classes
    public static DataManager Instance {
        
		//create instance via getter
		get {
            
			//check for existing instance
            //if no instance
            if (_Instance == null) {

                //create game object
                GameObject DataManagerObj = new GameObject();
                DataManagerObj.name = "DataManager";

                //create instance
                _Instance = DataManagerObj.AddComponent<DataManager>();

                //init
                _Instance.currentSave = new DataSave();

                //load
                _Instance.LoadData();
            }

            //return the instance
            return _Instance;
        }
    }
	
	//awake
	void Awake() {
		
		//prevent destruction
		DontDestroyOnLoad(this);
	}

    //save game data
    public void SaveData() {     

		//create formatter
        BinaryFormatter formatter = new BinaryFormatter();	

        //create name for saved file
        string filename = "save.luna";

        //create path for saved file
        string filePath = Application.persistentDataPath + "/" + filename;

        //create file stream
        FileStream fileStream = File.Create(filePath);

        //serialize data
        formatter.Serialize(fileStream, currentSave);

        //close file stream
        fileStream.Close();
    }

    //load game data
    public void LoadData(string theFileName = "save.luna") {

        //create file path
        string filePath = Application.persistentDataPath + "/" + theFileName;

        //if file exists
        if (File.Exists(filePath)) {

			//create formatter
			BinaryFormatter formatter = new BinaryFormatter();
			
            //open file stream
            FileStream fileStream = File.Open(filePath, FileMode.Open);

            //deserialize data
            currentSave = (DataSave)formatter.Deserialize(fileStream);

            //close file stream
            fileStream.Close();
        }

        //otherwise, create new save data
        else {

            //save
            SaveData();
        }
    }
	
	public void ResetData() {

		//init save
		_Instance.currentSave = new DataSave();

		//save
		SaveData();
	}

} //end class