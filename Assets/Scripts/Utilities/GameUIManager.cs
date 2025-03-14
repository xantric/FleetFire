using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using UnityEngine;

public class GameUIManager : NetworkBehaviour
{
    private static GameUIManager instance;
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform playerCardParent;
    [SerializeField] private GameObject scoreBoard;
    private Dictionary<ulong,  PlayerCard> playerCards = new Dictionary<ulong, PlayerCard>();


    public static string GetPlayerName()
    {
        LobbyManager _lobbyManager = FindObjectOfType<LobbyManager>();
        if (_lobbyManager == null) return "Null";
        if (_lobbyManager.JoinedLobby == null) return "Unkown";

        foreach(var player in _lobbyManager.JoinedLobby.Players)
        {
            if(player.Id == _lobbyManager.playerId)
            {
                return player.Data["name"].Value;
            }
        }
        return "Unkown";
    }
    public static void PlayerJoined(ulong clientID)
    {
        PlayerCard card = Instantiate(instance.playerCardPrefab, instance.playerCardParent);
        instance.playerCards.Add(clientID, card);
        var name = GetPlayerName();
        card.Initialize(clientID.ToString());
    }

    public static void PlayerLeft(ulong clientID)
    {
        if(instance.playerCards.TryGetValue(clientID, out PlayerCard playerCard))
        {
            Destroy(playerCard.gameObject);
            instance.playerCards.Remove(clientID);
        }
    }
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            scoreBoard.SetActive(true);
        }
        if(Input.GetKeyUp(KeyCode.Tab))
        {
            scoreBoard.SetActive(false);
        }


    }


    public static void SetKill(ulong clientID, int kills)
    {
        instance.SetKillsServerRpc(clientID, kills);
    }
    public static void SetDeath(ulong clientID, int deaths)
    {
        instance.SetDeathServerRpc(clientID, deaths);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetKillsServerRpc(ulong clientID, int kills)
    {
        SetKillsClientRpc(clientID, kills);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetDeathServerRpc(ulong clientID, int deaths)
    {
        SetDeathClientRpc(clientID, deaths);
    }

    [ClientRpc]
    public void SetKillsClientRpc(ulong clientID, int kills)
    {
        instance.playerCards[clientID].SetKills(kills);
    }

    [ClientRpc]
    public void SetDeathClientRpc(ulong clientID, int deaths)
    {
        instance.playerCards[clientID].SetDeaths(deaths);
    }
}
