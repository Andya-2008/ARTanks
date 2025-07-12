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
using UnityEditor;
using System.IO;
using System.Linq;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Dropdown joinCodeDropDown;
    [SerializeField] private TMP_Text txtUsername;
    [SerializeField] private TMP_Text txtError;
    [SerializeField] private TMP_Text txtVersion;
    [SerializeField] private TMP_Text txtRating;

    [SerializeField] private TMP_Text txtLeaderboardError;
    [SerializeField] private GameObject leaderboardItem;
    [SerializeField] private Transform contentContainer;

    public List<PlayerLeaderboardEntry> leaderboardList = null;

    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject friendsPanel;
    [SerializeField] private GameObject leaderboardPanel;

    [SerializeField] private GameObject clientMan;

    private int buildNumber;
    private float lastCheck = 0;
    private int rating = 0;

    private List<FriendInfo> friendlist = null;


    public void Start()
	{
        string path = Path.Combine(Application.persistentDataPath, "build_number.txt");

        if (File.Exists(path))
        {
            string buildNumberStr = File.ReadAllText(path);
            buildNumber = int.Parse(buildNumberStr);
            Debug.Log($"Build number: {buildNumber}");
        }
        else
        {
            Debug.LogWarning("Build number file not found!");
            buildNumber = 0;
        }
        //txtUsername.text = Crypto.DecryptString(PlayerPrefs.GetString("username"));
        getRating();
        
        txtUsername.text = GameObject.Find("ApplicationController").GetComponent<ApplicationController>().currentUser.Username;
        StartCoroutine(UpdateLobbyCoroutine(10.0f));
        Debug.Log(Application.version);
        txtVersion.text = "Build:" + buildNumber.ToString();
    }
	public async void StartHost()
    {
        string lobbyId = await HostSingleton.Instance.GameManager.StartHostAsync();
        GameObject appcontroller = GameObject.Find("ApplicationController");
        appcontroller.GetComponent<ApplicationController>().startHeartBeat(lobbyId);
        Debug.Log("StartHost StartClient: Client Singleton Check");
        if (ClientSingleton.Instance == null)
        {
            Instantiate(clientMan);
            await ClientSingleton.Instance.CreateClient(txtUsername.text);
        }

        //StartCoroutine(HeartbeatLobbyCoroutine(lobbyId, 15));
    }



    public async void StartClient()
    {

        Debug.Log("Main Menu StartClient: Client Singleton Check");
        if (ClientSingleton.Instance == null)
        {
            Instantiate(clientMan);
            await ClientSingleton.Instance.CreateClient(txtUsername.text);
        }

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

    public void checkJoinCode() {
        if (joinCodeField.text == "")
        {
            joinCodeDropDown.gameObject.SetActive(true);
        }
        else {
            joinCodeDropDown.gameObject.SetActive(false);
        }
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
        /*
        Debug.Log("UpdateLobbies: Client Singleton Check");
        if (ClientSingleton.Instance == null) {
            Instantiate(clientMan);
            await ClientSingleton.Instance.CreateClient(txtUsername.text);
        }
        */

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
        Debug.Log("friends:" + friends.Count);

        // Order by newest lobbies first
        options.Order = new List<QueryOrder>()
        {
            new QueryOrder(
                asc: false,
                field: QueryOrder.FieldOptions.Name)
        };

        Debug.Log("Query Lobbies");
        QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
        Debug.Log("Got Lobbies:" + lobbies.Results.Count);

        joinCodeDropDown.options.Clear();
        List<string> rooms = new List<string>();
        foreach (var l in lobbies.Results)
        {
            Debug.Log(l.Name + ":" + l.Data["relayJoinCode"].Value);
            if (l.AvailableSlots > 0 && friends.Any(f => f.Username==l.Name))
            {
                rooms.Add(l.Name);
            }
        }
        joinCodeDropDown.AddOptions(rooms);



        /*
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
            Debug.Log("Query Lobbies");
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
            System.Threading.Thread.Sleep(1000);
        }
        */

    }

    void DisplayPlayFabError(PlayFabError error)
    {
        txtError.text = error.ErrorMessage;
    }
    public void ManageFriends() {
        friendsPanel.SetActive(true);
        mainPanel.SetActive(false);
    }

    public void GoToLeaderboard()
    {
        leaderboardPanel.SetActive(true);
        mainPanel.SetActive(false);
        GetLeaderboard();
    }


    public void ManageGame()
    {
        friendsPanel.SetActive(false);
        mainPanel.SetActive(true);
    }
    
    public void ManageGameFromLeaderboard() {
        leaderboardPanel.SetActive(false);
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

    private void getRating() {
        PlayFabClientAPI.GetPlayerStatistics(new GetPlayerStatisticsRequest(),
        result => {
            foreach (var stat in result.Statistics)
            {
                if (stat.StatisticName == "Rating")
                {
                    Debug.Log("High score: " + stat.Value);
                    rating = stat.Value;
                }

            }


            if (rating > 0)
            {
                txtRating.text = "Rating: " + rating;
            }
            else {
                rating = 400;
                setRating("Rating", rating);
                txtRating.text = "Rating: " + rating;
            
            }
            PlayerPrefs.SetInt("Rating", rating);
            PlayerPrefs.Save();
        },
        error => Debug.LogError(error.GenerateErrorReport()));

    }

    private void setRating(string statName, int newRating) {
        var request = new UpdatePlayerStatisticsRequest
        {
            Statistics = new List<StatisticUpdate>
        {
            new StatisticUpdate
            {
                StatisticName = statName,
                Value = newRating
            }
        }
        };

        PlayFabClientAPI.UpdatePlayerStatistics(request,
            result => UnityEngine.Debug.Log("Stat updated"),
            error => UnityEngine.Debug.LogError(error.GenerateErrorReport()));

    }

    public void GetLeaderboard()
    {
        var request = new GetLeaderboardRequest
        {
            StatisticName = "Rating",
            StartPosition = 0,
            MaxResultsCount = 50
        };

        PlayFabClientAPI.GetLeaderboard(request,
            result =>
            {
                leaderboardList = result.Leaderboard;
                DisplayLeaderboard(leaderboardList);
                foreach (var entry in result.Leaderboard)
                {
                    Debug.Log($"{entry.Position + 1}. {entry.DisplayName ?? entry.PlayFabId} - {entry.StatValue}");
                }
            },
            error => UnityEngine.Debug.LogError(error.GenerateErrorReport()));
    }


    void DisplayLeaderboard(List<PlayerLeaderboardEntry> lbItems)
    {
        ClearLeaderboardDisplay();
        foreach (PlayerLeaderboardEntry plb in lbItems)
        {
            Debug.Log("LeaderboardItem:" + ((int)(plb.Position+1)).ToString() +":"+ plb.DisplayName + ":" + plb.StatValue);
            GameObject goLB = Instantiate(leaderboardItem, contentContainer);
            goLB.transform.Find("NameText").GetComponent<TMP_Text>().text = ((int)(plb.Position + 1)).ToString() + ". " + plb.DisplayName;
            goLB.transform.Find("ScoreText").GetComponent<TMP_Text>().text = plb.StatValue.ToString();
 
        }

    }

    void ClearLeaderboardDisplay()
    {

        foreach (Transform child in contentContainer)
        {
            Destroy(child.gameObject);
        }

    }




}
