using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class PlayerAnimation : MonoBehaviour {
	public Animator anim;
	private float inputH;
	private float inputV;
	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		// if(Input.GetKeyDown("1")) {
		// 	anim.Play("Crouch_Down", -1, 0f);
		// }
		inputH = Input.GetAxis("Horizontal");
		inputV = Input.GetAxis("Vertical");
		anim.SetFloat("inputH", inputH);
		anim.SetFloat("inputV", inputV);
	}
}
