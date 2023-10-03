/*
Selection
Manages a selection box that presents choices to the user.
Can be dismissed through user input options.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Collections.Generic;

public class Selection : MonoBehaviour {

    //selection indicator image
    public Image imgIndicator;

    //text objects
    public GameObject[] textObjects;

    //open a new selection box with given text and choices
    public void CreateSelection(List<string> theText) {

        //reset indicator
        ResetIndicator();

        //clear text
        ClearText();

        //iterate through text objects
        for (int i = 0; i < theText.Count; i++) {

            //retrieve text
            Text txt = textObjects[i].GetComponent<Text>();

            //update text
            txt.text = theText[i];
        }
    }

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

    //clear all text objects
    private void ClearText() {

        //for each text object
        foreach (GameObject aTxtObj in textObjects) {

            //retrieve text
            Text txt = aTxtObj.GetComponent<Text>();

            //update text
            txt.text = "";
        }
    }

    //reset indicator
    private void ResetIndicator() {

        //update indicator position
        Vector3 newPos = imgIndicator.transform.position;
        newPos.y = textObjects[0].transform.position.y;
        imgIndicator.transform.position = newPos;
    }

    //position selection above object with buffer (in pixels)
    public void PositionAt(GameObject theObject, float theBuffer = 10.0f) {

        //retrieve position
        Vector3 worldPos = theObject.transform.position;

        //offset for object height
        worldPos.y += theObject.GetComponent<Renderer>().bounds.size.y / 2;

        //convert position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

        //retrieve canvas scale factor
        float scaleFactor = gameObject.transform.parent.GetComponent<Canvas>().scaleFactor;

        //offset for window height
        screenPos.y += scaleFactor * gameObject.GetComponent<RectTransform>().rect.size.y / 2;

        //add additional buffer between objects
        screenPos.y += scaleFactor * theBuffer;

        //set position in converted coordinates
        gameObject.transform.position = screenPos;
    }

    //start a new selection
    public IEnumerator StartSelection(int theNumChoices, Action<int> theSelectedChoice) {

        //whether dialogue is dismissed
        bool isDismissed = false;

        //while not dismissed
        while (isDismissed == false) {

            //wait for choice to be made
            yield return StartCoroutine(WaitForChoice(theNumChoices, value => theSelectedChoice(value)));

            //hide
            Hide();

            //toggle flag
            isDismissed = true;
        }
    }

    //wait for user to select among choices
    public IEnumerator WaitForChoice(int theNumChoices, Action<int> theSelectedChoice) {

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
                theSelectedChoice(currentChoice);
            }

            //move forward through options
            else if (theNumChoices > 1 && 
                (Input.GetKeyDown(KeyCode.W) || 
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.D) || 
                Input.GetKeyDown(KeyCode.RightArrow))
                ) {

                //update choice
                currentChoice--;

                //check bounds on current choice
                //exceeds min
                if (currentChoice < 0) {

                    //wrap to last choice
                    currentChoice = theNumChoices - 1;
                }

                //update indicator position
                Vector3 newPos = imgIndicator.transform.position;
                newPos.y = textObjects[currentChoice].transform.position.y;
                imgIndicator.transform.position = newPos;
            }

            //move backwards through options
            else if (theNumChoices > 1 && 
                (Input.GetKeyDown(KeyCode.S) || 
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.A) || 
                Input.GetKeyDown(KeyCode.LeftArrow))
                ) {
                
                //update choice
                currentChoice++;

                //check bounds on current choice
                //exceeds max
                if (currentChoice >= theNumChoices) {

                    //wrap to first choice
                    currentChoice = 0;
                }

                //update indicator position
                Vector3 newPos = imgIndicator.transform.position; 
                newPos.y = textObjects[currentChoice].transform.position.y;
                imgIndicator.transform.position = newPos;
            }

            //keep waiting
            yield return 0;
        }
    }

} //end class