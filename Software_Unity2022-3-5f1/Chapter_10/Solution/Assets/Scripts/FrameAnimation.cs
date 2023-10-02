/*
FrameAnimation
Defines a single animation cycle composed of frames.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using System.Collections;

public class FrameAnimation : MonoBehaviour {

    //animation name
    public string animName;

    //animation frames
    private Sprite[] _frames;

    //frames to display per second
    private float _fps;

    //whether the animation loops
    private bool _isLoop;

    //time at which current frame started
    private float _startTime;

    //current frame index
    private int _currentFrame;

    //whether animation is playing
    private bool _isPlaying;

    //custom init
    public void InitFrameAnimation(string theName, Sprite[] theFrames = null, float theFPS = 1.0f, bool theIsLoop = true) {

        //init
        animName = theName;
        _frames = theFrames;
        _fps = theFPS;
        _isLoop = theIsLoop;
        _currentFrame = 0;
        _isPlaying = false;

        //update sprite
        if (_frames != null) {
            UpdateSprite(_frames[_currentFrame]);
        }
    }

    //update
    void Update() {

        //if playing
        if (_isPlaying == true) {

            //check frame
            CheckFrame();
        }
    }

    //check whether frame needs to change
    private void CheckFrame() {

        //duration of current frame
        float duration = Time.time - _startTime;

        //duration limit for a single frame
        float limit = 1.0f / _fps;

        //if limit exceeded
        if (duration >= limit) {

            //update frame
            UpdateFrame();
        }
    }

    //update the current frame
    private void UpdateFrame() {

        //update frame index
        _currentFrame = ++_currentFrame % _frames.Length;

        //if index exceeds bounds and looping
        if (_currentFrame >= _frames.Length && _isLoop == true) {

            //return to start
            GoToFrameAndPlay(0);
        }

        //if index exceeds bounds and not looping
        else if (_currentFrame >= _frames.Length && _isLoop == false) {

            //stop
            Stop();
        }

        //otherwise, use valid frame
        else {

            //update sprite
            UpdateSprite(_frames[_currentFrame]);
        }
    }

    //update the current sprite
    private void UpdateSprite(Sprite theSprite) {

        //update sprite
        gameObject.GetComponent<SpriteRenderer>().sprite = theSprite;

        //update start time
        _startTime = Time.time;
    }

    //play
    public void Play() {

        //if not already playing
        if (_isPlaying == false) {

            //toggle flag
            _isPlaying = true;

            //reset start time
            _startTime = Time.time;
        }
    }

    //pause
    public void Stop() {

        //toggle flag
        _isPlaying = false;
    }

    //go to a specific animation frame and play
    public void GoToFrameAndPlay(int theFrameIndex) {
		
		//update frame index
        _currentFrame = theFrameIndex;

        //update sprite
        UpdateSprite(_frames[_currentFrame]);

        //play
		Play();
    }

    //go to a specific animation frame and stop
    public void GoToFrameAndStop(int theFrameIndex) {

        //stop
        Stop();

        //update frame index
        _currentFrame = theFrameIndex;

        //update sprite
        UpdateSprite(_frames[_currentFrame]);
    }

} //end class