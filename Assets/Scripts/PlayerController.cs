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

    public BoxCollider2D bonkCheckPoint;
    
    public BoxCollider2D groundCheckPoint;
    public LayerMask groundLayer;

    [Header("Particles")]
    public ParticleSystem canSquashParticles;
    public ParticleSystem canDashSquashParticles;
    
    private Rigidbody2D rb;

    private bool isFacingRight = true;

    private bool isGrounded = false;
    private bool isJumping = false;
    private bool isJumpCut = false;
    private bool isJumpFalling = false;

    private bool isDiving = false;
    private bool isDashing = false;

    private bool canSquash = false;
    public bool CanSquash => canSquash;
    
    private bool canDashSquash = false;
    public bool CanDashSquash => canDashSquash;
    
    //Timers
    private float lastPressedJumpTime = 0f;
    private float lastPressedDashTime = 0f;
    private float lastOnGroundTime = 0f;

    public Vector3 lastVelocity;
    private float fastestDownwardsVelocityWhileDiving = 0f;
    
    private void Awake()
    {
        Instance = this;
        
        rb = GetComponent<Rigidbody2D>();
        
        movementData = new MovementData(movementDataConfig);
        
        ToggleCanSquashParticles(false);
        ToggleCanDashSquashParticles(false);
    }
    
    void Update()
    {
        if(Time.timeScale == 0f)
            return;
        
        lastPressedJumpTime -= Time.deltaTime;
        lastPressedDashTime -= Time.deltaTime;
        lastOnGroundTime -= Time.deltaTime;
        
        #region Inputs
        float horizontalInput = Input.GetAxis("Horizontal");
        
        if (horizontalInput != 0)
            CheckDirectionToFace(horizontalInput > 0);

        // if(Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        //     OnJumpInput();
        //
        // if (Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
        //     OnJumpInputUp();
        

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Space))
            OnDiveInput();
        
        if(Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.LeftShift))
            OnDashInput();
        #endregion

        if (!isJumping && !isGrounded) //If they aren't jumping and they aren't grounded, check if they've now become grounded
        {
            if (Physics2D.OverlapBox(groundCheckPoint.transform.position, groundCheckPoint.size, 0, groundLayer))
            {
                Grounded();
            }
        } 
        else if (isGrounded) //If they are grounded, check if they've become ungrounded
        {
            if (!Physics2D.OverlapBox(groundCheckPoint.transform.position, groundCheckPoint.size, 0, groundLayer))
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

        if (CanDash() && lastPressedDashTime > 0)
        {
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            // Sleep(Data.dashSleepTime); 

            Vector2 dashDirection = Vector2.left;
        
            //If not direction pressed, dash forward
            if (horizontalInput != 0f)
                dashDirection = horizontalInput > 0f ? Vector2.right : Vector2.left;
            else
                dashDirection = isFacingRight ? Vector2.right : Vector2.left;

            StartCoroutine(StartDash(dashDirection));
        }

        if (CanRun())
        {
            Run(horizontalInput);
        }

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

        if (!canDashSquash && Mathf.Abs(rb.velocity.x) >= movementData.velocityRequiredForDashSquashing)
        {
            ToggleCanDashSquashParticles(true);
        } 
        else if(canDashSquash && Mathf.Abs(rb.velocity.x) < movementData.velocityRequiredForDashSquashing)
        {
            ToggleCanDashSquashParticles(false);
        }
    }

    private void LateUpdate()
    {
        lastVelocity = rb.velocity;
        
        if(lastVelocity.y < fastestDownwardsVelocityWhileDiving)
            fastestDownwardsVelocityWhileDiving = lastVelocity.y;
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
        // isDiving = false;

        lastOnGroundTime = movementData.coyoteTimeBuffer;

        Jump();
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

        bool wasDiving = isDiving;
        
        isJumping = true;
        isJumpCut = false;
        isJumpFalling = false;
        isDiving = false;
        
        //Debug Purposes
        isJumpCut = true;

        lastPressedJumpTime = 0f;
        lastOnGroundTime = 0f;
        
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        float force = movementData.jumpForce;
        
        if (wasDiving)
        {
            float downwardVelocity = -fastestDownwardsVelocityWhileDiving;

            // Debug.Log(downwardVelocity + " is downward velocity");

            float baseFallVelocity = 15f;
            float maxFallVelocity = movementData.maxDiveSpeed;

            float percentAddedForce = 0f;
            percentAddedForce = (downwardVelocity - baseFallVelocity) / maxFallVelocity;
            percentAddedForce = Mathf.Clamp(percentAddedForce, 0f, 1f);

            // Debug.Log(percentAddedForce + " is percent?");
            
            float baseForce = movementData.jumpForce;
            float maxAdditionalForce = movementData.jumpForce * 0.5f;

            // force = baseForce + (maxAdditionalForce * percentAddedForce);
            force = baseForce + maxAdditionalForce;

            if (percentAddedForce > 0.25f)
            {
                GameController.Instance.ApplyShockwave(transform.position, percentAddedForce);
            }
        }
        
        Debug.Log("Force: " + force);
        
        rb.AddForce(Vector3.up * force, ForceMode2D.Impulse);

        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) //If the player is holding space bar, set their gravity to the normal scale
        {
            if (rb.gravityScale != movementData.gravityScaleWhenJumping)
            {
                SetGravityScale(movementData.gravityScaleWhenJumping);
            }
        }

        fastestDownwardsVelocityWhileDiving = 0f;
    }

    private bool CanRun()
    {
        if(isDashing)
            return false;

        return true;
    }

    private void Run(float horizontalInput)
    {
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

    private void CheckDirectionToFace(bool isMovingRight)
    {
        if (isMovingRight != isFacingRight)
            Turn();
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        
        isFacingRight = !isFacingRight;
    }

    private void OnDashInput()
    {
        lastPressedDashTime = movementData.jumpInputBufferTime;
    }

    private bool CanDash()
    {
        return true;
        return isGrounded; //Must be grounded to dash
    }
    
    private IEnumerator StartDash(Vector2 dir)
    {
        isDashing = true;
        
        float dashMaxForce = 20f;
        
        float duration = 0.1f;
        float timer = duration;
        WaitForFixedUpdate waiter = new WaitForFixedUpdate();
        while(timer > 0)
        {
            float lerp = timer / duration;
            //multiple lerp by quadratic curve to make it ease in
            lerp = Mathf.Pow(lerp, 2);
            
            Vector3 newVelocity = rb.velocity;
            newVelocity.x = dir.normalized.x * (dashMaxForce * lerp);
            rb.velocity = newVelocity;
            
            timer -= Time.deltaTime;
            yield return waiter;
        }

        isDashing = false;
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

    public void ToggleCanDashSquashParticles(bool canNowDashSquash)
    {
        canDashSquash = canNowDashSquash;
        
        if (canNowDashSquash)
        {
            canDashSquashParticles.Play();
        }
        else
        {
            canDashSquashParticles.Stop();
        }
    }

    public void SquashedBrick(Brick in_brick)
    {
        // AddExperience(in_brick.experienceValue);
        
        GameController.Instance.SpawnCarrot(in_brick.transform.position);

        Jump();
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
