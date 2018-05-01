using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public Sprite[] tutorials;
	public Image tutorialImage;
	public Button previousButton;
	public Button nextButton;
	public Button backButton;
	private int index = 0;

	// Use this for initialization
	void Start () {
		previousButton.GetComponent<Button>().onClick.AddListener(() => PreviousTutorial());
		nextButton.GetComponent<Button>().onClick.AddListener(() => NextTutorial());
		backButton.GetComponent<Button>().onClick.AddListener(() => CloseTutorial());

		ChangeTutorial();
	}

	void PreviousTutorial()
	{
		if (index > 0) index--;
        else index = tutorials.Length - 1;
        ChangeTutorial();
	}

	void NextTutorial()
    {
        if (index < tutorials.Length - 1) index++;
        else index = 0;
        ChangeTutorial();
    }

	void ChangeTutorial()
	{
		tutorialImage.GetComponent<Image>().sprite = tutorials[index];
	}

	void CloseTutorial()
	{
		GameObject.Find("RoomManager").GetComponent<RoomManager>().CloseTutorialUI();
	}
}
