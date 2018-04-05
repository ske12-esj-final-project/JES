using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItemUI : MonoBehaviour
{
	public Text weaponAmmoText;
	public Image weaponImage;
	
	void Start()
	{
		
	}

    public void Set(int ammo, Sprite sprite)
	{
		weaponAmmoText.text = ammo.ToString();
		weaponImage.sprite = sprite;
	}

	public void SetAmmo(int ammo)
	{
		weaponAmmoText.text = ammo.ToString();
	}
}
