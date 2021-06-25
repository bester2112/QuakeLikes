using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public float svGravity = -20.0f;
    public float svFriction = 6.0f;
    public float svGroundSpeed = 7.0f;
    public float svAcceleration = 14.0f;
    public float svDecceleration = 5.0f;
    public float svAirSpeed = 14.0f;
    public float svAirAcceleration = 0.5f;
    public float svAirDecceleration = 2.0f;
    public float svJumpForce = 8.0f;
    public float svJumpPadForce = 30.0f;

    public CharacterController characterController;

    public Transform groundCheckTransform;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundMask;

    public Vector3 velocity;
    protected bool jumpQueued;
    private bool jumpPadQueued;

    protected float x;
    protected float z;

    protected float timeStep = 0.0f;

    private Timer jumpTimer;

    protected bool isServer = false;

    protected void Start()
    {
        jumpTimer = new Timer();
    }

    protected void Update()
    {
        if (isServer)
        {
            return;
        }

        jumpTimer.Update();

        if (isGrounded())
        {
            MoveGrounded();
        }
        else
        {
            MoveAirborne();
        }

        if (jumpPadQueued)
        {
            Debug.Log("If wird getroffen");
            jumpPadQueued = false;
            Jump(svJumpPadForce);
        }

        characterController.Move(velocity * timeStep);
    }

    //Using Quake III Code explained here
    //https://github.com/id-Software/Quake-III-Arena/blob/master/code/game/bg_pmove.c
    private void Accelerate(Vector3 wishDirection, float wishSpeed, float acceleration)
    {
        float currentSpeed = Vector3.Dot(velocity, wishDirection);
        float addSpeed = wishSpeed - currentSpeed;

        if (addSpeed <= 0.0f)
        {
            return;
        }

        float acceleratedSpeed = acceleration * timeStep * wishSpeed;

        if (acceleratedSpeed > addSpeed)
        {
            acceleratedSpeed = addSpeed;
        }

        velocity += acceleratedSpeed * wishDirection;
    }

    private void MoveGrounded()
    {
        if (!jumpQueued)
        {
            Friction();
        }

        Vector3 wishDirection = transform.right * x + transform.forward * z;
        wishDirection.Normalize();

        float wishSpeed = wishDirection.magnitude * svGroundSpeed;

        Accelerate(wishDirection, wishSpeed, svAcceleration);

        velocity.y = svGravity * timeStep;

        if (jumpQueued)
        {
            Jump(svJumpForce);
            //FindObjectOfType<AudioManager>().Play("Jumping");
            if (jumpTimer.isFinished())
            {
                jumpTimer.startTimer(0.75f);
                AudioManager audioManager = FindObjectOfType<AudioManager>();
                audioManager.transform.position = transform.position;
                audioManager.Play("Jumping");
            }
        }
    }

    private void MoveAirborne()
    {
        Vector3 wishDirection = transform.right * x + transform.forward * z;
        wishDirection.Normalize();

        float wishSpeed = wishDirection.magnitude * svAirSpeed;
        float acceleration;

        if (Vector3.Dot(velocity, wishDirection) < 0.0f)
        {
            acceleration = svAirDecceleration;
        }
        else
        {
            acceleration = svAirAcceleration;
        }

        Accelerate(wishDirection, wishSpeed, acceleration);

        velocity.y += svGravity * timeStep;
    }

    public void Jump(float jumpForce)
    {
        velocity.y = jumpForce;
        Debug.Log(velocity.y + " " + jumpForce);
    }

    public void JumpPad()
    {
        Debug.Log("Pad setzt");
        jumpPadQueued = true;
    }

    private void Friction()
    {
        Vector3 vec = new Vector3(velocity.x, velocity.y, velocity.z);
        vec.y = 0.0f;

        float speed = vec.magnitude;

        float control = speed < svDecceleration ? svDecceleration : speed;
        float drop = control * svFriction * timeStep;

        float newSpeed = speed - drop;
        if (newSpeed < 0.0f)
        {
            newSpeed = 0.0f;
        }

        if (speed > 0.0f)
        {
            newSpeed /= speed;
        }

        velocity *= newSpeed;
    }

    private bool isGrounded()
    {
        return Physics.CheckSphere(groundCheckTransform.position, groundCheckDistance, groundMask);
    }

    public float getSpeed()
    {
        return new Vector2(velocity.x, velocity.z).magnitude;
    }
}
