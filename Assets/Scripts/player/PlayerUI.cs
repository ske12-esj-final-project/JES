using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    public Text inventoryAmmoText;
    private float delay = 4.0f; //This implies a delay of 2 seconds.
    public bool isMinimapOpen = false;
    public GameObject arrowRight;
    public GameObject arrowLeft;
    public Dictionary<float, GameObject> indicators = new Dictionary<float, GameObject>();
    private GameObject newArrowLeft;
    private GameObject newArrowRight;
    public Text warnSafeAreaTime;
    void Start()
    {
        countdownText.text = "";
        secondText.text = "";
        inventoryAmmoText.text = "0";
        safeAreaOverlay.SetActive(false);
        
        if (GameObject.Find("Minimap Camera") != null)
        {
            minimapCamera = GameObject.Find("Minimap Camera").GetComponent<Camera>();
        }
    }

    private void Update()
    {
        if (CrossPlatformInputManager.GetButtonDown("Map"))
        {
            OpenMinimap();
        }
        float currentPlayerAngle = transform.localEulerAngles.y;
        bool isIndicatorOutLeft = false;
        bool isIndicatorOutRight = false;
        foreach (KeyValuePair<float, GameObject> attachStat in indicators)
        {
            //Now you can access the key and value both separately from this attachStat as:
            float indicatorPosition = CalculateIndicatorPosition(currentPlayerAngle, attachStat.Key);
            attachStat.Value.GetComponent<RectTransform>().anchoredPosition = new Vector3(indicatorPosition, 0, 0);
            if (indicatorPosition < -200)
            {
                isIndicatorOutLeft = true;
            }
            if (indicatorPosition > 200)
            {
                isIndicatorOutRight = true;
            }
        }
        // Debug.Log("Indicator Pos: " + isIndicatorOutLeft + " " + isIndicatorOutRight);
        CreateArrow(isIndicatorOutLeft, isIndicatorOutRight);
    }
    void CreateArrow(bool isIndicatorOutLeft, bool isIndicatorOutRight)
    {
        if (isIndicatorOutLeft && newArrowLeft == null)
        {
            newArrowLeft = Instantiate(arrowLeft, compassBackground.transform.position, Quaternion.identity);
            newArrowLeft.transform.SetParent(compassBackground.transform);
            newArrowLeft.GetComponent<RectTransform>().anchoredPosition = new Vector3(-190, 0, 0);
        }
        else if (!isIndicatorOutLeft)
        {
            Destroy(newArrowLeft);
        }
        if (isIndicatorOutRight && newArrowRight == null)
        {
            newArrowRight = Instantiate(arrowRight, compassBackground.transform.position, Quaternion.identity);
            newArrowRight.transform.SetParent(compassBackground.transform);
            newArrowRight.GetComponent<RectTransform>().anchoredPosition = new Vector3(190, 0, 0);
        }
        else if (!isIndicatorOutRight)
        {
            Destroy(newArrowRight);
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

    public void DeleteWeapon(Weapon weapon)
    {
        weaponListUI.GetComponent<WeaponUI>().Delete(weapon);
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

    public void DisableCountdownText()
    {
        countdownText.text = "";
        secondText.text = "";
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
        if (!isMinimapOpen)
        {
            minimap.SetActive(false);
            bigMinimap.SetActive(true);
            isMinimapOpen = true;
        }
        else
        {
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
        nameWinScreen.text = json[0].ToString().Replace("\"", "");
        killWinScreen.text = "Kill " + json[1].ToString();
        scoreWinScreen.text = "Reward " + json[2].ToString();
        GameManager.SetScore(GameManager.GetScore() + int.Parse(json[2].ToString()));
        winScreen.SetActive(true);
    }

    public void ShowDamageIndicator(float playerDegree, float enemyDegree)
    {
        float indicatorPosition = CalculateIndicatorPosition(playerDegree, enemyDegree);
        var damageIndicator = Instantiate(damageIndicatorImage, compassBackground.transform.position, Quaternion.identity);
        damageIndicator.transform.SetParent(compassBackground.transform);
        damageIndicator.GetComponent<RectTransform>().anchoredPosition = new Vector3(indicatorPosition, 0, 0);
        StartCoroutine(SetIndicatorOnPlayer(enemyDegree, damageIndicator));
    }
    float CalculateIndicatorPosition(float playerDegree, float enemyDegree)
    {
        float indicatorPosition = 0;
        float newEnemyDegree = enemyDegree - 180;
        if (newEnemyDegree < 0) newEnemyDegree += 360;
        float degreeDiff = newEnemyDegree - playerDegree;
        float degreeUnit = degreeDiff / 360;
        if (degreeUnit > 1)
        {
            degreeUnit -= 1;
        }
        else if (degreeUnit < -1)
        {
            degreeUnit += 1;
        }
        if (degreeUnit > 0)
        {
            if (degreeUnit < 0.5)
            {
                indicatorPosition = degreeUnit * 1000;
            }
            else
            {
                indicatorPosition = (1.0f - degreeUnit) * 1000;
            }
        }
        else if (degreeUnit < 0)
        {
            if (degreeUnit > -0.5)
            {
                indicatorPosition = degreeUnit * 1000;
            }
            else
            {
                indicatorPosition = (1.0f - degreeUnit) * 1000;
            }
        }
        return indicatorPosition;
    }
    IEnumerator SetIndicatorOnPlayer(float enemyDegree, GameObject damageIndicator)
    {
        indicators.Add(enemyDegree, damageIndicator);
        yield return new WaitForSeconds(15);
        indicators.Remove(enemyDegree);
        Destroy(damageIndicator);
    }
    public void SetPlayerKill(string killNum)
    {
        currentKillText.text = killNum + " Kill";
    }
    public void SetWarnSafeAreaTime(string time)
    {
        warnSafeAreaTime.text = time + " s";
    }
    public void SetInventoryAmmoText(int _ammo)
    {
        inventoryAmmoText.text = _ammo.ToString();
    }
}