using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine.UI;
using Unity.Services.Lobbies;

public class LobbyListSingleUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playersText;
    [SerializeField] private TextMeshProUGUI gameModeText;


    private Lobby lobby;


    private void Awake() {
        
            GetComponent<Button>().onClick.AddListener(() => {
                try {
                    LobbyManager.Instance.JoinLobby(lobby);
                } catch (LobbyServiceException e) {
                    Debug.Log(e);
                    LobbyManager.Instance.RefreshLobbyList();
                }
                catch (System.Exception e) {
                    Debug.Log(e);
                    LobbyManager.Instance.RefreshLobbyList();
                }
            });
        
    }

    public void UpdateLobby(Lobby lobby) {
        this.lobby = lobby;

        lobbyNameText.text = lobby.Name;
        playersText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        gameModeText.text = lobby.Data[LobbyManager.KEY_GAME_MODE].Value;
    }


}