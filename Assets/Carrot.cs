using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carrot : MonoBehaviour
{
    public Collider2D collider2D;
    public CarrotPickupTrigger carrotPickupTrigger;

    public Rigidbody2D rb;
    
    public SpriteRenderer spriteRenderer;
    public Sprite seedSprite;
    public Sprite carrotSprite;

    private bool canPlant = false;
    private bool planted = false;

    
    public void Spawn(Vector2 in_position)
    {
        planted = false;
        canPlant = false;
        // collider2D.gameObject.SetActive(false);
        
        gameObject.layer = LayerMask.NameToLayer("Seed");
        
        carrotPickupTrigger.gameObject.SetActive(false);
        spriteRenderer.sprite = seedSprite;
        
        transform.position = in_position;
        
        float forceMagnitude = 20f;
        Vector2 force = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f).normalized * forceMagnitude;
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (!canPlant && rb.velocity.y < 0f)
        {
            canPlant = true;
            // collider2D.gameObject.SetActive(true);
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (canPlant)
        {
            Plant();
        }
    }

    public void Plant()
    {
        if (!planted)
        {
            Debug.Log("PLANTED!");

            planted = true;

            spriteRenderer.sprite = carrotSprite;
            carrotPickupTrigger.gameObject.SetActive(true);
            
            gameObject.layer = LayerMask.NameToLayer("Carrot");
            foreach (Transform child in transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("Carrot");
            }
        }
    }
}
