using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;

public class ClothUI : MonoBehaviour
{
    public Button previousButton;
    public Button nextButton;
    public Button backButton;
    public Button confirmChangeButton;
    private ClothManager clothManager;
    private int index;
    private SocketIOComponent socket;

    // Use this for initialization
    void Start()
    {
        index = GameManager.GetClothIndex();
        clothManager = GameObject.Find("Enemies").GetComponent<ClothManager>();
        previousButton.GetComponent<Button>().onClick.AddListener(() => PreviousCloth());
        nextButton.GetComponent<Button>().onClick.AddListener(() => NextCloth());
        backButton.GetComponent<Button>().onClick.AddListener(() => CloseClothUI());
        confirmChangeButton.GetComponent<Button>().onClick.AddListener(() => ConfirmChangeCloth());

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
    }

    void PreviousCloth()
    {
        if (index > 0) index--;
        else index = clothManager.clothTextureList.Length - 1;
        ChangeCloth();
    }

    void NextCloth()
    {
        if (index < clothManager.clothTextureList.Length - 1) index++;
        else index = 0;
        ChangeCloth();
    }

    void ChangeCloth()
    {
        clothManager.ChangeCloth(index);
    }

    void CloseClothUI()
    {
        clothManager.ChangeCloth(GameManager.GetClothIndex());
        GameObject.Find("RoomManager").GetComponent<RoomManager>().CloseClothUI();
    }

    void ConfirmChangeCloth()
    {
        GameManager.SetClothIndex(index);
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["d"] = string.Format("[{0}]", GameManager.GetClothIndex());
        socket.Emit("a1", new JSONObject(data));
        CloseClothUI();
    }

}
