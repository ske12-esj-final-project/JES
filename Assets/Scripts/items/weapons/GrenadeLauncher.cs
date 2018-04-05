using UnityEngine;

public class GrenadeLauncher : Weapon
{
    [System.Serializable]
    public class shootSettings
    {
        [Header("Shooting Settings")]
        public int ammo;
    }

    public shootSettings ShootSettings;

    [System.Serializable]
    public class reloadSettings
    {
        [Header("Reload Settings")]
        public bool casingOnReload;
        public float casingDelay;

        [Header("Bullet In Mag")]
        public bool hasBulletInMag;
        public Transform[] bulletInMag;
        public float enableBulletTimer = 1.0f;

        [Header("Bullet Or Shell Insert")]
        //If the weapon uses a bullet/shell insert style reload
        //Used for the bolt action sniper and pump shotgun for example
        public bool usesInsert;

    }
    public reloadSettings ReloadSettings;

    [System.Serializable]
    public class spawnpoints
    {
        [Header("Spawnpoints")]
        //Array holding casing spawn points 
        //(some weapons use more than one casing spawn)
        public Transform[] casingSpawnPoints;
        //Bullet raycast start point
        public Transform bulletSpawnPoint;
    }
    public spawnpoints Spawnpoints;

    public Transform casingPrefab;

    public Transform projectile;
}