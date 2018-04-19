using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public enum State { Start, Connect, Disconnect };
    public static GameObject player;
    private static State state = State.Start;
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

    public static void SetState(State _state)
    {
        state = _state;
    }

    public static State GetState()
    {
        return state;
    }

    public static void SetPlayer(GameObject _player)
    {
        player = _player;
    }

    public static GameObject GetThisPlayer()
    {
        return player;
    }

    public static GameObject GetPlayer(string _playerID)
    {
        return players[_playerID];
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

    public static void Reset()
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