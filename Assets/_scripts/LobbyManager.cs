using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Networking.Match;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance;

    private testRelay relay;
    private LobbyManagerUI ui;
    private string playerName;

    private void Awake()
    {
        Instance = this;
        relay = GetComponent<testRelay>();
        ui = GetComponent<LobbyManagerUI>();
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log(AuthenticationService.Instance.PlayerId + " : has connected");
        };
        //await AuthenticationService.Instance.SignInAnonymouslyAsync();
        playerName = "Player" + UnityEngine.Random.Range(0, 10000);

    }

    private Player getPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
            }
        };
        //https:youtu.be/-KDlEBfCBiU?t=2990
    }

    private void Update()
    {
        HandleLobbyHeartBeat();
        HandleLobbyUpdate();
    }


    private Lobby hostLobby;
    private Lobby joinedLobby;

    private float heartBeatTimer = 0;
    private float heartBeatLobbyTimer = 0;
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

    private async void HandleLobbyUpdate()
    {
        if (joinedLobby != null)
        {
            heartBeatLobbyTimer += Time.deltaTime;
            if (heartBeatLobbyTimer > 1.1f)
            {
                heartBeatLobbyTimer = 0;

                joinedLobby = await LobbyService.Instance.GetLobbyAsync(joinedLobby.Id);

                if (!IsPlayerInLobby())
                {
                    // Player was kicked out of this lobby
                    Debug.Log("Kicked from Lobby!");

                    joinedLobby = null;
                }

                if (joinedLobby.Data["KEY_START_GAME"].Value != "0")
                {
                    //Start Game
                    if (!IsLobbyHost())
                    {
                        relay.JoinRelay(joinedLobby.Data["KEY_START_GAME"].Value);
                    }

                    joinedLobby = null;
                }
            }
        }
    }

    private bool IsPlayerInLobby()
    {
        if (joinedLobby != null && joinedLobby.Players != null)
        {
            foreach (Player player in joinedLobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                {
                    // This player is in this lobby
                    return true;
                }
            }
        }
        return false;
    }

    public bool IsLobbyHost()
    {
        return joinedLobby != null && joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
    }

    public void CreateLobbyClick()
    {
        CreateLobby();
    }

    public async void CreateLobby(string lobbyName = "My Lobby", int maxPlayers = 4, bool isPrivate = false)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = getPlayer(),
                Data = new Dictionary<string, DataObject> {
                    {
                        "Map", new DataObject(DataObject.VisibilityOptions.Public, "SampleScene")
                    },
                    {
                        "KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member, "0")
                    }
                }
            };

            Lobby currentLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            hostLobby = currentLobby;
            joinedLobby = hostLobby;

            Debug.Log(currentLobby.Name + " " + currentLobby.MaxPlayers + " "+ currentLobby.Id + " " + currentLobby.LobbyCode);
            ui.JoinedLobbyUI();
            PrintPlayer(hostLobby);
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
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 10,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created)
                }
            };
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            ui.RefreshLobbyUI(queryResponse);
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

    public void JoinLobbyClick()
    {
        JoinLobby();
    }

    private async void JoinLobby()
    {
        try
        {
            QueryResponse queryResponse = await Lobbies.Instance.QueryLobbiesAsync();

            await Lobbies.Instance.JoinLobbyByIdAsync(queryResponse.Results[0].Id);
            ui.JoinedLobbyUI();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinLobbyByCode(string code)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Player = getPlayer()

            };
            Lobby joinLobby =  await Lobbies.Instance.JoinLobbyByCodeAsync(code, options);
            joinedLobby = joinLobby;

            Debug.Log($"JoinLobby by code {code}");
            ui.JoinedLobbyUI();
            PrintPlayer(joinLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void JoinLobbyQuick()
    {
        try
        {
            await Lobbies.Instance.QuickJoinLobbyAsync();
            ui.JoinedLobbyUI();

            Debug.Log("Join random Lobby");
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public void PrintPlayers()
    {
        PrintPlayer(joinedLobby);
    }

    public void PrintPlayer(Lobby lobby)
    {
        //Debug.Log(lobby.Name);
        foreach (var player in lobby.Players)
        {
            Debug.Log(player.Id +" " + player.Data["PlayerName"].Value + " " + lobby.Data["Map"]);
        }
        ui.RefreshPlayerInLobbyUI(lobby);
    }

    public async void UpdateLobby(string map = "SampleScene")
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id,new UpdateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                    {"Map", new DataObject(DataObject.VisibilityOptions.Public, map) }
                }
            });
            joinedLobby = hostLobby;

            PrintPlayer(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void UpdatePlayer(string name /*, string spriteName*/)
    {
        try
        {
            playerName = name;
            await LobbyService.Instance.UpdatePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions   
            {
                Data = new Dictionary<string, PlayerDataObject>
                {
                    {"PlayerName", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, playerName) }
                }
            });
            joinedLobby = hostLobby;

            PrintPlayer(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void LeaveLobby()
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, AuthenticationService.Instance.PlayerId);
            ui.LeaveLobbyUI();
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void KickPlayer(string idToKick)
    {
        try
        {
            await LobbyService.Instance.RemovePlayerAsync(joinedLobby.Id, idToKick);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private async void MigrateLobbyHost(string newIdHost = "1")
    {
        try
        {
            hostLobby = await Lobbies.Instance.UpdateLobbyAsync(hostLobby.Id, new UpdateLobbyOptions
            {
                HostId = newIdHost,
            });
            joinedLobby = hostLobby;

            PrintPlayer(hostLobby);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void DeleteLobby()
    {
        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(joinedLobby.Id);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void StartGame()
    {
        if (IsLobbyHost())
        {
            try
            {
                Debug.Log("Start game");

                string relayCode = await relay.CreateRelay();

                Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(joinedLobby.Id, new UpdateLobbyOptions
                {
                    Data = new Dictionary<string, DataObject> {
                        {
                            "KEY_START_GAME", new DataObject(DataObject.VisibilityOptions.Member, relayCode)
                        } 
                    }
                });

                joinedLobby = lobby;
            }
            catch (LobbyServiceException e)
            {

            }
        }
    }
}
