/*
LevelGenerator
Manages the generation of levels.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour {
  
    //level types
    public enum LevelType {

        //map
        Map = 0,

        //dungeon
        Red = 1,
        Green = 2,
        Blue = 3,
    }

    //current level type
    public LevelType levelType;

    //name of hero to be found
    public string heroName;
	
	//map size, in number of rows and columns
    public int numRow;
    public int numCol;

    //position for objects when map is loaded
    public Vector2 playerPos;

    //available positions in middleground layer
    private List<Vector2> _middleOpenPos;

    //init
    void Start() {

        //hide dialogue
        InteractionSystem.Instance.dialogue.Hide();

        //retrieve player
        GameObject player = GameObject.FindWithTag("Player");

        //disable user input
        player.GetComponent<UserMove>().SetUserInputEnabled(false);

        //populate middleground with open positions
        _middleOpenPos = CreatePos(numCol, numRow);

        //randomize order of open positions
        RandSortPos(_middleOpenPos);

        //create background map
        CreateBgMap();

        //position player
        PositionObjectAt("Player", playerPos);
		
		//update inventory
        CollectableInventory inventory = GameObject.FindWithTag("Inventory").GetComponent<CollectableInventory>();
        inventory.LoadItems();

        //show canvas
        GameObject canvas = GameObject.FindWithTag("Canvas");
        canvas.GetComponent<CanvasGroup>().alpha = 1.0f;

        //check level type
        //map
        if (levelType == LevelType.Map) {

            //generate map
            GenerateMap();
        }

        //dungeon
        else {

            //generate dungeon
            GenerateDungeon();
        }

        //load group
        player.GetComponent<Group>().LoadMembers();
        
        //enable user input
        player.GetComponent<UserMove>().SetUserInputEnabled(true);
    }

    //generate the world map level
    private void GenerateMap() {

        //retrieve game data
        Dictionary<string, bool> dungeonData = DataManager.Instance.currentSave.dungeonData;

        //check whether dungeons are completed
        //if so, remove knights
        if (dungeonData["Red"] == true) {

            //destroy
            Destroy(GameObject.Find("RedKnight"));
        }
        if (dungeonData["Green"] == true) {

            //destroy
            Destroy(GameObject.Find("GreenKnight"));
        }
        if (dungeonData["Blue"] == true) {

            //destroy
            Destroy(GameObject.Find("BlueKnight"));
        }

        //collectables
        SpawnObjectsWithTag("Collectable");
    }

    //generate a dungeon level
    private void GenerateDungeon() {

        //stairs
        SpawnObjectsWithTag("Stairs");

        //retrieve game data
        Dictionary<string, bool> heroData = DataManager.Instance.currentSave.heroData;
        Dictionary<string, bool> dungeonData = DataManager.Instance.currentSave.dungeonData;

        //check data
        bool isHeroFound = heroData[heroName];
        bool isDungeonComplete = dungeonData[System.Enum.GetName(typeof(LevelType), levelType)];

        //hero not yet found
        if (isHeroFound == false) {

            //generate random chance hero will spawn
            float randChance = Random.Range(0, 4);

            //25% chance
            if (randChance == 0) {

                //spawn hero
                SpawnObjectsWithTag("Hero");
            }

            //drakes
            SpawnObjectsWithTag("Drake");

            //movement
            MoveObjectsWithTag("Drake");
        }

        //hero found, but dragon not yet defeated
        else if (isHeroFound == true && isDungeonComplete == false) {

            //spawn dragon
            SpawnObjectsWithTag("Dragon");

            //movement
            MoveObjectsWithTag("Dragon");
        }

        //collectables
        SpawnObjectsWithTag("Collectable");
    }

    //populate the open map positions
	private List<Vector2> CreatePos(int theNumCol, int theNumRow) {
	
		//create collection to store open positions
		List<Vector2> allPos = new List<Vector2>();
		
        //populate open positions
        //iterate through columns
        for (int col = 0; col < theNumCol; col++) {

            //iterate through rows
            for (int row = 0; row < theNumRow; row++) {

                //store position
                Vector2 pos = new Vector2(col, row);

                //add position
                allPos.Add(pos);
            } 
        } 
		
		//return open positions
		return allPos;
	} 

    //randomize order of open positions
	//implements the Sattolo sorting method
	private void RandSortPos(List<Vector2> thePositions) {
		
		//start counter at last item in collection
        int indexCounter = thePositions.Count;
		
		//loop through items
        while (indexCounter > 1) {

            //decrement counter
            indexCounter--;

            //store a copy of the original value
            Vector2 original = new Vector2(thePositions[indexCounter].x, thePositions[indexCounter].y);

            //calculate random index value
            int randIndex = Random.Range(0, indexCounter);

            //swap the original value for the random 
            thePositions[indexCounter] = thePositions[randIndex];

            //swap the random value for the original
            thePositions[randIndex] = original;
        }
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
	
	//position object at specific coordinates
    private void PositionObjectAt(string theTag, Vector2 thePos) {

        //get player game object from scene
        GameObject obj = GameObject.FindWithTag(theTag);

        //verify that object exists in scene
        if (obj != null) {

            //store new position
            Vector3 newPos = obj.transform.position;

            //retrieve tile size
            int ts = StateManager.Instance.tileSize;

            //retrieve pixels to units
            int ptu = StateManager.Instance.pixelsToUnits;

            //calculate player position in world units
            //x position
            float xPos = (float)((thePos.x * ts) + (ts / 2) - (numCol * ts / 2)) / ptu;

            //y position
            float yPos = (float)((numRow * ts / 2) - (thePos.y * ts) - (ts / 2)) / ptu;

            //update position
            newPos.x = xPos;
            newPos.y = yPos;
            newPos.z = obj.transform.position.z;
            obj.transform.position = newPos;

            //exclude obj pos
            _middleOpenPos.Remove(thePos);
        }       
    } 

    //spawn objects associated with a specific tag
    private void SpawnObjectsWithTag(string theTag) {

        //retrieve objects associated with tag
        GameObject[] objects = GameObject.FindGameObjectsWithTag(theTag);

        //iterate through objects
        for (int i = 0; i < objects.Length; i++) {

            //retrieve spawn script
            MapSpawn spawnScript = objects[i].GetComponent<MapSpawn>();

            //if valid
            if (spawnScript) {

                //if more tiles have been specified than are open
                if (spawnScript.numTiles > _middleOpenPos.Count) {

                    //restrict to number of available tiles
                    spawnScript.numTiles = _middleOpenPos.Count;
                }

                //clone tiles
                GameObject[] spawnTiles = spawnScript.CloneTiles(spawnScript.tilePrefab, spawnScript.numTiles);

                //spawn tile at random position
                spawnScript.SpawnTilesAtRandPos(
                    spawnTiles,
                    _middleOpenPos,
                    StateManager.Instance.tileSize,
                    StateManager.Instance.pixelsToUnits,
                    numRow,
                    numCol
                    );
            }
        }
    }

    //move objects associated with a specific tag
    private void MoveObjectsWithTag(string theTag) {

        //retrieve all objects with given tag
        GameObject[] objects = GameObject.FindGameObjectsWithTag(theTag);

        //iterate through objects
        foreach (GameObject anObject in objects) {

            //retrieve move script
            AIMove move = anObject.GetComponent<AIMove>();

            //if valid
            if (move) {

                //start movement
                move.StartMove();
            }
        }
    }

} //end class