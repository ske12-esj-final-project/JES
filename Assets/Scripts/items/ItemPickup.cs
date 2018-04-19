using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocketIO;
public class ItemPickup : MonoBehaviour
{
    public int weaponIndex;
    public int capacity;
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
            player = GameManager.GetThisPlayer();
            inventory = player.GetComponent<Inventory>();
            
            if (gameObject.CompareTag("MedKit")) socket.Emit("d", GetEmitPickupData());

            else if (gameObject.CompareTag("Ammo"))
            {
                socket.Emit("g", GetEmitPickupData());
                inventory.currentAmmo += 10;
                player.GetComponent<PlayerUI>().SetInventoryAmmoText(inventory.currentAmmo);
            }

            else if (!inventory.isFull())
            {
                transform.gameObject.SetActive(false);
                inventory.AddItem(transform);
                socket.Emit("d", GetEmitPickupData());
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

    public void Setup(int _capacity)
    {
        capacity = _capacity;
    }
}
