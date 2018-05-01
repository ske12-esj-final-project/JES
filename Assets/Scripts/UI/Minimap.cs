using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {
	private GameObject player;
	
	// Update is called once per frame
	void LateUpdate () {
		player = GameObject.FindGameObjectWithTag("Player");
		if (player != null) {
			if (player.GetComponent<PlayerUI>().isMinimapOpen) {
				transform.position = new Vector3(0, 450, 0);
			} else {
				Vector3 newPosition = player.transform.position;
				newPosition.y = 120;
				transform.position = newPosition;
			}
		}
	}
}
