using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class EnemyAnimation : MonoBehaviour {

	private Animator anim;
	private Vector3 currentPosition;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
		currentPosition = this.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (currentPosition != this.transform.position) {
			anim.Play("Walk_Static", -1, 0f);
			currentPosition = this.transform.position;
		}
	}
}
