using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Disappearedplatform : MonoBehaviour
{
    private bool isPlayerOnPlatform = false;
    private float countdownTimer = 3.0f;
    private Vector3 initialPosition;
    private Collider2D coll;
    private SpriteRenderer sprite;

    private void Start()
    {
        initialPosition = transform.localPosition;
        coll=GetComponent<Collider2D>();
        sprite=GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isPlayerOnPlatform)
        {
            countdownTimer -= Time.deltaTime;
            if (countdownTimer <= 0)
            {
                Disappear();
            }
        }
    }
    private void FixedUpdate()
    {
        if (isPlayerOnPlatform)
        {
            GetComponent<SpriteRenderer>().enabled = !GetComponent<SpriteRenderer>().enabled;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isPlayerOnPlatform = true;
    }
    public void Reset(){
        isPlayerOnPlatform = false;
        countdownTimer = 3.0f;
         coll.enabled=true;
        sprite.enabled=true;
        Debug.Log("disappearing plat reset");
    }
    private void Disappear()
    {
        coll.enabled=false;
        sprite.enabled=false;
}
}