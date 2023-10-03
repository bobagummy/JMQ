/*
RandomMap
Generates a random tile map based of a given size using prefabs.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class RandomMap : MonoBehaviour {

	//array for holding tile prefabs
	//defined in Unity Inspector
    public GameObject[] tilePrefabs; 

    //randomly generate numeric map representation based on tiles
    public int[,] CreateMapWithSize(int theNumCol, int theNumRow) {
		
		//initialize map array
        int[,] mapArray = new int[theNumRow, theNumCol];

        //fill the map array with random tiles from prefab array
		//iterate through map columns (x)
		for (int col = 0; col < theNumCol; col++) {

			//iterate through map rows (y)
			for (int row = 0; row < theNumRow; row++) {

				//get a random tile from prefab array
				//random index value based on array size
				int randIndex = Random.Range(0, tilePrefabs.Length); 

				//store the tile's index value in the map array
				mapArray[row, col] = randIndex;
			}
		}
		
		//return map array
		return mapArray;
    }
	
    //display the map tiles on screen
    public void DisplayMap(int[,] theMapArray, int theTileSize, int thePixelsToUnits) {
		
		//retrieve array size
		int numRow = theMapArray.GetLength(0);
		int numCol = theMapArray.GetLength(1);
		
        //loop through the map array
        //iterate through map columns (x)
		for (int col = 0; col < numCol; col++) {

			//iterate through map rows (y)
			for (int row = 0; row < numRow; row++) {

                //clone prefab tile based on value stored in map array
                GameObject displayTile = (GameObject)Instantiate(tilePrefabs[theMapArray[row, col]]);
 
                //store new position
                Vector3 newPos = displayTile.transform.position;

                //calculate tile position 
				//x position
                float xPos = (float)((col * theTileSize) + (theTileSize / 2) - (numCol * theTileSize / 2)) / thePixelsToUnits;
				
				//y position
                float yPos = (float)((numRow * theTileSize / 2) - (row * theTileSize) - (theTileSize / 2)) / thePixelsToUnits;

                //update position
                newPos.x = xPos;
                newPos.y = yPos;
                newPos.z = gameObject.transform.position.z;
                displayTile.transform.position = newPos;

                //add the tile to the map game object in Unity scene
				//set parent
                displayTile.transform.parent = gameObject.transform;
            }
        }
    }

} //end class