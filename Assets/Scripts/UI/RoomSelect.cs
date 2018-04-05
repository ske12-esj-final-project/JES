using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class RoomSelect : MonoBehaviour
{
    public GameObject roomSelectList;
    public GameObject roomSelectItem;
    public Button closeButton;
    private SocketIOComponent socket;

    void Start()
    {
        closeButton.GetComponent<Button>().onClick.AddListener(() => CloseRoomSelect());
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        socket.On("q", UpdateRoom);
        RequestRoomInfo();
    }

    void OnEnable()
    {
        if (socket) RequestRoomInfo();
    }

    void OnDestroy()
    {
        StopRequestRoomInfo();
    }

    void UpdateRoom(SocketIOEvent evt)
    {
        JSONObject roomData = evt.data.GetField("d");
        Debug.Log(roomData);
        SetupRoomList(roomData);
    }

    void SetupRoomList(JSONObject roomData)
    {
        foreach (Transform child in roomSelectList.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < roomData.Count; i++)
        {
            GameObject roomObject = (GameObject)Instantiate(roomSelectItem, roomSelectList.transform);
            roomObject.GetComponent<RoomSelectItem>().Setup(roomData[i]);
        }
    }

    void CloseRoomSelect()
    {
        gameObject.SetActive(false);
        StopRequestRoomInfo();
    }

    void RequestRoomInfo()
    {
        socket.Emit("q");
    }

    void StopRequestRoomInfo()
    {
        socket.Emit("p");
    }
}
