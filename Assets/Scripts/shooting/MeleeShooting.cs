using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class MeleeShooting : Shooting
{
    private Melee melee;
    private bool isDrawing;
    private bool isRunning;
    private bool isJumping;
    public bool isMeleeAttacking;

    //Random number generated to choose 
    //attack animation for melee
    private int randomAttackAnim;
    void Awake()
    {
        melee = GetComponent<Melee>();
        // melee.Audios.mainAudioSource.clip = melee.Audios.shootSound;
        //Disable the weapon trail at start
        // melee.Effects.weaponTrail.GetComponent<TrailRenderer>().enabled = false;
    }

    void Update()
    {
        //Check which animation is currently playing
        AnimationCheck();

        //Left click (if automatic fire is false)
        if (CrossPlatformInputManager.GetButton("Fire1") && isDisableShooting())
        {
            randomAttackAnim = Random.Range(1, 4);
            Debug.Log("Attack Melee");
            //Play attack animation, if not currently attacking or drawing weapon
            if (!isMeleeAttacking && !isDrawing)
            {
                anim.SetTrigger("Attack " + randomAttackAnim);
                EmitShoot();
                //Play weapon sound
                // melee.Audios.mainAudioSource.Play();
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
            // melee.Effects.weaponTrail.GetComponent<TrailRenderer>().enabled = true;
        }
        else
        {
            //If not attacking
            isMeleeAttacking = false;
            //Disable the weapon trail
            // melee.Effects.weaponTrail.GetComponent<TrailRenderer>().enabled = false;
        }
    }
    void EmitShoot()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["d"] = string.Format("");
        socket.Emit("9", new JSONObject(data));
    }
}
