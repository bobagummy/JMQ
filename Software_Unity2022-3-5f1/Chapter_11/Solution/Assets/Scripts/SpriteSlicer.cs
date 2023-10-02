/*
SpriteSlicer
Slices a sprite sheet according to size. 

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class SpriteSlicer : MonoBehaviour {

    //sprite sheet
    public Texture2D spriteSheet;

    //sprite size, in pixels
    public int spriteSize;

    //individual frames
    public Sprite[,] frames;
	
	//awake
	void Awake() {
		
		//prevent destruction
		DontDestroyOnLoad(this);
	}

    //init
    void Start() {

        //slice sprite sheet into frames
        frames = SliceSprites(spriteSheet, spriteSize);
    }

    //get frames from sprite sheet
    public Sprite[,] SliceSprites(Texture2D theSheet, int theSpriteSize) {

        //retrieve dimensions
        int numRow = theSheet.height / theSpriteSize;
        int numCol = theSheet.width / theSpriteSize;

        //store frames
        Sprite[,] allFrames = new Sprite[numRow, numCol];

        //iterate through rows
        for (int row = 0; row < numRow; row++) {

            //iterate through columns
            for (int col = 0; col < numCol; col++) {

                //create rectangle at position in sheet
                Rect sliceRect = new Rect(
                    col * theSpriteSize,
                    row * theSpriteSize,
                    theSpriteSize,
                    theSpriteSize
                    );

                //create pivot point
                Vector2 slicePivot = new Vector2(0.5f, 0.5f);

                //create slice
                Sprite slice = Sprite.Create(theSheet, sliceRect, slicePivot);

                /* 
                Modify the row calculation to switch the sprite sheet 
                origin from bottom-left (default) to top-left. This 
                allows the sprite sheet locations to be read from top 
                to bottom, left to right.
                */
                //add to collection
                allFrames[numRow - 1 - row, col] = slice;
            }
        }

        //return
        return allFrames;
    }

} //end class