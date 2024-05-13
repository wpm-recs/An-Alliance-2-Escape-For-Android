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
using Unity.Netcode.Transports.UTP;
using UnityEngine.UI;
using TMPro;

public class relayHelper : MonoBehaviour
{
    private string joinCode;
    public static relayHelper Instance{get;private set;}
    private void Awake(){
        Instance = this;
    }
    // Start is called before the first frame update
    // private async void Start()
    // {
    //     await UnityServices.InitializeAsync();
    //     AuthenticationService.Instance.SignedIn += ()=>
    //     {
    //         Debug.Log("Signed in"+AuthenticationService.Instance.PlayerId);
    //     };
    //     await AuthenticationService.Instance.SignInAnonymouslyAsync();
    // }
    
    private async Task<Allocation> AllocateRelay() {
        try {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);

            return allocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);

            return default;
        }
    }

    public async Task<string> CreateRelay() {
        try {
            Allocation allocation = await AllocateRelay();
            joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("joinCode:"+joinCode);
            RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartHost();
            return joinCode;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return "0";
        }
    }

    public async Task<JoinAllocation> JoinRelay(string joinCode) {
        try {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
            NetworkManager.Singleton.StartClient();
            return joinAllocation;
        } catch (RelayServiceException e) {
            Debug.Log(e);
            return default;
        }
    }

    public void UpdateKey(string key){
        joinCode = key;
        Debug.Log("keycode: "+joinCode);
    }
    // Update is called once per frame
    void Update()
    {
    }
}
