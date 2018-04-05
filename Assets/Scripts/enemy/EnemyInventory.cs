using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInventory : MonoBehaviour {

	public void ChangeItem(int index)
	{
		Transform weapons = transform.GetChild(1);
		for (int i = 0; i < weapons.childCount; i++)
		{
			if (i == index)
			{
				weapons.GetChild(i).gameObject.SetActive(true);
			}
			else
			{
				weapons.GetChild(i).gameObject.SetActive(false);
			}
		}
	}
}
