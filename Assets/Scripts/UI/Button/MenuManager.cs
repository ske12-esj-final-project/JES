using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class MenuManager : MonoBehaviour
{
    public GameObject MainMenu;
    public GameObject LoginMenu;
    public GameObject RegisterMenu;
    public Text loginErrorText;
    public Text registerErrorText;
    public Button loginButton;
    public Button registerButton;
    public Button guestButton;
    public Button registerBackButton;
    public Button loginBackButton;
    public InputField usernameRegisterInput;
    public InputField passwordRegisterInput;
    public InputField emailRegisterInput;
    public Button submitRegisterButton;
    public InputField usernameLoginInput;
    public InputField passwordLoginInput;
    public Button submitLoginButton;
    public LoadingScreenControl loadingScreen;

    private Regex usernameRegex = new Regex("^[a-zA-Z0-9_.-]*$");
    private Regex passwordRegex = new Regex("^[a-zA-Z0-9_.-]*$");
    private Regex emailRegex = new Regex(@"^[-!#$%&'*+/0-9=?A-Z^_a-z{|}~](\.?[-!#$%&'*+/0-9=?A-Z^_a-z{|}~])*@[a-zA-Z](-?[a-zA-Z0-9])*(\.[a-zA-Z](-?[a-zA-Z0-9])*)+$");

    void Start()
    {
        loginButton.GetComponent<Button>().onClick.AddListener(OnClickLogin);
        registerButton.GetComponent<Button>().onClick.AddListener(OnClickRegister);
        guestButton.GetComponent<Button>().onClick.AddListener(OnClickGuest);
        submitRegisterButton.GetComponent<Button>().onClick.AddListener(OnClickSubmitRegister);
        submitLoginButton.GetComponent<Button>().onClick.AddListener(OnClickSubmitLogin);
        registerBackButton.GetComponent<Button>().onClick.AddListener(OnClickBack);
        loginBackButton.GetComponent<Button>().onClick.AddListener(OnClickBack);

        loginErrorText.text = "";
        registerErrorText.text = "";

        submitRegisterButton.interactable = false;

        usernameRegisterInput.onValueChanged.AddListener(delegate { ValidateRegisterForm(); });

        passwordRegisterInput.onValueChanged.AddListener(delegate { ValidateRegisterForm(); });

        emailRegisterInput.onValueChanged.AddListener(delegate { ValidateRegisterForm(); });
    }

    void OnClickLogin()
    {
        MainMenu.SetActive(false);
        LoginMenu.SetActive(true);
    }

    void OnClickRegister()
    {
        MainMenu.SetActive(false);
        RegisterMenu.SetActive(true);
    }

    void OnClickGuest()
    {

    }

    void OnClickSubmitRegister()
    {
        StartCoroutine("SubmitRegister");
    }

    void OnClickSubmitLogin()
    {
        StartCoroutine("SubmitLogin");
    }

    void OnClickBack()
    {
        RegisterMenu.SetActive(false);
        LoginMenu.SetActive(false);
        MainMenu.SetActive(true);
    }

    void ValidateRegisterForm()
    {
        if (IsUsernameValid() && IsPasswordValid() && IsEmailValid())
        {
            registerErrorText.text = "";
            submitRegisterButton.interactable = true;
        }
    }

    bool IsUsernameValid()
    {
        if (usernameRegisterInput.text == "") {
            registerErrorText.text = "Username is required";
            submitRegisterButton.interactable = false;
            return false;
        }

        if (!usernameRegex.IsMatch(usernameRegisterInput.text))
        {
            registerErrorText.text = "Invalid username format";
            submitRegisterButton.interactable = false;
            return false;
        }

        return true;
    }

    bool IsPasswordValid()
    {
        if (passwordRegisterInput.text == "") {
            registerErrorText.text = "Password is required";
            submitRegisterButton.interactable = false;
            return false;
        }

        if (!passwordRegex.IsMatch(passwordRegisterInput.text))
        {
            registerErrorText.text = "Invalid password format";
            submitRegisterButton.interactable = false;
            return false;
        }

        return true;
    }

    bool IsEmailValid()
    {
        if (emailRegisterInput.text == "") {
            registerErrorText.text = "Email is required";
            submitRegisterButton.interactable = false;
            return false;
        }

        if (!emailRegex.IsMatch(emailRegisterInput.text))
        {
            registerErrorText.text = "Invalid email format";
            submitRegisterButton.interactable = false;
            return false;
        }

        return true;
    }

    UnityWebRequest Post(string url, string data)
    {
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(data);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    IEnumerator SubmitRegister()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["username"] = usernameRegisterInput.text;
        data["password"] = passwordRegisterInput.text;
        data["email"] = emailRegisterInput.text;
        string jsonStr = new JSONObject(data).ToString();

        UnityWebRequest request = Post("http://jes.api.user.safesuk.me/v1/users/register", jsonStr);

        yield return request.SendWebRequest();

        StoreTokenAndGoMainScreen(request);
    }

    IEnumerator SubmitLogin()
    {
        Dictionary<string, string> data = new Dictionary<string, string>();

        data["username"] = usernameLoginInput.text;
        data["password"] = passwordLoginInput.text;
        string jsonStr = new JSONObject(data).ToString();

        UnityWebRequest request = Post("http://jes.api.user.safesuk.me/v1/users/login", jsonStr);

        yield return request.SendWebRequest();

        StoreTokenAndGoMainScreen(request);

    }

    void StoreTokenAndGoMainScreen(UnityWebRequest request)
    {
        if (request.isNetworkError || request.isHttpError)
        {
            Debug.LogError(request.error);
        }

        else
        {
            string res = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
            JSONObject response = new JSONObject(res);
            PlayerPrefs.SetString("token", response[1].ToString().Replace("\"", ""));
            loadingScreen.LoadScene();
        }
    }
}
