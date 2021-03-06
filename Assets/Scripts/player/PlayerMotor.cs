﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour {

	[SerializeField]
	private Camera camera;

	private Animator anim;

	public bool isEnemy;

	[SerializeField]
	private float cameraRotationLimit = 85f;

	private Vector3 velocity = Vector3.zero;
	private Vector3 enemyPosition = Vector3.zero;

	private Vector3 rotation = Vector3.zero;

	private Vector3 enemyRotation = Vector3.zero;

	private float cameraRotationX = 0f;

	private float currentCameraRotationX = 0f;

	private Vector3 thrusterForce = Vector3.zero;

	private Rigidbody rb;

	Vector3 lastPos;
	float threshold = 0.01f;
	bool isWalk = false;
	float Speed_f  = 0f;

	void Start() {
		rb = GetComponent<Rigidbody> ();
		anim = transform.GetChild(0).GetComponent<Animator>();
		lastPos = transform.position;
		enemyPosition = transform.position;
	}

	public void Move(Vector3 _velocity) {
		velocity = _velocity;
	}

	public void EnemyMove(Vector3 _velocity) {
		enemyPosition = _velocity;
	}

	public void EnemyRotate(Vector3 q){
		enemyRotation = q;
	}

	public void Rotate(Vector3 _rotation) {
		rotation = _rotation;
	}

	public void RotateCamera(float _cameraRotationX) {
		cameraRotationX = _cameraRotationX;
	}

	public void ApplyThruster(Vector3 _thrusterForce) {
		thrusterForce = _thrusterForce;
	}

	void FixedUpdate() {
		PerformMovement ();
		PerformRotation ();
		if (this.gameObject.tag == "Enemy") {
			PerformEnemyMovement ();
			PerformEnemyRotation ();
		}
	}


	void Update()
	{
		Vector3 from = transform.position;		
		 Vector3 offset = transform.position - lastPos; 
		 Vector3 nv = new Vector3(decimal2Place(transform.position.x),0,decimal2Place(transform.position.z));
		 Vector3 lv = new Vector3(decimal2Place(enemyPosition.x),0,decimal2Place(enemyPosition.z));
		if ( Vector3.Distance(nv,lv) > threshold ){ 
			// Debug.Log("check is "+anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Static") );
			if(!isWalk && !anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Static")){
				// Debug.Log("move");
				isWalk = true;
				anim.SetBool("isWalk",isWalk);
			}

		} 
		else {
			// anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Static") 
			if(isWalk){
				isWalk = false;
				anim.SetBool("isWalk",isWalk);
				anim.SetFloat("Speed_f", 0.0f);
				// Debug.Log("idle");
			}

		} 		
	}

	void PerformMovement() {
		if ( velocity != Vector3.zero) {
			rb.MovePosition (rb.position + velocity * Time.fixedDeltaTime);
		}

		if (thrusterForce != Vector3.zero) {
			rb.AddForce (thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
		}
	}

	float decimal2Place(float num){
		return float.Parse(num.ToString("0.##") );
	}

	void PerformEnemyMovement() {
		Vector3 from = transform.position;
		Vector3 to = enemyPosition;
		if (to != Vector3.zero) {
			transform.position = Vector3.Lerp (from, to, 0.1f);
		}
		
		//  Vector3 offset = transform.position - lastPos; 
		//  Vector3 nv = new Vector3(decimal2Place(transform.position.x),0,decimal2Place(transform.position.z));
		//  Vector3 lv = new Vector3(decimal2Place(enemyPosition.x),0,decimal2Place(enemyPosition.z));

		// //  Debug.Log("distance "+Vector3.Distance(nv,lv));
		// // Debug.Log( anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") );
		// if ( Vector3.Distance(nv,lv) >= threshold ){ 
		// 	Debug.Log("check is "+anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Static") );
		// 	if(!isWalk){
		// 		anim.SetBool("isWalk",true);
		// 		Debug.Log("move");
		// 		isWalk = true;
		// 	}

		// } 
		// else {
		// 	// anim.GetCurrentAnimatorStateInfo(0).IsName("Walk_Static") 
		// 	if(isWalk){
		// 		isWalk = false;
		// 		anim.SetBool("isWalk",isWalk);
		// 		// anim.SetFloat("Speed_f", 0.0f);
		// 		Debug.Log("idle");
		// 	}

		// } 
	}

	void PerformRotation() {
		rb.MoveRotation (rb.rotation * Quaternion.Euler (rotation));
		if (camera != null) {
			currentCameraRotationX -= cameraRotationX;
			currentCameraRotationX = Mathf.Clamp (currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);
			camera.transform.localEulerAngles = new Vector3 (currentCameraRotationX, 0f, 0f);
		}
	}

	void PerformEnemyRotation() {

		Quaternion from = transform.rotation;
			
		Quaternion to = Quaternion.Euler(0, enemyRotation.y, 0);

		Quaternion fromArm = this.transform.GetChild(1).transform.rotation;
		// Debug.Log(fromArm);
		Quaternion toArm = Quaternion.Euler(enemyRotation.x, enemyRotation.y, 0);

		if(toArm != Quaternion.identity){
			this.transform.GetChild(1).transform.rotation = Quaternion.Lerp(fromArm, toArm, 0.1f);
		}

		if (to != Quaternion.identity) {
			transform.rotation = Quaternion.Lerp (from, to, 0.1f);
		}
	}

}
