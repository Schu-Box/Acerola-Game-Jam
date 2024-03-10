using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotPickupTrigger : MonoBehaviour
{
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
        Debug.Log("pick up");
        PlayerController.Instance.AddExperience(experienceValue);
        Destroy(transform.parent.gameObject);
    }
}
