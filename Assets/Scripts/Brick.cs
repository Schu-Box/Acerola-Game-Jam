using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private float velocityRequiredForBonking = 3f;
    private float velocityRequiredForSquashing = -30f;

    private Rigidbody2D rb;

    private Vector3 lastVelocity;

    public int experienceValue = 1;

    private bool isSetup = false;
    public void Setup()
    {
        isSetup = true;
        
        rb = GetComponent<Rigidbody2D>();
        
        rb.gravityScale = BrickSpawner.Instance.brickGravityScale;
    }

    void Update()
    {
        if(!isSetup)
            return;
        
        lastVelocity = rb.velocity;
    }
    
    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.CompareTag("Player"))
        {
            PlayerController playerController = collision2D.gameObject.GetComponent<PlayerController>();
            
            //If it overlaps with the bonkCheckPoint, bonk the player
            if (Physics2D.OverlapBox(playerController.bonkCheckPoint.position, playerController.groundCheckSize, 0, playerController.groundLayer))
            {
                if(lastVelocity.magnitude > velocityRequiredForBonking && lastVelocity.y < 0) //If velocity is greater than the required amount and the brick is moving downwards
                {
                    Debug.Log("Bonked at velocity : " + lastVelocity.magnitude);

                    playerController.Bonked();
                }
            } 
            else if(Physics2D.OverlapBox(playerController.groundCheckPoint.position, playerController.groundCheckSize, 0, playerController.groundLayer))
            {
                // Debug.Log("player velocity is: " + playerController.lastVelocity.y);
                
                if(playerController.lastVelocity.y < velocityRequiredForSquashing)
                {
                    // Debug.Log("Squashed at velocity : " + playerController.lastVelocity.y);
                    
                    playerController.AddExperience(experienceValue);
                    
                    Squashed();
                }
            }
        }
    }

    public void Squashed()
    {
        Destroy(gameObject);
    }
}
