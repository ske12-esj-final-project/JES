using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
public class ItemPickup : MonoBehaviour
{
    public int weaponIndex;
    private Inventory inventory;
    private GameObject player;
    public float speed = 20f;
    private SocketIOComponent socket;
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Emit Pickup");
            player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
            if (gameObject.tag != "Ammo" && !inventory.isFull())
            {
                transform.gameObject.SetActive(false);
                inventory.AddItem(transform);
                socket.Emit("d", GetEmitPickupData());
            }

            else
            {
                socket.Emit("g", GetEmitPickupData());
                inventory.currentAmmo += 10;
            }
        }
    }
    JSONObject GetEmitPickupData()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string id = transform.name.Replace("\"", "");
        string s = string.Format("[@{0}@]", id);
        data["d"] = s;
        return new JSONObject(data);
    }
}
