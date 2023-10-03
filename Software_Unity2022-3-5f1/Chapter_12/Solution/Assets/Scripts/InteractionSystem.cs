/*
Interaction System
Manages a turn-based interaction system.

Copyright 2015 John M. Quick
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System;

public class InteractionSystem : MonoBehaviour {

    //dialogue box in scene
    public Dialogue dialogue;

    //selection box in scene
    public Selection selection;

    //targeting indicator in scene
    public Targeting targeting;
	
	//interaction generator in scene
    public InteractionGenerator generator;

    //store interaction characters
    public List<GameObject> allCharacters;

    //whether interaction is complete
    private bool _isComplete;

    //define actions for characters
    /*
    Note: In the Star-Sun-Moon game, each character 
    takes a turn by challenging one opponent. Both 
    characters reveal their moves at the same time. 
    Star beats Sun, Sun beats Moon, and Moon beats Star. 
    Any character who loses a match is eliminated. The 
    winning team is the first to eliminate all opponents. 
    */
    public enum Actions {
        Star = 0,
        Sun = 1,
        Moon = 2
    }

    //define outcomes for interactions
    /*
    Note: Each interaction between two characters in 
    the Star-Sun-Moon game has three possible outcomes: 
    Win, Lose, or Draw.
    */
    public enum Outcomes {
        Win = 0,
        Lose = 1,
        Draw = 2
    }

    //singleton instance
    private static InteractionSystem _Instance;

    //singleton accessor
    //access InteractionSystem.Instance from other classes
    public static InteractionSystem Instance {

        //create instance via getter
        get {

            //if no instance
            if (_Instance == null) {

                //create game object
                GameObject InteractionSystemObj = new GameObject();
                InteractionSystemObj.name = "InteractionSystem";

                //create instance
                _Instance = InteractionSystemObj.AddComponent<InteractionSystem>();

                //retrieve scene objects
                _Instance.dialogue = GameObject.FindWithTag("Dialogue").GetComponent<Dialogue>();
                _Instance.selection = GameObject.FindWithTag("Selection").GetComponent<Selection>();
                _Instance.targeting = GameObject.FindWithTag("Targeting").GetComponent<Targeting>();

                //init
                _Instance.allCharacters = new List<GameObject>();

            } //end if

            //return the instance
            return _Instance;
        } 
    }
	
	//awake
	void Awake() {
		
		//prevent destruction
		DontDestroyOnLoad(this);
	}
    
    //start a new interaction between characters
    public void CreateInteraction(List<GameObject> theCharacters) {
		
		//show canvas
		GameObject.FindWithTag("Canvas").GetComponent<CanvasGroup>().alpha = 1.0f;
		
		//create dialogue
        dialogue.CreateDialogueWithText(
            "The heroes have been challenged to a game of Star-Sun-Moon!"
            );

        //show dialogue
        dialogue.Show();

        //toggle flag
        _isComplete = false;

        //reset characters
        allCharacters.Clear();

        //create container
        GameObject container = new GameObject();
        container.name = "Characters";

        //for all characters
        foreach (GameObject aChar in theCharacters) {

            //clone
            GameObject newChar = Instantiate<GameObject>(aChar);

            //parent
            newChar.transform.parent = container.transform;

            //activate
            CharacterData charData = newChar.GetComponent<CharacterData>();
            charData.isActive = true;

            //add to collection
            allCharacters.Add(newChar);
        }

        //retrieve teams
        List<GameObject> allHeroes = FindCharacters(true, true);
        List<GameObject> allOpponents = FindCharacters(true, false);

        //position teams
        //heroes
        for (int i = 0; i < allHeroes.Count; i++) {

            //centered at bottom of screen
            Vector3 pos = Vector3.zero;
            float w = allHeroes[i].GetComponent<Renderer>().bounds.size.x;
            float h = allHeroes[i].GetComponent<Renderer>().bounds.size.y;
            pos.x = w / 2 + i * w - (w * (float)allHeroes.Count / 2);
            pos.y = -Camera.main.orthographicSize + 3 * h;
            allHeroes[i].transform.position = pos;

            //animation
            allHeroes[i].GetComponent<CharacterAnimator>().SetUpAnimationWithName("ActUp");
        }

        //opponents
        for (int j = 0; j < allOpponents.Count; j++) {

            //centered at top of screen
            Vector3 pos = Vector3.zero;
            float w = allOpponents[j].GetComponent<Renderer>().bounds.size.x;
            float h = allOpponents[j].GetComponent<Renderer>().bounds.size.y;
            pos.x = w / 2 + j * w - (w * (float)allOpponents.Count / 2);
            pos.y = Camera.main.orthographicSize - 3 * h;
            allOpponents[j].transform.position = pos;

            //animation
            allOpponents[j].GetComponent<CharacterAnimator>().SetUpAnimationWithName("ActDown");
        }
    }

    //start an interaction between characters
    public IEnumerator StartInteraction() {

        //delay
        yield return new WaitForSeconds(2.0f);

        //store current character index
        int currentChar = 0;

        //while interaction is not complete
        while (_isComplete == false) {

            //retrieve character data
            CharacterData charData = allCharacters[currentChar].GetComponent<CharacterData>();
                
            //if character is active
            if (charData.isActive == true) {
                    
                //start turn
                yield return StartCoroutine(StartTurn(allCharacters[currentChar]));

                //delay
                yield return new WaitForSeconds(2.0f);
                    
                //check interaction
                CheckInteraction();
            }

            //increment index
            currentChar++;

            //check bounds on index
            if (currentChar >= allCharacters.Count) {

                //reset index
                currentChar = 0;
            }
        }
    }

    //start a new turn for a character
    public IEnumerator StartTurn(GameObject theObject) {

        //retrieve character data
        CharacterData charData = theObject.GetComponent<CharacterData>();

        //animation
        //retrieve character animator
        CharacterAnimator charAnimator = theObject.GetComponent<CharacterAnimator>();
		
		//update dialogue
        dialogue.UpdateDialogueWithText(
            charData.characterName +
            "'s turn!"
            );

        //if hero
        if (charData.isHero == true) {

            //select action
            yield return StartCoroutine(TurnAction(theObject));

            //select target
            yield return StartCoroutine(TurnTarget(theObject));

            //generate random opponent action
            GameObject theOpponent = FindCharacters(true, false)[charData.currentTarget];
            theOpponent.GetComponent<CharacterData>().currentAction = UnityEngine.Random.Range(0, Enum.GetValues(typeof(Actions)).Length);

            //animation
            //update character animation
            charAnimator.SetUpAnimationWithName("ActUp");
            charAnimator.currentAnim.GoToFrameAndStop(1);
			generator.ShowActionFor(theObject, charData.currentAction);

            //update opponent animation
            CharacterAnimator opponentAnimator = theOpponent.GetComponent<CharacterAnimator>();
            opponentAnimator.SetUpAnimationWithName("ActDown");
            opponentAnimator.currentAnim.GoToFrameAndStop(1);
			generator.ShowActionFor(theOpponent, theOpponent.GetComponent<CharacterData>().currentAction);

            //delay
            yield return new WaitForSeconds(1.0f);

            //animation
            //update character animation
            charAnimator.currentAnim.GoToFrameAndStop(0);

            //update opponent animation
            opponentAnimator.currentAnim.GoToFrameAndStop(0);
			
			//hide actions
            generator.HideActions();

            //apply action
            ApplyActionToTarget(theObject, theOpponent);
        }

        //if opponent
        else if (charData.isHero == false) {

            //generate random action
            charData.currentAction = UnityEngine.Random.Range(0, Enum.GetValues(typeof(Actions)).Length);

            //find potential targets
            List<GameObject> targets = FindCharacters(true, true);
            
            //generate random target
            charData.currentTarget = UnityEngine.Random.Range(0, targets.Count);

            //allow player to select defense action
            GameObject theHero = FindCharacters(true, true)[charData.currentTarget];
            yield return StartCoroutine(TurnAction(theHero));

            //animation
            //update character animation
            charAnimator.SetUpAnimationWithName("ActDown");
            charAnimator.currentAnim.GoToFrameAndStop(1);
            generator.ShowActionFor(theObject, charData.currentAction);

            //update opponent animation
            CharacterAnimator opponentAnimator = theHero.GetComponent<CharacterAnimator>();
            opponentAnimator.SetUpAnimationWithName("ActUp");
            opponentAnimator.currentAnim.GoToFrameAndStop(1);
            generator.ShowActionFor(theHero, theHero.GetComponent<CharacterData>().currentAction);

            //delay
            yield return new WaitForSeconds(1.0f);

            //animation
            //update character animation
            charAnimator.currentAnim.GoToFrameAndStop(0);

            //update opponent animation
            opponentAnimator.currentAnim.GoToFrameAndStop(0);
			
			//hide actions
            generator.HideActions();

            //apply action
            ApplyActionToTarget(theObject, theHero);
        }
    }

    //determine action for character's turn
    public IEnumerator TurnAction(GameObject theObject) {

        //retrieve character data
        CharacterData charData = theObject.GetComponent<CharacterData>();

        //update dialogue
        dialogue.UpdateDialogueWithText(
            charData.characterName +
            ": Choose your action."
            );

        //create selection
        List<string> txtSelection = new List<string>();
        txtSelection.AddRange(Enum.GetNames(typeof(Actions)));
        selection.CreateSelection(txtSelection);

        //position selection
        selection.PositionAt(theObject);

        //show selection
        selection.Show();

        //select action
        yield return StartCoroutine(
            selection.StartSelection(
            txtSelection.Count,
            value => charData.currentAction = value)
            );
    }

    //determine target for character's turn
    public IEnumerator TurnTarget(GameObject theObject) {

        //retrieve character data
        CharacterData charData = theObject.GetComponent<CharacterData>();

        //update dialogue
        dialogue.UpdateDialogueWithText(
            charData.characterName + 
            ": Choose your target."
            );

        //find potential targets
        List<GameObject> targets = FindCharacters(true, false);

        //select target
        yield return StartCoroutine(
            targeting.StartTargeting(
            targets.Count, 
            value => charData.currentTarget = value, 
            targets)
            );
    }

    //apply action to target
    public void ApplyActionToTarget(GameObject theActor, GameObject theTarget) {

        //retrieve character data
        CharacterData actorData = theActor.GetComponent<CharacterData>();
        CharacterData targetData = theTarget.GetComponent<CharacterData>();

        //determine result
        int actorOutcome = CompareActions(actorData.currentAction, targetData.currentAction);

        //create fade color
        Color fadeColor = Color.white;
        fadeColor.a = 0.25f;

        //check result
        switch (actorOutcome) {

            //win
            case (int)Outcomes.Win:

                //deactivate target
                targetData.isActive = false;

                //set target color
                theTarget.GetComponent<SpriteRenderer>().color = fadeColor;

                //update dialogue
                dialogue.UpdateDialogueWithText(
                        actorData.characterName + "'s " + 
                        Enum.GetName(typeof(Actions), actorData.currentAction) + 
                        " beats " + 
                        targetData.characterName + "'s " + 
                        Enum.GetName(typeof(Actions), targetData.currentAction) + 
                        "!"
                        );

                //audio
                //if player wins
				if (actorData.isHero == true) {

					//play sound effect
					AudioManager.Instance.PlayClipFromSource(
						AudioManager.Instance.sfxPositive, 
						AudioManager.Instance.sfxSource
						);
				}

				//if player loses
				else if (actorData.isHero == false) {
					
					//play sound effect
					AudioManager.Instance.PlayClipFromSource(
						AudioManager.Instance.sfxNegative, 
						AudioManager.Instance.sfxSource
						);
				}

                break;

            //lose
            case (int)Outcomes.Lose:

                //deactivate actor
                actorData.isActive = false;

                //set actor color
                theActor.GetComponent<SpriteRenderer>().color = fadeColor;

                //update dialogue
                dialogue.UpdateDialogueWithText(
                        targetData.characterName + "'s " + 
                        Enum.GetName(typeof(Actions), targetData.currentAction) + 
                        " beats " + 
                        actorData.characterName + "'s " + 
                        Enum.GetName(typeof(Actions), actorData.currentAction) + 
                        "!"
                        );

                //audio
                //if player loses
				if (actorData.isHero == true) {

					//play sound effect
					AudioManager.Instance.PlayClipFromSource(
						AudioManager.Instance.sfxNegative, 
						AudioManager.Instance.sfxSource
						);
				}

				//if player wins
				else if (actorData.isHero == false) {
					
					//play sound effect
					AudioManager.Instance.PlayClipFromSource(
						AudioManager.Instance.sfxPositive, 
						AudioManager.Instance.sfxSource
						);
				}

                break;

            //draw
            case (int)Outcomes.Draw:

                //update dialogue
                dialogue.UpdateDialogueWithText(
                        actorData.characterName + "'s " + 
                        Enum.GetName(typeof(Actions), actorData.currentAction) + 
                        " ties " + 
                        targetData.characterName + "'s " + 
                        Enum.GetName(typeof(Actions), targetData.currentAction) + 
                        "."
                        );

                //audio
				AudioManager.Instance.PlayClipFromSource(
                        AudioManager.Instance.sfxNegative, 
                        AudioManager.Instance.sfxSource
                        );
						
                break;

            //default
            default:
                Debug.Log("[InteractionSystem] Error: Outcome not recognized");
                break;
        }
    }

    //compare actions and determine winner
    public int CompareActions(int theFirstAction, int theSecondAction) {

        //store result
        int outcome = (int)Outcomes.Lose;

        //draw
        if (theFirstAction == theSecondAction) {

            //return
            return (int)Outcomes.Draw;
        }

        //possible win states
        else if (
            
            //star beats sun
            (theFirstAction == (int)Actions.Star && 
            theSecondAction == (int)Actions.Sun) ||

            //sun beats moon
            (theFirstAction == (int)Actions.Sun && 
            theSecondAction == (int)Actions.Moon) ||

            //moon beats star
            (theFirstAction == (int)Actions.Moon && 
            theSecondAction == (int)Actions.Star)

            ) {

            //return
            return (int)Outcomes.Win;
        }

        //return
        return outcome;
    }

    //check for end of interaction
    public void CheckInteraction() {

        //check active characters
        List<GameObject> activeOpponents = FindCharacters(true, false);
        List<GameObject> activeHeroes = FindCharacters(true, true);

        //if no opponents remain
        if (activeOpponents.Count <= 0) {

            //heroes win
            EndInteraction(true);
        }

        //if no heroes remain
        else if (activeHeroes.Count <= 0) {

            //heroes lose
            EndInteraction(false);
        }
    }

    //end interaction
    public void EndInteraction(bool isHeroWin) {

        //toggle flag
        _isComplete = true;

        //heroes win
        if (isHeroWin == true) {

            //retrieve dungeon data
            string dungeonName = SceneManager.GetActiveScene().name.Replace("Interaction","");

            //update save data
            DataManager.Instance.currentSave.dungeonData[dungeonName] = true;

            //update dialogue
            dialogue.UpdateDialogueWithText("Heroes win!");

            //animation
            //retrieve characters
            List<GameObject> allHeroes = FindCharacters(true, true);
            allHeroes.AddRange(FindCharacters(false, true));

            //iterate through characters
            foreach (GameObject aHero in allHeroes) {

                //make visible
                aHero.GetComponent<SpriteRenderer>().color = Color.white;

                //retrieve animator
                CharacterAnimator animator = aHero.GetComponent<CharacterAnimator>();

                //set animation
                animator.SetUpAnimationWithName("ActDown", 2.0f);

                //play
                animator.currentAnim.Play();
            }

            //audio
            //stop music
			AudioManager.Instance.StopSource(
				AudioManager.Instance.bgmSource
				);

			//play sound effect
			AudioManager.Instance.PlayClipFromSource(
				AudioManager.Instance.sfxWin, 
				AudioManager.Instance.sfxSource
				);
        }

        //opponents win
        else if (isHeroWin == false) {

            //update dialogue
            dialogue.UpdateDialogueWithText("Dragons win!");

            //animation
            //retrieve characters
            List<GameObject> allOpponents = FindCharacters(true, false);
            allOpponents.AddRange(FindCharacters(false, false));

            //iterate through characters
            foreach (GameObject anOpponent in allOpponents) {

                //make visible
                anOpponent.GetComponent<SpriteRenderer>().color = Color.white;

                //retrieve animator
                CharacterAnimator animator = anOpponent.GetComponent<CharacterAnimator>();

                //set animation
                animator.SetUpAnimationWithName("ActDown", 2.0f);

                //play
                animator.currentAnim.Play();
            }

            //audio
            //stop music
			AudioManager.Instance.StopSource(
				AudioManager.Instance.bgmSource
				);

			//play sound effect
			AudioManager.Instance.PlayClipFromSource(
				AudioManager.Instance.sfxLoss, 
				AudioManager.Instance.sfxSource
				);
        }

        //switch scene
        StartCoroutine(StateManager.Instance.SwitchSceneTo("Map", 8.0f));
    }
        
    //find characters
    public List<GameObject> FindCharacters(bool theIsActive, bool theIsHero) {

        //store active characters
        List<GameObject> chars = new List<GameObject>();

        //check all characters
        for (int i = 0; i < allCharacters.Count; i++) {

            //retrieve character data
            CharacterData charData = allCharacters[i].GetComponent<CharacterData>();

            //whether character is active
            bool isActive = charData.isActive;

            //whether current character is hero
            bool isHero = charData.isHero;

            //if matching active and matching type
            if (isActive == theIsActive && isHero == theIsHero) {

                //add character
                chars.Add(allCharacters[i]);
            }
        }

        //return
        return chars;
    }

} //end class