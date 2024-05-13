using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ParticleGeneratorBehavior : MonoBehaviour
{
    public GameObject mudParticlePrefab;
    public GameObject iceParticlePrefab;
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
        Instantiate(mudParticlePrefab, spawnPosition, Quaternion.identity);

        float mirroredSpawnX = -24f + (-spawnX + (-24f));
        Vector3 mirroredSpawnPosition = new Vector3(mirroredSpawnX, -15f, 0f);
        Instantiate(iceParticlePrefab, mirroredSpawnPosition, Quaternion.identity);
    }
}