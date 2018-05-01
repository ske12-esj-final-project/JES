using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseInsert : StateMachineBehaviour {

	 public int reloadedBullet;
 
   
	// OnStateExit is called when a transition ends and the state 
	//machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetInteger("bullet", reloadedBullet);
		animator.SetBool("IsInserting", false);
	}
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		animator.SetBool("IsInserting", true);
	}
}
