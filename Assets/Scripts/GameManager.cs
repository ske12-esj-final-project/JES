using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    private static string playerID = null;
    private static bool isConnected = false;

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
}