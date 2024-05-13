using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointBehavior : MonoBehaviour
{
    Vector3 position;
    private AudioSource AudioSource;
    public AudioClip checkpoint;
    public float checkPointCountDownMax = 10f;
    private float checkPointCountDown = 0f;


    // Start is called before the first frame update
    void Start()
    {
        position=transform.position;
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(checkPointCountDown > 0f)
        {
            checkPointCountDown -= Time.deltaTime;
        }
        else
        {
            checkPointCountDown = 0f;
        }
    }
    void OnTriggerEnter2D(Collider2D collision){
        if(checkPointCountDown == 0f){
            if(collision.gameObject.GetComponent<MaskDudeBehaviorNet>()!=null && collision.gameObject.GetComponent<MaskDudeBehaviorNet>().IsOwner){
                collision.gameObject.GetComponent<MaskDudeBehaviorNet>().SpawnPoint=position;
                AudioPlay();
            }
            if(collision.gameObject.GetComponent<MaskDudeBehavior>()!=null){
                collision.gameObject.GetComponent<MaskDudeBehavior>().SpawnPoint=position;
                AudioPlay();
            }
            if(collision.gameObject.GetComponent<FrogBehaviorNet>()!=null && collision.gameObject.GetComponent<FrogBehaviorNet>().IsOwner){
                collision.gameObject.GetComponent<FrogBehaviorNet>().SpawnPoint=position;
                AudioPlay();
            }
            if(collision.gameObject.GetComponent<FrogBehavior>()!=null){
                collision.gameObject.GetComponent<FrogBehavior>().SpawnPoint=position;
                AudioPlay();
            }
        }
    }

    void AudioPlay()
    {
        AudioSource.clip = checkpoint;
        AudioSource.Play();
        StartCountDown();
    }
    void StartCountDown()
    {
        checkPointCountDown = checkPointCountDownMax;
    }
}
