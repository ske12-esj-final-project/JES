using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SocketIO;

public class PlayerRotationScript : MonoBehaviour {
	private SocketIOComponent socket;
	private Vector3 currentRotation;
	// Use this for initialization
	
	void Start () {
		GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();
		
		currentRotation = transform.rotation.eulerAngles;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateRotate(transform.rotation.eulerAngles);
	}
	void UpdateRotate(Vector3 rotation)
    {
        if (currentRotation != rotation)
        {

            currentRotation = rotation;
            Dictionary<string, string> data = new Dictionary<string, string>();
			string s = string.Format("[{0:0.###},{1:0.###}]", rotation.x, rotation.y);
            data["d"] = s;
            socket.Emit("8", new JSONObject(data));
        }
    }
}
