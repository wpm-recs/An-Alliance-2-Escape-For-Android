using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndPointBehavior : MonoBehaviour
{

    private bool muskDudeCollided = false;
    private bool ninjiaFrogCollided = false;
    private AudioSource AudioSource;
    public AudioClip endpoint;

    void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "MaskDude")
        {
            muskDudeCollided = true;
            CheckBothCharactersCollided();
        }
        else if (collision.gameObject.name == "NinjaFrog")
        {
            ninjiaFrogCollided = true;
            CheckBothCharactersCollided();
        }
    }

    private void CheckBothCharactersCollided()
    {
        if (muskDudeCollided && ninjiaFrogCollided)
        {
            AudioSource.clip = endpoint;
            AudioSource.Play();
            Invoke("EnterNextLevelWithDelay", endpoint.length);
        }
    }

    private void EnterNextLevelWithDelay()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
