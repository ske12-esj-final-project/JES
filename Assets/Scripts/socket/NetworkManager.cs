using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    private GameObject player;
    private static Dictionary<string, GameObject> players;
    private SocketIOComponent socket;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Use this for initialization
    void Start()
    {
        players = GameManager.GetPlayers();
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        socket.On("3", Spawn);
        socket.On("4", NewEnemySpawn);
        socket.On("7", UpdateEnemyPosition);
        socket.On("8", UpdateEnemyRotate);
        socket.On("10", EnemyShoot);
        socket.On("a", UpdatePlayerStatus);
        socket.On("e", CurrentPlayerAlive);
        socket.On("x", Countdown);
        socket.On("w", FinishCountdown);
        socket.On("a2", InterruptCountdown);
        socket.On("m", EnemyLeaveRoom);
        socket.On("disconnect", OnDisconnect);
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            float x = Mathf.Round(player.transform.position.x * 100f) / 100f;
            float y = Mathf.Round(player.transform.position.y * 100f) / 100f;
            float z = Mathf.Round(player.transform.position.z * 100f) / 100f;
            data["d"] = string.Format("[{0}, {1}, {2}]", x, y, z);
            socket.Emit("1", new JSONObject(data));
        }

        if (Input.GetKeyDown(KeyCode.Escape) && IsLobby())
        {
            player.GetComponent<FirstPersonController>().SetCursorLock(false);
            socket.Emit("m");
            GameManager.Reset();
            SceneManager.LoadScene("Rooms", LoadSceneMode.Single);
        }
    }

    bool IsLobby()
    {
        return SceneManager.GetActiveScene().name == "Lobby";
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        Dictionary<string, string> data = new Dictionary<string, string>();
        string playerID = PlayerPrefs.GetString("playerID");
        data["d"] = string.Format("[@{0}@]", playerID);
        socket.Emit("v", new JSONObject(data));
    }

    IEnumerator SetupPlayer()
    {
        yield return new WaitForSeconds(1);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["d"] = string.Format("[{0}]", GameManager.GetClothIndex());
        socket.Emit("2", new JSONObject(data));
    }

    void Countdown(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        int second = int.Parse(jsonData[0].ToString());
        player.GetComponent<PlayerUI>().SetCountdownText();
        player.GetComponent<PlayerUI>().SetSecondText(second);
    }

    void InterruptCountdown(SocketIOEvent evt)
    {
        player.GetComponent<PlayerUI>().DisableCountdownText();
    }

    void FinishCountdown(SocketIOEvent evt)
    {
        GameManager.Reset();
        SceneManager.LoadScene("Terrain", LoadSceneMode.Single);
        StartCoroutine("StartGame");
    }

    void Spawn(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        PlayerSpawn(jsonData[0]);
        if (IsLobby()) EnemySpawn(jsonData[1]);
    }

    void PlayerSpawn(JSONObject playerData)
    {
        string playerID = playerData[0].ToString();
        float xAxis = float.Parse(playerData[1].ToString());
        float yAxis = float.Parse(playerData[2].ToString());
        float zAxis = float.Parse(playerData[3].ToString());
        player = Instantiate(playerPrefab, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
        GameManager.SetPlayer(player);
        player.GetComponent<ClothManager>().ChangeCloth(int.Parse(playerData[6].ToString()));
        RegisterPlayer(playerID, player);
    }

    void EnemySpawn(JSONObject enemies)
    {
        foreach (var enemy in enemies.list)
        {
            float xAxis = float.Parse(enemy[1].ToString());
            float yAxis = float.Parse(enemy[2].ToString());
            float zAxis = float.Parse(enemy[3].ToString());
            GameObject user = Instantiate(enemyPrefab, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
            user.GetComponent<ClothManager>().ChangeCloth(int.Parse(enemy[6].ToString()));
            RegisterPlayer(enemy[0].ToString(), user);
        }
    }

    void NewEnemySpawn(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        float xAxis = float.Parse(jsonData[1].ToString());
        float yAxis = float.Parse(jsonData[2].ToString());
        float zAxis = float.Parse(jsonData[3].ToString());
        GameObject user = Instantiate(enemyPrefab, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
        user.GetComponent<ClothManager>().ChangeCloth(int.Parse(jsonData[6].ToString()));
        RegisterPlayer(jsonData[0].ToString(), user);
    }

    void RegisterPlayer(string _netID, GameObject _player)
    {
        string _playerID = _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    void UpdateEnemyPosition(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Vector3 newPosition = new Vector3(
            float.Parse(jsonData[1].ToString()),
            float.Parse(jsonData[2].ToString()),
            float.Parse(jsonData[3].ToString())
        );
        players[jsonData[0].ToString()].GetComponent<PlayerMotor>().EnemyMove(newPosition);
    }

    void UpdateEnemyRotate(SocketIOEvent evt)
    {

        JSONObject jsonData = evt.data.GetField("d");
        Vector3 rotate = new Vector3(
            float.Parse(jsonData[1].ToString()),
            float.Parse(jsonData[2].ToString()),
            0
        );
        players[jsonData[0].ToString()].GetComponent<PlayerMotor>().EnemyRotate(rotate);
    }

    void EnemyShoot(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Transform enemyArm = players[jsonData[0].ToString()].transform.GetChild(1).transform;
        for (int i = 0; i < enemyArm.childCount; i++)
        {
            GameObject g = enemyArm.GetChild(i).gameObject;
            if (g.activeInHierarchy)
            {
                g.GetComponent<EnemyArmController>().Shoot();
            }
        }
    }

    void UpdatePlayerStatus(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        float newHealth = float.Parse(jsonData[2].ToString());
        players[jsonData[0].ToString()].GetComponent<PlayerStatus>().SetHealth(newHealth);
        float playerDegree = players[jsonData[0].ToString()].transform.localEulerAngles.y;
        float enemyDegree = players[jsonData[1].ToString()].transform.localEulerAngles.y;
        player.GetComponent<PlayerUI>().ShowDamageIndicator(playerDegree, enemyDegree);
    }

    void CurrentPlayerAlive(SocketIOEvent evts)
    {
        JSONObject jsonData = evts.data.GetField("d");
        player.GetComponent<PlayerUI>().SetCurrentPlayerAlive(jsonData[0].ToString());
    }

    void EnemyLeaveRoom(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        string playerID = jsonData[0].ToString();
        Destroy(players[playerID]);
        players.Remove(playerID);
    }

    void OnDisconnect(SocketIOEvent evt)
    {
        if (player != null)
        {
            player.GetComponent<FirstPersonController>().SetCursorLock(false);
        }

        GameManager.Reset();
        GameManager.SetState(GameManager.State.Disconnect);
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        Destroy(GameObject.Find("SocketIO"));
        Destroy(this.gameObject);
    }
}
