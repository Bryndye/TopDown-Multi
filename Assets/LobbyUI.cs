using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies.Models;

public class LobbyUI : MonoBehaviour
{
    public TextMeshProUGUI LobbyName;
    public TextMeshProUGUI PlayersCount;
    public string CodeLobby;
    [SerializeField] private Lobby myLobby;

    public void SetInfoOnUILobby(Lobby lobby)
    {
        myLobby = lobby;
        PlayersCount.text = $"{myLobby.Players.Count} / {myLobby.MaxPlayers }";
        LobbyName.text = $"{myLobby.Name}";
        CodeLobby = myLobby.LobbyCode;
    }

    public void JoinLobby()
    {
        if (myLobby != null)
        {
            LobbyManager.Instance.JoinLobbyByCode(myLobby.LobbyCode);
        }
    }
}
