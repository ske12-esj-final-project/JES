using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;

public class RoomManager : MonoBehaviour
{
    public GameObject roomSelectPanel;
    public Button openRoomSelectButton;
    public LoadingScreenControl loadingScreen;
    private SocketIOComponent socket;
    public GameObject gameManager;
    private GameObject[] rooms;
    private static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    private bool isPlayerCreated = false;

    void Awake()
    {
        if (GameManager.instance == null)
        {
            Instantiate(gameManager);
        }
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        openRoomSelectButton.GetComponent<Button>().onClick.AddListener(() => OpenRoomSelect());

        if (!GameManager.isPlayerConnected())
        {
            GameManager.SetPlayerConnected(true);
            StartCoroutine("PlayerConnect");
        }

        socket.On("z", AddPlayerToGame);
        socket.On("y", PlayerJoinRoom);
    }

    void Start()
    {
        isPlayerCreated = false;
    }
    
    IEnumerator PlayerConnect()
    {
        yield return new WaitForSeconds(1);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["d"] = string.Format("[@{0}@, @{1}@]", "1234", PlayerPrefs.GetString("token"));
        socket.Emit("z", new JSONObject(data));
    }

    void AddPlayerToGame(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        string playerID = jsonData[0].ToString();
        playerID = playerID.Replace("\"", "");
        RegisterPlayer(playerID);
    }

    void RegisterPlayer(string _playerID)
    {
        players.Add(_playerID, null);
        GameManager.SetPlayerID(_playerID);
    }

    void PlayerJoinRoom(SocketIOEvent evt)
    {
        if (!isPlayerCreated)
        {
            Debug.Log("Player Join room");
            GameObject.Find("NetworkManager").GetComponent<NetworkManager>().StartCoroutine("SetupPlayer");
            loadingScreen.LoadScene();
            isPlayerCreated = true;
        }
    }

    void OpenRoomSelect()
    {
        roomSelectPanel.SetActive(true);
    }
}
