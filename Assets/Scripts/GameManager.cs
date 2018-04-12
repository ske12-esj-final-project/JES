using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private static string playerID = null;
    private static bool isConnected = false;
    private static int clothIndex = 0;
    private static Dictionary<string, GameObject> players = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> weapons = new Dictionary<string, GameObject>();
    private static Dictionary<string, GameObject> ammos = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public static void SetPlayerID(string _playerID)
    {
        playerID = _playerID;
    }

    public static string GetPlayerID()
    {
        return playerID;
    }

    public static void SetPlayerConnected(bool _isConnected)
    {
        isConnected = _isConnected;
    }

    public static bool isPlayerConnected()
    {
        return isConnected;
    }

    public static void SetClothIndex(int _index)
    {
        clothIndex = _index;
    }

    public static int GetClothIndex()
    {
        return clothIndex;
    }

    public static Dictionary<string, GameObject> GetPlayers()
    {
        return players;
    }

    public static Dictionary<string, GameObject> GetWeapons()
    {
        return weapons;
    }

    public static Dictionary<string, GameObject> GetAmmos()
    {
        return ammos;
    }

    public static GameObject getPlayer(string ID)
    {
        return players[ID];
    }

    public static void ResetRoom()
    {
        foreach (KeyValuePair<string, GameObject> pair in players)
        {
            Destroy(pair.Value);
        }

        foreach (KeyValuePair<string, GameObject> pair in weapons)
        {
            Destroy(pair.Value);
        }

        foreach (KeyValuePair<string, GameObject> pair in ammos)
        {
            Destroy(pair.Value);
        }

        players.Clear();
        weapons.Clear();
        ammos.Clear();
    }
}