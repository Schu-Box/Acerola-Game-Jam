using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public MovementData movementData;

    public Transform bonkCheckPoint;
    
    public Transform groundCheckPoint;
    [HideInInspector] public Vector2 groundCheckSize = new Vector2(1f, 0.05f);
    public LayerMask groundLayer;
    
    private Rigidbody2D rb;

    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isJumpCut = false;
    private bool isJumpFalling = false;
    
    //Jumping
    private float lastPressedJumpTime = 0f;

    private float lastOnGroundTime = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        lastPressedJumpTime -= Time.deltaTime;
        lastOnGroundTime -= Time.deltaTime;
        
        #region Inputs
        if(Input.GetKeyDown(KeyCode.Space))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            OnJumpInputUp();
        }
        #endregion

        if (!isJumping && !isGrounded) //If they aren't jumping and they aren't grounded, check if they've now become grounded
        {
            if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
            {
                Grounded();
            }
        } 
        else if (isGrounded) //If they are grounded, check if they've become ungrounded
        {
            if (!Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer))
            {
                Ungrounded();
            }
            else
            {
                lastOnGroundTime = movementData.coyoteTimeBuffer;
            }
        }

        if (isJumping && rb.velocity.y < 0) //Reached peak of jump
        {
            isJumping = false;
        }

        if (lastOnGroundTime > 0 && !isJumping) 
        {
            isJumpCut = false;
        
            if (!isJumping)
            {
                isJumpFalling = false;
            }
        }

        if (CanJump() && lastPressedJumpTime > 0)
        {
            Jump();
        }

        Run();
        
        //if we want to add a fast fall when player presses down
        // if (rb.velocity.y < 0 && moveInput.y < 0)
        // {
        //     SetGravityScale(movementData.gravityScale * movementData.fastFallGravityMult);
        //     rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -movementData.maxFastFallSpeed));
        // }

        if (isJumpCut || rb.velocity.y < 0f) //If we're falling or stopped holding jump
        {
            if (rb.gravityScale != movementData.gravityScaleWhenFalling)
            {
                SetGravityScale(movementData.gravityScaleWhenFalling);
            }
            // SetGravityScale(movementData.gravityScale * movementData.fallGravityMult);
            // rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -movementData.maxFallSpeed));
        }
    }

    private void SetGravityScale(float newGravityScale)
    {
        // Debug.Log("Gravity now " + newGravityScale);
        
        rb.gravityScale = newGravityScale;
    }

    private void Grounded()
    {
        // Debug.Log("Grounded");
        
        isGrounded = true;
        isJumpCut = false;

        lastOnGroundTime = movementData.coyoteTimeBuffer;
    }

    private void Ungrounded()
    {
        // Debug.Log("Ungrounded");
        
        isGrounded = false;
    }

    private bool CanJump()
    {
        return lastOnGroundTime > 0 && !isJumping;
    }

    private bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }

    private void OnJumpInput()
    {
        // Debug.Log("jump input");
        
        lastPressedJumpTime = movementData.jumpInputBufferTime;
    }

    private void OnJumpInputUp()
    {
        if (CanJumpCut())
        {
            // Debug.Log("Jump is cut");
        
            isJumpCut = true;
        }
    }

    private void Jump()
    {
        // Debug.Log("JUMP");
        
        isJumping = true;
        isJumpCut = false;
        isJumpFalling = false;

        lastPressedJumpTime = 0f;
        lastOnGroundTime = 0f;

        float force = movementData.jumpForce;
        if (rb.velocity.y < 0) // In case the player has downward velocity somehow, we offset it
        {
            force -= rb.velocity.y;
        }
        
        rb.AddForce(Vector3.up * force, ForceMode2D.Impulse);

        if (Input.GetKey(KeyCode.Space)) //If the player is holding space bar, set their gravity to the normal scale
        {
            if (rb.gravityScale != movementData.gravityScaleWhenJumping)
            {
                SetGravityScale(movementData.gravityScaleWhenJumping);
            }
        }
    }

    private void Run()
    {
        float horizontalInput = Input.GetAxis("Horizontal");

        float targetSpeed = horizontalInput * movementData.movementSpeed;

        float accelerationRate;
        if (lastOnGroundTime > 0f)
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.acceleration : movementData.deceleration;
        }
        else
        {
            accelerationRate = (Mathf.Abs(targetSpeed) > 0.01f) ? movementData.acceleration * movementData.accelerationInAir : movementData.deceleration * movementData.decelerationInAir;
        }
        
        //Increases acceleration and maxSpeed when at apex to make the jump feel more responsive
        if ((isJumping || isJumpFalling) && Mathf.Abs(rb.velocity.y) < movementData.jumpHangTimeThreshold)
        {
            accelerationRate *= movementData.jumpHangAccelerationMult;
            targetSpeed *= movementData.jumpHangMaxSpeedMult;
        }
        
        //Conserve momentum?
        
        float speedDif = targetSpeed - rb.velocity.x;

        float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelerationRate, movementData.velPower) * Mathf.Sign(speedDif);

        rb.AddForce(movement * Vector2.right);
    }

    public void Bonked()
    {
        Debug.Log("BONKED");

        Destroy(gameObject);
    }
}
