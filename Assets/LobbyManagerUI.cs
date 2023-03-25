using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManagerUI : MonoBehaviour
{
    private LobbyManager lobbyManager;

    [SerializeField] private GameObject listLobbyMenu;
    public Transform LobbyList;
    [SerializeField] private GameObject prefabLobby;

    [SerializeField] private GameObject lobbyPlayersMenu;
    public Transform PlayersList;
    [SerializeField] private GameObject prefabPlayer;

    private void Awake()
    {
        lobbyManager = GetComponent<LobbyManager>();
    }

    public void RefreshLobbyUI(QueryResponse queryResponse)
    {
        foreach (Transform lobbyG in GetChildren(LobbyList))
        {
            Destroy(lobbyG);
        }

        foreach (var lobbyInList in queryResponse.Results)
        {
            LobbyUI lobbyUi = Instantiate(prefabLobby, LobbyList).GetComponent<LobbyUI>();
            lobbyUi.SetInfoOnUILobby(lobbyInList);
        }
    }

    public void RefreshPlayerInLobbyUI(Lobby currentLobby)
    {
        foreach (Transform playerUI in GetChildren(PlayersList))
        {
            Destroy(playerUI.gameObject);
        }

        foreach (var lobbyInList in currentLobby.Players)
        {
            PlayerLobbyUI playerUI = Instantiate(prefabPlayer, PlayersList).GetComponent<PlayerLobbyUI>();
            playerUI.SetInfoOnUIPLyaer(lobbyInList);
        }
    }

    public List<Transform> GetChildren(Transform parent)
    {
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            children.Add(child);
        }
        return children;
    }

    public void JoinedLobbyUI()
    {
        listLobbyMenu.gameObject.SetActive(false);
        lobbyPlayersMenu.gameObject.SetActive(true);
    }

    public void LeaveLobbyUI()
    {
        listLobbyMenu.gameObject.SetActive(true);
        lobbyPlayersMenu.gameObject.SetActive(false);
    }
}
