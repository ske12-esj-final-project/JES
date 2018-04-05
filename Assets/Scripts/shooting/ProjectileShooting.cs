using System.Collections;
using UnityEngine;
using SocketIO;

public class ProjectileShooting : Shooting
{
    private ProjectileWeapon projectileWeapon;

    bool isReloading;
    bool outOfAmmo;
    bool isShooting;
    bool isAimShooting;
    bool isAiming;
    bool isDrawing;
    bool isRunning;
    bool isJumping;

    void Awake()
    {
        projectileWeapon.Effects.lightFlash.GetComponent<Light>().enabled = false;
    }

    void Update()
    {
        //Check which animation is currently playing
        AnimationCheck();
        //Left click (if automatic fire is false)
        if (Input.GetMouseButton(0) && isDisableShooting())
        {
            StartCoroutine(ProjectileShoot(projectileWeapon));
        }

        //Right click hold to aim
        //Disable aiming for melee weapons and grenade
        if (Input.GetMouseButton(1))
        {
            anim.SetBool("isAiming", true);
        }
        else
        {
            anim.SetBool("isAiming", false);
        }

        //R key to reload
        //Not used for projectile guns, grenade or melee guns
        if (Input.GetKeyDown(KeyCode.R) && !isReloading)
        {
            Reload();
        }

        //Run when holding down left shift and moving
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetAxis("Vertical") > 0)
        {
            anim.SetFloat("Run", 0.2f);
        }
        else
        {
            //Stop running
            anim.SetFloat("Run", 0.0f);
        }

        //Space key to jump
        //Disable jumping while reloading
        if (Input.GetKeyDown(KeyCode.Space) && !isReloading)
        {
            //Play jump animation
            anim.Play("Jump");
        }

        //If out of ammo
        if (projectileWeapon.currentAmmo == 0)
        {
            outOfAmmo = true;
            //if ammo is higher than 0
        }
        else if (projectileWeapon.currentAmmo > 0)
        {
            outOfAmmo = false;
        }
    }

    bool isDisableShooting()
    {
        return !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping;
    }

    IEnumerator ProjectileShoot(ProjectileWeapon projectileWeapon)
    {
        if (!anim.GetBool("isAiming"))
        {
            anim.Play("Fire");
        }
        else
        {
            anim.SetTrigger("Shoot");
        }

        //Remove 1 bullet
        projectileWeapon.currentAmmo -= 1;

        //Play shoot sound
        projectileWeapon.Audios.mainAudioSource.clip = projectileWeapon.Audios.shootSound;
        projectileWeapon.Audios.mainAudioSource.Play();

        StartCoroutine(MuzzleFlash(projectileWeapon));

        //Spawn the projectile
        Instantiate(projectileWeapon.projectile,
                     projectileWeapon.bulletSpawnPoint.transform.position,
                     projectileWeapon.bulletSpawnPoint.transform.rotation);

        //Hide the current projectile mesh
        projectileWeapon.currentProjectile.GetComponent
            <SkinnedMeshRenderer>().enabled = false;

        yield return new WaitForSeconds(projectileWeapon.reloadTime);

        //Play reload animation
        anim.Play("Reload");

        //Play shoot sound
        projectileWeapon.Audios.mainAudioSource.clip = projectileWeapon.Audios.reloadSound;
        projectileWeapon.Audios.mainAudioSource.Play();

        //Show the current projectile mesh
        projectileWeapon.currentProjectile.GetComponent
            <SkinnedMeshRenderer>().enabled = true;

    }

    void Reload()
    {
        //Play reload animation
        anim.Play("Reload");

        //Play reload sound
        projectileWeapon.Audios.mainAudioSource.clip = projectileWeapon.Audios.reloadSound;
        projectileWeapon.Audios.mainAudioSource.Play();
    }

    //Refill ammo
    void RefillAmmo()
    {
        projectileWeapon.currentAmmo = projectileWeapon.ShootSettings.ammo;
        // playerUI.SetCurrentAmmoText(projectileWeapon.currentAmmo);
    }

    void AnimationCheck()
    {
        isShooting = anim.GetCurrentAnimatorStateInfo(0).IsName("Fire");
        isAimShooting = anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Fire");
        isRunning = anim.GetCurrentAnimatorStateInfo(0).IsName("Run");
        isJumping = anim.GetCurrentAnimatorStateInfo(0).IsName("Jump");
        isDrawing = anim.GetCurrentAnimatorStateInfo(0).IsName("Draw");

        //Check if reloading
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
        {
            // If reloading
            isReloading = true;
            //Refill ammo
            RefillAmmo();
        }
    }
}
