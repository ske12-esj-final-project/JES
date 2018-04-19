using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour {

	public string player;
	public string enemy;
	public Sprite[] weaponIconArray;
	public Text playerText;
	public Image weaponIcon;
	public Text enemyText;
	// Use this for initialization
	void Start () {
		// playerText = transform.GetChild(0).gameObject.GetComponent<Text>();
		// weaponIcon = transform.GetChild(1).gameObject.GetComponent<Image>(); 
		// enemyText = transform.GetChild(2).gameObject.GetComponent<Text>();
	}

	public void SetFeed(string player, string enemy, int weaponIndex) {
		// feedText.text
		playerText.text = player;
		enemyText.text = enemy;
		weaponIcon.sprite = weaponIconArray[weaponIndex];
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
