using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    public List<Sprite> damageSpriteList;
    public SpriteRenderer damageSpriteRenderer;
    
    private float velocityRequiredForBonking = 3f;

    private Rigidbody2D rb;

    private Vector3 lastVelocity;

    public int experienceValue = 1;

    public int health = 3;
    private int damage = 0;

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
    
    // public void OnCollisionEnter2D(Collision2D collision2D)
    // {
    //     if (collision2D.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("Collided with player!");
    //         
    //         PlayerController playerController = collision2D.gameObject.GetComponent<PlayerController>();
    //         
    //         //If it overlaps with the bonkCheckPoint, bonk the player
    //         // if (Physics2D.OverlapBox(playerController.bonkCheckPoint.transform.position, playerController.bonkCheckPoint.size, 0, playerController.groundLayer))
    //         // {
    //         //     if(lastVelocity.magnitude > velocityRequiredForBonking && lastVelocity.y < 0) //If velocity is greater than the required amount and the brick is moving downwards
    //         //     {
    //         //         Debug.Log("Bonked at velocity : " + lastVelocity.magnitude);
    //         //
    //         //         playerController.Bonked();
    //         //     }
    //         // } 
    //         
    //         // if(Physics2D.OverlapBox(playerController.groundCheckPoint.transform.position, playerController.groundCheckPoint.size, 0, playerController.groundLayer))
    //         // {
    //         //     // Debug.Log("player velocity is: " + playerController.lastVelocity.y);
    //         //     
    //         //     // if(playerController.CanSquash)
    //         //     // {
    //         //         // Debug.Log("Squashed at velocity : " + playerController.lastVelocity.y);
    //         //         
    //         //         Squashed();
    //         //     // }
    //         // } 
    //         
    //         if (playerController.CanDashSquash)
    //         {
    //             Squashed();
    //         }
    //     }
    // }

    public void Bonked()
    {
        Debug.Log("BONKED!");
        PlayerController.Instance.Bonked();
    }

    public void Squashed()
    {
        Debug.Log("SQUASH!");
        
        damage++;

        damageSpriteRenderer.sprite = damageSpriteList[damage];
        
        if (damage >= health)
        {
            GameController.Instance.ApplyShockwave(transform.position);
            
            PlayerController.Instance.SquashedBrick(this);
            Destroy(gameObject);
        }
    }
}
