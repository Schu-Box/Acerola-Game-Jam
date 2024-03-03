using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;
    
    public MovementDataConfig movementDataConfig;
    public MovementData movementData;

    public Transform bonkCheckPoint;
    
    public Transform groundCheckPoint;
    [HideInInspector] public Vector2 groundCheckSize = new Vector2(1f, 0.05f);
    public LayerMask groundLayer;

    [Header("Particles")]
    public ParticleSystem canSquashParticles;
    
    private Rigidbody2D rb;

    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isJumpCut = false;
    private bool isJumpFalling = false;

    private bool isDiving = false;

    private bool canSquash = false;
    
    //Timers
    private float lastPressedJumpTime = 0f;
    private float lastOnGroundTime = 0f;

    public Vector3 lastVelocity;
    
    private void Awake()
    {
        Instance = this;
        
        rb = GetComponent<Rigidbody2D>();
        
        movementData = new MovementData(movementDataConfig);
        
        ToggleCanSquashParticles(false);
    }
    
    void Update()
    {
        lastPressedJumpTime -= Time.deltaTime;
        lastOnGroundTime -= Time.deltaTime;
        
        #region Inputs
        if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnJumpInput();
        }

        if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        {
            OnJumpInputUp();
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnDiveInput();
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

        if (isJumping && rb.velocity.y < 0) //Reached peak of jump, no longer considered jumping
        {
            isJumping = false;
        }

        if (CanJump() && lastPressedJumpTime > 0)
        {
            Jump();
        }

        Run();

        if (isJumpCut || rb.velocity.y < 0f && !isJumpFalling) //If we've reached peak of jump or cut the jump, we've started falling
        {
            isJumpFalling = true;
            
            if (rb.gravityScale != movementData.gravityScaleWhenFalling)
            {
                SetGravityScale(movementData.gravityScaleWhenFalling);
            }
        }
        
        if (isDiving)
        {
            Vector3 newVelocity = rb.velocity;
            newVelocity.y -= movementData.diveSpeedGainedPerSecond * Time.deltaTime;
            newVelocity.y = Mathf.Clamp(newVelocity.y, -movementData.maxDiveSpeed, movementData.maxDiveSpeed);
            rb.velocity = newVelocity;
        }

        if (!canSquash && -rb.velocity.y >= movementData.velocityRequiredForSquashing)
        {
            ToggleCanSquashParticles(true);
        } 
        else if (canSquash && -rb.velocity.y < movementData.velocityRequiredForSquashing)
        {
            ToggleCanSquashParticles(false);
        }
    }

    private void LateUpdate()
    {
        lastVelocity = rb.velocity;
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
        isJumpFalling = false;
        isDiving = false;

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

    private void OnDiveInput()
    {
        if (CanDive())
        {
            // Debug.Log("DIVING");
        
            isDiving = true;
            
            Vector3 newVelocity = rb.velocity;
            newVelocity.y = -movementData.diveStartSpeedIncrease;
            rb.velocity = newVelocity;
        }
    }

    private bool CanDive()
    {
        if (isJumping || isJumpFalling && !isDiving)
        {
            return true;
        }

        return false;
    }

    private void Jump()
    {
        // Debug.Log("JUMP");
        
        isJumping = true;
        isJumpCut = false;
        isJumpFalling = false;
        isDiving = false;

        lastPressedJumpTime = 0f;
        lastOnGroundTime = 0f;

        float force = movementData.jumpForce;
        if (rb.velocity.y < 0) // In case the player has downward velocity somehow, we offset it
        {
            force -= rb.velocity.y;
        }
        
        rb.AddForce(Vector3.up * force, ForceMode2D.Impulse);

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) //If the player is holding space bar, set their gravity to the normal scale
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

        GameController.Instance.GameOver();
    }

    public void ToggleCanSquashParticles(bool canNowSquash)
    {
        canSquash = canNowSquash;
        
        if (canNowSquash)
        {
            canSquashParticles.Play();
        }
        else
        {
            canSquashParticles.Stop();
        }
    }
    
    #region Experience

    private int experience = 0;

    private int experienceRequiredToLevelUp = 5;

    public float experienceIncreaseRatePerLevel = 1.5f;

    public void AddExperience(int amount)
    {
        experience += amount;
        
        GameController.Instance.UpdateExperienceUI(experience);
        
        if(experience >= experienceRequiredToLevelUp)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        experienceRequiredToLevelUp = (int)(experienceRequiredToLevelUp * experienceIncreaseRatePerLevel);
        
        GameController.Instance.DisplayLevelUp();
    }

    #endregion
}
