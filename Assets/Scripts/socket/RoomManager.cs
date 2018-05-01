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
    public GameObject titleUI;
    public GameObject tutorialUI;
    public Text nameText;
    public Text scoreText;
    public Button openRoomSelectButton;
    public Button closeRoomSelectButton;
    public Button clothButton;
    public Button tutorialButton;
    public Button quitButton;
    public GameObject startButton;
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
        closeRoomSelectButton.GetComponent<Button>().onClick.AddListener(() => CloseRoomSelect());
        clothButton.GetComponent<Button>().onClick.AddListener(() => OpenClothUI());
        tutorialButton.GetComponent<Button>().onClick.AddListener(() => OpenTutorialUI());
        quitButton.GetComponent<Button>().onClick.AddListener(() => QuitGame());

        if (GameManager.GetState() != GameManager.State.Connect)
        {
            GameManager.SetState(GameManager.State.Connect);
            StartCoroutine("PlayerConnect");
        }

        GameObject enemy = GameObject.Find("Enemies");
        enemy.GetComponent<ClothManager>().ChangeCloth(GameManager.GetClothIndex());
        isPlayerCreated = false;

        nameText.text = GameManager.GetUsername();
        scoreText.text = GameManager.GetScore().ToString();
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
        titleUI.SetActive(false);
        clothUI.SetActive(false);
        roomSelectPanel.SetActive(true);
        quitButton.gameObject.SetActive(false);
    }

    void CloseRoomSelect()
    {
        titleUI.SetActive(true);
        roomSelectPanel.SetActive(false);
        quitButton.gameObject.SetActive(true);
    }

    void OpenClothUI()
    {
        roomSelectPanel.SetActive(false);
        clothButton.gameObject.SetActive(false);
        tutorialButton.gameObject.SetActive(false);
        clothUI.SetActive(true);
        startButton.SetActive(false);
        quitButton.gameObject.SetActive(false);
    }

    public void CloseClothUI()
    {
        roomSelectPanel.SetActive(false);
        tutorialButton.gameObject.SetActive(true);
        clothButton.gameObject.SetActive(true);
        clothUI.SetActive(false);
        startButton.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }

    void OpenTutorialUI()
    {
        tutorialUI.SetActive(true);
        tutorialButton.gameObject.SetActive(false);
    }

    public void CloseTutorialUI()
    {
        tutorialUI.SetActive(false);
        tutorialButton.gameObject.SetActive(true);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}
