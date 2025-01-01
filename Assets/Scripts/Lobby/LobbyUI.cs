using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    LobbyManager lobbyManager;
    [Header("Player Name")]
    public GameObject playerNameCanvas;
    public TMP_InputField playerNameInput;
    public TMP_Text playerNameText;
    [Header("Lobby List")]
    public GameObject lobbyList;
    public GameObject lobbyListContent;
    public GameObject lobbyObjectPrefab;

    [Header("Create Lobby")]
    public GameObject createLobbyCanvas;
    public TMP_InputField lobbyNameInput;
    public TMP_Dropdown maxPlayersDropdown;
    bool isPrivate = false;

    [Header("Lobby Waiting Room")]
    public GameObject lobbyWaitingRoom;
    public GameObject lobbyWaitingRoomContent;
    public GameObject playerObjectPrefab;
    public TMP_Text lobbyNameText;
    public GameObject StartButton;

    [Header("Update Time")]
    public float updateTime = 5, updateTimer = 0;
    
    void Start()
    {
        lobbyManager = GetComponent<LobbyManager>();
        
        lobbyList.SetActive(false);
        createLobbyCanvas.SetActive(false);
        lobbyWaitingRoom.SetActive(false);

        playerNameCanvas.SetActive(true);

        playerNameInput.text = "Player" + UnityEngine.Random.Range(0, 1000).ToString();
        lobbyNameInput.text = "Lobby" + UnityEngine.Random.Range(0, 1000).ToString();
    }

    void Update()
    {
        updateTimer += Time.deltaTime;
        if(updateTimer >= updateTime)
        {
            updateTimer = 0;
            if(lobbyList.activeSelf)
                UpdateLobbyList();
            if(lobbyWaitingRoom.activeSelf)
                UpdateLobbyWaitingRoom();
        }
    }

    //Player Name
    public void SetPlayerName()
    {
        lobbyManager.playerName = playerNameInput.text;
        playerNameText.text = playerNameInput.text;
        playerNameCanvas.SetActive(false);
        lobbyManager.Authenticate();
        lobbyList.SetActive(true);
        playerNameText.gameObject.SetActive(true);
    }

    //Create Lobby
    public void ShowCreateLobby()
    {
        createLobbyCanvas.SetActive(true);
        lobbyList.SetActive(false);
    }
    public void CreateLobby()
    {
        int maxPlayers = 4 - maxPlayersDropdown.value;
        isPrivate = false;
        lobbyManager.CreateLobby(lobbyNameInput.text, maxPlayers, isPrivate);
        createLobbyCanvas.SetActive(false);
        Invoke("UpdateLobbyWaitingRoom", 5);
    }    
    public void CancelCreateLobby()
    {
        createLobbyCanvas.SetActive(false);
        lobbyList.SetActive(true);
    }


    //Lobby List
    public void UpdateLobbyList()
    {
        foreach (Transform child in lobbyListContent.transform)
        {
            if(child.gameObject.name != "Title" && child.gameObject.name != "Index")
                Destroy(child.gameObject);
        }
        //"Lobbies: " + lobbyManager.lobbies.Count);
        foreach (var lobby in lobbyManager.lobbies)
        {
            //"Lobby: " + lobby.Name + " - " + lobby.Id + " - " + lobby.LobbyCode);
            if(lobby.Players.Count >= lobby.MaxPlayers) //lobby is full
                continue;
            GameObject lobbyObject = Instantiate(lobbyObjectPrefab, lobbyListContent.transform);
            lobbyObject.transform.GetChild(0).GetComponent<TMP_Text>().text = lobby.Name;
            lobbyObject.transform.GetChild(1).GetComponent<TMP_Text>().text = lobby.Players.Count + "/" + lobby.MaxPlayers;
            lobbyObject.GetComponentInChildren<Button>().onClick.AddListener(() => JoinLobby(lobby.Id));
        }
    }
    void JoinLobby(string lobbyId)
    {
        lobbyManager.JoinLobbyById(lobbyId);
        lobbyList.SetActive(false);
        Invoke("UpdateLobbyWaitingRoom", 5);
    }

    //Lobby Waiting Room
    public void UpdateLobbyWaitingRoom()
    {
        if(lobbyManager.RelayCode != "0")
        {
            lobbyWaitingRoom.SetActive(false);
        }
        if(lobbyManager.JoinedLobby == null)
        {
            //left the lobby
            lobbyWaitingRoom.SetActive(false);
            lobbyList.SetActive(true);
            return;
        }
        lobbyNameText.text = lobbyManager.JoinedLobby.Name;
        foreach (Transform child in lobbyWaitingRoomContent.transform)
        {
            if(child.gameObject.name != "Title" && child.gameObject.name != "Index")
                Destroy(child.gameObject);
        }
        foreach (var player in lobbyManager.players)
        {
            GameObject playerObject = Instantiate(playerObjectPrefab, lobbyWaitingRoomContent.transform);
            playerObject.GetComponentInChildren<TMP_Text>().text = player.Data["name"].Value;
            playerObject.GetComponentInChildren<Button>().onClick.AddListener(() => KickPlayer(player.Id, playerObject));
            playerObject.GetComponentInChildren<Button>().gameObject.SetActive(lobbyManager.IsHost() && (player.Id != lobbyManager.playerId));
        }
        if(!lobbyWaitingRoom.activeSelf)
            Invoke("UpdateLobbyWaitingRoom", 1);
        lobbyWaitingRoom.SetActive(true);
        
        if(lobbyManager.IsHost())
        {
            StartButton.SetActive(true);
            StartButton.GetComponent<Button>().onClick.AddListener(() => StartGame());
        }
        else
        {
            StartButton.SetActive(false);
        }
    }
    void StartGame()
    {
        lobbyWaitingRoom.SetActive(false);
        lobbyManager.StartGame();
    }
    void KickPlayer(string playerId, GameObject playerObject)
    {
        playerObject.SetActive(false);
        lobbyManager.KickPlayer(playerId);
        Invoke("UpdateLobbyWaitingRoom",5);
    }
    public void LeaveLobby()
    {
        lobbyManager.LeaveLobby();
        lobbyWaitingRoom.SetActive(false);
        lobbyList.SetActive(true);
    }
}
