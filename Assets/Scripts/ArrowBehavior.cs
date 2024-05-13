using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArrowBehavior : MonoBehaviour
{
    public int Direction;
    private const float Speed=10.0f;
    private int moveFlag = 0;
    public MaskDudeBehavior shooter;
    // Start is called before the first frame update
    void Start()
    {
        shooter = GameObject.Find("MaskDude").GetComponent<MaskDudeBehavior>();
        Direction=shooter.FaceDirection;
    }

    // Update is called once per frame
    void Update()
    {
        MoveALittleBit();
        Fly();
    }
    void Fly()
    {
        Vector3 Position = transform.position;
        Position.x += Time.deltaTime * Speed*Direction;
        transform.position = Position;
    }
    void MoveALittleBit()
    {
        if(moveFlag == 0)
            transform.position += new Vector3(0, 0.01f, 0);
        else
            transform.position -= new Vector3(0, 0.01f, 0);
        
        moveFlag = 1 - moveFlag;
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.CompareTag("DisappearingWall"))
        { 
            Destroy(other.gameObject);
        }     
        Destroy(gameObject);
        Debug.Log("kdkahsfbhf");
    }
    

    
}
