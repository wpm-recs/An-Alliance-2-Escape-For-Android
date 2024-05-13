using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MudParticleGeneratorBehavior : MonoBehaviour
{
    public GameObject particlePrefab;
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
        Vector3 spawnPosition = new Vector3(spawnX, -15f, 0f);
        Instantiate(particlePrefab, spawnPosition, Quaternion.identity);
    }
}
