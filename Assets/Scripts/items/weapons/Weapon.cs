using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    public string Name;
    public int Damage;

    public int currentAmmo;
    public Sprite weaponImage;

    [System.Serializable]
    public class effects
    {
        [Header("Muzzleflash Holders")]
        public bool useMuzzleflash = false;
        public GameObject sideMuzzle;
        public GameObject topMuzzle;
        public GameObject frontMuzzle;
        //Array of muzzleflash sprites
        public Sprite[] muzzleflashSideSprites;

        [Header("Light Front")]
        public bool useLightFlash = false;
        public Light lightFlash;

        [Header("Particle System")]
        public bool playSmoke = false;
        public ParticleSystem smokeParticles;
        public bool playSparks = false;
        public ParticleSystem sparkParticles;
        public bool playTracers = false;
        public ParticleSystem bulletTracerParticles;

        [Header("Melee Components")]
        public GameObject weaponTrail;
    }
    public effects Effects;

    [System.Serializable]
    public class audios
    {
        public AudioSource mainAudioSource;
        public AudioClip shootSound;
        public AudioClip reloadSound;
    }

    public audios Audios;
}