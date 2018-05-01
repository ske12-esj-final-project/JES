using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartAnimation : MonoBehaviour {
	public float speed = 10.0f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		RectTransform tempPos = GetComponent<RectTransform>();
		if (tempPos.localPosition.x > 250) {
			GetComponent<RectTransform>().localPosition = new Vector3(-300, tempPos.localPosition.y, tempPos.localPosition.z);
		} else {
			GetComponent<RectTransform>().localPosition = new Vector3(tempPos.localPosition.x + speed * Time.deltaTime, tempPos.localPosition.y, tempPos.localPosition.z);
		}
	}
}
