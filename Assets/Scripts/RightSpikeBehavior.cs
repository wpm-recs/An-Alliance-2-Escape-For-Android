using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightSpikeBehavior : MonoBehaviour
{
    private Vector3 InitialPosition;
    public float speed=1.0f;
    public float distence=10f;
    public bool active=false;
    public Transform p1 = null;
    public float TestLength =2.0f;
    private float timer = 0f;
    public float delayTime = 6.0f;
    private bool isMoving = false;
    void Awake(){
    }
    // Start is called before the first frame update
    void Start()
    {
        InitialPosition=transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("timer"+timer.ToString());
        if (!isMoving)
        {
            timer += Time.deltaTime;
            if (timer >= delayTime)
            {
                isMoving = true;
            }
        }
        else
        {
            if(p1!=null)
                TestPlayer();
                if(active) Move();
        }
    }
    void TestPlayer(){
        // Debug.Log("distance:"+Mathf.Abs(p1.position.y-InitialPosition.y).ToString());
        // Debug.Log("TestLength:"+TestLength.ToString());
        // Debug.Log("active:"+(Mathf.Abs(p1.position.y-InitialPosition.y)<=TestLength).ToString());
        if(Mathf.Abs(p1.position.y-InitialPosition.y)<=TestLength){
            active=true;
        }
    }
    void Move(){
        Vector3 p=transform.position;
        if((p-InitialPosition).magnitude<=distence){
            p+=transform.right*speed*Time.deltaTime;
            transform.position=p;
        }
        else{
            Reset();
        }
    }

    public void Reset()
    {
        transform.position = InitialPosition;
        active = false;
    }
}
