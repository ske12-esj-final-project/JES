using System.Collections;
using UnityEngine;

public class MeleeShooting : Shooting
{
    private Melee melee;
    bool isDrawing;
    bool isRunning;
    bool isJumping;
    bool isMeleeAttacking;

    //Random number generated to choose 
    //attack animation for melee
    int randomAttackAnim;

    void Awake()
    {
		melee = GetComponent<Melee>();
        melee.Audios.mainAudioSource.clip = melee.Audios.shootSound;
        //Disable the weapon trail at start
        melee.Effects.weaponTrail.GetComponent<TrailRenderer>().enabled = false;
    }

    void Update()
    {
        //Check which animation is currently playing
        AnimationCheck();

        //Left click (if automatic fire is false)
        if (Input.GetMouseButton(0) && isDisableShooting())
        {
            randomAttackAnim = Random.Range(1, 4);

            //Play attack animation, if not currently attacking or drawing weapon
            if (!isMeleeAttacking && !isDrawing)
            {
                anim.SetTrigger("Attack " + randomAttackAnim);
                //Play weapon sound
                melee.Audios.mainAudioSource.Play();
            }

        }
    }

	bool isDisableShooting()
    {
        return !isRunning && !isJumping;
    }

    void AnimationCheck()
    {
        isRunning = anim.GetCurrentAnimatorStateInfo(0).IsName("Run");
        isJumping = anim.GetCurrentAnimatorStateInfo(0).IsName("Jump");
        isDrawing = anim.GetCurrentAnimatorStateInfo(0).IsName("Draw");

        //Check if any melee attack animation is playing
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 1") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 2") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("Attack 3"))
        {
            //If attacking
            isMeleeAttacking = true;
            //Enable the weapon trail, only shown when attacking
            melee.Effects.weaponTrail.GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            //If not attacking
            isMeleeAttacking = false;
            //Disable the weapon trail
            melee.Effects.weaponTrail.GetComponent<TrailRenderer>().enabled = false;
        }
    }
}
