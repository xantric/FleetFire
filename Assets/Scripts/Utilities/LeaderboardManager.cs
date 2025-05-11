using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using Unity.Collections;
using UnityEngine.SceneManagement;
using Unity.Services.Authentication;

public class LeaderboardManager : NetworkBehaviour
{
    public GameObject LeaderBoard;
    [SerializeField] private PlayerCard playerCardFinalPrefab;
    [SerializeField] private Transform playerCardParentFinal;

    // Use this struct to pack leaderboard data for RPC.
    public struct LeaderboardEntry : INetworkSerializable
    {
        public ulong clientID;
        public FixedString64Bytes playerName;
        public int kills;
        public int deaths;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientID);
            serializer.SerializeValue(ref playerName);
            serializer.SerializeValue(ref kills);
            serializer.SerializeValue(ref deaths);
        }
    }

    private void Awake()
    {
        LeaderBoard.SetActive(false);
    }

    /// <summary>
    /// Called on the server to collect leaderboard data and broadcast it to clients.
    /// </summary>
    public void ShowLeaderboard()
    {
        Cursor.lockState = CursorLockMode.None;
        // Ensure this runs only on the server
        if (!IsServer) return;

        List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

        // Iterate over each lobby player (assumes LobbyManager.players is updated and each entry has a "clientId" key)
        for (int i = 0; i < LobbyManager.players.Count; i++)
        {
            Player lobbyPlayer = LobbyManager.players[i];
            // Retrieve client ID from lobby metadata
            ulong clientID = 0;
            if (lobbyPlayer.Data.ContainsKey("clientId"))
            {
                ulong.TryParse(lobbyPlayer.Data["clientId"].Value, out clientID);
            }
            // Get kills and deaths from GameManager (which must be running on the server)
            int kills = GameManager.GetKills((ulong) i);
            int deaths = GameManager.GetDeaths((ulong) i);

            LeaderboardEntry entry = new LeaderboardEntry
            {
                clientID = clientID,
                playerName = lobbyPlayer.Data["name"].Value,  // converts to FixedString64Bytes automatically
                kills = kills,
                deaths = deaths
            };

            entries.Add(entry);
        }
        // Now broadcast the leaderboard to all clients.
        UpdateLeaderboardClientRpc(entries.ToArray());
    }

    [ClientRpc]
    private void UpdateLeaderboardClientRpc(LeaderboardEntry[] entries)
    {
        LeaderBoard.SetActive(true);

        // Clear existing leaderboard UI, if any.
        foreach (Transform child in playerCardParentFinal)
        {
            Destroy(child.gameObject);
        }

        // For each entry received, instantiate a PlayerCard UI and update it.
        foreach (LeaderboardEntry entry in entries)
        {
            PlayerCard card = Instantiate(playerCardFinalPrefab, playerCardParentFinal);
            card.Initialize(entry.playerName.ToString());
            card.SetKills(entry.kills);
            card.SetDeaths(entry.deaths);
        }
    }

    public void ReturnToLobby()
    {
        Debug.Log("Returned to Lobby pressed");

        // Leave the lobby (cloud-side)
        if (LobbyManager.Instance.JoinedLobby != null)
        {
            LobbyManager.Instance.LeaveLobbyOnExit();
        }

        // Reset local lobby and player data
        LobbyManager.players.Clear();
        LobbyManager.Instance.JoinedLobby = null;

        // Stop networking session
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // Optionally, unload persistent networked objects here if you're using DontDestroyOnLoad
        foreach (var obj in GameObject.FindObjectsOfType<NetworkObject>())
        {
            if (obj.IsSpawned)
                obj.Despawn(true);
        }

        SceneManager.LoadScene("MainMenu");
    }

    [ServerRpc(RequireOwnership = false)]
    private void ReturnToMainMenuServerRpc()
    {
        //if (AuthenticationService.Instance.IsSignedIn)
        //{
        //    AuthenticationService.Instance.SignOut();
        //}
        LobbyManager.Instance.LeaveLobbyOnExit();

    }


}
