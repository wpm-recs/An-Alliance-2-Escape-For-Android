using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatformBehavior : MonoBehaviour
{
    private bool isPlayerOnPlatform = false;
    public float countdownTimer = 2.5f;
    private float fallingSpeed = 30f;
    private Vector3 initialPosition;

    private void Start()
    {
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (isPlayerOnPlatform)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                Fall();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.position.y > transform.position.y + 0.1)
            isPlayerOnPlatform = true;
    }

    private void Fall()
    {
        Vector3 p = transform.localPosition;
        p -= ((fallingSpeed * Time.smoothDeltaTime) * transform.up);
        transform.localPosition = p;

        if (countdownTimer <= -5f)
        {
            countdownTimer = 2.5f;
            transform.localPosition = initialPosition;
            isPlayerOnPlatform = false;
        }
    }
}
