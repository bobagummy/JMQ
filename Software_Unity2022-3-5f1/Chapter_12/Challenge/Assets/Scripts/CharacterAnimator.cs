/*
CharacterAnimator
Manages multiple animations for a character. 

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CharacterAnimator : MonoBehaviour {
	
	//animation collection
    private Dictionary<string, int[,]> _animations;

    //animation names
    public string[] animNames;

    //frame locations (name index, row, col)
    //set in Unity Inspector
    public Vector3[] animLocs;

    //current frame animation
    public FrameAnimation currentAnim;

    //awake
    void Awake() {

        //populate animation dictionary
        _animations = CreateAnimDict(animNames, animLocs);

        //init animation
        currentAnim = gameObject.AddComponent<FrameAnimation>();
    }

    //create animation dictionary from names and locations
    private Dictionary<string, int[,]> CreateAnimDict(string[] theNames, Vector3[] theLocs) {

        //store result
        Dictionary<string, int[,]> anim = new Dictionary<string, int[,]>();

        //loop through animation names
        for (int i = 0; i < theNames.Length; i++) {

            //find locations for animation
            //x = index matching animation name
            Vector3[] vLocs = Array.FindAll(theLocs, loc => loc.x == i);

            //convert locations to int
            //each animation has N rows with 2 frame columns
            int[,] iLocs = new int[vLocs.Length, 2];
            for (int j = 0; j < vLocs.Length; j++) {

                //j = frame index
                //y = row value; z = col value
                iLocs[j, 0] = (int)vLocs[j].y;
                iLocs[j, 1] = (int)vLocs[j].z;
            }

            //add to dictionary
            anim.Add(theNames[i], iLocs);
        }

        //return
        return anim;
    }
	
	//find frames for specific animation
	private Sprite[] FindFramesFor(string theName) {

		//check animation
		int[,] frameLoc;
		if (_animations.TryGetValue(theName, out frameLoc)) {

			//retrieve number of frame locations
			int numFrames = frameLoc.GetLength(0);

			//store frames
			Sprite[] frames = new Sprite[numFrames];

			//retrieve master set of frames
			Sprite[,] allFrames = GameObject.FindWithTag("SpriteSlicer").GetComponent<SpriteSlicer>().frames;

			//loop through locations
			for (int i = 0; i < numFrames; i++) {

				//retrieve frame from master set
				frames[i] = allFrames[frameLoc[i, 0], frameLoc[i, 1]];
			}

			//return
			return frames;
		}

		//return
		return null;
	}
	
	//set up animation
    public void SetUpAnimationWithName(string theName, float theFPS = 1.0f, bool theIsLoop = true) {
        
        //find frames
        Sprite[] frames = FindFramesFor(theName);

        //init animation
        currentAnim.InitFrameAnimation(theName, frames, theFPS, theIsLoop);
    }

} //end class