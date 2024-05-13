using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyAssets : MonoBehaviour {



    public static LobbyAssets Instance { get; private set; }


    [SerializeField] private Sprite MuskDudeSprite;
    [SerializeField] private Sprite NinjaFrogSprite;


    private void Awake() {
        Instance = this;
    }

    public Sprite GetSprite(LobbyManager.PlayerCharacter playerCharacter) {
        switch (playerCharacter) {
            default:
            case LobbyManager.PlayerCharacter.MuskDude:   return MuskDudeSprite;
            case LobbyManager.PlayerCharacter.NinjaFrog:    return NinjaFrogSprite;
        }
    }

}