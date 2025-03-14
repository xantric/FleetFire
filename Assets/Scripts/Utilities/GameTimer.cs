using UnityEngine;
using Unity.Netcode;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : NetworkBehaviour
{
    public float gameDuration = 60f; // Game duration in seconds
    public TextMeshProUGUI timerText;
    
    private bool gameStarted = false;
    private bool gameEnded = false;
    
    private NetworkVariable<float> remainingTime = new NetworkVariable<float>(
        60f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    private NetworkVariable<int> playersSpawned = new NetworkVariable<int>(
        0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public static GameTimer Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        timerText.gameObject.SetActive( false );
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            remainingTime.Value = gameDuration;
        }
    }

    public static void PlayerSpawned(bool IsServer, int count)
    {
        if (IsServer)
        {
            //Debug.LogError(count);
            //Debug.LogError("hELLO: " + NetworkManager.Singleton.ConnectedClients.Count.ToString());
            if (count >= LobbyManager.players.Count)
            {
             
                Instance.StartGame();
            }
        }
    }

    private void StartGame()
    {
        if (!gameStarted)
        {
            timerText.gameObject.SetActive(true);
            gameStarted = true;
            StartCoroutine(TimerRoutine());
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (remainingTime.Value > 0)
        {
            yield return new WaitForSeconds(1f);
            remainingTime.Value -= 1f;
            //Debug.LogWarning("Time Remaining: " +  remainingTime.Value.ToString());
        }

        if (!gameEnded)
        {
            gameEnded = true;
            EndGame();
        }
    }

    private void EndGame()
    {
        ShowLeaderboardServerRpc();
    }

    [ServerRpc]
    private void ShowLeaderboardServerRpc()
    {
        Debug.Log("Game Over! Show the leaderboard UI here.");
        // Call your leaderboard UI function here
        NetworkManager.Singleton.SceneManager.LoadScene("LeaderBoard", LoadSceneMode.Single);
    }

    public float GetRemainingTime()
    {
        return remainingTime.Value;
    }

    private void Update()
    {
        if (timerText != null)
        {
            
            timerText.text = $"Time: {Mathf.CeilToInt(remainingTime.Value)}s";
        }
    }
}
