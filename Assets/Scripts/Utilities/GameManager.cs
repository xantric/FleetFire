using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static Dictionary<ulong, Player> GetPlayers()
    {
        return players;
    }

    [SerializeField]
    private float RespawnTime = 5f;

    [SerializeField]
    private SpawnPositionManager _spawnPositionManager;

    [SerializeField] private List<Transform> spawnLocations = new List<Transform>();

    private static GameManager instance;

    private static Dictionary<ulong, Player> players = new Dictionary<ulong, Player>();

    private List<ulong> deadPlayers = new List<ulong>();


    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }
    public static void InitializeNewPlayer(ulong ClientID)
    {
        players.Add(ClientID, new Player());
    }
    public static void PlayerDisconnected(ulong ClientID)
    {
        players.Remove(ClientID);
    }
    public static void PlayerDied(ulong player, ulong killer)
    {
        Debug.LogWarning("PlayerDied");
        if(players.TryGetValue(killer, out Player killerPlayer))
        {
            killerPlayer.score++;
        }
        if(players.TryGetValue(player, out Player deadPlayer))
        {
            deadPlayer.Deaths++;
            deadPlayer.DeathTime = Time.time;
        }

        GameUIManager.SetKill(killer, killerPlayer.score);
        GameUIManager.SetDeath(player, deadPlayer.Deaths);
        instance.deadPlayers.Add(player);
        return;
    }
    public static int GetDeaths(ulong clientID)
    {
        Debug.LogWarning(players.Count);
        if (players.TryGetValue(clientID, out Player _player))
        {
            return _player.Deaths;
        }
        else return 0;
    }

    public static int GetKills(ulong clientID)
    {
        //Debug.LogWarning(players.Count);
        if (players.TryGetValue(clientID, out Player _player))
        {
            return _player.score;
        }
        else return 0;
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer)
        {
            enabled = false;
            return;
        }
    }
    private void FixedUpdate()
    {
        for(int i = 0; i < deadPlayers.Count; i++)
        {
            if(players[deadPlayers[i]].DeathTime < Time.time - RespawnTime) 
            {
                RespawnPlayer(deadPlayers[i]);
                deadPlayers.RemoveAt(i);
                return;
            }
        }
    }

    public void RespawnPlayer(ulong clientID)
    {
        polygon_fps_controller.SetPlayerPosition(clientID, spawnLocations[Random.Range(0, spawnLocations.Count)].position);
        polygon_fps_controller.TogglePlayer(clientID, true);
        
        if (HealthSystem.playerHealths.TryGetValue(clientID, out HealthSystem playerHealth))
        {
            playerHealth.ResetHealthServerRpc();
        }

        
    }
    public class Player
    {
        public int Deaths = 0;
        public int score = 0;
        public float DeathTime = -99f;
    }
    
}
