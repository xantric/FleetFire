using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{

    public static LobbyManager Instance { get; private set; }

    public Lobby JoinedLobby;
    public string RelayCode = "0";
    public List<Lobby> lobbies = new List<Lobby>();
    public static List<Player> players = new List<Player>();
    public float heartBeatInterval = 15f, heartBeatTimer = 0.0f;
    public float lobbyListInterval = 2f, lobbyListTimer = 0.0f;
    public float lobbyUpdateInterval = 5f, lobbyUpdateTimer = 0.0f;
    public float playerUpdateInterval = 2.5f, playerUpdateTimer = 0.0f;
    public string playerName, playerId;
    bool authenticated = false;
    bool GameStartedProcessing = false;
    public GameObject Canvas;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        if(JoinedLobby != null)
        {
            HandleHeartbeat();
            UpdateLobby();
            UpdatePlayers();
            CheckGameStarted();
            if(RelayCode != "0" && !IsHost() && !GameStartedProcessing)
            {
                Debug.Log("Relay Code: " + RelayCode);
                Debug.Log(RelayCode.Length);
                Invoke("JoinGame", 2.5f);
                GameStartedProcessing = true;
            }
            else if(RelayCode != "0" && IsHost())
            {
                Debug.Log("Starting game...");
            }
        }
        else if(authenticated)
            ListLobbies();
    }
    public async void Authenticate()
    {
        InitializationOptions options = new InitializationOptions();
        options.SetProfile(playerName);
        
        await UnityServices.InitializeAsync(options); // Initialize Unity Services

        // Sign in anonymously

        if(AuthenticationService.Instance.IsSignedIn) // Check if the user is signed in
        {
            playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in as: " + playerId);
        }
        else
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log("Signed in as: " + playerId);
        }
        authenticated = true;
    }
    public Player CreatePlayer(string playerName, ulong clientId)
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { "name", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, playerName) },
                { "clientId", new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, clientId.ToString()) }

            }
        };
    }
    void UpdatePlayers()
    {
        playerUpdateTimer += Time.deltaTime;
        if (playerUpdateTimer > playerUpdateInterval && JoinedLobby != null)
        {
            players.Clear();
            foreach (var player in JoinedLobby.Players)
            {
                players.Add(player);
            }
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
    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false)
    {
        try
        {
            ulong myClientId = NetworkManager.Singleton.LocalClient.ClientId;
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Player = CreatePlayer(playerName, myClientId),
                Data = new Dictionary<string, DataObject>
                {
                    {"gameStarted", new DataObject(DataObject.VisibilityOptions.Member, "0")}
                }
            };
            
            
            var lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options); // Create a lobby with a name and a max number of players
            Debug.Log("Lobby created: " + lobby.Id + " - " + lobby.Name + " - " + lobby.LobbyCode + " by " + playerName);
            JoinedLobby = lobby;
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to create lobby: " + e.Message);
        }
    }
    async void HandleHeartbeat()
    {
        if(!IsHost())
            return;
        heartBeatTimer += Time.deltaTime;
        try
        {
            if (heartBeatTimer > heartBeatInterval && JoinedLobby != null) // Check if the heartbeat interval has passed
            {
                Debug.Log("Sending heartbeat...");
                heartBeatTimer = 0.0f;
                await LobbyService.Instance.SendHeartbeatPingAsync(JoinedLobby.Id); // Send a heartbeat to the lobby
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to send heartbeat: " + e.Message);
        }
    }
    public async void ListLobbies()
    {
        lobbyListTimer += Time.deltaTime;
        try
        {
            if (lobbyListTimer > lobbyListInterval) // Check if the lobby list interval has passed
            {
                lobbyListTimer = 0.0f;
                // List all lobbies
//                Debug.Log("Listing lobbies...");
                var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
                
                lobbies.Clear();
                foreach (var lobby in queryResponse.Results)
                {
                    lobbies.Add(lobby);
//                    Debug.Log("Lobby: " + lobby.Name + " - " + lobby.Id + " - " + lobby.LobbyCode);
                }
//                Debug.Log("Lobbies listed: " + lobbies.Count);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to list lobbies: " + e.Message);
        }
    }
    public async void JoinLobbyById(string lobbyId)
    {
        try
        {
            ulong myClientId = NetworkManager.Singleton.LocalClient.ClientId;
            JoinLobbyByIdOptions joinLobbyByIdOptions = new JoinLobbyByIdOptions
            {
                Player = CreatePlayer(playerName, myClientId)
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
    public async void JoinLobbyByCode(string lobbyCode)
    {
        try
        {
            ulong myClientId = NetworkManager.Singleton.LocalClient.ClientId;
            JoinLobbyByCodeOptions joinLobbyByCodeOptions = new JoinLobbyByCodeOptions
            {
                Player = CreatePlayer(playerName, myClientId)
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

    public async void UpdateLobby()
    {
        try
        {
            if(JoinedLobby != null)
            {
                if(IsKicked())
                {
                    Debug.Log("Kicked from lobby");
                    JoinedLobby = null;
                    return;
                }
                lobbyUpdateTimer += Time.deltaTime;
                if (lobbyUpdateTimer > lobbyUpdateInterval)
                {
                    lobbyUpdateTimer = 0.0f;
                    Lobby lobby = await LobbyService.Instance.GetLobbyAsync(JoinedLobby.Id);
                    JoinedLobby = lobby;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Failed to update lobby: " + e.Message);
        }
    }
    public bool IsKicked()
    {
        if(JoinedLobby == null)
            return false;
        foreach (var player in JoinedLobby.Players)
        {
            if(player.Id == AuthenticationService.Instance.PlayerId)
                return false;
        }
        return true;
    }
    public async void LeaveLobby()
    {
        try
        {
            players.Clear();
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
    public bool IsHost()
    {
        if(JoinedLobby == null) 
            return false;
        if(JoinedLobby.HostId == AuthenticationService.Instance.PlayerId)
            return true;
        return false;
    }
    public async void StartGame()
    {
        if(GameStartedProcessing)
            return;
        if(JoinedLobby != null)
        {
            GameStartedProcessing = true;
            string relayCode =  await GetComponent<Relay>().CreateRelay(JoinedLobby.MaxPlayers);
            Lobby lobby = await Lobbies.Instance.UpdateLobbyAsync(JoinedLobby.Id, new UpdateLobbyOptions{
                Data = new Dictionary<string, DataObject>
                {
                    {"gameStarted", new DataObject(DataObject.VisibilityOptions.Member, relayCode)},
                }
            });
            
        }
    }
    public void CheckGameStarted()
    {
        if(JoinedLobby != null)
        {
            if(JoinedLobby.Data.ContainsKey("gameStarted"))
            {
                if(JoinedLobby.Data["gameStarted"].Value != "0")
                {
                    RelayCode = JoinedLobby.Data["gameStarted"].Value;
                }
            }
        }
    }
    void JoinGame()
    {
        Canvas.SetActive(false);
        GetComponent<Relay>().JoinRelay(RelayCode);
        Debug.Log("Joining relay...");
        GetComponent<LobbyUI>().enabled = false;                
        GetComponent<LobbyUI>().lobbyWaitingRoom.SetActive(false);
        this.enabled = false;
    }

    public static string GetPlayerNameFromClientId(ulong clientId)
    {
        
        string clientIdStr = clientId.ToString();
        foreach (var player in players)
        {
            Debug.LogError(player.Data["clientId"].Value);
            if (player.Data.ContainsKey("clientId") && player.Data["clientId"].Value == clientIdStr)
            {
                return player.Data["name"].Value;
            }
        }
        return "Unknown Player";
    }

    public async void LeaveLobbyOnExit()
    {
        if (JoinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(JoinedLobby.Id, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogWarning($"Failed to remove player from lobby: {e}");
            }

            JoinedLobby = null;
        }

        players.Clear();
    }

}