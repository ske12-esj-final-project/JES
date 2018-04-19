using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpectateUI : MonoBehaviour
{
    public Button previousPlayerButton;
    public Button nextPlayerButton;
    private static List<string> playerList;
    private int index = 0;
    private GameObject currentPlayer;

    // Use this for initialization
    void Start()
    {
        playerList = new List<string>(GameManager.GetPlayers().Keys);
        previousPlayerButton.GetComponent<Button>().onClick.AddListener(() => PreviousPlayer());
        nextPlayerButton.GetComponent<Button>().onClick.AddListener(() => NextPlayer());
        Spectate();
    }

    void PreviousPlayer()
    {
        if (index > 0) index--;
        else index = playerList.Count - 1;
        Spectate();
    }

    void NextPlayer()
    {
        if (index < playerList.Count - 1) index++;
        else index = 0;
        Spectate();
    }

    void Spectate()
    {
        SetCurrentPlayerCamera(false);
        GameObject player = GameManager.GetPlayer(playerList[index]);
        if (player != null) currentPlayer = player;
        SetCurrentPlayerCamera(true);
    }

    void SetCurrentPlayerCamera(bool _enabled)
    {
        if (currentPlayer)
        {
            currentPlayer.transform.GetChild(2).gameObject.SetActive(_enabled);
        }
    }
}
