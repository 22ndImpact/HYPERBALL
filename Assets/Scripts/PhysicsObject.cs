using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour
{
    public Collider2D col2D;

    //public Vector2 currentPosition;
    public Vector2 previousPosition;
    public Vector2 velocity;

    protected virtual void Awake()
    {
        
    }

    protected virtual void Start ()
    {
        //Add yourself to the PhysicsEngine object list
        PhysicsEngine2D.inst.AddPhysicsObject(this);

        //Links the col2D reference with the objects collider
        col2D = GetComponent<Collider2D>();
	}
	
	protected virtual void Update ()
    {
        //this.transform.position += new Vector3(1, 0, 0) * Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {

    }

    protected virtual void LateUpdate()
    {
        //Calculate the velocity of this frame based on current position and the previous position last frame, just before the previous position gets updated.
        velocity = ((Vector2)this.gameObject.transform.position - previousPosition) / Time.deltaTime;
        
        //Sets the previous position in the last line of executed code (hopefully)
        previousPosition = this.gameObject.transform.position;
    }
}
