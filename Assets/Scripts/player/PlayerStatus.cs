using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class PlayerStatus : MonoBehaviour
{
    public string playerID;
    public float health;
    private PlayerUI playerUI;
    private GameObject player;
    private VignetteAndChromaticAberration vignette;
    public float playerMaxHealth;
	private bool isPlayingEffect;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerUI = player.GetComponent<PlayerUI>();
        vignette = player.transform.GetChild(2).GetComponent<VignetteAndChromaticAberration>();
    }

    public void SetHealth(float newHealth)
    {
        health = newHealth;
        playerUI.SetPlayerHealth(health / playerMaxHealth);
        if (health > 0 && !isPlayingEffect) StartCoroutine("PlayEffectHit");
    }

    public void SetPlayerID(string newPlayerID)
    {
        playerID = newPlayerID;
    }

    IEnumerator PlayEffectHit()
    {
		isPlayingEffect = true;
        float duration = 2f;
        vignette.intensity = 0.5f * (health / 100);
        vignette.chromaticAberration = 10 * (health / 100);
		yield return new WaitForSeconds(duration);
        vignette.intensity = 0;
        vignette.chromaticAberration = 0;
		isPlayingEffect = false;
    }
}
