using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using System.Threading.Tasks;
using Unity.Services.Relay.Models;
using Unity.Networking.Transport.Relay;
using Unity.Netcode;
//using Mirror;
using UnityEngine.Networking;
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using System.Security.Cryptography;

public class move : NetworkBehaviour
{
    private NetworkVariable<int> num = new NetworkVariable<int>(1,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    // public override void OnNetworkSpawn()
    // {
    //     num.OnValueChanged += (int prev, int current) => {Debug.Log("num changed from "+prev+" to "+ current);};
    // }
    private GameObject egg;
    private float speed = 20f;
    private GameObject playerPrefab;
    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("OwnerClientId: " + OwnerClientId.ToString() + "Start");
    }

    // public override void OnNetworkSpawn()
    // {
    //     //Debug.Log("OwnerClientId: " + OwnerClientId.ToString() + "Spawn");
    //     Debug.Log("OnSceneEventHooked");
    //     NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;
    //     //NetworkServer.RegisterHandler(MsgType.Connect, OnPlayerConnected);
    // }
    private void SceneManager_OnSceneEvent(SceneEvent sceneEvent){
        Debug.Log($"Scene event {sceneEvent.SceneEventType} " +
                $"for scene {sceneEvent.SceneName} " +
                $"with a {nameof(SceneEventProgressStatus)}: ");
    }
    // Update is called once per frame
    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn");
        playerPrefab = GameObject.Find("GreenUpPrefab(Clone))");
        //playerPrefab.GetComponent<Renderer>().enabled = false;
        playerPrefab.GetComponent<SpriteRenderer>().enabled = false;
        //playerPrefab.GetComponent<NetworkObject>().NetworkHide(0);
        // playerPrefab.GetComponent<NetworkObject>().NetworkHide(1);
    }
    void Update()
    {
        if(IsOwner){
            if(Input.GetKey(KeyCode.W)) gameObject.transform.position += new Vector3(0, speed * Time.smoothDeltaTime, 0);
            if(Input.GetKey(KeyCode.S)) gameObject.transform.position += new Vector3(0, -speed * Time.smoothDeltaTime, 0);
            if(Input.GetKey(KeyCode.A)) gameObject.transform.position += new Vector3(-speed * Time.smoothDeltaTime, 0, 0);
            if(Input.GetKey(KeyCode.D)) gameObject.transform.position += new Vector3(speed * Time.smoothDeltaTime, 0, 0);

            if(Input.GetKeyDown(KeyCode.P)) {
                egg = Instantiate(Resources.Load("egg_prefab") as GameObject);
                egg.GetComponent<NetworkObject>().Spawn(true);
            }

            if(Input.GetKeyDown(KeyCode.O)) {
                TestServerRpc();
            }

            if(Input.GetKeyDown(KeyCode.I)) {
                TestClientRpc();
            }

            if(Input.GetKeyDown(KeyCode.U)) {
                TestStatus();
            }
        }
    }
    private void TestStatus(){
        Debug.Log("IsServer: "+IsServer.ToString());
        Debug.Log("IsClient: "+IsClient.ToString());
    }
    private void TestServerFunction(){
        Debug.Log("IsOwner" + IsOwner.ToString());
        Debug.Log("IsServer: "+IsServer.ToString());
        Debug.Log("IsClient: "+IsClient.ToString());
        Debug.Log("TestServerFunction called" + OwnerClientId);
    }

    [ServerRpc]  //always executed on server side
    private void TestServerRpc(){
        //Debug.Log("TestServerRpc called" + OwnerClientId);
        TestServerFunction();
    }

    [ClientRpc] //only can be called by server, but to be executed on all clients
    private void TestClientRpc(){
        Debug.Log("IsOwner" + IsOwner.ToString());
        Debug.Log("IsServer: "+IsServer.ToString());
        Debug.Log("IsClient: "+IsClient.ToString());
        Debug.Log("TestClientRpc called" + OwnerClientId);
    }
}
