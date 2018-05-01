using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using SocketIO;

public class BackButton : MonoBehaviour
{

    public Button backButton;
    private SocketIOComponent socket;

    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        backButton.GetComponent<Button>().onClick.AddListener(() => BackToMenu());
    }

    // Update is called once per frame
    public void BackToMenu()
    {
        socket.Emit("m");
        GameManager.Reset();
        SceneManager.LoadScene("Rooms", LoadSceneMode.Single);
    }
}
