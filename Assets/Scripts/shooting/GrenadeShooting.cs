using System.Collections;
using UnityEngine;

public class GrenadeShooting : Shooting
{
    private Grenade grenade;
    bool isRunning;
    bool isJumping;
    bool isGrenadeReloading;

    void Awake()
    {
        grenade = (Grenade)GetComponents<Weapon>()[0];
        isGrenadeReloading = true;
    }

    void Update()
    {
        AnimationCheck();
        //Left click to throw grenade
        //Disable if currently "reloading" grenade, and if running or jumping
        if (Input.GetMouseButtonDown(0) &&
                !isGrenadeReloading && !isRunning && !isJumping)
        {
            //Disable grenade throwing
            isGrenadeReloading = true;
            //Play throwing animations
            anim.SetTrigger("Throw");

            //Start throwing grenade
            StartCoroutine(GrenadeThrow());
            //Start hide grenade timer
            StartCoroutine(HideGrenadeTimer());
        }
    }

    //Spawn grenade projectile
    IEnumerator GrenadeThrow()
    {
        //Play grenade sound
        grenade.Audios.mainAudioSource.clip = grenade.Audios.shootSound;
        grenade.Audios.mainAudioSource.Play();

        //Wait for set amount of time to throw grenade
        yield return new WaitForSeconds(grenade.throwDelay);
        //Spawn the grenade projectile
        Instantiate(grenade.projectile,
            grenade.grenadeSpawnPoint.transform.position,
            grenade.grenadeSpawnPoint.transform.rotation);
    }

    //Used to hide and show the grenade mesh
    IEnumerator HideGrenadeTimer()
    {
        //Wait for set amount of time
        yield return new WaitForSeconds(grenade.hideGrenadeTimer);
        //Hide the current grenade projectile mesh
        grenade.currentProjectile.GetComponent
        <SkinnedMeshRenderer>().enabled = false;

        //Wait for set amount of time, to show the grenade again
        yield return new WaitForSeconds(grenade.showGrenadeTimer);
        //Show the current grenade projectile mesh
        grenade.currentProjectile.GetComponent
        <SkinnedMeshRenderer>().enabled = true;
    }

    void AnimationCheck()
    {
        isRunning = anim.GetCurrentAnimatorStateInfo(0).IsName("Run");
        isJumping = anim.GetCurrentAnimatorStateInfo(0).IsName("Jump");
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            isGrenadeReloading = false;
        }
    }
}
