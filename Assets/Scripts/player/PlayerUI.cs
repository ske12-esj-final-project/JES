using UnityEngine;
using UnityEngine.UI;

using UnityStandardAssets.CrossPlatformInput;

public class PlayerUI : MonoBehaviour
{
    public Image healthBar;
    public Text status;
    public GameObject weaponListUI;
    public Text countdownText;
    public Text secondText;
    public GameObject safeAreaOverlay;
    public GameObject crosshair;
    public Text currentAmountPlayerAlive;
    public GameObject minimap;
    public Camera minimapCamera;
    public GameObject winScreen;
    public Text killWinScreen;
    public Text nameWinScreen;

    void Start()
    {
        countdownText.text = "";
        secondText.text = "";
        safeAreaOverlay.SetActive(false);
    }
    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Map"))
        {
            Debug.Log("Open Map");
            OpenMinimap();
        }
    }
    public void SetPlayerStatus(string text)
    {
        status.text = text;
    }

    public void SetupWeapon(Weapon weapon)
    {
        weaponListUI.GetComponent<WeaponUI>().Setup(weapon);
    }

    public void SetPlayerHealth(float percentage)
    {
        healthBar.fillAmount = percentage;
    }

    public void SetCountdownText()
    {
        countdownText.text = "Match will start in";
    }

    public void SetSecondText(int second)
    {
        secondText.text = second.ToString();
    }

    public void ShowSafeAreaOverlay()
    {
        safeAreaOverlay.SetActive(true);
    }

    public void HideSafeAreaOverlay()
    {
        safeAreaOverlay.SetActive(false);
    }
    
    public void SetCurrentPlayerAlive(string number)
    {
        currentAmountPlayerAlive.text = number + " Left";
    }

    public void OpenMinimap()
    {
        // minimap.transform.localScale += new Vector3(1.0F, 1.0F, 1.0F);
    }

    public void PlayCrosshair()
    {
        crosshair.GetComponent<Animator>().SetTrigger("Firing");
    }

    public void ShowWinScreen(JSONObject json)
    {
        nameWinScreen.text = json[0].ToString();
        killWinScreen.text = "Kill " + json[1].ToString();
        winScreen.SetActive(true);
    }
}