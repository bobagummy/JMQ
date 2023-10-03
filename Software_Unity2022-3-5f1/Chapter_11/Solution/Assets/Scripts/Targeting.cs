/*
Targeting
Manages a visual indicator that moves between targets.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;

public class Targeting : MonoBehaviour {

    //indicator image
    public Image imgIndicator;
	
	//delegate accepts/returns int representing choice
    public delegate int SelectedChoice(int theChoice);

    //show
    public void Show() {

        //set alpha to visible
        gameObject.GetComponent<CanvasGroup>().alpha = 1.0f;
    }

    //hide
    public void Hide() {

        //set alpha to invisible
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

	//start a new target selection
    public IEnumerator StartTargeting(int theNumChoices, SelectedChoice theChoice, List<GameObject> theTargets) {

        //show
        Show();

        //whether dialogue is dismissed
        bool isDismissed = false;

        //while not dismissed
        while (isDismissed == false) {

            //wait for choice to be made
		yield return StartCoroutine(
			WaitForChoice(
			theNumChoices, 
			value => theChoice(value), 
			theTargets)
			);

            //hide
            Hide();

            //toggle flag
            isDismissed = true;
        }
    }

    //wait for user to select among choices
    public IEnumerator WaitForChoice(int theNumChoices, SelectedChoice theChoice, List<GameObject> theTargets) {

        //store current selection
        int currentChoice = 0;

        //while input has not been received
        bool isChoiceSelected = false;
        while (isChoiceSelected == false) {

            //check input
            //select current option
            if (Input.GetKeyUp(KeyCode.Space)) {
				
				//audio
                //play sound effect
				AudioManager.Instance.PlayClipFromSource(
					AudioManager.Instance.sfxChoice, 
					AudioManager.Instance.sfxSource
					);

                //toggle flag
                isChoiceSelected = true;

                //return choice
                theChoice(currentChoice);
            }

            //move forward through options
            else if (theNumChoices > 1 &&
                (Input.GetKeyDown(KeyCode.W) ||
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.D) ||
                Input.GetKeyDown(KeyCode.RightArrow))
                ) {

                //update choice
                currentChoice++;

                //check bounds on current choice
                //exceeds max
                if (currentChoice >= theNumChoices) {

                    //wrap to first choice
                    currentChoice = 0;
                }
            }

            //move backwards through options
            else if (theNumChoices > 1 &&
                (Input.GetKeyDown(KeyCode.S) ||
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.A) ||
                Input.GetKeyDown(KeyCode.LeftArrow))
                ) {

                //update choice
                currentChoice--;

                //check bounds on current choice
                //exceeds min
                if (currentChoice < 0) {

                    //wrap to last choice
                    currentChoice = theNumChoices - 1;

                }
            }

            //calculate indicator position relative to target
            Vector3 targetPos = theTargets[currentChoice].transform.position;
            float targetHeight = theTargets[currentChoice].GetComponent<Renderer>().bounds.size.y;

            //convert to screen coordinates
            Vector3 newPos =
                Camera.main.WorldToScreenPoint(new Vector3(
                    targetPos.x,
                    targetPos.y + targetHeight,
                    0));

            //update position
            Vector3 indicatorPos = imgIndicator.transform.position;
            indicatorPos.x = newPos.x;
            indicatorPos.y = newPos.y;
            imgIndicator.transform.position = indicatorPos;

            //keep waiting
            yield return 0;
        }
    }

} //end class
