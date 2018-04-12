using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour {
	private GameObject compassImage;
	private GameObject player;
	// Use this for initialization
	void Start () {
		compassImage = GameObject.Find("CompassImg");
		player = GameObject.FindGameObjectWithTag("Player");
		Debug.Log(compassImage.GetComponent<RawImage>());
	}
	
	// Update is called once per frame
	void Update () {
		compassImage.GetComponent<RawImage>	().uvRect = new Rect(player.transform.localEulerAngles.y / 360, 0, 1, 1);
	}
}
