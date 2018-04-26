using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsEngine2D : MonoBehaviour
{
    //Makign an static instance of the class to be accessed globally
    public static PhysicsEngine2D inst;

    //The list of all currently existing physics objects in the scene
    public List<PhysicsObject> PhysicsObjects;
    public List<Paddle> Paddles;
    public List<Ball> Balls;

    public float hitTimeSlowScale;
    public float paddleHitTimeSlowDuration;
    public float normalHitTimeSlowDuration;

    public float EdgeBounceMultiplier;

	public AudioClip paddleHitSound;
	public AudioClip wallHitSound;

    private void Awake()
    {
        //Initializing the global singlton
        inst = this;
    }

    // Use this for initialization
    void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		if(CheckBallPaddleCollission() != null)
        {
            Paddle hitPaddle = CheckBallPaddleCollission();
            //Runs the collision logic with those 2 objects
            Collision(Balls[0], hitPaddle);
        }

        #region Debug Time Freeze
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(Time.timeScale == 0)
            {
                Time.timeScale = 1;
            }
            else
            {
                Time.timeScale = 0;
            }
        }
        #endregion
    }

    //Used to add new physics objects to the tracked list, also subdivides object types into trackable lists
    public void AddPhysicsObject(PhysicsObject _physicsObject)
    {
        //Add the new item to the list
        PhysicsObjects.Add(_physicsObject);

        //If it is a ball add to the ball list too
        if(_physicsObject.gameObject.GetComponent<Ball>() != null)
        {
            Balls.Add(_physicsObject.gameObject.GetComponent<Ball>());
        }

        //If it is a paddle add to the paddle list too
        if (_physicsObject.gameObject.GetComponent<Paddle>() != null)
        {
            Paddles.Add(_physicsObject.gameObject.GetComponent<Paddle>());
        }
    }

    //Check to see if the ball has hit a paddle, and if so returns that paddle
    Paddle CheckBallPaddleCollission()
    {
        //#WARNING# This assumings there is only 1 ball and that it is only ever colliding with 1 object at a time.
        //Loops over every paddle
        foreach(Paddle _paddle in Paddles)
        {
            //Checks for an intersection
            if (Balls[0].col2D.Distance(_paddle.col2D).isOverlapped)
            {
                //Returns the intersecting paddle
                return _paddle;
            }
        }
        return null;
    }

    public void Collision(Ball _ball, Paddle _paddle)
    {
        Debug.Log("Collision Detected");
        CollisionCorrection(_ball, _paddle);
    }

    public void CollisionCorrection(Ball _ball, Paddle _paddle)
    {
        //Vector3 combinedVelocities = _ball.velocity - _paddle.velocity;

        Vector3 workingBallVelocity = (Vector3)_ball.velocity * Time.deltaTime;
        Vector3 workingPaddleVelocity = (Vector3)_paddle.velocity * Time.deltaTime;

        int polarity = -1;

        float collisionSensitivity = 0.01f;

        //Loops for a maximum of 100 times before exiting out
        for (int i = 0; i < 100; i++)
        {
            //Debug.Log("Current Overlap: " + _ball.col2D.Distance(_paddle.col2D).isOverlapped);

            //Debug.Log("Adjusting Positions");
            //Adjust positions
            //Debug.Log("Adjusting ball by: " + (_ball.velocity * -1 * Time.deltaTime).ToString("F4"));
            _ball.transform.position += workingBallVelocity * polarity;
            //Debug.Log("Adjusting paddle by: " + (_paddle.velocity * -1 * Time.deltaTime).ToString("F4"));
            _paddle.transform.position += workingPaddleVelocity * polarity;

            //Check if they are still collided
            if(_ball.col2D.Distance(_paddle.col2D).isOverlapped)
            {
                //If yes set polarity
                //Debug.Log("Still Colliding, Swapping Polarity");
                polarity = -1;                    
            }
            //If no 
            else
            {
                //Debug.Log("Not colliding anymore. Distance of: " + _ball.col2D.Distance(_paddle.col2D).distance);
                //Are they within an acceptable distance
                if (_ball.col2D.Distance(_paddle.col2D).distance < collisionSensitivity)
                {
                    //Debug.Log("Within range, breaking out");
                    //If yes Break Out
                    //Time.timeScale = 0;
                    break;
                }
                else
                {
                    //Debug.Log("Still in range, changing polarity");
                    //If no set polarity
                    polarity = 1;
                }
            }
            //Debug.Log("Halfing working velocity");
            //Half the new distance
            workingBallVelocity *= 0.5f;
            workingPaddleVelocity *= 0.5f;

        }

        //A flag to see if the game needs to freeze time or not
        bool paddleHit = false;
        Vector3 newDirection = _ball.Direction;
        Ball.BallSpeeds newSpeed = Ball.BallSpeeds.InitialSpeed;
        string collisionType = "";

        //Check the position of the ball
        //Right Side
        if(_ball.transform.position.x > (_paddle.transform.position.x + _paddle.col2D.bounds.extents.x))
        {
            //Debug.Log("Hit the right side");
            //Makes the current X direction positive
            newDirection.x = 1;

            //Determine the offset from the middle of the paddle to determine change in y velocity
            float VerticleOffset = (_ball.transform.position.y - _paddle.transform.position.y) / _paddle.col2D.bounds.extents.y;
            //Apply a modified Y direction based on the verticle offset
            newDirection.y += VerticleOffset * EdgeBounceMultiplier;

            //Check if the paddle is reaching (and not on the way back in)
            if (_paddle.paddleState == Paddle.PaddleState.Reaching)
            {
                //If so set the paddle hit to true and set the correct speed
                paddleHit = true;
                //_ball.ChangeVelocity(newDirection, Ball.BallSpeeds.CannonSpeed);
                newSpeed = Ball.BallSpeeds.CannonSpeed;
            }
            else
            {
                //Otherwise just change to the correct speed
                //_ball.ChangeVelocity(newDirection, Ball.BallSpeeds.WallSpeed);
                newSpeed = Ball.BallSpeeds.WallSpeed;
            }
            collisionType = "right";
            //Debug.Log("Verticle Offset: " + VerticleOffset);
        }
        //Left Side
        else if(_ball.transform.position.x < (_paddle.transform.position.x - _paddle.col2D.bounds.extents.x))
        {
            //Debug.Log("Hit the left side");
            //Makes the current X direction negative
            newDirection.x = -1;

            //Determine the offset from the middle of the paddle to determine change in y velocity
            float VerticleOffset = (_ball.transform.position.y - _paddle.transform.position.y) / _paddle.col2D.bounds.extents.y;

            //Apply a modified Y direction based on the verticle offset
            newDirection.y += VerticleOffset * EdgeBounceMultiplier;

            //Check if the paddle is reaching (and not on the way back in)
            if (_paddle.paddleState == Paddle.PaddleState.Reaching)
            {
                //If so set the paddle hit to true and set the correct speed
                paddleHit = true;
                //_ball.ChangeVelocity(newDirection, Ball.BallSpeeds.CannonSpeed);
                newSpeed = Ball.BallSpeeds.CannonSpeed;
            }
            else
            {
                //Otherwise just change to the correct speed
                //_ball.ChangeVelocity(newDirection, Ball.BallSpeeds.WallSpeed);
                newSpeed = Ball.BallSpeeds.WallSpeed;
            }
            collisionType = "left";


            //Debug.Log("Verticle Offset: " + VerticleOffset);
        }
        //Top Side
        else if (_ball.transform.position.y > (_paddle.transform.position.y + _paddle.col2D.bounds.extents.y))
        {
            //Debug.Log("Hit the top side");
            //Makes the current Y direction positive
            newDirection.y = (Mathf.Abs(newDirection.y));
            //Change the velocity and set the right speed
            //_ball.ChangeVelocity(newDirection, Ball.BallSpeeds.WallSpeed);
            newSpeed = Ball.BallSpeeds.WallSpeed;
            collisionType = "top";
			CameraShake.Shake (0.8f);
		}
        //Bottom Side
        else if (_ball.transform.position.y < (_paddle.transform.position.y - _paddle.col2D.bounds.extents.y))
        {
            //Debug.Log("Hit the bottom side");
            //Makes the current Y direction negative
            newDirection.y = (Mathf.Abs(newDirection.y) * -1);
            //Change the velocity and set the right speed
            //_ball.ChangeVelocity(newDirection, Ball.BallSpeeds.WallSpeed);
            newSpeed = Ball.BallSpeeds.WallSpeed;
            collisionType = "bottom";
			CameraShake.Shake (0.8f);
		}
        else
        {
            //Debug.Log("WTF happened?");
        }

        //If the paddle hit the ball while reaching out
        if (paddleHit)
        {
            //Retract the ball
            _paddle.StartRetracting(true);
            //Slow time
            StartCoroutine(slowTime(hitTimeSlowScale, paddleHitTimeSlowDuration, newDirection, newSpeed, collisionType));

			//Distortion effect -- ZAC
			DistortionWave.Play (_ball.transform.position);
			CameraShake.Shake (0.5f);
			SoundController.PlayOneShot (paddleHitSound);
		}
        //Just bounce off
        else
        {
            StartCoroutine(slowTime(hitTimeSlowScale, normalHitTimeSlowDuration, newDirection, newSpeed, collisionType));
        }

        
    }

    IEnumerator slowTime(float _timeScale, float _duration, Vector3 _newDirection, Ball.BallSpeeds _newBallSpeed, string _collisionType)
    {
        //Change the velocity to flat up against the wall

        //Set to simulated stalling allowing us to control the variables directly
        GameDirector.inst.ball.SimulatedStalling = true;

        //Set target local scale back to normal ball shape
        GameDirector.inst.ball.TargetLocalScale = GameDirector.inst.ball.StartingLocalScale;

        //Set the target Position
        GameDirector.inst.ball.TargetPosition = GameDirector.inst.ball.transform.position;

        #region Old Squish Angle Code
        ////Unparent the ball visual so global scaling
        //GameDirector.inst.ball.ballVisual.parent = null;

        ////If moving horizontally
        //if(Mathf.Abs(GameDirector.inst.ball.Direction.x) >= Mathf.Abs(GameDirector.inst.ball.Direction.y))
        //{
        //    //If moving Right
        //    if(GameDirector.inst.ball.Direction.x >= 0)
        //    {
        //        //Always rotate to face the right
        //        GameDirector.inst.ball.SimulatedTargetRotation = new Vector3(0, 0, 270);

        //        //If Hitting Top
        //        //Squish Y
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(0.25f, 1, 0.25f);

        //        //If Hitting Bottom
        //        //Squish Y
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(0.25f, 1, 0.25f);

        //        //If Hitting Right
        //        //Squish X
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(1, 0.25f, 0.25f);
        //    }
        //    //If moving Left
        //    else
        //    {
        //        //Always rotate to face the left
        //        GameDirector.inst.ball.SimulatedTargetRotation = new Vector3(0, 0, 90);

        //        //If Hitting Top
        //        //Squish Y
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(1, 0.25f, 0.25f);

        //        //If Hitting Bottom
        //        //Squish Y
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(1, 0.25f, 0.25f);

        //        //If Hitting Left
        //        //Squish X
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(0.25f, 1, 0.25f);
        //    }
        //}
        ////If moving vertically
        //else
        //{
        //    //If moving Up
        //    if (GameDirector.inst.ball.Direction.y >= 0)
        //    {
        //        //Always rotate to face up
        //        GameDirector.inst.ball.SimulatedTargetRotation = new Vector3(0, 0, 0);

        //        //If Hitting Top
        //        //Squish Y
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(0.25f, 1, 0.25f);

        //        //If Hitting Right
        //        //Squish X
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(1, 0.25f, 0.25f);

        //        //If Hitting Left
        //        //Squish X
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(1, 0.25f, 0.25f);
        //    }
        //    //If moving Down
        //    else
        //    {
        //        //Always rotate to face down
        //        GameDirector.inst.ball.SimulatedTargetRotation = new Vector3(0, 0, 180);

        //        //If Hitting Bottom
        //        //Squish Y
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(0.25f, 1, 0.25f);

        //        //If Hitting Right
        //        //Squish X
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(1, 0.25f, 0.25f);

        //        //If Hitting Left
        //        //Squish X
        //        GameDirector.inst.ball.SimulatedLocalScale = new Vector3(1, 0.25f, 0.25f);
        //    }
        //}
        ////Reparent the ball visual for calculations
        //GameDirector.inst.ball.ballVisual.parent = GameDirector.inst.ball.transform;
        #endregion

        //Stop Time
        Time.timeScale = _timeScale;
        yield return new WaitForSecondsRealtime(_duration);
        Time.timeScale = 1.0f;

        GameDirector.inst.ball.SimulatedStalling = false;
        

        //Change the velocity to the intended angles
        GameDirector.inst.ball.ChangeVelocity(_newDirection, _newBallSpeed);
    }


}
