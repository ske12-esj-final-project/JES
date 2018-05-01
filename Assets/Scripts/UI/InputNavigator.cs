using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InputNavigator : MonoBehaviour {

	EventSystem system;

	// Use this for initialization
	void Start () {
		system = EventSystem.current;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			Selectable next = system.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

			if (next != null)
			{
				InputField inputField = next.GetComponent<InputField>();
				if(inputField != null)
				{
					inputField.OnPointerClick(new PointerEventData(system));
				}

				system.SetSelectedGameObject(next.gameObject, new BaseEventData(system));
			}
		}
	}
}
