using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponItemUI : MonoBehaviour
{
    public Text weaponAmmoText;
    public Image weaponImage;

    public void Set(Weapon weapon)
    {
        weaponAmmoText.text = weapon.currentAmmo.ToString();
        weaponImage.sprite = weapon.weaponImage;
    }

    public void Enable()
    {
        GetComponent<Outline>().enabled = true;
    }

    public void Disable()
    {
        GetComponent<Outline>().enabled = false;
    }
}
