using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using PlayFab;
using PlayFab.ClientModels;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Dropdown joinCodeDropDown;
    [SerializeField] private TMP_Text txtUsername;
    [SerializeField] private TMP_Text txtError;
    

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject friendsPanel;

    
    private float lastCheck = 0;

    private List<FriendInfo> friendlist = null;


    public void Start()
	{
        //txtUsername.text = Crypto.DecryptString(PlayerPrefs.GetString("username"));
        txtUsername.text = GameObject.Find("ApplicationController").GetComponent<ApplicationController>().currentUser.Username;
        StartCoroutine(UpdateLobbyCoroutine(5.0f));
    }
	public async void StartHost()
    {
        string lobbyId = await HostSingleton.Instance.GameManager.StartHostAsync();
        GameObject appcontroller = GameObject.Find("ApplicationController");
        appcontroller.GetComponent<ApplicationController>().startHeartBeat(lobbyId);

        
        //StartCoroutine(HeartbeatLobbyCoroutine(lobbyId, 15));
    }



    public async void StartClient()
    {
        string joincode;
        if (joinCodeField.text != "")
        {
            joincode = joinCodeField.text;
        }
        else {
            joincode = await ClientSingleton.Instance.GameManager.FindLobbyJoinCode(joinCodeDropDown.options[joinCodeDropDown.value].text);
            //joincode = joinCodeDropDown.options[joinCodeDropDown.value].text.Split(":")[1];
        }

        await ClientSingleton.Instance.GameManager.StartClientAsync(joincode);
    }

	private void Update()
	{

	}

    public void LogOut() {
        PlayerPrefs.DeleteKey("username");
        PlayerPrefs.DeleteKey("userid");
        PlayerPrefs.DeleteKey("email");
        PlayerPrefs.DeleteKey("password");
        GameObject.Destroy(GameObject.Find("NetworkManager"));
        GameObject.Destroy(GameObject.Find("ApplicationController"));
        SceneManager.LoadScene("NetBootstrap");
    }



	public async void UpdateLobbies() {
        Debug.Log("UpdateLobbies");
        try
        {
            PlayFabClientAPI.GetFriendsList(new GetFriendsListRequest
            {
            }, result => {
                friendlist = result.Friends;
                GetFriendLobbies(friendlist); // triggers your UI
            }, DisplayPlayFabError);
            //await UnityServices.InitializeAsync();
            
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    async void GetFriendLobbies(List<FriendInfo> friends)
    {
        joinCodeDropDown.options.Clear();
        QueryLobbiesOptions options = new QueryLobbiesOptions();
        options.Count = 25;

        foreach (FriendInfo fi in friends)
        {
            QueryFilter qf = new QueryFilter(
                    field: QueryFilter.FieldOptions.Name,
                    op: QueryFilter.OpOptions.EQ,
                    value: fi.Username
                    );
            List<QueryFilter> queryFilters = new List<QueryFilter>();
            queryFilters.Add(qf);


            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Name)
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
            Debug.Log("Got Lobbies:" + lobbies.Results.Count);

            joinCodeDropDown.options.Clear();
            List<string> rooms = new List<string>();
            foreach (var l in lobbies.Results)
            {
                Debug.Log(l.Name + ":" + l.Data["relayJoinCode"].Value);
                if (l.AvailableSlots > 0)
                {
                    rooms.Add(l.Name);
                }


            }
            joinCodeDropDown.AddOptions(rooms);

        }

    }

    void DisplayPlayFabError(PlayFabError error)
    {
        txtError.text = error.ErrorMessage;
    }
    public void ManageFriends() {
        friendsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void ManageGame()
    {
        friendsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }

    public IEnumerator UpdateLobbyCoroutine(float waitTimeSeconds)
    {
        //var delay = new WaitForSecondsRealtime(waitTimeSeconds);

        while (true)
        {
            UpdateLobbies();
            yield return new WaitForSeconds(waitTimeSeconds);
        }
    }



}
