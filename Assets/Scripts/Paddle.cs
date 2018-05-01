using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : PhysicsObject
{
    #region Variables
    //Setting up the possible states the paddle can be in
    public enum PaddleState
    {
        Static,
        Charging,
        Reaching,
        Retracting
    }
    public PaddleState paddleState;

	public AudioClip chargeSound;
	public AudioClip launchSound;
	public AudioClip retractSound;

	#region Transform Variables
	public Vector2 StartingPosition;
    #endregion

    #region Input Variables
    //The key map to activate the cannon.
    public KeyCode fireKey;
    #endregion

    #region  Charging Variables
    //The amount of time it takes to fully charge the paddle
    public float timeToCharge;
    //The amount of time the cannon has been charging
    public float currentChargeTime;
    //The current percent through a full charge the paddle is
    public float chargePercentage
    {
        get
        {
            return Mathf.Clamp01(currentChargeTime / timeToCharge);
        }
    }
    //The distance the paddle will be pulled back at full charge
    public float chargeDistance;
    #endregion

    #region Reaching Variables
    //Paddle Animation Curves
    public AnimationCurve ReachCurve;
    //The percent the paddle was charge when it was shot
    public float chargedPercent;
    //The position of the paddle when it was fired
    public Vector3 chargedPosition;
    //The time it takes for a fully changed shot to reach its intended distance
    public float reachTime;
    //The maximum distance the paddle will reach at full charge
    public float maxReachDistance;
    //The reach distance that is currently set due to charge
    public float goalReachDistance;
    //The current percentage through a full reach the paddle is
    public float currentReachPercentage;
    //The current charge percentage when evaluated by the reach curve
    public float evaluatedReachPercentage
    {
        get
        {
            return ReachCurve.Evaluate(currentReachPercentage);
        }
    }
    #endregion

    #region Retracting Variables
    //This curve is used to hold the currently selected curve based on hit or not
    private AnimationCurve CurrentRetractCurve;
    public AnimationCurve EmptyRetractCurve;
    public AnimationCurve HitRetractCurve;

    //The position the paddle was at when it stopped reaching and begun retracting
    public Vector3 reachedPosition;

    //The time the paddle will take to retract, is change to hit or empty based on circumstances of retraction
    public float retractTime;
    public float hitRetractTime;
    public float emptyRetractTime;

    //The current percentage the paddle is through retracting to base
    public float currentRetractPercentage;
    public float evaluatedRetractPercentage
    {
        get
        {
            return 1f - CurrentRetractCurve.Evaluate(1f - currentRetractPercentage);
        }
    }
    #endregion

    #endregion

    #region Functions
    protected override void Awake()
    {

    }
    protected override void Start()
    {
        //Run the Start of the base object
        base.Start();

        //Set the starting position in world space
        StartingPosition = (Vector2)transform.position;

        //Resets the paddle at the start of the game, (used to initialize paddles at the start of rounds too)
        ResetPaddle();

    }
    protected override void Update()
    {
        //Run the Update of the base object
        base.Update();

        //Updates the input given by the player
        UpdateInput();

        //Runs a specific update function based on current state
        switch(paddleState)
        {
            case PaddleState.Static:
                UpdateStatic();
                break;
            case PaddleState.Charging:
                UpdateCharging();
                break;
            case PaddleState.Reaching:
                UpdateReaching();
                break;
            case PaddleState.Retracting:
                UpdateRetracting();
                break;
        }
    }
    public void UpdateInput()
    {
        #region State Modifing inputs
        //If you are static and you have hit the fire key this frame, set to charging
        if(paddleState == PaddleState.Static)
        {
            if(Input.GetKey(fireKey))
            {
                //Grab the paddles player
                Player player = transform.parent.gameObject.GetComponent<Player>();
                //Create and set the charging flag to false
                bool Charging = false;
                //Check is any of the plaerys paddles are charging
                foreach(Paddle _paddle in player.Paddles)
                {
                    if(_paddle.paddleState == PaddleState.Charging)
                    {
                        Charging = true;
                    }
                }
                //If none of them are then charge yourself
                if(Charging == false)
                {
                    StartCharging();
                }
                
            }
        }

        //If you are charging
        if(paddleState == PaddleState.Charging)
        {
            //And you are not holding the charge button, fire the paddle
            if (!Input.GetKey(fireKey))
            {
                StartReaching();
            }

        }
        #endregion
    }

    #region Paddle State Functions
    #region State "Start" functions
    public void StartCharging()
    {
        paddleState = PaddleState.Charging;
		SoundController.PlayOneShot (chargeSound, 1.7f);
    }
    public void StartReaching()
    {
        //Set the paddle state
        paddleState = PaddleState.Reaching;
        //Set the chargedPercent to the current charge value to determine the distance of the paddle reach
        chargedPercent = chargePercentage;
        //Reset the current charge time to 0 effectivly resetting the charge
        currentChargeTime = 0;
        //Sets the current reach goal based on charged percent
        goalReachDistance = maxReachDistance * chargedPercent;
        //Set the chargedPosition to your current position
        chargedPosition = transform.position;
		SoundController.PlayOneShot (launchSound, chargedPercent * 2f);
	}
    public void StartRetracting(bool _BallCollision)
    {
        //Set the paddle state
        paddleState = PaddleState.Retracting;

        //If it was triggered by a ball collission
        if (_BallCollision)
        {
            CurrentRetractCurve = HitRetractCurve;
            retractTime = hitRetractTime * currentReachPercentage;
        }
        else
        {
            CurrentRetractCurve = EmptyRetractCurve;
            retractTime = emptyRetractTime;
        }

        //Reset the reach percentage
        currentReachPercentage = 0;
        //Set the currently reached position
        reachedPosition = transform.position;
		SoundController.PlayOneShot (retractSound);
	}
    #endregion
    #region State "Update" Functions
    public void UpdateStatic()
    {

    }
    public void UpdateCharging()
    {
        //Increase the charge time
        currentChargeTime += Time.deltaTime;

        //Store the current position
        Vector3 storedPosition = this.transform.position;

        //Change the position of the paddle
        storedPosition.x = Mathf.LerpUnclamped(StartingPosition.x, StartingPosition.x - chargeDistance, chargePercentage);

        //Update the position of the paddle in game to reflect the reach
        transform.position = storedPosition;
    }
    public void UpdateReaching()
    {
        //Update the current reach progress based on the set reachTime
        currentReachPercentage += Time.deltaTime / reachTime;

        //Store the current position
        Vector3 storedPosition = this.transform.position;

        //Change the position of the paddle
        storedPosition.x = Mathf.LerpUnclamped(chargedPosition.x, Mathf.Lerp(StartingPosition.x, StartingPosition.x + goalReachDistance, currentReachPercentage), evaluatedReachPercentage);

        //Update the position of the paddle in game to reflect the reach
        transform.position = storedPosition;

        //Once the currentReachPercentage gets to 1 is starts retracting
        if (currentReachPercentage >= 1)
        {
            StartRetracting(false);
        }
    }
    public void UpdateRetracting()
    {
        //Update the current retract progress based on the set retractTime
        currentRetractPercentage += Time.deltaTime / retractTime;

        //Store the current position
        Vector3 storedPosition = this.transform.position;

        //Change the position of the paddle
        storedPosition.x = Mathf.LerpUnclamped(reachedPosition.x, StartingPosition.x, evaluatedRetractPercentage);

        //Update the position of the paddle in game to reflect the reach
        transform.position = storedPosition;

        //Once the currentReachPercentage gets to 1 is starts retracting
        if (currentRetractPercentage >= 1)
        {
            ResetPaddle();
        }
    }
    #endregion
    #endregion

    public void ResetPaddle()
    {
        //Set the paddle to static at the beginning of the game
        paddleState = PaddleState.Static;
        //Reset the retract percentage
        currentRetractPercentage = 0;
    }
    #endregion
}
