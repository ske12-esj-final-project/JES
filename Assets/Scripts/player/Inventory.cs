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

    Dictionary<KeyCode, Transform> keyItems = new Dictionary<KeyCode, Transform>();

    private PlayerUI playerUI;
    public int currentAmmo;
    public int availableSlots = 2;
    private Transform bareHand;

    void Start()
    {
        keyItems.Add(KeyCode.Alpha1, null);
        keyItems.Add(KeyCode.Alpha2, null);

        playerUI = transform.GetComponent<PlayerUI>();

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        bareHand = transform.GetChild(1).GetChild(0);

        currentWeapon = bareHand;
    }

    void Update()
    {
        // If item key is pressed
        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (Input.GetKeyDown(pair.Key) && pair.Value != null && currentWeapon != pair.Value)
            {
                ChangeItem(pair.Value);
            }
        }

        if (Input.GetKeyDown(KeyCode.G) && currentWeapon != bareHand)
        {
            DiscardCurrentItem();
        }
    }

    public void AddItem(Transform item)
    {
        KeyCode key = KeyCode.None;
        ItemPickup pickup = item.GetComponent<ItemPickup>();

        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (pair.Value == null)
            {
                key = pair.Key;
                break;
            }
        }

        if (key == KeyCode.None) return;

        int idx = pickup.weaponIndex;
        Transform weapon = transform.GetChild(1).GetChild(idx);
        
        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (pair.Value == weapon)
            {
                currentAmmo += 10;
                playerUI.SetInventoryAmmoText(currentAmmo);
                return;
            }
        }

        weapon.GetComponent<Weapon>().Setup(pickup.transform.name, pickup.capacity);
        keyItems[key] = weapon;
        availableSlots--;

        playerUI.SetupWeapon(weapon.gameObject.GetComponent<Weapon>());

        if (availableSlots == 1)
        {
            ChangeItem(weapon);
        }
    }

    void DiscardCurrentItem()
    {
        playerUI.DeleteWeapon(currentWeapon.gameObject.GetComponent<Weapon>());
        EmitDiscardWeapon();
        currentWeapon.gameObject.SetActive(false);
        KeyCode key = KeyCode.None;
        KeyCode replacedKey = KeyCode.None;
        
        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (pair.Value == currentWeapon)
            {
                key = pair.Key;
                availableSlots++;
                break;
            }
        }

        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (pair.Key != key && pair.Value != null)
            {
                replacedKey = pair.Key;
                break;
            }
        }

        keyItems[key] = null;
        if (availableSlots == 2) ChangeItem(bareHand);
        else if (availableSlots == 1) ChangeItem(keyItems[replacedKey]);
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

        if (currentWeapon != bareHand)
        {
            playerUI.EnableWeapon(currentWeapon.gameObject.GetComponent<Weapon>());
        }

        EmitCurrentWeapon(currentWeapon.GetSiblingIndex());
    }
    public void DropItemsWhenDie()
    {

        foreach (KeyValuePair<KeyCode, Transform> pair in keyItems)
        {
            if (pair.Value == null)
            {
                break;
            }
        }
    }

    void EmitCurrentWeapon(int index)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string player = transform.name.Replace("\"", "");
        string s = string.Format("[@{0}@, {1}]", player, index);
        data["d"] = s;
        socket.Emit("b", new JSONObject(data));
    }

    void EmitDiscardWeapon()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        Weapon wpn = currentWeapon.GetComponent<Weapon>();
        string ID = wpn.ID.Replace("\"", "");
        float distance = 2;
        Vector3 pos = transform.position + transform.forward * distance;
        string s = string.Format("[@{0}@,{1},{2},{3},{4}]", ID, wpn.currentAmmo, pos.x, pos.y, pos.z);
        data["d"] = s;
        socket.Emit("i", new JSONObject(data));
    }
}