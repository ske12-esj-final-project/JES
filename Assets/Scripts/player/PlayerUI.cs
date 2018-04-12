using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
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
    public GameObject bigMinimap;
    public Camera minimapCamera;
    public GameObject winScreen;
    public Text killWinScreen;
    public Text nameWinScreen;
    public Text scoreWinScreen;
    public GameObject compassBackground;
    public GameObject damageIndicatorImage;
    public Text currentKillText;
    private float delay = 4.0f; //This implies a delay of 2 seconds.
    public bool isMinimapOpen = false;

    void Start()
    {
        countdownText.text = "";
        secondText.text = "";
        safeAreaOverlay.SetActive(false);
        minimapCamera = GameObject.Find("Minimap Camera").GetComponent<Camera>();
    }
    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Map"))
        {
            OpenMinimap();
        }
    }
    public void SetPlayerStatus(string text)
    {
        status.text = text;
    }

    public void EnableWeapon(Weapon weapon)
    {
        weaponListUI.GetComponent<WeaponUI>().Enable(weapon);
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
        if (!isMinimapOpen) {
            minimap.SetActive(false);
            bigMinimap.SetActive(true);
            isMinimapOpen = true;
        } else {
            minimap.SetActive(true);
            bigMinimap.SetActive(false);
            isMinimapOpen = false;
        }
    }

    public void PlayCrosshair()
    {
        crosshair.GetComponent<Animator>().SetTrigger("Firing");
    }

    public void ShowWinScreen(JSONObject json)
    {
        nameWinScreen.text = json[0].ToString();
        killWinScreen.text = "Kill " + json[1].ToString();
        scoreWinScreen.text = "Reward " + json[2].ToString();
        winScreen.SetActive(true);
    }

    public void ShowDamageIndicator(float playerDegree, float enemyDegree)
    {
        float indicatorPosition = 0;
        float degreeDiff = playerDegree - (180 - enemyDegree);
        if (degreeDiff < 0) degreeDiff += 360;
        if(degreeDiff < 180) {
            indicatorPosition = (degreeDiff/180) * 1000;
        } else {
            indicatorPosition = ((360 - degreeDiff)/180) * - 1000;
        }
        Debug.Log("Player Degree: " + playerDegree);
        Debug.Log("Enemy Degree: " + enemyDegree);
        Debug.Log(indicatorPosition);
        var damageIndicator = Instantiate (damageIndicatorImage, compassBackground.transform.position , Quaternion.identity);
        damageIndicator.transform.parent = compassBackground.transform;
        damageIndicator.GetComponent<RectTransform> ().anchoredPosition = new Vector3(indicatorPosition, 0, 0);
    }
    public void SetPlayerKill(string killNum)
    {
        currentKillText.text = killNum + " Kill";
    }
}