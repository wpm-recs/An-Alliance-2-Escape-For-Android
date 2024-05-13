using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatformBehavior : MonoBehaviour
{
    private bool isPlayerOnPlatform = false;
    private float movingSpeed = 3f;
    private Vector3 targetPosition;
    private Vector3 startPosition;
    private bool isMoving = false;
    private bool isReturning = false;
    private float returnDelay = 2f;
    private float returnTimer = 0f;
    public Vector3 movingPosition;
    void Start()
    {
        targetPosition = transform.position + movingPosition;
        startPosition = transform.position;
    }

    void Update()
    {
        if (isPlayerOnPlatform && isMoving)
        {
            Move();
        }
        else if (isReturning)
        {
            Return();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((transform.position - startPosition).magnitude <= 0.01f)
        {
            isMoving = true;
        }
        isPlayerOnPlatform = true;
        //isMoving = true;

    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, movingSpeed * Time.deltaTime);

        if (transform.position == targetPosition)
        {
            isMoving = false;
            returnTimer = 0f;
            isReturning = true;
        }
    }

    private void Return()
    {
        returnTimer += Time.deltaTime;

        if (returnTimer >= returnDelay)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPosition, movingSpeed * Time.deltaTime);

            if (transform.position == startPosition)
            {
                isReturning = false;
                isPlayerOnPlatform = false;
                isMoving = true;
            }
        }
    }
    public void Reset()
    {
        transform.position = startPosition;
        isPlayerOnPlatform = false;
        isMoving = false;
        isReturning = false;
    }
}
