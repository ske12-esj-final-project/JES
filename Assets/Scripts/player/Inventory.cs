using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using SocketIO;

public class Inventory : MonoBehaviour
{
    SocketIOComponent socket;

    [Header("Items")]
    public Transform currentWeapon;

    //Dictionary <KeyCode, Transform>
    Dictionary<KeyCode, Transform> keyItems = new Dictionary<KeyCode, Transform>();

    private PlayerUI playerUI;
    public int currentAmmo;
    public int availableSlots = 2;

    void Start()
    {
        keyItems.Add(KeyCode.Alpha1, null);
        keyItems.Add(KeyCode.Alpha2, null);

        playerUI = transform.GetComponent<PlayerUI>();

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        currentWeapon = transform.GetChild(2).GetChild(0);
    }

    void Update()
    {
        // If item key is pressed
        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (Input.GetKeyDown(pair.Key) && pair.Value != null)
            {
                ChangeItem(pair.Value);
            }
        }
    }

    public void AddItem(Transform item)
    {
        KeyCode key = KeyCode.None;
        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (pair.Value == null)
            {
                key = pair.Key;
                break;
            }
        }

        if (key == KeyCode.None) return;

        int idx = item.GetComponent<ItemPickup>().weaponIndex;
        Transform weapon = transform.GetChild(2).GetChild(idx);
        keyItems[key] = weapon;
        playerUI.SetupWeapon(weapon.gameObject.GetComponent<Weapon>());
        availableSlots--;

        if (availableSlots == 1)
        {
            ChangeItem(weapon);
        }
    }

    public bool isFull()
    {
        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (pair.Value == null) return false;
        }

        return true;
    }

    // Change cuurent weapon by index
    public void ChangeItem(Transform weapon)
    {
        if (currentWeapon != null)
        {
            currentWeapon.gameObject.SetActive(false);
        }

        currentWeapon = weapon;
        currentWeapon.gameObject.SetActive(true);
        EmitCurrentWeapon(currentWeapon.GetSiblingIndex());
    }

    void EmitCurrentWeapon(int index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string player = transform.name.Replace("\"", "");
        string s = string.Format("[@{0}@, {1}]", player, index);
        data["d"] = s;
        socket.Emit("b", new JSONObject(data));
    }
}