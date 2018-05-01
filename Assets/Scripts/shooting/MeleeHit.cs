using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using SocketIO;

public class MeleeHit : MonoBehaviour
{
    Transform parent;
    public SocketIOComponent socket;
    // Use this for initialization
    void Start()
    {
        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
        parent = transform.parent.transform.parent;
    }

    // Update is called once per frame
    void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Enemy" && parent.GetComponent<MeleeShooting>().isMeleeAttacking)
        {
            Debug.Log("Emit Enemy Hit With Hand");
            EmitEnemyHit(other.transform.name.ToString(), 10);
        }
    }

    void EmitEnemyHit(string playerId, float damage)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string player = playerId.Replace("\"", "");
        string s = string.Format("[@{0}@, {1}]", player, damage);
        data["d"] = s;
        socket.Emit("0", new JSONObject(data));
    }
}
