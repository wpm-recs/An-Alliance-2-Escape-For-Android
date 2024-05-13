using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleBehavior : MonoBehaviour
{

    public float speed;
    public float targetHeight;

    public float oscillationSpeed;
    public float oscillationAmount;

    private float initialXPosition;
    private float randomPhase;

    private void Start()
    {
        initialXPosition = transform.position.x;
        randomPhase = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        if (transform.position.y < targetHeight)
        {

            float oscillationOffset = Mathf.Sin((Time.time * oscillationSpeed) + randomPhase) * oscillationAmount;
            float xPosition = initialXPosition + oscillationOffset;
            Vector3 newPosition = new Vector3(xPosition, transform.position.y + (speed * Time.smoothDeltaTime), transform.position.z);
            transform.position = newPosition;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
