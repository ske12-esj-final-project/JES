using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class RoomSelectItem : MonoBehaviour
{

    public Text nameText;
    public Text playerText;
    public Text statusText;
    private SocketIOComponent socket;

    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
    }

    public void Setup(JSONObject room)
    {
        nameText.text = RemoveDbq(room[1].ToString());
        playerText.text = string.Format("{0}/{1}", room[2], room[3]);
        statusText.text = RemoveDbq(room[4].ToString());

        this.GetComponent<Button>().onClick.AddListener(() => JoinRoom(room[0].ToString()));
    }

    void JoinRoom(string roomID)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string playerID = PlayerPrefs.GetString("playerID");
        string s = string.Format("[@{0}@, {1}]", playerID, RemoveDbq(roomID));
        data["d"] = s;
        socket.Emit("y", new JSONObject(data));
    }

    string RemoveDbq(string str)
    {
        return str.Replace("\"", "");
    }
}
