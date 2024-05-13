using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flagBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public int authFlag = 0;
    void Start()
    {
        if(GameObject.FindGameObjectsWithTag("GameController").Length == 1)
            DontDestroyOnLoad(this.gameObject);
        else{
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
