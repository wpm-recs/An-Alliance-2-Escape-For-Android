using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void StartSelectLevel()
    {
        SceneManager.LoadScene("SelectLevel");
    }

    public void StartNetworkGame(){
        SceneManager.LoadScene("Lobby");
    }

    public void OnApplicationQuit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
