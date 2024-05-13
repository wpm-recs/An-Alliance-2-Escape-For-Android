using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectLevel : MonoBehaviour
{
    public void StartLevelOneLocal()
    {
        SceneManager.LoadScene("Level_1");
    }

    public void StartLevelTwoLocal()
    {
        SceneManager.LoadScene("Level_2");
    }

    public void StartLevelThreeLocal()
    {
        SceneManager.LoadScene("Level_3");
    }
}
