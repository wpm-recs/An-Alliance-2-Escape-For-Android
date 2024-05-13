using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ArrowBehaviorNet : NetworkBehaviour
{
    public int Direction;
    private const float Speed=10.0f;
    private int moveFlag = 0;
    private MaskDudeBehaviorNet shooter;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void OnNetworkSpawn()
    {
        shooter = GameObject.Find("MaskDude(Clone)").GetComponent<MaskDudeBehaviorNet>();
        Direction=shooter.FaceDirection.Value;
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
            transform.position += new Vector3(0, 0.011f, 0);
        else
            transform.position -= new Vector3(0, 0.011f, 0);
        
        moveFlag = 1 - moveFlag;
    }

    void OnCollisionEnter2D(Collision2D other) 
    {
        if(other.gameObject.CompareTag("DisappearingWall"))
        {
            //Destroy(other.gameObject);
            other.gameObject.GetComponent<NetworkObject>().Despawn();
        } 
        gameObject.GetComponent<NetworkObject>().Despawn();
    }
    

    
}
