using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class LobbyUI : MonoBehaviour {


    public static LobbyUI Instance { get; private set; }


    [SerializeField] private Transform playerSingleTemplate;
    [SerializeField] private Transform container;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI gameModeText;
    [SerializeField] private Button leaveLobbyButton;
    [SerializeField] private Button changeGameModeButton;
    [SerializeField] private Button startGameButton;

    private string sceneName;


    private void Awake() {
        Instance = this;

        playerSingleTemplate.gameObject.SetActive(false);

        leaveLobbyButton.onClick.AddListener(() => {
            LobbyManager.Instance.LeaveLobby();
        });

        changeGameModeButton.onClick.AddListener(() => {
            LobbyManager.Instance.ChangeGameMode();
        });

        startGameButton.onClick.AddListener(() => {
            startGameButton.interactable = false;
            LobbyManager.Instance.StartGame();
        });
    }

    private void Start() {
        LobbyManager.Instance.OnJoinedLobby += UpdateLobby_Event;
        LobbyManager.Instance.OnJoinedLobbyUpdate += UpdateLobby_Event;
        LobbyManager.Instance.OnLobbyGameModeChanged += UpdateLobby_Event;
        LobbyManager.Instance.OnLeftLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnKickedFromLobby += LobbyManager_OnLeftLobby;
        LobbyManager.Instance.OnGameStarted += LobbyManager_OnLobbyGameStarted;
        Hide();
    }
    void Update(){
        sceneName = GameObject.Find("GameModeText").GetComponent<TextMeshProUGUI>().text;
    }

    private void LobbyManager_OnLobbyGameStarted(object sender, System.EventArgs e) {
        Invoke(nameof(GameStarted), 8f);
    }
    private void GameStarted(){
        Hide();
        if(LobbyManager.Instance.IsLobbyHost()){
            NetworkManager.Singleton.SceneManager.LoadScene(GetSceneName(),LoadSceneMode.Single);
        }
    }
    private string GetSceneName() {
        switch(sceneName){
            case("Level_1"):
                return "Level_1_Net";
            case("Level_2"):
                return "Level_2_Net";
            case("Level_3"):
                return "Level_3_Net";
            default:
                return "abort";
        }
    }
    private void LobbyManager_OnLeftLobby(object sender, System.EventArgs e) {
        ClearLobby();
        Hide();
    }

    private void UpdateLobby_Event(object sender, LobbyManager.LobbyEventArgs e) {
        UpdateLobby();
    }

    private void UpdateLobby() {
        UpdateLobby(LobbyManager.Instance.GetJoinedLobby());
    }

    private void UpdateLobby(Lobby lobby) {
        ClearLobby();
        try{
            if(LobbyManager.Instance.IsLobbyHost()){
            //通过PlayerId获取Player
            Player player = lobby.Players.Find(x => x.Id == AuthenticationService.Instance.PlayerId);
            if(player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.NinjaFrog.ToString()){
                LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.MuskDude);
            }
            }
            else{
                //通过PlayerId获取Player
                Player player = lobby.Players.Find(x => x.Id == AuthenticationService.Instance.PlayerId);
                if(player.Data[LobbyManager.KEY_PLAYER_CHARACTER].Value == LobbyManager.PlayerCharacter.MuskDude.ToString()){
                    LobbyManager.Instance.UpdatePlayerCharacter(LobbyManager.PlayerCharacter.NinjaFrog);
                }
            }
        }catch (System.Exception e) {
            Debug.Log(e);
        }
        

        foreach (Player player in lobby.Players) {
            Transform playerSingleTransform = Instantiate(playerSingleTemplate, container);
            playerSingleTransform.gameObject.SetActive(true);
            LobbyPlayerSingleUI lobbyPlayerSingleUI = playerSingleTransform.GetComponent<LobbyPlayerSingleUI>();

            lobbyPlayerSingleUI.SetKickPlayerButtonVisible(
                LobbyManager.Instance.IsLobbyHost() &&
                player.Id != AuthenticationService.Instance.PlayerId // Don't allow kick self
            );

            lobbyPlayerSingleUI.UpdatePlayer(player);
        }

        changeGameModeButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());
        startGameButton.gameObject.SetActive(LobbyManager.Instance.IsLobbyHost());

        lobbyNameText.text = lobby.Name;
        playerCountText.text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        gameModeText.text = lobby.Data[LobbyManager.KEY_GAME_MODE].Value;

        Show();
    }

    private void ClearLobby() {
        foreach (Transform child in container) {
            if (child == playerSingleTemplate) continue;
            Destroy(child.gameObject);
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}