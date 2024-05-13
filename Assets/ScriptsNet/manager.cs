using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using UnityEngine;

public class manager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // AuthenticationService.Instance.SignOut();
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            Destroy(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
