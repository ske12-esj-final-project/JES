using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.CrossPlatformInput;

public class GunShooting : Shooting
{
    private Gun gun;

    bool isReloading;
    bool outOfAmmo;
    bool isShooting;
    bool isAimShooting;
    bool isAiming;
    bool isDrawing;
    bool isRunning;
    bool isJumping;

    float recoil;
    float maxRecoilX = 20f;
    float maxRecoilY;
    float lastFired;

    void Awake()
    {
        gun = (Gun)GetComponents<Weapon>()[0];

        gun.Effects.sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
        gun.Effects.topMuzzle.GetComponent<SpriteRenderer>().enabled = false;
        gun.Effects.frontMuzzle.GetComponent<SpriteRenderer>().enabled = false;

        gun.Effects.lightFlash.GetComponent<Light>().enabled = false;
    }

    void Update()
    {
        AnimationCheck();
        //Left click (if automatic fire is false) Input.GetButton("Fire1")
        if (CrossPlatformInputManager.GetButton("Fire1") && isDisableShooting())
        {
            playerUI.PlayCrosshair();
            PerformShoot(gun);
        }

        MakeRecoil();

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
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && inventory.currentAmmo != 0)
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
        if (gun.currentAmmo == 0)
        {
            outOfAmmo = true;
            //if ammo is higher than 0
        }
        else if (gun.currentAmmo > 0)
        {
            outOfAmmo = false;
        }
    }

    bool isDisableShooting()
    {
        return !isReloading && !outOfAmmo && !isShooting && !isAimShooting && !isRunning && !isJumping;
    }

    void PerformShoot(Gun gun)
    {
        EmitShoot();
        if (gun.ShootSettings.isAutomaticFire == true)
        {
            //Shoot automatic
            //Left click hold (if automatic fire is true)
            if (Time.time - lastFired > 1 / gun.ShootSettings.fireRate)
            {
                Shoot(gun);
                lastFired = Time.time;
            }
        }
        else
        {
            Shoot(gun);
        }
    }

    public void Shoot(Gun gun)
    {
        //Play shoot animation
        if (!anim.GetBool("isAiming"))
        {
            anim.Play("Fire");
        }
        else
        {
            anim.SetTrigger("Shoot");
        }

        //Remove 1 bullet
        gun.currentAmmo -= 1;
        playerUI.SetupWeapon(gun);

        recoil += 0.01f;

        //Play shoot sound
        gun.Audios.mainAudioSource.clip = gun.Audios.shootSound;
        gun.Audios.mainAudioSource.Play();

        //Start casing instantiate
        if (!gun.ReloadSettings.casingOnReload)
        {
            StartCoroutine(CasingDelay());
        }

        //Show the muzzleflash
        StartCoroutine(MuzzleFlash(gun));

        if (gun is Shotgun) LaunchShotgunShots(); else LaunchNormalShot();
    }

    void MakeRecoil()
    {
        float recoilSpeed = gun.ShootSettings.recoilSpeed;
        if (recoil > 0.00f)
        {
            maxRecoilY = Random.Range(-maxRecoilX, maxRecoilX);
            Quaternion maxRecoil = Quaternion.Euler(-maxRecoilX, maxRecoilY, 0.0f);
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            recoil -= 0.005f;
        }
        else
        {
            recoil = 0.00f;
            transform.localRotation = Quaternion.Slerp
            (transform.localRotation, Quaternion.identity, 0);
        }
    }

    void LaunchNormalShot()
    {
        //Raycast bullet
        RaycastHit hit;

        //Send out the raycast from the "bulletSpawnPoint" position
        if (Physics.Raycast(gun.Spawnpoints.bulletSpawnPoint.transform.position,
        gun.Spawnpoints.bulletSpawnPoint.transform.forward, out hit,
        gun.ShootSettings.bulletDistance))
        {
            // If a rigibody is hit, add bullet force to it
            if (hit.rigidbody != null)
            {
                if (hit.transform.tag == "Enemy")
                {
                    Impact.impactStrategies["Enemy (Static)"].makeImpact(prefabs, hit);
                    EmitEnemyHit(hit.transform.name.ToString(), gun.Damage);
                }

                else
                {
                    Impact.impactStrategies[hit.transform.tag].makeImpact(prefabs, hit);
                }
            }
        }
    }

    void LaunchShotgunShots()
    {
        Shotgun shotgun = (Shotgun)gun;
        //Send out shotgun raycast with set amount of pellets
        for (int i = 0; i < shotgun.pellets; ++i)
        {

            float randomRadius = Random.Range
                (0, shotgun.spreadSize);
            float randomAngle = Random.Range
                (0, 2 * Mathf.PI);

            //Raycast direction
            Vector3 direction = new Vector3(
                randomRadius * Mathf.Cos(randomAngle),
                randomRadius * Mathf.Sin(randomAngle),
                15);

            direction = transform.TransformDirection(direction.normalized);

            RaycastHit hit;
            if (Physics.Raycast(shotgun.Spawnpoints.bulletSpawnPoint.transform.position, direction,
                                 out hit, shotgun.ShootSettings.bulletDistance))
            {

                // If a rigibody is hit, add bullet force to it
                if (hit.rigidbody != null)
                {
                    Impact.impactStrategies[hit.transform.tag].makeImpact(prefabs, hit);
                }
            }
        }
    }

    IEnumerator CasingDelay()
    {
        //Wait set amount of time for casing to spawn
        yield return new WaitForSeconds(gun.ReloadSettings.casingDelay);
        //Spawn a casing at every casing spawnpoint
        for (int i = 0; i < gun.Spawnpoints.casingSpawnPoints.Length; i++)
        {
            Object.Instantiate(gun.casingPrefab,
                         gun.Spawnpoints.casingSpawnPoints[i].transform.position,
                         gun.Spawnpoints.casingSpawnPoints[i].transform.rotation);
        }
    }
    IEnumerator BulletInMagTimer()
    {
        //Wait for set amount of time 
        yield return new WaitForSeconds
            (gun.ReloadSettings.enableBulletTimer);

        //Show the bullet inside the mag
        for (int i = 0; i < gun.ReloadSettings.bulletInMag.Length; i++)
        {
            gun.ReloadSettings.bulletInMag[i].GetComponent
                <MeshRenderer>().enabled = true;
        }
    }

    //Reload
    void Reload()
    {
        //Play reload animation
        anim.Play("Reload");

        //Play reload sound
        gun.Audios.mainAudioSource.clip = gun.Audios.reloadSound;
        gun.Audios.mainAudioSource.Play();

        //Spawn casing on reload, only used on some weapons
        if (gun.ReloadSettings.casingOnReload == true)
        {
            StartCoroutine(CasingDelay());
        }

        if (outOfAmmo == true && gun.ReloadSettings.hasBulletInMag == true)
        {
            //Hide the bullet inside the mag if ammo is 0
            for (int i = 0; i < gun.ReloadSettings.bulletInMag.Length; i++)
            {
                gun.ReloadSettings.bulletInMag[i].GetComponent
                    <MeshRenderer>().enabled = false;
            }
            //Start the "show bullet" timer
            StartCoroutine(BulletInMagTimer());
        }
    }

    //Refill ammo
    void RefillAmmo(int refillAmmo)
    {
        gun.currentAmmo = refillAmmo;
        playerUI.SetupWeapon(gun);
    }

    int FindRefillAmmo()
    {
        if (inventory.currentAmmo >= gun.ShootSettings.ammo)
        {
            return gun.ShootSettings.ammo;
        }

        else
        {
            return inventory.currentAmmo;
        }
    }

    void AnimationCheck()
    {
        isShooting = anim.GetCurrentAnimatorStateInfo(0).IsName("Fire");
        isAimShooting = anim.GetCurrentAnimatorStateInfo(0).IsName("Aim Fire");
        isRunning = anim.GetCurrentAnimatorStateInfo(0).IsName("Run");
        isJumping = anim.GetCurrentAnimatorStateInfo(0).IsName("Jump");
        isDrawing = anim.GetCurrentAnimatorStateInfo(0).IsName("Draw");

        //Check if finished reloading when using "insert" style reload
        //Used for bolt action sniper and pump shotgun for example
        if (gun.ReloadSettings.usesInsert == true &&
            anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            isReloading = false;
        }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Draw"))
        {
            isReloading = false;
        }

        //Check if reloading
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Reload"))
        {
            // If reloading
            isReloading = true;
        }
        else
        {
            //If not using "insert" style reload
            if (!gun.ReloadSettings.usesInsert)
            {
                if (isReloading == true && inventory.currentAmmo >= 0)
                {
                    inventory.currentAmmo += gun.currentAmmo;
                    int refillAmmo = FindRefillAmmo();
                    inventory.currentAmmo -= refillAmmo;
                    RefillAmmo(refillAmmo);
                }

                isReloading = false;
            }
        }
    }
    void EmitShoot()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        data["d"] = string.Format("");
        socket.Emit("9", new JSONObject(data));
    }
    void EmitEnemyHit(string playerId, float damage)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        string player = playerId.Replace("\"", "");
        string s = string.Format("[@{0}@, {1}]", player, damage);
        data["d"] = s;
        socket.Emit("0", new JSONObject(data));
    }
}
