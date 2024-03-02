using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brick : MonoBehaviour
{
    private float velocityRequiredForBonking = 5f;

    private Rigidbody2D rb;

    private Vector3 lastVelocity;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
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
                Debug.Log("Velocity: " + lastVelocity.magnitude);
                
                if(lastVelocity.magnitude > velocityRequiredForBonking && lastVelocity.y < 0) //If velocity is greater than the required amount and the brick is moving downwards
                {
                    playerController.Bonked();
                }
            }
        }
    }
}
