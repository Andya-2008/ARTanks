using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GameFuseCSharp;
using TMPro;
using Unity.Services.Lobbies;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton clientPrefab;
    [SerializeField] private HostSingleton hostPrefab;
    [SerializeField] private GameObject SignInPanel;
    [SerializeField] private GameObject SignUpPanel;
    [SerializeField] private GameObject TextLoading;

    [SerializeField] private string gameID;
    [SerializeField] private string gameToken;
    [SerializeField] TMP_InputField signUpUsername;
    [SerializeField] TMP_InputField signUpEmail;
    [SerializeField] TMP_InputField signUpPassword;
    [SerializeField] TMP_InputField signUpCPassword;
    [SerializeField] TextMeshProUGUI signUpErrorText;

    [SerializeField] TMP_InputField signInEmail;
    [SerializeField] TMP_InputField signInPassword;
    [SerializeField] TextMeshProUGUI signInErrorText;


    private bool isConnected = false;
    public bool isAuthenticated = false;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        GameFuse.SetVerboseLogging(true);
        GameFuse.SetUpGame(gameID, gameToken, this.GameSetUpCallback);
    }
    /*
    private async void Awake()
    {
        if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
        {
            Debug.Log("Email:Password:" + PlayerPrefs.GetString("email") + PlayerPrefs.GetString("password"));
        }
        else {
            GameFuse.SetVerboseLogging(true);
            GameFuse.SetUpGame(gameID, gameToken, this.GameSetUpCallback);
        }
    }
    */
    async void GameSetUpCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse setup success");
            isConnected = true;

            if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
            {
                string email = Crypto.DecryptString(PlayerPrefs.GetString("email"));
                string password = Crypto.DecryptString(PlayerPrefs.GetString("password"));
                Debug.Log(email + ":" + password);
                if (email.Trim() != "" && password.Trim() != "")
                {
                    GameFuse.SignIn(email, password, this.SignInCallback);
                }
                else {
                    TextLoading.SetActive(false);
                    SignUpPanel.SetActive(true);
                }
            }
            else {
                TextLoading.SetActive(false);
                SignUpPanel.SetActive(true);
            }

        }
        else
        {
            Debug.Log("GameFuse setup failed: " + message);
        }
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            Debug.Log("ApplicationController: Instantiate Host");
            HostSingleton hostSingleton = Instantiate(hostPrefab);
            DontDestroyOnLoad(hostSingleton);
            Debug.Log("ApplicationController: Create Host:" + hostSingleton.name);
            hostSingleton.CreateHost();

            Debug.Log("ApplicationController: Instantiate Client");
            ClientSingleton clientSingleton = Instantiate(clientPrefab);
            Debug.Log("ApplicationController: Create Client");
            
            bool unityAuthenticated = await clientSingleton.CreateClient(GameFuseUser.CurrentUser.GetUsername());
            Debug.Log("ApplicationController:  Authenticated:" + isAuthenticated);
            

            if (isAuthenticated && unityAuthenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
            
        }
    }

    public void showSignIn() {
        SignUpPanel.SetActive(false);
        SignInPanel.SetActive(true);
    }

    public void showSignUp() {
        SignUpPanel.SetActive(true);
        SignInPanel.SetActive(false);

    }


    // Function to show Sign In Screen
    public void SignInUser()
    {
        string email = signInEmail.text;
        string password = signInPassword.text;

        if (email == "" || password == "")
        {
            return;
        }

        if (isConnected)
        {
            GameFuse.SignIn(email, password, this.SignInCallback);
        }
        else
        {
            signInErrorText.text = "Not connected with Server! Please try again later.";
        }
    }

    // Callback function for sign in
    public async void SignInCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log("GameFuse sign in success");
            isAuthenticated = true;
            saveUserInfo(signInEmail.text, signInPassword.text);
            await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);

        }
        else
        {
            Debug.Log("GameFuse sign in failed: " + message);
            signInErrorText.text = message;
        }
    }




    public void SignUpUser()
    {
        string username = signUpUsername.text;
        string email = signUpEmail.text;
        string password = signUpPassword.text;
        string cpassword = signUpCPassword.text;

        if (username == "" || email == "" || password == "" || cpassword == "")
        {
            return;
        }

        if (password != cpassword)
        {
            signUpErrorText.text = "Password and Confirm Password does not match!";
            return;
        }

        if (isConnected)
        {
            GameFuse.SignUp(email, password, cpassword, username, this.SignUpCallback);
        }
        else
        {
            signUpErrorText.text = "Not connected with Server! Please try again later.";
        }
    }

    // Callback function for sign up
    public async void SignUpCallback(string message, bool hasError)
    {
        if (!hasError)
        {
            Debug.Log(message);
            Debug.Log("GameFuse sign up success");
            isAuthenticated = true;
            saveUserInfo(signUpEmail.text, signUpPassword.text);

            await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);

        }
        else
        {
            Debug.Log("GameFuse sign up failed: " + message);
            signUpErrorText.text = message;
        }
    }

    public async void ForgotPassword() {
        if (signInEmail.text != null && signInEmail.text != "")
        {
            GameFuse.Instance.SendPasswordResetEmail(signInEmail.text, ForgotPasswordEmailSent);
        }
        else {
            signInErrorText.text = "You must first fill out the email field.";
        }
    }
    private void ForgotPasswordEmailSent(string message, bool hasError)
    {
        if (hasError)
        {
            signInErrorText.text = message;
        }
        else
        {
            signInErrorText.text = "Password Reset email has been sent.  Check your junk folder.";
        }
    }
    private void saveUserInfo(string email, string password) {
        PlayerPrefs.SetString("username", Crypto.EncryptString(GameFuseUser.CurrentUser.GetUsername()));
        PlayerPrefs.SetInt("userid", GameFuseUser.CurrentUser.GetID());
        PlayerPrefs.SetString("email", Crypto.EncryptString(email));
        PlayerPrefs.SetString("password", Crypto.EncryptString(password));
    }

    public IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
    {
        //var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            LobbyService.Instance.SendHeartbeatPingAsync(lobbyId);
            Debug.Log("Send Heart Beat:" + lobbyId + ":" + Time.time);
            yield return new WaitForSeconds(waitTimeSeconds);
        }
    }
}
