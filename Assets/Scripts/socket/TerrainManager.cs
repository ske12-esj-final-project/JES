using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using SocketIO;

public class TerrainManager : MonoBehaviour
{
    private SocketIOComponent socket;
    public GameObject safeArea, warnSafeArea;
    public GameObject Ammo;
    private Vector3 currentSafeAreaPos, newSafeAreaPos;
    private Vector3 currentSafeAreaScale, newSafeAreaScale;
    public LoadingScreenControl loadingScreen;
    public GameObject deadPerson, enemyDeadPerson;

    private static Dictionary<string, GameObject> players;
    private static Dictionary<string, GameObject> weapons;
    private static Dictionary<string, GameObject> ammos;
    public GameObject[] weaponMapper;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        players = GameManager.GetPlayers();
        weapons = GameManager.GetWeapons();
        ammos = GameManager.GetAmmos();
        StartCoroutine("StartGame");
        socket.On("b", UpdateEnemyCurrentWeapon);
        socket.On("c", SpawnWeapon);
        socket.On("d", RemovePickupWeapon);
        socket.On("g", RemovePickupAmmo);
        socket.On("h", SpawnAmmo);
        socket.On("j", WarnSafeAreaTime);
        socket.On("k", PlayerWin);
        socket.On("n", PlayerDead);
        socket.On("o", PlayerKill);
        socket.On("p", EnemyDead);
        socket.On("u", GetSafeAreaInfo);
        socket.On("t", WarnSafeArea);
    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(1);
        Dictionary<string, string> data = new Dictionary<string, string>();
        string playerID = PlayerPrefs.GetString("playerID");
        data["d"] = string.Format("[@{0}@]", playerID);
        socket.Emit("v", new JSONObject(data));
        currentSafeAreaPos = safeArea.transform.position;
        currentSafeAreaScale = safeArea.transform.localScale;
    }

    void UpdateEnemyCurrentWeapon(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        int currentWeaponIndex = int.Parse(jsonData[1].ToString());
        GameObject p = players[jsonData[0].ToString()];
        p.GetComponent<EnemyInventory>().ChangeItem(currentWeaponIndex);
    }

    void SpawnWeapon(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        foreach (var item in jsonData.list)
        {
            float xAxis = float.Parse(item[2].ToString());
            float yAxis = float.Parse(item[3].ToString());
            float zAxis = float.Parse(item[4].ToString());
            GameObject weapon = weaponMapper[0];
            int idx = int.Parse(item[1].ToString());
            if (!weapons.ContainsKey(item[0].ToString()))
            {
                weapon = Instantiate(weaponMapper[idx - 1], new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
                if (item[0].ToString() == "11") RegisterMedkit(item[0].ToString(), weapon);
                RegisterWeapon(item[0].ToString(), int.Parse(item[5].ToString()), weapon);
            }
        }
    }

    void SpawnAmmo(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        foreach (var item in jsonData.list)
        {
            float xAxis = float.Parse(item[1].ToString());
            float yAxis = float.Parse(item[2].ToString());
            float zAxis = float.Parse(item[3].ToString());
            GameObject ammo = Instantiate(Ammo, new Vector3(xAxis, yAxis, zAxis), Quaternion.identity);
            RegisterAmmo(item[0].ToString(), ammo);
        }
    }

    void RegisterWeapon(string _weaponID, int capacity, GameObject _weapon)
    {
        weapons.Add(_weaponID, _weapon);
        _weapon.name = _weaponID;
        _weapon.GetComponent<ItemPickup>().Setup(capacity);
    }

    void RegisterAmmo(string _ammoID, GameObject _ammo)
    {
        ammos.Add(_ammoID, _ammo);
        _ammo.name = _ammoID;
    }

    void RegisterMedkit(string _medkitID, GameObject medkit)
    {
        weapons.Add(_medkitID, medkit);
        medkit.name = _medkitID;
    }

    void RemovePickupWeapon(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Destroy(weapons[jsonData[0].ToString()]);
        weapons.Remove(jsonData[0].ToString());
    }

    void RemovePickupAmmo(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        Destroy(ammos[jsonData[0].ToString()]);
        ammos.Remove(jsonData[0].ToString());
    }

    void GetSafeAreaInfo(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
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
        if (warnSafeArea != null)
        {
            warnSafeArea.transform.position = new Vector3(
                float.Parse(jsonData[0].ToString()),
                float.Parse(jsonData[1].ToString()),
                float.Parse(jsonData[2].ToString())
            );
            warnSafeArea.transform.localScale = new Vector3(
                float.Parse(jsonData[3].ToString()),
                float.Parse(jsonData[4].ToString()),
                float.Parse(jsonData[5].ToString())
            );
        }
    }

    void PlayerDead(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        GameObject player = GameManager.GetThisPlayer();
        GameObject.Find("Minimap").SetActive(false);
        player.GetComponent<FirstPersonController>().SetCursorLock(false);
        int idx = player.GetComponent<ClothManager>().clothIndex;
        player.SetActive(false);
        GameObject deadPersonObject = Instantiate(deadPerson, new Vector3(
            player.transform.position.x,
            player.transform.position.y - 0.9f,
            player.transform.position.z), Quaternion.identity);

        deadPersonObject.GetComponent<ClothManager>().ChangeCloth(idx);
        Transform UI = deadPersonObject.transform.GetChild(1).GetChild(0);
        UI.GetComponent<DeadUI>().Setup(jsonData);
    }

    void EnemyDead(SocketIOEvent evt)
    {
        JSONObject jsonData = evt.data.GetField("d");
        GameObject player = GameManager.GetThisPlayer();
        GameObject enemy = players[jsonData[0].ToString()];
        int idx = enemy.GetComponent<ClothManager>().clothIndex;
        enemy.gameObject.SetActive(false);

        GameObject enemyDead = Instantiate(enemyDeadPerson, new Vector3(
            players[jsonData[0].ToString()].transform.position.x,
            players[jsonData[0].ToString()].transform.position.y - 0.9f,
            players[jsonData[0].ToString()].transform.position.z),
        Quaternion.identity);

        enemyDead.GetComponent<ClothManager>().ChangeCloth(idx);
        string playerName = jsonData[1].ToString().Replace("\"", "");
        string enemyName = jsonData[2].ToString().Replace("\"", "");
        int weaponIndex = int.Parse(jsonData[3].ToString());
        player.transform.GetChild(2).GetChild(12).GetComponent<KillFeed>().SetUp(playerName, enemyName, weaponIndex);
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
        PlayerUI playerUI = GameManager.GetThisPlayer().GetComponent<PlayerUI>();
        playerUI.SetCurrentPlayerAlive(jsonData[0].ToString());
    }

    void PlayerWin(SocketIOEvent evts)
    {
        JSONObject jsonData = evts.data.GetField("d");
        GameObject player = GameManager.GetThisPlayer();
        player.GetComponent<FirstPersonController>().SetCursorLock(false);
        player.GetComponent<FirstPersonController>().enabled = false;
        player.GetComponent<Inventory>().currentWeapon.GetComponent<Shooting>().enabled = false;
        safeArea.GetComponent<SafeArea>().enabled = false;
        PlayerUI playerUI = player.GetComponent<PlayerUI>();
        playerUI.ShowWinScreen(jsonData);
    }

    void PlayerKill(SocketIOEvent evts)
    {
        JSONObject jsonData = evts.data.GetField("d");
        PlayerUI playerUI = GameManager.GetThisPlayer().GetComponent<PlayerUI>();
        playerUI.SetPlayerKill(jsonData[0].ToString());
    }

    void WarnSafeAreaTime(SocketIOEvent evts)
    {
        JSONObject jsonData = evts.data.GetField("d");
        PlayerUI playerUI = GameManager.GetThisPlayer().GetComponent<PlayerUI>();
        playerUI.SetWarnSafeAreaTime(jsonData[0].ToString());
    }
}
