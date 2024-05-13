using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePause : MonoBehaviour
{
    bool paused=false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SwitchPauseTest();
    }
    void SwitchPauseTest(){
        if(Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("ESC");
            if(paused){
                paused=false;
                Time.timeScale=1;
            }
            else {
                paused=true;
                Time.timeScale=0;
            }
        }
    }
}
