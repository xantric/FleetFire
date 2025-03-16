using UnityEngine;
using Unity.Netcode;
using System.Collections;
using TMPro;
using UnityEngine.SceneManagement;

public class GameTimer : NetworkBehaviour
{
    public float gameDuration = 60f; // Game duration in seconds
    public TextMeshProUGUI timerText;
    public LeaderboardManager _leaderboard;

    private bool gameStarted = false;
    private bool gameEnded = false;
    private float endTime;

    // This NetworkVariable is updated only by the server.
    private NetworkVariable<float> remainingTime = new NetworkVariable<float>(
        60f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    public static GameTimer Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        Debug.Log($"GameTimer spawned on client: {NetworkManager.Singleton.LocalClientId}");
        if (IsServer)
        {
            remainingTime.Value = gameDuration;
        }

        // Subscribe to changes so that all clients update the UI when remainingTime changes.
        remainingTime.OnValueChanged += OnRemainingTimeChanged;
    }

    private void OnDestroy()
    {
        remainingTime.OnValueChanged -= OnRemainingTimeChanged;
    }

    private void OnRemainingTimeChanged(float oldValue, float newValue)
    {
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(newValue)}s";
    }

    // Called when a player is spawned.
    // This example assumes that LobbyManager.players holds the expected count.
    public static void PlayerSpawned(int count)
    {
        if (Instance == null) return;

        if (Instance.IsServer)
        {
            if (count >= LobbyManager.players.Count)
            {
                Instance.StartGame();
            }
        }
        else
        {
            // For clients, simply show the timer UI when the player count is sufficient.
            if (count >= LobbyManager.players.Count)
            {
                Debug.LogWarning("Client: enabling timer UI.");
                if (Instance.timerText != null)
                    Instance.timerText.gameObject.SetActive(true);
            }
        }
    }

    private void StartGame()
    {
        if (!gameStarted && IsServer)
        {
            gameStarted = true;
            // Calculate endTime deterministically.
            endTime = Time.time + gameDuration;
            StartCoroutine(TimerRoutine());
            // Tell all clients to show the timer UI.
            StartTimerClientRpc();
        }
    }

    [ClientRpc]
    private void StartTimerClientRpc()
    {
        if (timerText != null)
            timerText.gameObject.SetActive(true);
        gameStarted = true;
    }

    private IEnumerator TimerRoutine()
    {
        while (Time.time < endTime)
        {
            // Calculate remaining time based on endTime.
            remainingTime.Value = endTime - Time.time;
            yield return null;
        }
        remainingTime.Value = 0f;
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

    [ServerRpc(RequireOwnership = false)]
    private void ShowLeaderboardServerRpc()
    {
        Debug.Log("Game Over! Showing leaderboard.");
        ShowLeaderboardClientRpc();
    }

    [ClientRpc]
    private void ShowLeaderboardClientRpc()
    {
        if (_leaderboard != null)
            _leaderboard.ShowLeaderboard();
    }

    public float GetRemainingTime()
    {
        return remainingTime.Value;
    }
}
