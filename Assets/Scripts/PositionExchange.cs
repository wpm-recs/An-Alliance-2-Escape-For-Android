using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionExchange : MonoBehaviour
{
    public float cooldownmax=1.0f;
    private float cooldownnow;
    private Vector3 temporaryposition;
    public Transform charactor1;
    public Transform charactor2;
    // Start is called before the first frame update
    void Start()
    {
        cooldownnow=cooldownmax;
    }

    // Update is called once per frame
    void Update()
    {
        cooldownnow-=Time.deltaTime;
        if(cooldownnow<=0&&Input.GetKey(KeyCode.S)&&Input.GetKey(KeyCode.K)&&Mathf.Abs(charactor1.gameObject.GetComponent<Rigidbody2D>().velocity.x)<0.01&&Mathf.Abs(charactor2.gameObject.GetComponent<Rigidbody2D>().velocity.x)<0.01){
            temporaryposition=charactor1.position;
            charactor1.position=charactor2.position;
            charactor2.position=temporaryposition;
            cooldownnow=cooldownmax;
        }
    }
}
