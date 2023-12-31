﻿/*
Fade
Manages scene fade in/out transition.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class Fade : MonoBehaviour {

    //alpha limits
    public float minAlpha, maxAlpha;

    //duration, in seconds
    public float duration;

    //easing flags
    public bool isEaseIn, isEaseOut;

    //time when fade cycle started
    private float _startTime;

    //current alpha level
    private float _alpha;

    //whether currently fading in
    private bool isFadingIn;

    //whether fade is paused
    private bool isPaused;

    //texture used to create effect
    private Texture2D _texture;

    //screen rectangle
    private Rect _screenRect;

    //awake
    void Awake() {

        //set limits
        minAlpha = 0.0f;
        maxAlpha = 1.0f;
        duration = 1.0f;

        //set easing
        isEaseIn = true;
        isEaseOut = true;

        //set flags
        isPaused = false;
        isFadingIn = false;

        //set alpha
        _alpha = maxAlpha;

        //create fade texture
        _texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
        _texture.SetPixel(0, 0, Color.black);
        _texture.Apply();

        //create screen rect
        _screenRect = new Rect(0, 0, Screen.width, Screen.height);
    }

    //update
    void Update() {

        //if not paused
        if (isPaused == false) {

            //update alpha
            UpdateAlpha();
        }
    }

    //toggle fade
    public void ToggleFade() {

        //reverse flag
        isFadingIn = !isFadingIn;

        //unpause
        isPaused = false;

        //reset start time
        _startTime = Time.time;
    }

    //ease in
    public float EaseIn(float thePctTime, int thePower) {

        //percent complete
        float pct;

        //calculate percent complete
        pct = Mathf.Pow(thePctTime, thePower);

        //return
        return pct;
    }

    //ease out
    public float EaseOut(float thePctTime, int thePower) {

        //percent complete
        float pct;

        //calculate percent complete
        pct = 1.0f - Mathf.Pow(1.0f - thePctTime, thePower);

        //return
        return pct;
    }

    //update alpha
    private void UpdateAlpha() {

        //calculate cumulative duration
        float cumulativeDuration = Time.time - _startTime;

        //calculate percentage of duration complete
        float pct = Mathf.Clamp01(cumulativeDuration / duration);

        //check fade direction
        switch (isFadingIn) {

            //fading in
            case true:

                //if easing in
                if (isEaseIn == true) {

                    //update pct for easing
                    pct = EaseIn(pct, 2);
                }

                //alpha less than max
                if (_alpha < maxAlpha) {

                    //update alpha
                    _alpha = minAlpha + pct * (maxAlpha - minAlpha);
                }

                //otherwise
                else {

                    //set to max
                    _alpha = maxAlpha;

                    //pause
                    isPaused = true;
                }

                break;

            //fading out
            case false:

                //if easing out
                if (isEaseOut == true) {

                    //update pct for easing
                    pct = EaseIn(pct, 2);
                }

                //alpha greater than min
                if (_alpha > minAlpha) {

                    //update alpha
                    _alpha = minAlpha + (1.0f - pct) * (maxAlpha - minAlpha);
                }

                //otherwise
                else {

                    //set to min
                    _alpha = minAlpha;

                    //pause
                    isPaused = true;
                }

                break;

            //default
            default:
                break;
        }
    }

    //update texture
    void OnGUI() {

        //update color
        Color color = Color.black;
        color.a = _alpha;
        GUI.color = color;

        //place texture in front of all objects
        GUI.depth = -100;

        //draw texture over screen
        GUI.DrawTexture(_screenRect, _texture);
    }

} //end class