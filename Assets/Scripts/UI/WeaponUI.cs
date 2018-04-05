using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponUI : MonoBehaviour
{
    public GameObject itemUIPrefab;
    private Dictionary<Weapon, GameObject> mapper = new Dictionary<Weapon, GameObject>();

    public void Setup(Weapon weapon)
    {
        if (mapper.ContainsKey(weapon))
        {
            mapper[weapon].GetComponent<WeaponItemUI>().Set(weapon.currentAmmo, weapon.weaponImage);
        }

        else
        {
            GameObject itemUI = (GameObject)Instantiate(itemUIPrefab, transform);
            itemUI.GetComponent<WeaponItemUI>().Set(weapon.currentAmmo, weapon.weaponImage);
            mapper.Add(weapon, itemUI);
        }
    }
}
