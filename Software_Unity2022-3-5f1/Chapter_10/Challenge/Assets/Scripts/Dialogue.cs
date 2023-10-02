/*
Dialogue
Represents a dialogue box that presents text to the user.
Can be dismissed by time or through user input options.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class Dialogue : MonoBehaviour {

    //store text lines
    public string[] lines;

    //open a new dialogue box with given text 
    public void CreateDialogueWithText(string[] theText) {

        //clear lines
        ClearLines();

        //iterate through lines
        for (int i = 0; i < theText.Length; i++) {

            //set line text
            lines[i] = theText[i];
        }

        //update text
        UpdateText();
    }

    //open a new dialogue box with a single line of text 
    public void CreateDialogueWithText(string theText) {

        //clear lines
        ClearLines();

        //set first line
        lines[0] = theText;

        //update text
        UpdateText();
    }

    //update text in dialogue box
    public void UpdateDialogueWithText(string theText) {

        //whether text has been placed on line
        bool isTextPlaced = false;

        //start by trying to fill the first blank line
        //iterate through lines
        for (int i = 0; i < lines.Length; i++) {

            //check for blank
            if (String.IsNullOrEmpty(lines[i])) {

                //set text
                lines[i] = theText;

                //toggle flag
                isTextPlaced = true;

                //break
                break;
            }
        }

        //if no blank lines are available
        if (isTextPlaced == false) {

            //reduce index of existing lines by 1
            for (int j = 0; j < lines.Length - 1; j++) {

                //set current line equal to following line
                lines[j] = lines[j + 1];
            }

            //update last line with provided text
            lines[lines.Length - 1] = theText;
        }

        //update text
        UpdateText();
    }

    //update text
    private void UpdateText() {

        //retrieve text
        Text txt = gameObject.GetComponentInChildren<Text>();

        //clear text
        txt.text = "";

        //iterate through lines
        for (int i = 0; i < lines.Length; i++) {

            //add text
            txt.text += lines[i];

            //add line break
            txt.text += "\n";
        }
    }

    //clear text
    private void ClearLines() {

        //clear lines
        for (int i = 0; i < lines.Length; i++) {
            lines[i] = "";
        }
    }

    //show
    public void Show() {

        //set alpha to visible
        gameObject.GetComponent<CanvasGroup>().alpha = 0.8f;
    }

    //hide
    public void Hide() {

        //set alpha to invisible
        gameObject.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    //hide on key press
    public IEnumerator HideOnInput() {

        //whether dialogue is dismissed
        bool isDismissed = false;

        //while not dismissed
        while (isDismissed == false) {

            //check input
            if (Input.GetKeyUp(KeyCode.Space)) {

                //hide
                Hide();

                //toggle flag
                isDismissed = true;
            }

            //keep waiting
            yield return 0;
        }
    }

    //hide after delay
    public IEnumerator HideAfterDelay(float theDelay = 0.0f) {

        //handle delay
        yield return new WaitForSeconds(theDelay);

        //hide
        Hide();
    }

} //end class