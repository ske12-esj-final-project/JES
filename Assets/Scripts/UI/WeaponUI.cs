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
            mapper[weapon].GetComponent<WeaponItemUI>().Set(weapon);
        }

        else
        {
            GameObject itemUI = (GameObject)Instantiate(itemUIPrefab, transform);
            itemUI.GetComponent<WeaponItemUI>().Set(weapon);
            mapper.Add(weapon, itemUI);
        }
    }

    public void Enable(Weapon weapon)
    {
        foreach (KeyValuePair<Weapon, GameObject> pair in mapper)
        {
            if (pair.Key == weapon)
            {
                mapper[weapon].GetComponent<WeaponItemUI>().Enable();
            }

            else mapper[pair.Key].GetComponent<WeaponItemUI>().Disable();
        }
    }
}
