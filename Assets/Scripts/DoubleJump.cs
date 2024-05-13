using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleJump : MonoBehaviour
{
    // Start is called before the first frame update
    OnGroundDector onGroundDector;
    int doubleJumpFlag=1;
    Rigidbody2D Rigidbody;
    void Start()
    {
        onGroundDector=GetComponent<OnGroundDector>();
        Rigidbody=GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(onGroundDector.TestOnGround()){
            doubleJumpFlag=1;
        }else if(!onGroundDector.TestOnGround()&&doubleJumpFlag==1&&Input.GetKeyDown(KeyCode.LeftShift)){
            Rigidbody.AddForce(transform.up * 6.0f, ForceMode2D.Impulse);
            doubleJumpFlag=0;
        }
    }
}

