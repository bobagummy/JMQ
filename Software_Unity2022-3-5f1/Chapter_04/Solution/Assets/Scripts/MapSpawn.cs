/*
MapSpawn
Spawns a tile prefab at random positions on the map.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapSpawn : MonoBehaviour {
    
	//number of tiles to spawn
	public int numTiles;
		
	//tile prefab defined in Unity inspector
	public GameObject tilePrefab;
	
	//clone a given tile prefab a specified number of times
	public GameObject[] CloneTiles(GameObject thePrefab, int theNumClones) {

        //store clones in array
        GameObject[] cloneTiles = new GameObject[theNumClones];
            
        //populate cloned tiles array
        for (int i = 0; i < theNumClones; i++) {

            //clone prefab tile
            GameObject cloneTile = (GameObject)Instantiate(thePrefab);

            //add the tile to the map game object in Unity scene
            //set parent
            cloneTile.transform.parent = gameObject.transform;

            //add to clone array
            cloneTiles[i] = cloneTile;
        }

		//return clone array
		return cloneTiles;
	}
	
	//spawn tiles at randomly generated map positions
	public void SpawnTilesAtRandPos(GameObject[] theTiles, List<Vector2> theOpenPos, int theTileSize, int thePixelsToUnits, int theNumRow, int theNumCol) {

        //loop through tiles
        foreach (GameObject aTile in theTiles) {

            //select the next open map position
            float randCol = theOpenPos[0].x; 
            float randRow = theOpenPos[0].y;
			
			//remove the used position
            theOpenPos.RemoveAt(0);

            //store new position
            Vector3 newPos = aTile.transform.position;
            
            //calculate tile position
            //x position
            float xPos = ((randCol * theTileSize) + (theTileSize / 2) - (theNumCol * theTileSize / 2)) / thePixelsToUnits;

            //y position
            float yPos = ((theNumRow * theTileSize / 2) - (randRow * theTileSize) - (theTileSize / 2)) / thePixelsToUnits;

            //update position
            newPos.x = xPos;
            newPos.y = yPos;
            newPos.z = gameObject.transform.position.z;
            aTile.transform.position = newPos;
        }
    }
	
} //end class