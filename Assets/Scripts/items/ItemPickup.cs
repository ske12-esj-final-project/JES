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
            player = GameObject.FindGameObjectWithTag("Player");
            inventory = player.GetComponent<Inventory>();
            if (gameObject.tag != "Ammo" && !inventory.isFull())
            {
                transform.gameObject.SetActive(false);
                inventory.AddItem(transform);
                EmitPickUpWeapon();
            }

            else inventory.currentAmmo += 10;
        }
    }
    void EmitPickUpWeapon() {
        Debug.Log("Emit Pickup");
        Dictionary<string, string> data = new Dictionary<string, string>();
        string weaponID = transform.name.Replace("\"", "");
        string s = string.Format("[@{0}@]", weaponID);
        data["d"] = s;
        socket.Emit("d", new JSONObject(data));
    }
}
