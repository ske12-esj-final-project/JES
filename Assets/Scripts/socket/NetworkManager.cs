using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using SocketIO;

public class NetworkManager : MonoBehaviour
{
    private SocketIOComponent socket;
    public GameObject playerPrefab, enemyPrefab;
    public GameObject Handgun1, Handgun2, Revolver1;
    public GameObject Shotgun2;
    public GameObject SMG1, SMG2, SMG3;
    public GameObject AssaultRifle1, AssaultRifle2;
    public GameObject Sniper1;
    public GameObject Ammo;
    private GameObject safeArea;
    private Vector3 currentSafeAreaPos, newSafeAreaPos;
    private Vector3 currentSafeAreaScale, newSafeAreaScale;
    public LoadingScreenControl loadingScreen;
    public GameObject deadPerson, enemyDeadPerson;

    private GameObject player = null;
    private static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>();
    private PlayerUI playerUI;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }


    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        safeArea = GameObject.Find("Safe Area");
        safeArea.GetComponent<SafeArea>().SetSafeAreaEnabled(false);
        socket.On("3", Spawn);
        socket.On("4", NewEnemySpawn);
        socket.On("7", UpdateEnemyPosition);
        socket.On("8", UpdateEnemyRotate);
        socket.On("10", EnemyShoot);
        socket.On("a", UpdatePlayerStatus);
        socket.On("b", UpdateEnemyCurrentWeapon);
        socket.On("c", SpawnWeapon);
        socket.On("d", RemovePickupWeapon);
        socket.On("x", Countdown);
        socket.On("w", FinishCountdown);
        socket.On("u", GetSafeAreaInfo);
        socket.On("t", WarnSafeArea);
        socket.On("p", EnemyDead);
        socket.On("n", PlayerDead);
        socket.On("e", CurrentPlayerAlive);
        socket.On("k", PlayerWin);
    }

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
    }

    void Countdown(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        int second = int.Parse(jsonData[0].ToString());
        playerUI.SetCountdownText();
        playerUI.SetSecondText(second);
        Debug.Log("Time Left " + jsonData[0]);
    }

    void FinishCountdown(SocketIOEvent evt)
    {
        ResetRoom();
        SceneManager.LoadScene("Terrain", LoadSceneMode.Single);
        StartCoroutine("StartGame");
    }

    void UpdatePlayerStatus(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        float newHealth = float.Parse(jsonData[2].ToString());
        players[jsonData[0].ToString()].GetComponent<PlayerStatus>().SetHealth(newHealth);
    }

    void Spawn(SocketIOEvent evt)
    {
        Debug.LogError("Spawn");
        JSONObject jsonData = evt.data.GetField("d");
        PlayerSpawn(jsonData[0]);
        EnemySpawn(jsonData[1]);
    }

    void PlayerSpawn(JSONObject playerData)
    {
        string playerID = playerData[0].ToString();
        float xAxis = float.Parse(playerData[1].ToString());
        float yAxis = float.Parse(playerData[2].ToString());
        float zAxis = float.Parse(playerData[3].ToString());
        player = Instantiate(playerPrefab, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
        playerUI = player.GetComponent<PlayerUI>();
        player.transform.name = playerID;
        players[playerID] = player;
    }

    void EnemySpawn(JSONObject enemies)
    {
        foreach (var enemy in enemies.list)
        {
            float xAxis = float.Parse(enemy[1].ToString());
            float yAxis = float.Parse(enemy[2].ToString());
            float zAxis = float.Parse(enemy[3].ToString());
            GameObject user = Instantiate(enemyPrefab, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
            RegisterPlayer(enemy[0].ToString(), user);
        }
    }

    void EnemyShoot(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        for (int i = 0; i < players[jsonData[0].ToString()].transform.GetChild(1).transform.childCount; i++)
        {
            GameObject g = players[jsonData[0].ToString()].transform.GetChild(1).transform.GetChild(i).gameObject;
            if (g.activeInHierarchy)
            {
                g.GetComponent<EnemyArmController>().Shoot();
            }
        }
    }

    void NewEnemySpawn(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        float xAxis = float.Parse(jsonData[1].ToString());
        float yAxis = float.Parse(jsonData[2].ToString());
        float zAxis = float.Parse(jsonData[3].ToString());
        GameObject user = Instantiate(enemyPrefab, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
        RegisterPlayer(jsonData[0].ToString(), user);
    }

    IEnumerator SetupPlayer()
    {
        yield return new WaitForSeconds(1);
        Dictionary<string, string> data = new Dictionary<string, string>();
        string playerID = GameManager.GetPlayerID();
        data["d"] = string.Format("[@{0}@]", playerID);
        socket.Emit("2", new JSONObject(data));
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        Dictionary<string, string> data = new Dictionary<string, string>();
        string playerID = GameManager.GetPlayerID();
        data["d"] = string.Format("[@{0}@]", playerID);
        safeArea.GetComponent<SafeArea>().SetSafeAreaEnabled(true);
        socket.Emit("v", new JSONObject(data));
        currentSafeAreaPos = safeArea.transform.position;
        currentSafeAreaScale = safeArea.transform.localScale;
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
        // Debug.Log(jsonData);
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

    void UpdateEnemyCurrentWeapon(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Debug.Log("UpdateEnemyCurrentWeapon");
        Debug.Log(jsonData);
        int currentWeaponIndex = int.Parse(jsonData[1].ToString());
        players[jsonData[0].ToString()].GetComponent<EnemyInventory>().ChangeItem(currentWeaponIndex);
    }

    void SpawnWeapon(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Debug.Log(jsonData);
        foreach (var item in jsonData.list)
        {
            float xAxis = float.Parse(item[2].ToString());
            float yAxis = float.Parse(item[3].ToString());
            float zAxis = float.Parse(item[4].ToString());
            GameObject weapon = Handgun1;
            Debug.Log("Create Weapon");
            switch (int.Parse(item[1].ToString()))
            {
                case 1:
                    weapon = Instantiate(Handgun1, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 2:
                    weapon = Instantiate(Handgun2, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 3:
                    weapon = Instantiate(Revolver1, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 4:
                    weapon = Instantiate(Shotgun2, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 5:
                    weapon = Instantiate(SMG1, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 6:
                    weapon = Instantiate(SMG2, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 7:
                    weapon = Instantiate(SMG3, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 8:
                    weapon = Instantiate(AssaultRifle1, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 9:
                    weapon = Instantiate(AssaultRifle2, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                case 10:
                    weapon = Instantiate(Sniper1, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                    break;
                default:
                    break;
            }
            RegisterWeapon(item[0].ToString(), weapon);
        }
    }

    void RegisterWeapon(string _weaponID, GameObject _weapon)
    {
        weapons.Add(_weaponID, _weapon);
        _weapon.name = _weaponID;
    }

    void RemovePickupWeapon(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Debug.Log("Remove pickup item: " + jsonData);
        Destroy(weapons[jsonData[0].ToString()]);
        weapons.Remove(jsonData[0].ToString());
    }

    void GetSafeAreaInfo(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Debug.Log("SAFE " + jsonData);
        newSafeAreaPos = new Vector3(
            float.Parse(jsonData[0].ToString()),
            float.Parse(jsonData[1].ToString()),
            float.Parse(jsonData[2].ToString())
        );
        newSafeAreaScale = new Vector3(
            float.Parse(jsonData[3].ToString()),
            float.Parse(jsonData[4].ToString()),
            float.Parse(jsonData[5].ToString())
        );

        StartCoroutine("MoveSafeArea");
    }

    void WarnSafeArea(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");

    }

    void PlayerDead(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        player.SetActive(false);
        GameObject deadPersonObject = Instantiate(deadPerson, new Vector3(
            player.transform.position.x,
            player.transform.position.y - 0.9f,
            player.transform.position.z), Quaternion.identity);

        deadPersonObject.GetComponent<DeadUI>().Setup(jsonData);
    }

    void EnemyDead(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        players[jsonData[0].ToString()].gameObject.SetActive(false);
        Instantiate(enemyDeadPerson, new Vector3(
            players[jsonData[0].ToString()].transform.position.x,
            players[jsonData[0].ToString()].transform.position.y - 0.9f,
            players[jsonData[0].ToString()].transform.position.z),
        Quaternion.identity);
    }

    void KillFeed(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");

    }

    IEnumerator MoveSafeArea()
    {
        float timeToMove = 10f;
        float t = 0f;
        while (t < 1)
        {
            if (safeArea == null) break;
            t += Time.deltaTime / timeToMove;
            safeArea.transform.position = Vector3.Lerp(currentSafeAreaPos, newSafeAreaPos, t);
            safeArea.transform.localScale = Vector3.Lerp(currentSafeAreaScale, newSafeAreaScale, t);
            yield return null;
        }

        currentSafeAreaPos = newSafeAreaPos;
        currentSafeAreaScale = newSafeAreaScale;
    }

    void CurrentPlayerAlive(SocketIOEvent evts)
    {
        JSONObject jsonData = evts.data.GetField("d");
        playerUI.SetCurrentPlayerAlive(jsonData[0].ToString());
    }

    void PlayerWin(SocketIOEvent evts)
    {
        JSONObject jsonData = evts.data.GetField("d");
        player.GetComponent<FirstPersonController>().enabled = false;
        playerUI.ShowWinScreen(jsonData);
    }

    public void ResetRoom()
    {
        foreach (KeyValuePair<string, GameObject> pair in players)
        {
            Destroy(pair.Value);
        }

        players.Clear();

        foreach (KeyValuePair<string, GameObject> pair in weapons)
        {
            Destroy(pair.Value);
        }

        weapons.Clear();
    }
}
