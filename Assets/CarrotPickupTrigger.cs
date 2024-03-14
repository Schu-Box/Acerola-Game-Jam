using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotPickupTrigger : MonoBehaviour
{
    private bool pickedUp = false;
    
    public int experienceValue = 1;
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PickupCarrot();
        }
    }

    private void PickupCarrot()
    {
        if(pickedUp)
            return;
        pickedUp = true;
        
        PlayerController.Instance.AddExperience(experienceValue);
        Destroy(transform.parent.gameObject);
    }
}
