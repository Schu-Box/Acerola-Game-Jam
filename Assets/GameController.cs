using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public Transform brickParent;
    public GameObject brickPrefab;

    private float spawnWidth = 18f;
    public float timeBetweenSpawns = 2f;
    private float timer = 0f;

    void Start()
    {
        timer = timeBetweenSpawns;
    }
    
    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            SpawnNewBrick();
            timer = timeBetweenSpawns;
        }
    }

    private void SpawnNewBrick()
    {
        Vector3 randomPoint = new Vector3(Random.Range(-spawnWidth, spawnWidth), brickParent.transform.position.y, 0);
        GameObject newBrick = Instantiate(brickPrefab, randomPoint, Quaternion.identity, brickParent);
    }
}
