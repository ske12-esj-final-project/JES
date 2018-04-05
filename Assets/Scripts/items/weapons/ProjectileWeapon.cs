using UnityEngine;

public class ProjectileWeapon : Weapon
{
    public class shootSettings
    {
        public int ammo;
    }

    public shootSettings ShootSettings;

    public Transform projectile;
    public Transform currentProjectile;
    public Transform bulletSpawnPoint;
    public float reloadTime;
}