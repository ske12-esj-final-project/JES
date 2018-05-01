using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothManager : MonoBehaviour
{
    public Texture[] clothTextureList;
    public Texture[] armTextureList;
    public int clothIndex;

    public void ChangeCloth(int index)
    {
        clothIndex = index;
        Transform model = transform.GetChild(0).GetChild(0);
        model.GetComponent<Renderer>().material.mainTexture = clothTextureList[index];
        Transform arms = transform.GetChild(1);

        if (arms.name == "Arm")
        {
            for (int i = 0; i < arms.childCount; i++)
            {
                Transform arm = arms.GetChild(i).GetChild(1);
                arm.GetComponent<Renderer>().material.mainTexture = armTextureList[index];
            }
        }
    }
}
