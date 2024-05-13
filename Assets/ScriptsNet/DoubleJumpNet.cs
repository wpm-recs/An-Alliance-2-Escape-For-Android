using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoubleJumpNet : NetworkBehaviour
{
    private bool doubleJumpFlag = false;
    private int JumpForce = 6;
    private OnGroundDector OngroundDector;
    private Rigidbody2D Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        OngroundDector=GetComponent<OnGroundDector>();
        Rigidbody=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsHost){
            if(Input.GetKeyDown(KeyCode.LeftShift)){
                Debug.Log("DoubleJump trigger");
                DoubleJumpClientRpc();
            }
        }
        if(!IsHost){
            if(OngroundDector.TestOnGround()){
                doubleJumpFlag = true;
            }
        }
    }

    [ClientRpc]
    private void DoubleJumpClientRpc(){
        if(!IsHost){
            Debug.Log("DoubleJumpClientRpc called");
            if(doubleJumpFlag && !OngroundDector.TestOnGround()){
                //Rigidbody.velocity = new Vector2(Rigidbody.velocity.x,0);
                Rigidbody.AddForce(transform.up*JumpForce,ForceMode2D.Impulse);
                doubleJumpFlag = false;
            }
        }
    }
}
