using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float smoothing;
    public float delayTime;

    private bool isDelayed = true;
    public float cinemachineDelayTime = 2.0f;
    private float timer = 0f;
    public Vector3 desiredPos;
    private GameObject canva;
    private GameObject healthBarCanva;

    private void Awake()
    {
        healthBarCanva = GameObject.Find("HealthBarCanvas");
    }

    void Start()
    {
        timer = 0f;
        transform.position = desiredPos;
        canva = GameObject.Find("Canvas");
        if(healthBarCanva != null) healthBarCanva.SetActive(false);
    }

    void Update()
    {
        if (isDelayed)
        {
            //Debug.Log("timer"+timer+" "+gameObject.name);
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                isDelayed = false;
                canva.SetActive(false);
                if(healthBarCanva != null) healthBarCanva.SetActive(true);
                Invoke("EnableCinemachine", cinemachineDelayTime);
            }
        }
        else
        {
            if (target != null)
            {
                Vector3 targetPos = target.position;
                Vector2 Framposition = Vector2.Lerp(transform.position, targetPos, smoothing);
                transform.position = new Vector3(Framposition.x, Framposition.y, -10);

            }
        }
    }
    void EnableCinemachine()
    {
        gameObject.GetComponent<CinemachineBrain>().enabled = true;
    }
}

