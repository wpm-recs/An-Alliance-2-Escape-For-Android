using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPointBehaviorNet : NetworkBehaviour
{

    private bool muskDudeCollided = false;
    private bool ninjaFrogCollided = false;
    private AudioSource AudioSource;
    public AudioClip endpoint;

    public override void OnNetworkSpawn()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "MaskDude(Clone)")
        {
            muskDudeCollided = true;
            Debug.Log("muskDudeCollided");
            CheckBothCharactersCollided();
        }
        if (collision.gameObject.name == "NinjaFrog(Clone)")
        {
            ninjaFrogCollided = true;
            Debug.Log("ninjaFrogCollided");
            CheckBothCharactersCollided();
        }
    }

    private void CheckBothCharactersCollided()
    {
        if (muskDudeCollided && ninjaFrogCollided)
        {
            AudioSource.clip = endpoint;
            AudioSource.Play();
            Invoke("EnterNextLevelWithDelay", endpoint.length);
        }
    }

    private void EnterNextLevelWithDelay()
    {
        if(IsHost){
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }
    }
    public override void OnNetworkDespawn()
    {
        if(!IsHost){
            NetworkManager.Singleton.Shutdown();
            SceneManager.LoadScene("MainMenu");
        }
    }
}
