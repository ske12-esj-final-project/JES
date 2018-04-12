using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class SafeArea : MonoBehaviour
{
    private PlayerUI playerUI;
    private SocketIOComponent socket;
    private Vector3 defaultPosition = new Vector3(300f, 3f, 60f);
    private Vector3 defaultScale = new Vector3(300f, 40f, 300f);


    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        DontDestroyOnLoad(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is back in safe area");
            playerUI = other.GetComponent<PlayerUI>();
            playerUI.HideSafeAreaOverlay();
            socket.Emit("r");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player is out of safe area");
            playerUI = other.GetComponent<PlayerUI>();
            playerUI.ShowSafeAreaOverlay();
            socket.Emit("s");
        }
    }

    public void SetSafeAreaEnabled(bool isEnabled)
    {
        GetComponent<Collider>().isTrigger = isEnabled;
        GetComponent<Renderer>().enabled = isEnabled;
    }

    public void ResetSafeArea()
    {
        Debug.Log("ResetSafeArea");
        gameObject.transform.position = defaultPosition;
        gameObject.transform.localScale = defaultScale;
    }
}
