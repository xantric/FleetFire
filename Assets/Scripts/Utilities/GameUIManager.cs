using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport.Error;
using Unity.Services.Authentication;
using UnityEngine;

public class GameUIManager : NetworkBehaviour
{
    private static GameUIManager instance;
    [SerializeField] private PlayerCard playerCardPrefab;
    [SerializeField] private Transform playerCardParent;
    [SerializeField] private GameObject scoreBoard;
    private Dictionary<ulong,  PlayerCard> playerCards = new Dictionary<ulong, PlayerCard>();


    public static void PlayerJoined(ulong clientID, string playerName)
    {
        PlayerCard card = Instantiate(instance.playerCardPrefab, instance.playerCardParent);
        instance.playerCards.Add(clientID, card);
        card.Initialize(playerName);
    }

    public static void PlayerLeft(ulong clientID)
    {
        if(instance.playerCards.TryGetValue(clientID, out PlayerCard playerCard))
        {
            if (playerCard.gameObject != null) Destroy(playerCard.gameObject);
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

    public static void SetPlayerName(ulong clientID, string name)
    {
        instance.SetPlayerNameServerRpc(clientID, name);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetPlayerNameServerRpc(ulong clientID, string name)
    {
        instance.SetPlayerNameClientRpc(clientID, name);
    }

    [ClientRpc]
    public void SetPlayerNameClientRpc(ulong ClientID, string name)
    {
        instance.playerCards[ClientID].SetName(name);
    }
}
