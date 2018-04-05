using System.Collections;
using UnityEngine;

public class GrenadeLauncherShooting : Shooting
{
    private GrenadeLauncher grenadeLauncher;
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
        grenadeLauncher = GetComponent<GrenadeLauncher>();
        grenadeLauncher.Effects.lightFlash.GetComponent<Light>().enabled = false;
    }

    void Update()
    {
        AnimationCheck();
        //Left click (if automatic fire is false)
        if (Input.GetMouseButton(0) && isDisableShooting())
        {
            GrenadeLauncherShoot(grenadeLauncher);
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
        if (grenadeLauncher.currentAmmo == 0)
        {
            outOfAmmo = true;
            //if ammo is higher than 0
        }
        else if (grenadeLauncher.currentAmmo > 0)
        {
            outOfAmmo = false;
        }

    }

    void GrenadeLauncherShoot(GrenadeLauncher grenadeLauncher)
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
        grenadeLauncher.currentAmmo -= 1;

        //Play shoot sound
        grenadeLauncher.Audios.mainAudioSource.clip = grenadeLauncher.Audios.shootSound;
        grenadeLauncher.Audios.mainAudioSource.Play();

        //Spawn the projectile
        Instantiate(grenadeLauncher.projectile,
            grenadeLauncher.Spawnpoints.bulletSpawnPoint.transform.position,
            grenadeLauncher.Spawnpoints.bulletSpawnPoint.transform.rotation);

        //Show the muzzleflash 
        StartCoroutine(MuzzleFlash(grenadeLauncher));
    }

    bool isDisableShooting()
    {
        return !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping;
    }

	//Reload
    void Reload()
    {
        //Play reload animation
        anim.Play("Reload");

        //Play reload sound
        grenadeLauncher.Audios.mainAudioSource.clip = grenadeLauncher.Audios.reloadSound;
        grenadeLauncher.Audios.mainAudioSource.Play();

        //Spawn casing on reload, only used on some weapons
        if (grenadeLauncher.ReloadSettings.casingOnReload == true)
        {
            StartCoroutine(CasingDelay());
        }

        if (outOfAmmo == true && grenadeLauncher.ReloadSettings.hasBulletInMag == true)
        {
            //Hide the bullet inside the mag if ammo is 0
            for (int i = 0; i < grenadeLauncher.ReloadSettings.bulletInMag.Length; i++)
            {
                grenadeLauncher.ReloadSettings.bulletInMag[i].GetComponent
                    <MeshRenderer>().enabled = false;
            }
            //Start the "show bullet" timer
            StartCoroutine(BulletInMagTimer());
        }
    }

	    IEnumerator CasingDelay()
    {
        //Wait set amount of time for casing to spawn
        yield return new WaitForSeconds(grenadeLauncher.ReloadSettings.casingDelay);
        //Spawn a casing at every casing spawnpoint
        for (int i = 0; i < grenadeLauncher.Spawnpoints.casingSpawnPoints.Length; i++)
        {
            Object.Instantiate(grenadeLauncher.casingPrefab,
                         grenadeLauncher.Spawnpoints.casingSpawnPoints[i].transform.position,
                         grenadeLauncher.Spawnpoints.casingSpawnPoints[i].transform.rotation);
        }
    }
    IEnumerator BulletInMagTimer()
    {
        //Wait for set amount of time 
        yield return new WaitForSeconds
            (grenadeLauncher.ReloadSettings.enableBulletTimer);

        //Show the bullet inside the mag
        for (int i = 0; i < grenadeLauncher.ReloadSettings.bulletInMag.Length; i++)
        {
            grenadeLauncher.ReloadSettings.bulletInMag[i].GetComponent
                <MeshRenderer>().enabled = true;
        }
    }

    //Refill ammo
    void RefillAmmo()
    {
        grenadeLauncher.currentAmmo = grenadeLauncher.ShootSettings.ammo;
        playerUI.SetupWeapon(grenadeLauncher);
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
        else
        {
            isReloading = false;
        }
    }
}
