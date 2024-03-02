using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    private float timeUntilGameOver = 1f;
    
    private Dictionary<GameObject, Coroutine> gameOverTimers = new Dictionary<GameObject, Coroutine>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.gameObject.CompareTag("Player"))
        {
            gameOverTimers.Add(other.gameObject, StartCoroutine(GameOverTimer()));
        }
    }
    
    private IEnumerator GameOverTimer()
    {
        yield return new WaitForSeconds(timeUntilGameOver);
        GameController.Instance.GameOver();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
         if(gameOverTimers.ContainsKey(other.gameObject))
         {
              StopCoroutine(gameOverTimers[other.gameObject]);
              gameOverTimers.Remove(other.gameObject);
         }
    }
}
