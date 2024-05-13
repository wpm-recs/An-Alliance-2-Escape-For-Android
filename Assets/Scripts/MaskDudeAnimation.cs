using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaskDudeAnimation : MonoBehaviour
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
        if(Input.GetKey(KeyCode.A)||Input.GetKey(KeyCode.D)){
            Isrunning=true;
        }
        else{
            Isrunning=false;
        }
        setfloats();
    }
    void setfloats(){
        Animator.SetBool("isrunning",Isrunning);
        Animator.SetFloat("velocityY",Rigidbody.velocity.y);
        Animator.SetBool("isonground",OnGroundDector.TestOnGround());
    }
}
