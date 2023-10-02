/*
Group
Manages the player's group of heroes.

Copyright 2015 John M. Quick.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Group : MonoBehaviour {

    //group members
    public List<GameObject> members;

    //follow distance between characters
    public float followDist;

    //init
    void Start() {

        //init collection
        members = new List<GameObject>();

        //add leader
        members.Add(gameObject);
    }

    //update member positions based on movement
    public void UpdatePos(float theSpeed, Vector2 theDir) {

        //update position for each hero in party
        //start at 1 to skip leader
        for (int i = 1; i < members.Count; i++) {

            //get hero object
            GameObject theMember = members[i];

            //store current position
            float currentX = theMember.transform.position.x;
            float currentY = theMember.transform.position.y;

            //get next position
            //the position of the character immediately in front in line
            float nextX = members[i - 1].transform.position.x;
            float nextY = members[i - 1].transform.position.y;

            //get distance between hero and character being followed
            float distToFollowedX = nextX - currentX;
            float distToFollowedY = nextY - currentY;

            //get distance between hero and leader of group
            float distToLeaderX = members[0].transform.position.x - currentX;
            float distToLeaderY = members[0].transform.position.y - currentY;

            //store new position
            float newX = currentX;
            float newY = currentY;

            //check whether follow distance is sufficient and update movement
            //move only if at least min distance away from both followed character and leader
            //x axis
            if (Mathf.Abs(distToFollowedX) >= followDist &&
                Mathf.Abs(distToLeaderX) >= followDist) {

                //positive
                if (distToFollowedX > 0) {

                    //update movement
                    newX += theSpeed * Time.deltaTime;
                }

                //negative
                else if (distToFollowedX < 0) {

                    //update movement
                    newX -= theSpeed * Time.deltaTime;
                }

                //manage snapping to next y position
                //if currently moving on x and not yet at next y position
                if (theDir.x != 0 && newY != nextY) {

                    //update movement
                    newY = nextY;
                }
            }

            //y axis
            if (Mathf.Abs(distToFollowedY) >= followDist &&
                Mathf.Abs(distToLeaderY) >= followDist) {

                //positive
                if (distToFollowedY > 0) {

                    //update movement
                    newY += theSpeed * Time.deltaTime;
                }

                //negative
                else if (distToFollowedY < 0) {

                    //update movement
                    newY -= theSpeed * Time.deltaTime;
                }

                //manage snapping to next x position
                //if currently moving on y and not yet at next x position
                if (theDir.y != 0 && newX != nextX) {

                    //update movement
                    newX = nextX;
                }
            }

            //update hero position
            theMember.transform.position = 
                new Vector3(
                    newX, 
                    newY, 
                    gameObject.transform.position.z + i
                );
        }
    }

} //end class