using UnityEngine;

public class Grenade : Weapon
{
    //The projectile spawned when shooting
    public Transform projectile;
    //The static projectile on the weapon
    //This will be hidden when shooting
    public Transform currentProjectile;

    //Delay when releasing left click to throw
    public float throwDelay = 0.15f;
    //Delay to hide grenade model
    public float hideGrenadeTimer = 0.75f;
    //Delay to show grenade model
    public float showGrenadeTimer = 0.75f;
    public Transform grenadeSpawnPoint;
}