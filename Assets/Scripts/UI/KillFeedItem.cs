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

	public void SetFeed(string player, string enemy, int weaponIndex) {
		playerText.text = player;
		enemyText.text = enemy;
		weaponIcon.sprite = weaponIconArray[weaponIndex];
	}
}
