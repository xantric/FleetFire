using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    LobbyManager lobbyManager;
    [Header("Player Name")]
    public GameObject playerNameCanvas;
    public TMP_InputField playerNameInput;
    string playerName = "Player";
    [Header("Lobby List")]
    public GameObject lobbyList;
    public GameObject lobbyListContent;
    public GameObject lobbyObjectPrefab;
    List<Lobby> lobbies = new List<Lobby>();

    [Header("Create Lobby")]
    public GameObject createLobbyCanvas;
    public TMP_InputField lobbyNameInput;
    public TMP_Dropdown maxPlayersDropdown;
    public Toggle publicToggle;
    string lobbyName = "Lobby";
    bool isPrivate = false;

    [Header("Lobby Waiting Room")]
    public GameObject lobbyWaitingRoom;
    public GameObject lobbyWaitingRoomContent;
    public GameObject playerObjectPrefab;
    List<Player> players = new List<Player>();

    [Header("Join Private Lobby")]
    public GameObject joinPrivateLobbyCanvas;
    public TMP_InputField joinPrivateLobbyInput;
    public TMP_Text joinPrivateLobbyError;
    void Start()
    {
        playerName += Random.Range(0, 1000).ToString();
        lobbyName += Random.Range(0, 1000).ToString();
        lobbyManager = GetComponent<LobbyManager>();
        
        lobbyList.SetActive(false);
        createLobbyCanvas.SetActive(false);

        playerNameCanvas.SetActive(true);
        playerNameInput.text = playerName;

    }
    public async void UpdateLobbyList()
    {
        lobbyManager.ListLobbies(lobbies);
        foreach (Transform child in lobbyListContent.transform)
        {
            if(child.gameObject.name != "Title" && child.gameObject.name != "Index")
                Destroy(child.gameObject);
            
        }
        await Task.Delay(1500);
        Debug.Log("Lobbies: " + lobbies.Count);
        foreach (Lobby lobby in lobbies)
        {
            Debug.Log("Lobby: " + lobby.Name);
            CreateLobbyObject(lobby);
        }
    }
    void CreateLobbyObject(Lobby lobby)
    {
        GameObject lobbyObject = Instantiate(lobbyObjectPrefab, lobbyListContent.transform);
        lobbyObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = lobby.Name;
        lobbyObject.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
        lobbyObject.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => lobbyManager.JoinLobbyById(lobby.Id, playerName));
    }
    public void SetPlayerName()
    {
        playerName = playerNameInput.text;
        playerNameCanvas.SetActive(false);
        lobbyList.SetActive(true);
        Debug.Log("Player Name: " + playerName);
        lobbyManager.Authenticate(playerName);
    }
    public void CreateLobbyWindow()
    {
        lobbyList.SetActive(false);
        createLobbyCanvas.SetActive(true);
        lobbyNameInput.text = lobbyName;
    }
    public void CancelCreateLobby()
    {
        createLobbyCanvas.SetActive(false);
        lobbyList.SetActive(true);
    }
    public void CreateLobby()
    {
        lobbyName = lobbyNameInput.text;
        isPrivate = !publicToggle.isOn;
        lobbyManager.CreateLobby(lobbyName, 4-maxPlayersDropdown.value, playerName, isPrivate);
        createLobbyCanvas.SetActive(false);
    }
}
