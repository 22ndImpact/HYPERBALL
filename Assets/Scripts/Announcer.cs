using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Announcer : MonoBehaviour
{
    public TextMesh PlayerOneTopLine;
    public TextMesh PlayerOneBottomLine;

    public TextMesh PlayerTwoTopLine;
    public TextMesh PlayerTwoBottomLine;

    public GameObject playerOne;
    public GameObject playerTwo;

    public float ScrollInTime;
    public float ScrollOutTime;

    public Vector3 ScrollInOffset;
    public Vector3 ScrollOutOffset;

    public AnimationCurve ScrollInCurve;
    public AnimationCurve ScrollOutCurve;

    public AnimationCurve ScrollInAlphaCurve;
    public AnimationCurve ScrollOutAlphaCurve;

    public Vector3 PlayerOneStartingPosition;
    public Vector3 PlayerTwoStartingPosition;

    public float DurationAttractMode;
    public float DurationGameReady;
    public float DurationPlayerScore;
    public float DurationGameOver;

    Color PlayerOneTopLineStartingColor;
    Color PlayerOneBottomLineStartingColor;
    Color PlayerTwoTopLineStartingColor;
    Color PlayerTwoBottomLineStartingColor;

    List<int> TextQueue = new List<int>();
    bool On;
    bool skipToFadeOut = false;

    private void Awake()
    {
        //Remembers where the player announcers started
        //Debug.Log("Player 1 announcer position: " + playerOne.transform.position);
        PlayerOneStartingPosition = playerOne.transform.position;

        //Debug.Log("Player 2 announcer position: " + playerTwo.transform.position);
        PlayerTwoStartingPosition = playerTwo.transform.position;


        //Remembers the starting colors of the text
        PlayerOneTopLineStartingColor = PlayerOneTopLine.color;
        PlayerOneBottomLineStartingColor = PlayerOneBottomLine.color;
        PlayerTwoTopLineStartingColor = PlayerTwoTopLine.color;
        PlayerTwoBottomLineStartingColor = PlayerTwoBottomLine.color;

        //Hides the text at the start of the game
        TurnOff();
    }

    // Use this for initialization
    void Start ()
    {
        
	}
	
	// Update is called once per frame
	void Update ()
    {
	}

    public void TurnOn()
    {
        playerOne.SetActive(true);
        playerTwo.SetActive(true);
        On = true;
    }

    public void TurnOff()
    {
        playerOne.SetActive(false);
        playerTwo.SetActive(false);
        On = false;
    }

    public void StartAttractMode()
    {
        //If the announcer isnt announcing anything
        if(On == false)
        {
            //Set up all the announcer texts
            string PlayerOneTop = "HOLD PADDLE TO HIT HARDER";
            string PlayerOneBottom = "CHARGE A PADDLE TO START";

            string PlayerTwoTop = "HOLD PADDLE TO HIT HARDER";
            string PlayerTwoBottom = "CHARGE A PADDLE TO START";

            //Start the text display sequence
            StartCoroutine(DisplayText(DurationAttractMode, PlayerOneTop, PlayerOneBottom, PlayerTwoTop, PlayerTwoBottom));
        }
        //otherwise
        else
        {
            //Sets the flag to let the current coroutine know to skip right to the fade out
            skipToFadeOut = true;

            //Add its index to the queue
            TextQueue.Add(0);
        }

    }

    public void StartGameReady()
    {
        //If the announcer isnt announcing anything
        if (On == false)
        {
            //Set up all the announcer texts
            string PlayerOneTop = "GAME READY";
            string PlayerOneBottom = "";

            string PlayerTwoTop = "GAME READY";
            string PlayerTwoBottom = "";

            //Start the text display sequence
            StartCoroutine(DisplayText(DurationGameReady, PlayerOneTop, PlayerOneBottom, PlayerTwoTop, PlayerTwoBottom));
        }
        //otherwise
        else
        {
            //Sets the flag to let the current coroutine know to skip right to the fade out
            skipToFadeOut = true;
            //Add its index to the queue
            TextQueue.Add(1);
        }

    }

    public void StartPlayerOneScore()
    {
        //If the announcer isnt announcing anything
        if (On == false)
        {
            //Set up all the announcer texts
            string PlayerOneTop = "YOU SCORED A POINT";
            string PlayerOneBottom = GameDirector.inst.Player1Score + " - " + GameDirector.inst.Player2Score;

            string PlayerTwoTop = "ENEMY SCORED A POINT";
            string PlayerTwoBottom = GameDirector.inst.Player2Score + " - " + GameDirector.inst.Player1Score;

            //Start the text display sequence
            StartCoroutine(DisplayText(DurationPlayerScore, PlayerOneTop, PlayerOneBottom, PlayerTwoTop, PlayerTwoBottom));
        }
        //otherwise
        else
        {
            //Sets the flag to let the current coroutine know to skip right to the fade out
            skipToFadeOut = true;
            //Add its index to the queue
            TextQueue.Add(2);
        }

    }

    public void StartPlayerTwoScore()
    {
        //If the announcer isnt announcing anything
        if (On == false)
        {
            //Set up all the announcer texts
            string PlayerOneTop = "ENEMY SCORED A POINT";
            string PlayerOneBottom = GameDirector.inst.Player1Score + " - " + GameDirector.inst.Player2Score;

            string PlayerTwoTop = "YOU SCORED A POINT";
            string PlayerTwoBottom = GameDirector.inst.Player2Score + " - " + GameDirector.inst.Player1Score;

            //Start the text display sequence
            StartCoroutine(DisplayText(DurationPlayerScore, PlayerOneTop, PlayerOneBottom, PlayerTwoTop, PlayerTwoBottom));
        }
        //otherwise
        else
        {
            //Sets the flag to let the current coroutine know to skip right to the fade out
            skipToFadeOut = true;
            //Add its index to the queue
            TextQueue.Add(3);
        }

    }

    public void StartPlayerOneWins()
    {
        //If the announcer isnt announcing anything
        if (On == false)
        {
            //Set up all the announcer texts
            string PlayerOneTop = "VICTORY";
            string PlayerOneBottom = GameDirector.inst.Player1Score + " - " + GameDirector.inst.Player2Score;

            string PlayerTwoTop = "DEFEAT";
            string PlayerTwoBottom = GameDirector.inst.Player2Score + " - " + GameDirector.inst.Player1Score;

            //Start the text display sequence
            StartCoroutine(DisplayText(DurationGameOver, PlayerOneTop, PlayerOneBottom, PlayerTwoTop, PlayerTwoBottom));
        }
        //otherwise
        else
        {
            //Sets the flag to let the current coroutine know to skip right to the fade out
            skipToFadeOut = true;
            //Add its index to the queue
            TextQueue.Add(4);
        }

    }

    public void StartPlayerTwoWins()
    {
        //If the announcer isnt announcing anything
        if (On == false)
        {
            //Set up all the announcer texts
            string PlayerOneTop = "DEFEAT";
            string PlayerOneBottom = GameDirector.inst.Player1Score + " - " + GameDirector.inst.Player2Score;

            string PlayerTwoTop = "VICTORY";
            string PlayerTwoBottom = GameDirector.inst.Player2Score + " - " + GameDirector.inst.Player1Score;

            //Start the text display sequence
            StartCoroutine(DisplayText(DurationGameOver, PlayerOneTop, PlayerOneBottom, PlayerTwoTop, PlayerTwoBottom));
        }
        //otherwise
        else
        {
            //Sets the flag to let the current coroutine know to skip right to the fade out
            skipToFadeOut = true;
            //Add its index to the queue
            TextQueue.Add(5);
        }

    }

    IEnumerator DisplayText(float _duration, string _P1T, string _P1B, string _P2T, string _P2B)
    {
        //Debug.Log("Turning on Text");

        //Turns the text on
        TurnOn();

        //Sets the text
        PlayerOneTopLine.text = _P1T;
        PlayerOneBottomLine.text = _P1B;
        PlayerTwoTopLine.text = _P2T;
        PlayerTwoBottomLine.text = _P2B;

        //Invisible Color
        Color invisibleColor = Color.white;
        invisibleColor.a = 0;


        //Scrolls in
        for (float i = 0; i < ScrollInTime; i += Time.unscaledDeltaTime)
        {
            //exit if the skip flag is checked
            if (skipToFadeOut == true)
            {
                break;
            }

            playerOne.transform.position = Vector3.LerpUnclamped(PlayerOneStartingPosition + ScrollInOffset, PlayerOneStartingPosition, ScrollInCurve.Evaluate(i / ScrollInTime));
            PlayerOneTopLine.color = Color.LerpUnclamped(invisibleColor, PlayerOneTopLineStartingColor, ScrollInAlphaCurve.Evaluate(i / ScrollInTime));
            PlayerOneBottomLine.color = Color.LerpUnclamped(invisibleColor, PlayerOneBottomLineStartingColor, ScrollInAlphaCurve.Evaluate(i / ScrollInTime));

            playerTwo.transform.position = Vector3.LerpUnclamped(PlayerTwoStartingPosition - ScrollInOffset, PlayerTwoStartingPosition, ScrollInCurve.Evaluate(i / ScrollInTime));
            PlayerTwoTopLine.color = Color.LerpUnclamped(invisibleColor, PlayerTwoTopLineStartingColor, ScrollInAlphaCurve.Evaluate(i / ScrollInTime));
            PlayerTwoBottomLine.color = Color.LerpUnclamped(invisibleColor, PlayerTwoBottomLineStartingColor, ScrollInAlphaCurve.Evaluate(i / ScrollInTime));
            yield return null;
        }

        //Waits for duration
        for (float i = 0; i < _duration; i += Time.unscaledDeltaTime)
        {            //exit if the skip flag is checked
            if (skipToFadeOut == true)
            {
                break;
            }

            yield return null;
        }

        //Scrolls out
        for (float i = 0; i < ScrollOutTime; i += Time.unscaledDeltaTime)
        {
            playerOne.transform.position = Vector3.LerpUnclamped(PlayerOneStartingPosition + ScrollOutOffset, PlayerOneStartingPosition, ScrollOutCurve.Evaluate(1f - (i / ScrollOutTime)));
            PlayerOneTopLine.color = Color.LerpUnclamped(invisibleColor, PlayerOneTopLineStartingColor, ScrollOutAlphaCurve.Evaluate(1f - (i / ScrollOutTime)));
            PlayerOneBottomLine.color = Color.LerpUnclamped(invisibleColor, PlayerOneBottomLineStartingColor, ScrollOutAlphaCurve.Evaluate(1f - (i / ScrollOutTime)));

            playerTwo.transform.position = Vector3.LerpUnclamped(PlayerTwoStartingPosition - ScrollOutOffset, PlayerTwoStartingPosition, ScrollOutCurve.Evaluate(1f - (i / ScrollOutTime)));
            PlayerTwoTopLine.color = Color.LerpUnclamped(invisibleColor, PlayerTwoTopLineStartingColor, ScrollOutAlphaCurve.Evaluate(1f - (i / ScrollOutTime)));
            PlayerTwoBottomLine.color = Color.LerpUnclamped(invisibleColor, PlayerTwoBottomLineStartingColor, ScrollOutAlphaCurve.Evaluate(1f - (i / ScrollOutTime)));
            yield return null;
        }

        yield return null;
        //Turns off the text
        TurnOff();

        //Sets the skip flag back to false
        skipToFadeOut = false;

        //Check if the queue is empty. 
        if(TextQueue.Count > 0)
        {
            //Stores the value
            int TextValue = TextQueue[0];

            //Removes the value from the list
            TextQueue.RemoveAt(0);

            //Plays the corrisponding animation
            switch (TextValue)
            {
                //Attract Mode
                case 0:
                    StartAttractMode();
                    break;
                //Game Ready
                case 1:
                    StartGameReady();
                    break;
                //Player One Scores
                case 2:
                    StartPlayerOneScore();
                    break;
                //Player Two Scores
                case 3:
                    StartPlayerTwoScore();
                    break;
                //Player One Wins
                case 4:
                    StartPlayerOneWins();
                    break;
                //Player Two Wins
                case 5:
                    StartPlayerTwoWins();
                    break;
            }
        }
        
        
    }
}
