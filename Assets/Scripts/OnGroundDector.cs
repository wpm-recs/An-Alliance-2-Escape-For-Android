using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundDector : MonoBehaviour
{
    private const float CheckRadius=0.01f;
    public LayerMask layermask;
    public bool isground;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        isground=Physics2D.OverlapCircle(transform.position,CheckRadius,layermask);
    }
    public bool TestOnGround(){
        return isground;
    }
}
