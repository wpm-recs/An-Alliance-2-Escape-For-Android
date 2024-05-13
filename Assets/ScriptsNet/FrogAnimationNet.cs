using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class FrogAnimationNet : NetworkBehaviour
{
    private bool Isrunning=false;
    private Animator Animator;
    private Rigidbody2D Rigidbody;
    private OnGroundDector OnGroundDector;
    // Start is called before the first frame update
    void Start()
    {
        Animator=GetComponent<Animator>();
        Rigidbody=GetComponent<Rigidbody2D>();
        OnGroundDector=GetComponent<OnGroundDector>();
    }

    // Update is called once per frame
    void Update()
    {
        if(IsOwner){
            if(Input.GetKey(KeyCode.J)||Input.GetKey(KeyCode.L)){
                Isrunning=true;
            }
            else{
                Isrunning=false;
            }
            setfloats();
        }
    }
    void setfloats(){
        Animator.SetBool("isrunning",Isrunning);
        Animator.SetFloat("velocityY",Rigidbody.velocity.y);
        Animator.SetBool("isonground",OnGroundDector.TestOnGround());
    }
}
