using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClothManager : MonoBehaviour
{
    public Texture[] clothTextureList;
    public Button previousButton;
    public Button nextButton;
	public Button confirmChangeButton;
    private int index = 0;

    // Use this for initialization
    void Start()
    {
        previousButton.GetComponent<Button>().onClick.AddListener(() => PreviousCloth());
        nextButton.GetComponent<Button>().onClick.AddListener(() => NextCloth());
		confirmChangeButton.GetComponent<Button>().onClick.AddListener(() => ConfirmChangeCloth());
    }

    void PreviousCloth()
    {
        if (index > 0) index--;
        else index = clothTextureList.Length - 1;
        ChangeCloth();
    }

    void NextCloth()
    {
        if (index < clothTextureList.Length - 1) index++;
        else index = 0;
        ChangeCloth();
    }

    void ChangeCloth()
    {
        GetComponent<Renderer>().material.mainTexture = clothTextureList[index];
    }

    void ConfirmChangeCloth()
    {
		GameManager.SetClothIndex(index);
    }

}
