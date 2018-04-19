using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;

public class RoomManager : MonoBehaviour
{
    public GameObject roomSelectPanel;
    public GameObject clothUI;
    public Button openRoomSelectButton;
    public Button clothButton;
    public GameObject startButton;
    public GameObject backbutton;
    public GameObject submitbutton;
    public LoadingScreenControl loadingScreen;
    private SocketIOComponent socket;
    public GameObject gameManager;
    private GameObject[] rooms;
    private static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    private bool isPlayerCreated = false;

    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.On("z", AddPlayerToGame);
        socket.On("y", PlayerJoinRoom);

        openRoomSelectButton.GetComponent<Button>().onClick.AddListener(() => OpenRoomSelect());
        clothButton.GetComponent<Button>().onClick.AddListener(() => OpenClothUI());
        backbutton.GetComponent<Button>().onClick.AddListener(() => CloseClothUI());
        submitbutton.GetComponent<Button>().onClick.AddListener(() => CloseClothUI());

        if (GameManager.GetState() != GameManager.State.Connect)
        {
            GameManager.SetState(GameManager.State.Connect);
            StartCoroutine("PlayerConnect");
        }

        GameObject enemy = GameObject.Find("Enemies");
        enemy.GetComponent<ClothManager>().ChangeCloth(GameManager.GetClothIndex());
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
        PlayerPrefs.SetString("playerID", _playerID);
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
        clothUI.SetActive(false);
        roomSelectPanel.SetActive(true);
    }

    void OpenClothUI()
    {
        roomSelectPanel.SetActive(false);
        clothButton.gameObject.SetActive(false);
        clothUI.SetActive(true);
        startButton.SetActive(false);
    }
    void CloseClothUI()
    {
        roomSelectPanel.SetActive(false);
        clothButton.gameObject.SetActive(true);
        clothUI.SetActive(false);
        startButton.SetActive(true);
    }
}
