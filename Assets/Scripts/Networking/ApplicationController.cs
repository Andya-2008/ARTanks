using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies;
using PlayFab;
using PlayFab.ClientModels;


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

    private string titleId = "3480A";
    private bool isConnected = false;
    public bool isAuthenticated = false;

    public UserAccountInfo currentUser;

    private Coroutine hbl;


    private async void Start()
    {
        DontDestroyOnLoad(gameObject);
        if (PlayerPrefs.HasKey("email") && PlayerPrefs.HasKey("password"))
        {
            string email = Crypto.DecryptString(PlayerPrefs.GetString("email"));
            string password = Crypto.DecryptString(PlayerPrefs.GetString("password"));
            Debug.Log(email + ":" + password);
            if (email.Trim() != "" && password.Trim() != "")
            {

                var request = new LoginWithEmailAddressRequest
                {

                    Email = email.Trim(),
                    Password = password.Trim(),
                    TitleId = titleId
                };
                PlayFabClientAPI.LoginWithEmailAddress(request, this.pfSignInCallback, this.pfSignInErrorCallback);
            }
            else
            {
                TextLoading.SetActive(false);
                SignUpPanel.SetActive(true);
            }
        }
        else
        {
            TextLoading.SetActive(false);
            SignUpPanel.SetActive(true);
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
            Debug.Log("ApplicationController: Create Client: " + currentUser.PlayFabId);
            bool unityAuthenticated = await clientSingleton.CreateClient(currentUser.PlayFabId);
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

        var request = new LoginWithEmailAddressRequest
        {

            Email = email.Trim(),
            Password = password.Trim()
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, this.pfSignInCallback, this.pfSignInErrorCallback);

    }

    void pfSignInErrorCallback(PlayFabError error)
    {
        Debug.Log("PlayFab sign in failed: " + error.ErrorMessage);
        signInErrorText.text = error.ErrorMessage;
    }

    // Callback function for sign in
    public async void pfSignInCallback(LoginResult result)
    {
        Debug.Log("PlayFab sign in success");
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), (result) => {
            
            currentUser = result.AccountInfo;
            Debug.Log("username=" + currentUser.Username);
            isAuthenticated = true;
            if (signInEmail.text != "" && signInPassword.text != "")
            {
                saveUserInfo(signInEmail.text, signInPassword.text);
            }
            LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }, error => {
            Debug.Log("Error retrieving Account Info");
        });

        

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
        var request = new RegisterPlayFabUserRequest

        {
            Email = email,
            Password = password,
            Username = username,
            DisplayName = username
        };
        Debug.Log("Playfab Sign Up:" + email + ":" + username + ":" + password);
        PlayFabClientAPI.RegisterPlayFabUser(request, this.pfSignUpCallback, this.pfSignUpErrorCallback);

    }


    // Callback function for sign up
    public async void pfSignUpCallback(RegisterPlayFabUserResult result)
    {
        Debug.Log("register success: " + result.PlayFabId);
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest(), (result) => {
            
            currentUser = result.AccountInfo;
            Debug.Log("username=" + currentUser.Username);
            saveUserInfo(signUpEmail.text, signUpPassword.text);
            LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }, error => {
            Debug.Log("Error retrieving Account Info");
        });


        isAuthenticated = true;
        

        

    }

    // Callback function for sign up
    public async void pfSignUpErrorCallback(PlayFabError error)
    {

            Debug.Log("GameFuse sign up failed: " + error.ErrorMessage);
            signUpErrorText.text = error.ErrorMessage;

    }


    public async void ForgotPassword()
    {
        if (signInEmail.text != null && signInEmail.text != "")
        {
            var request = new SendAccountRecoveryEmailRequest
            {
                Email = signInEmail.text,
                TitleId = titleId
            };
            PlayFabClientAPI.SendAccountRecoveryEmail(request, ForgotPasswordEmailSentCallback, ForgotPasswordEmailErrorCallback);
        }
        else
        {
            signInErrorText.text = "You must first fill out the email field.";
        }
    }
    private void ForgotPasswordEmailSentCallback(SendAccountRecoveryEmailResult result)
    {

        signInErrorText.text = "Password Reset email has been sent.  Check your junk folder.";

    }

    private void ForgotPasswordEmailErrorCallback(PlayFabError error)
    {

            signInErrorText.text = error.ErrorMessage;

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
        PlayerPrefs.SetString("username", Crypto.EncryptString(currentUser.Username));
        PlayerPrefs.SetString("userid", currentUser.PlayFabId);
        PlayerPrefs.SetString("email", Crypto.EncryptString(email));
        PlayerPrefs.SetString("password", Crypto.EncryptString(password));
    }


    public void startHeartBeat(string lobbyId) {
        hbl = StartCoroutine(HeartbeatLobbyCoroutine(lobbyId, 15.0f));
    }
    public void stopLobbyHeartBeat()
    {
        StopCoroutine(hbl);
        hbl = null;
    }

    IEnumerator HeartbeatLobbyCoroutine(string lobbyId, float waitTimeSeconds)
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
