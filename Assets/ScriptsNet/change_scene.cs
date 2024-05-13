using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class change_scene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(changeScene);
    }
    private void changeScene() {
        var status = NetworkManager.Singleton.SceneManager.LoadScene("Level_1_Net",LoadSceneMode.Single);
        if (status != SceneEventProgressStatus.Started)
        {
            Debug.LogWarning($"Failed to load newScene " +
                $"with a {nameof(SceneEventProgressStatus)}: {status}");
        }
    // UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
