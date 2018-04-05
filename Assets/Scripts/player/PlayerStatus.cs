using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatus : MonoBehaviour {
	public string playerID;
	public float health;
	private PlayerUI playerUI;
	private GameObject player;
	public float playerMaxHealth;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
        playerUI = player.GetComponent<PlayerUI>();
	}

	public void SetHealth(float newHealth) {
		health = newHealth;
		playerUI.SetPlayerHealth(health/playerMaxHealth);
	}

	public void SetPlayerID(string newPlayerID) {
		playerID = newPlayerID;
	}
}
