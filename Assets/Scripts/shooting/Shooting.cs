using System.Collections;
using UnityEngine;
using SocketIO;

public class Shooting : MonoBehaviour
{
    public SocketIOComponent socket;
    public Animator anim;
    public GameObject player;
    public Inventory inventory;
    public Prefabs prefabs;
    public Impact Impact;
    private ImpactTags impactTags;
    public PlayerUI playerUI;

    void Start()
    {
        //Set the animator component
        anim = GetComponent<Animator>();

        GameObject go = GameObject.Find("SocketIO");
        socket = go.GetComponent<SocketIOComponent>();

        GameObject global = GameObject.Find("Global");
        prefabs = global.GetComponent<Prefabs>();
        Impact = global.GetComponent<Impact>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerUI = player.GetComponent<PlayerUI>();

        inventory = player.GetComponent<Inventory>();
    }

    public IEnumerator MuzzleFlash(Weapon weapon)
    {
        //Show muzzleflash if useMuzzleFlash is true
        if (weapon.Effects.useMuzzleflash == true)
        {
            //Show a random muzzleflash from the array
            weapon.Effects.sideMuzzle.GetComponent<SpriteRenderer>().sprite = weapon.Effects.muzzleflashSideSprites
                [Random.Range(0, weapon.Effects.muzzleflashSideSprites.Length)];
            weapon.Effects.topMuzzle.GetComponent<SpriteRenderer>().sprite = weapon.Effects.muzzleflashSideSprites
                [Random.Range(0, weapon.Effects.muzzleflashSideSprites.Length)];

            //Show the muzzleflashes
            weapon.Effects.sideMuzzle.GetComponent<SpriteRenderer>().enabled = true;
            weapon.Effects.topMuzzle.GetComponent<SpriteRenderer>().enabled = true;
            weapon.Effects.frontMuzzle.GetComponent<SpriteRenderer>().enabled = true;
        }

        //Enable the light flash if true
        if (weapon.Effects.useLightFlash == true)
        {
            weapon.Effects.lightFlash.GetComponent<Light>().enabled = true;
        }

        //Play smoke particles if true
        if (weapon.Effects.playSmoke == true)
        {
            weapon.Effects.smokeParticles.Play();
        }
        //Play spark particles if true
        if (weapon.Effects.playSparks == true)
        {
            weapon.Effects.sparkParticles.Play();
        }
        //Play bullet tracer particles if true
        if (weapon.Effects.playTracers == true)
        {
            weapon.Effects.bulletTracerParticles.Play();
        }

        //Show the muzzleflash for 0.02 seconds
        yield return new WaitForSeconds(0.02f);

        if (weapon.Effects.useMuzzleflash == true)
        {
            //Hide the muzzleflashes
            weapon.Effects.sideMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            weapon.Effects.topMuzzle.GetComponent<SpriteRenderer>().enabled = false;
            weapon.Effects.frontMuzzle.GetComponent<SpriteRenderer>().enabled = false;
        }

        //Disable the light flash if true
        if (weapon.Effects.useLightFlash == true)
        {
            weapon.Effects.lightFlash.GetComponent<Light>().enabled = false;
        }
    }

    IEnumerator CasingDelay(Weapon weapon)
    {
        Gun gun = (Gun)weapon;
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
}
