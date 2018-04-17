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

        //Check the position of the ball
        if(_ball.transform.position.x > (_paddle.transform.position.x + _paddle.col2D.bounds.extents.x))
        {
            Debug.Log("Hit the right side");
            _ball.Direction.x = 1;
        }
        else if(_ball.transform.position.x < (_paddle.transform.position.x - _paddle.col2D.bounds.extents.x))
        {
            Debug.Log("Hit the left side");
            _ball.Direction.x = -1;
        }
        else if (_ball.transform.position.y > (_paddle.transform.position.y + _paddle.col2D.bounds.extents.y))
        {
            Debug.Log("Hit the top side");
            _ball.Direction.y = 1;
        }
        else if (_ball.transform.position.y < (_paddle.transform.position.y - _paddle.col2D.bounds.extents.y))
        {
            Debug.Log("Hit the bottom side");
            _ball.Direction.y = -1;
        }
        else
        {
            Debug.Log("WTF happened?");
        }

        if (_paddle.paddleState == Paddle.PaddleState.Reaching)
        {
            _paddle.StartRetracting(true);
        }

        //Time.timeScale = 0;
    }
}
