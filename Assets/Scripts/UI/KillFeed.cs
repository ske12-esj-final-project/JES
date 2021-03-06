﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillFeed : MonoBehaviour {

	public GameObject killItem;

	public void SetUp(string player, string enemy, int weaponIndex)
	{
		StartCoroutine(CreateFeedItem(player, enemy, weaponIndex));
	}

	IEnumerator CreateFeedItem(string player, string enemy, int weaponIndex)
	{
		GameObject itemUI = (GameObject)Instantiate(killItem, transform);
		itemUI.GetComponent<KillFeedItem>().SetFeed(player, enemy, weaponIndex);
		yield return new WaitForSeconds(3);
		Destroy(itemUI);
	}
}
