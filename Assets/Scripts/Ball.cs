using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : PhysicsObject
{
    public float Speed;
    public Vector2 Direction;

    public Transform ballVisual;
    public float StretchRate;
    public Vector3 TargetPosition;
    public Vector3 TargetLocalScale;

    public float MaximumScaleDifferential;

    public Vector3 CurrentPercentSpeed;

    public Vector3 StartingLocalScale;

    public bool SimulatedStalling = false;

    public enum BallSpeeds
    {
        CannonSpeed,
        WallSpeed,
        InitialSpeed
    }
    public BallSpeeds ballSpeeds;

    public float CannonSpeed;
    public float WallSpeed;
    public float InitialSpeed;

    #region Debug Variables
    public Collider2D paddle;
    public bool overlapping;
    #endregion

    public void ChangeVelocity(Vector3 _direction, BallSpeeds _ballSpeed)
    {
        //Change Direction
        Direction = _direction.normalized;

        //Change Speed
        switch (_ballSpeed)
        {
            case BallSpeeds.CannonSpeed:
                Speed = CannonSpeed;
                break;
            case BallSpeeds.WallSpeed:
                Speed = WallSpeed;
                break;
            case BallSpeeds.InitialSpeed:
                Speed = InitialSpeed;
                break;
        }
    }

    protected override void Awake()
    {
        //Remembers the starting local scale
        StartingLocalScale = ballVisual.localScale;
        TargetLocalScale = StartingLocalScale;
    }

    protected override void Start ()
    {
        //Run the Start of the base object
        base.Start();


	}
	
	protected override void Update ()
    {
        //Run the Update of the base object
        base.Update();

        //Applies the current speed and direction to the position of the object in game
        UpdatePosition();

        //Updates the squash and stretch properties of the ball
        UpdateBallVisual();
	}

    public void Reset()
    {
        //Resets the ball position
        transform.position = Vector3.zero;
        ChangeVelocity(Vector3.zero, Ball.BallSpeeds.InitialSpeed);

        //Reset the ball visual
        //Debug.Log("Starting Scale: " + StartingLocalScale);
        ballVisual.localScale = StartingLocalScale;
        TargetLocalScale = StartingLocalScale;
        ballVisual.localPosition = Vector3.zero;
        TargetPosition = Vector3.zero;

        //Change to ball back to white
        GetComponent<SpriteRenderer>().color = Color.white;

        //resets the ball trail
        ballVisual.GetComponent<TrailRenderer>().Clear();
    }

    void UpdatePosition()
    {
        transform.position += (Vector3)(Direction * (Speed * Time.deltaTime));
    }

    void UpdateBallVisual()
    {
        if(GameDirector.inst.gameState == GameDirector.GameState.InGame)
        {
            if (SimulatedStalling == false)
            {
                //Rotating the ball based on direction
                float rot_z = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
                ballVisual.transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
            }


            //ballVisual.transform.LookAt(ballVisual.transform.position + (Vector3)velocity, Vector3.forward);
            if (velocity.x != 0 || velocity.y != 0)
            {

                float SpeedPercent = Speed / CannonSpeed;
                float WarpPercent = SpeedPercent * MaximumScaleDifferential;

                //Dont calculate the targetscale when simulating
                if (SimulatedStalling == false)
                {
                    TargetLocalScale.y = StartingLocalScale.y / (1 - WarpPercent);
                    TargetLocalScale.x = StartingLocalScale.x * (1 - WarpPercent);
                }
                //Lerp the scale of the visual towards the goal
                ballVisual.transform.localScale = Vector3.LerpUnclamped(ballVisual.localScale, TargetLocalScale, StretchRate);


                //Dont calculate the target position when simulating
                if (SimulatedStalling == false)
                {
                    //Save it
                    TargetPosition = transform.position;

                    //Change the X
                    TargetPosition.x = transform.position.x - (ballVisual.lossyScale.y / 2 * Direction.x) + (transform.lossyScale.x * Direction.x / 2);
                    TargetPosition.y = transform.position.y - (ballVisual.lossyScale.y / 2 * Direction.y) + (transform.lossyScale.x * Direction.y / 2);
                }
                //Lerp the position to the target position
                ballVisual.position = Vector3.LerpUnclamped(ballVisual.position, TargetPosition, StretchRate);
            }
        }
    }
}
