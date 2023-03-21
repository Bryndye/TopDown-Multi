using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log(AuthenticationService.Instance.PlayerId + " : has connected");
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
    }


    private Lobby hostLobby;
    private float heartBeatTimer = 0;
    private async void HandleLobbyHeartBeat()
    {
        if (hostLobby != null)
        {
            heartBeatTimer += Time.deltaTime;
            if (heartBeatTimer > 15)
            {
                heartBeatTimer = 0;

                await LobbyService.Instance.SendHeartbeatPingAsync(hostLobby.Id);
            }
        }
    }
    public void CreateLobbyClick()
    {
        CreateLobby();
    }

    public async void CreateLobby(string lobbyName = "My Lobby", int maxPlayers = 4)
    {
        try
        {
            Lobby currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers);
               hostLobby = currentLobby;
            Debug.Log(currentLobby.Name + " " + currentLobby.MaxPlayers);
        }
        catch(LobbyServiceException e) { 
            Debug.Log(e);
        }
    }
    public void RefreshListLobbiesClick()
    {
        listLobbies();
    }


    private async void listLobbies()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            Debug.Log(queryResponse.Results.Count);
            foreach (Lobby result in queryResponse.Results)
            {
                Debug.Log($"{result.Name} : {result.MaxPlayers}");
            }
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }
}
