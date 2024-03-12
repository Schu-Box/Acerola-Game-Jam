using System;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Shockwave : MonoBehaviour
{
    public Transform masterParent;
    public Transform scaleParent; //Only effected by feedback, always between 0 and 1
    public MMF_Player spawnFeedback;

    private float minShockwaveForce = 10f;
    private float maxShockwaveForce = 100f;

    private float additionalUpwardForce = 50f;

    private float minShockwaveRadius = 0.5f;
    private float maxShockwaveRadius = 2f;
    
    private List<Brick> shockwavedBricks = new List<Brick>();

    private float shockwavePercent = 0f;

    public void Spawn(Vector3 in_position, float in_percentage)
    {
        shockwavePercent = in_percentage;
        
        Vector3 shockwaveScale = Vector3.one * Mathf.Lerp(minShockwaveRadius, maxShockwaveRadius, in_percentage);
        
        masterParent.position = in_position;
        masterParent.localScale = shockwaveScale;
        //TODO: If shockwave size increases, do it here NOT ON SCALEPARENT!
        
        scaleParent.localScale = Vector3.zero;
        
        spawnFeedback.PlayFeedbacks();
    }
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trig entered");
        
        Brick brick = other.gameObject.GetComponent<Brick>();
        if (brick != null && !shockwavedBricks.Contains(brick))
        {
            shockwavedBricks.Add(brick); //Prevent double shockwaving
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            
            float shockwaveForce = Mathf.Lerp(minShockwaveForce, maxShockwaveForce, shockwavePercent);

            Vector2 force = (rb.position - (Vector2)transform.position).normalized * shockwaveForce;
            force.y += (additionalUpwardForce * shockwavePercent);
            
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }

    // public void OnCollisionEnter2D(Collision2D collision)
    // {
    //     Debug.Log("trig entered");
    //
    //     if (collision.otherCollider.gameObject.CompareTag("Brick"))
    //     {
    //         Debug.Log("ADD FORCE!");
    //         Rigidbody2D rb = collision.otherCollider.GetComponent<Rigidbody2D>();
    //         rb.AddForce((rb.position - (Vector2)transform.position).normalized * shockwaveForce, ForceMode2D.Impulse);
    //     }
    // }
}
