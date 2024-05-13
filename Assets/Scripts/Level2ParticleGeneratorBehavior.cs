using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2ParticleGeneratorBehavior : MonoBehaviour
{
    public GameObject sandParticlePrefab;
    public Vector2 spawnRange;
    public float spawnInterval;
    private float timer;

    private void Start()
    {
        timer = spawnInterval;
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnBall();
            timer = spawnInterval;
        }
    }

    private void SpawnBall()
    {
        float spawnX = Random.Range(spawnRange.x, spawnRange.y);
        Vector3 spawnPosition = new Vector3(spawnX, -30f, 0f);
        Instantiate(sandParticlePrefab, spawnPosition, Quaternion.identity);

    }
}
