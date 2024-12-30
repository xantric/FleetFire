using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    Lobby JoinedLobby;
    float heartBeatInterval = 15f, heartBeatTimer = 0.0f;
    float lobbyUpdateInterval = 5f, lobbyUpdateTimer = 0.0f;
    void Update()
    {
        HandleHeartbeat(JoinedLobby.Id);
        UpdateLobbyDemo(JoinedLobby.Id);
    }
    async void UpdateLobbyDemo(string id)
    {
        var lobby = await UpdateLobby(id);
        if(lobby != null)
        {
            Debug.Log("Lobby updated: " + lobby.Name);
            JoinedLobby = lobby;
        }
    }
    
    public async void Authenticate(string playerName)
    {
        InitializationOptions options = new InitializationOptions();
        options.SetProfile(playerName);
        
        await UnityServices.InitializeAsync(options); // Initialize Unity Services

        await AuthenticationService.Instance.SignInAnonymouslyAsync(); // Sign in anonymously

        if(AuthenticationService.Instance.IsSignedIn) // Check if the user is signed in
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        }

    }
    public async void CreateLobby(string lobbyName, int maxPlayers, string PlayerName, bool isPrivate = false)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = CreatePlayer(PlayerName)
            };

            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options); // Create a lobby with a name and a max number of players
            Debug.Log("Lobby created: " + lobby.Id + " - " + lobby.Name + " - " + lobby.LobbyCode + " by " + PlayerName);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }
    async void HandleHeartbeat(string lobbyId = null)
    {
        heartBeatTimer += Time.deltaTime;
        try
        {
            if (heartBeatTimer > heartBeatInterval && lobbyId != null) // Check if the heartbeat interval has passed
            {
                heartBeatTimer = 0.0f;
                await LobbyService.Instance.SendHeartbeatPingAsync(lobbyId); // Send a heartbeat to the lobby
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send heartbeat: " + e.Message);
        }
    }
    public async void ListLobbies(List<Lobby> lobbies, QueryLobbiesOptions filters = null)
    {
        try
        {
            lobbies.Clear();
            // List all lobbies
            Debug.Log("Listing lobbies...");
            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync(filters);
            foreach (var lobby in queryResponse.Results)
            {
                lobbies.Add(lobby);
                Debug.Log("Lobby: " + lobby.Name + " - " + lobby.Id + " - " + lobby.LobbyCode);
            }
            Debug.Log("Lobbies listed: " + lobbies.Count);
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to list lobbies: " + e.Message);
        }
    }
    public async void JoinLobbyById(string lobbyId, string playerName)
    {
        try
        {
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = CreatePlayer(playerName)
            };
            var joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId, joinLobbyByIdOptions); // Join a lobby by ID
            PrintPlayers(joinedLobby);
            JoinedLobby = joinedLobby;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to join lobby: " + e.Message);
        }
    }
    public async void JoinLobbyByCode(string lobbyCode, string playerName)
    {
        try
        {
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = CreatePlayer(playerName)
            };
            var joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, joinLobbyByCodeOptions); // Join a lobby by ID
            PrintPlayers(joinedLobby);
            JoinedLobby = joinedLobby;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to join lobby: " + e.Message);
        }
    }
    public Player CreatePlayer(string playerName)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) }
            }
        };
    }
    void GetPlayers(Lobby lobby, List<Player> players)
    {
        players.Clear();
        foreach (var player in lobby.Players)
        {
            Debug.Log(player.Id + " - " + player.Data["name"].Value);
            players.Add(player);
        }
    }
    void PrintPlayers(Lobby lobby)
    {
        Debug.Log("Players in lobby: " + lobby.Name);
        foreach (var player in lobby.Players)
        {
            Debug.Log(player.Id + " - " + player.Data["name"].Value);
        }
    }
    async Task<Lobby> UpdateLobby(string lobbyId = null)
    {
        try
        {
            if(lobbyId != null)
            {
                lobbyUpdateTimer += Time.deltaTime;
                if (lobbyUpdateTimer > lobbyUpdateInterval)
                {
                    lobbyUpdateTimer = 0.0f;
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
                    return lobby;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to update lobby: " + e.Message);
        }
        return null;
    }
    public async void LeaveLobby()
    {
        try
        {
            if(JoinedLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId); // Leave a lobby
                JoinedLobby = null;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to leave lobby: " + e.Message);
        }
    }
    public async void KickPlayer(string playerId)
    {
        try
        {
            if(JoinedLobby != null)
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, playerId); // Kick a player from the lobby
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to kick player: " + e.Message);
        }
    }    

}