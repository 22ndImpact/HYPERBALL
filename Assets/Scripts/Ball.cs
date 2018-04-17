using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : PhysicsObject
{
    public float Speed;
    public Vector2 Direction;

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
	}

    public void Reset()
    {
        //Resets the ball position
        transform.position = Vector3.zero;
        ChangeVelocity(Vector3.zero, Ball.BallSpeeds.InitialSpeed);

        //resets the ball trail
        GetComponent<TrailRenderer>().Clear();
    }

    void UpdatePosition()
    {
        transform.position += (Vector3)(Direction * (Speed * Time.deltaTime));
    }
}
