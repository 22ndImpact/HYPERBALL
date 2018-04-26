using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameDirector : MonoBehaviour
{
    public int ScoreToWin;
    public int Player1Score;
    public int Player2Score;
    public float BallSpinSpeed;
    public float BallSpinTime;
    public float BallSpinTimeExtraMin;
    public float BallSpinTimeExtraMax;
    public float PossibleLaunchAngle;
    public static GameDirector inst;
    public enum GameState
    {
        PreGame,
        GameStarting,
        InGame,
        RoundOver,
        GameOver
    }
    public GameState gameState;
    bool playersReadyToStart;
    float SpinSleepTimer;
    public float FieldExtents;

    float SleepTimer;
    public bool GameSleeping;
    public float SleepDisplayGameReady;
    public float SleepBeforeBallShoots;
    public float SleepDisplayScores;
    public float SleepDisplayVictory;

    public Ball ball;
    public GameObject Spinner;
    public Player[] Players;
    public Announcer announcer;

    

    bool PlayersHaveCharged = false;

    private void Awake()
    {
        //Instantiate Singlton
        inst = this;
    }

    void Start()
    {
        //Set initial game state
        gameState = GameState.PreGame;

        //Runs the reset game function to "start a new game"
        ResetGame();
    }

    void Update()
    {
        UpdateStates();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(0);
        }
    }

    void UpdateStates()
    {
        switch (gameState)
        {
            case GameState.PreGame:
                SplashScreenLoop();
                break;
            case GameState.GameStarting:
                break;
            case GameState.InGame:
                UpdateInGame();
                break;
            case GameState.RoundOver:
                UpdateRoundOver();
                break;
            case GameState.GameOver:
                UpdateGameOver();
                break;
        }
    }

    void SplashScreenLoop()
    {
        //Do a bunch of ui stuff

        #region Check to see when players are ready
        //Only check if players are not ready
        if (playersReadyToStart == false)
        {
            //Loop through player 1's cannons
            foreach (Paddle paddle1 in Players[0].Paddles)
            {
                //If any are found to be charging
                if (paddle1.paddleState == Paddle.PaddleState.Charging)
                {
                    //Loop through player 2's cannons
                    foreach (Paddle paddle2 in Players[1].Paddles)
                    {
                        //If any of the are charging
                        if (paddle2.paddleState == Paddle.PaddleState.Charging)
                        {
                            //Set players to ready and exit loop
                            playersReadyToStart = true;
                            //Debug.Log("Players Ready");
                            break;
                        }
                    }
                    break;
                }
            }
        }
        #endregion

        #region Check to see when players start the game
        //Only check when players are ready
        if (playersReadyToStart == true)
        {
            //If this is the first frame they have been charged, play the announcer clip
            if(PlayersHaveCharged == false)
            {
                PlayersHaveCharged = true;
                announcer.StartGameReady();
            }

            //Create a variable to track any inputs
            bool Player1Input = false;
            bool Player2Input = false;

            //Loop through player 1's cannons
            foreach (Paddle paddle in Players[0].Paddles)
            {
                //If any are found to be charging
                if (paddle.paddleState == Paddle.PaddleState.Charging)
                {
                    //Set player 1's input to true
                    Player1Input = true;
                    //Leave the loop
                    break;
                }
            }

            //If on input was dected for player 1
            if (Player1Input == false)
            {
                //Loop through player 2's cannons
                foreach (Paddle paddle in Players[1].Paddles)
                {
                    //If any are found to be charging
                    if (paddle.paddleState == Paddle.PaddleState.Charging)
                    {
                        //Set player 1's input to true
                        Player2Input = true;
                        //Leave the loop
                        break;
                    }
                }
            }

            //If both player 1 and 2 have no input trigger the next game state
            if (Player1Input == false && Player2Input == false)
            {
                //Start the game
                StartGame();
                //Reset the other variables store to determine the game start input
                playersReadyToStart = false;
            }

        }
        #endregion
    }

    void StartGame()
    {
        //Set the game state to Game Starting
        gameState = GameState.GameStarting;

        //Spin and Shoot the ball
        StartCoroutine(SpinBall(BallSpinTime + Random.Range(BallSpinTimeExtraMin, BallSpinTimeExtraMax)));
    }

    IEnumerator SpinBall(float SpinTime)
    {
        //Debug.Log("Start Spin");

        //Make The spinner visible
        Spinner.GetComponent<SpriteRenderer>().enabled = true;

        float SpinTimer = SpinTime;
        bool Spinning = true;

        while (Spinning == true)
        {
            //Spin the ball
            this.transform.Rotate(0, 0, -1 * BallSpinSpeed * Time.deltaTime);

            //Store the current Z rotation
            float CurrentRotation = (this.transform.rotation).eulerAngles.z;

            //It the timer is running and above 0
            if (SpinTimer > 0)
            {
                //Reduce timer, keeping the ball spinning
                SpinTimer -= Time.deltaTime;
            }
            //If the timer is <= 0 and the spinner is within range, break the coroutine.
            else if ((CurrentRotation > 60 - PossibleLaunchAngle && CurrentRotation < 60 + PossibleLaunchAngle) ||
                        (CurrentRotation > 120 - PossibleLaunchAngle && CurrentRotation < 120 + PossibleLaunchAngle) ||
                        (CurrentRotation > 240 - PossibleLaunchAngle && CurrentRotation < 240 + PossibleLaunchAngle) ||
                        (CurrentRotation > 300 - PossibleLaunchAngle && CurrentRotation < 300 + PossibleLaunchAngle))
            {
                //Debug.Log("Stop the spinning");
                Spinning = false;
            }
            yield return null;
        }

        //Pause for effect
        SpinSleepTimer = SleepBeforeBallShoots;
        while (SpinSleepTimer > 0)
        {
            SpinSleepTimer -= Time.deltaTime;
            yield return null;
        }

        //Make The spinner invisible
        Spinner.GetComponent<SpriteRenderer>().enabled = false;

        //Shoot the ball
        LaunchBall();
    }

    void LaunchBall()
    {
        //Debug.Log("LaunchBall");

        Spinner.GetComponent<SpriteRenderer>().enabled = false;

        //ball.rb.velocity = transform.up * ball.InitialSpeed;
        ball.ChangeVelocity(transform.up, Ball.BallSpeeds.InitialSpeed);

        gameState = GameState.InGame;
    }

    void UpdateInGame()
    {
        //Keep checking to see if the ball had gone over either of the lines
        #region Checking if the ball is still within bounds
        if (ball.transform.position.x < -FieldExtents)
        {
            EndRound(2);
        }
        else if (ball.transform.position.x > FieldExtents)
        {
            EndRound(1);
        }
        #endregion
    }

    void EndRound(int playerPoint)
    {
		//Shake the screen -- ZAC
		CameraShake.Shake (1f);

		//Set the game state
		gameState = GameState.RoundOver;

        //Check to see what player won
        if (playerPoint == 1)
        {
            Player1Score += 1;
            //Debug.Log("player 1 wins");
            //Run the announcer text
            announcer.StartPlayerOneScore();
        }
        else if(playerPoint == 2)
        {
            Player2Score += 1;
            //Debug.Log("player 2 wins");
            //Run the announcer text
            announcer.StartPlayerTwoScore();
        }

        //Start the game over timer
        SleepTimer = SleepDisplayScores;

        //Stop the ball moving
        ball.ChangeVelocity(Vector3.zero, Ball.BallSpeeds.InitialSpeed);

        //Change to ball to red to display out of bounds
        ball.GetComponent<SpriteRenderer>().color = Color.red;

        //Turns the ball off
        ball.gameObject.SetActive(false);
    }

    void UpdateRoundOver()
    {
        SleepTimer -= Time.deltaTime;

        //Reset the game if the timer has reached 0
        if (SleepTimer <= 0)
        {
            //If the required score has been met
            if(Player1Score >= ScoreToWin || Player2Score >= ScoreToWin)
            {
                GameOver();
                
            }
            else
            {
                NewRound();
                
            }
        }
    }

    void NewRound()
    {
        Debug.Log("New Round");
        //Turns the ball on
        ball.gameObject.SetActive(true);
        //Reset the ball, duh
        ball.Reset();

        //Start a new game
        StartGame();
    }

    void GameOver()
    {
        //Debug.Log("Game Over");

        //Start the announcer text
        if (Player1Score > Player2Score)
        {
            //Debug.Log("Player 1 Wins");
            announcer.StartPlayerOneWins();
        }
        else
        {
            //Debug.Log("Player 2 Wins");
            announcer.StartPlayerTwoWins();
        }

        //Set the new timer
        SleepTimer = SleepDisplayVictory;

        //Set to game over
        gameState = GameState.GameOver;
    }

    void UpdateGameOver()
    {
        //Debug.Log("updating game over screen: " + SleepTimer);
        SleepTimer -= Time.deltaTime;

        //Display the text and pause for end game
        if (SleepTimer <= 0)
        {
            ResetGame();
        }
    }

    void ResetGame()
    {
        //Reset the fact that players have charged their paddles for game start
        PlayersHaveCharged = false;

        Debug.Log("New Game");
        //Turns the ball on
        ball.gameObject.SetActive(true);
        //Reset the ball, duh
        ball.Reset();

        //Changes the game state
        gameState = GameState.PreGame;

        //Starts the attract mode annoucner loop
        announcer.StartAttractMode();

        //Resets the scores
        Player1Score = 0;
        Player2Score = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(new Vector3(-FieldExtents, -10, 0), new Vector3(-FieldExtents, 10, 0));

        Gizmos.DrawLine(new Vector3(FieldExtents, -10, 0), new Vector3(FieldExtents, 10, 0));
    }


}
