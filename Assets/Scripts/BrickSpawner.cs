using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    public static BrickSpawner Instance;
    
    public Transform brickParent;
    public GameObject brickPrefab;

    public float spawnWidth = 17f;
    public float timeBetweenSpawns = 2f;
    private float timer = 0f;

    public float brickGravityScale = 1f;

    public int durability = 2;

    public GameObject particlePrefab;
    
    void Start()
    {
        Instance = this;
        
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
        Brick newBrick = Instantiate(brickPrefab, randomPoint, Quaternion.identity, brickParent).GetComponent<Brick>();
        newBrick.health = durability;

        newBrick.Setup();
    }
}
