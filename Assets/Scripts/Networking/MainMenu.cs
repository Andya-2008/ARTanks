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
    private Coroutine hbl;
    

    public void Start()
	{
        //txtUsername.text = Crypto.DecryptString(PlayerPrefs.GetString("username"));
        txtUsername.text = GameObject.Find("ApplicationController").GetComponent<ApplicationController>().currentUser.Username;
        
	}
	public async void StartHost()
    {
        string lobbyId = await HostSingleton.Instance.GameManager.StartHostAsync();
        GameObject appcontroller = GameObject.Find("ApplicationController");
        hbl = StartCoroutine(appcontroller.GetComponent<ApplicationController>().HeartbeatLobbyCoroutine(lobbyId, 2.0f));
        //StartCoroutine(HeartbeatLobbyCoroutine(lobbyId, 15));
    }

    public async void StopLobbyHeartBeat() {
        StopCoroutine(hbl);
        hbl = null;
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

            //await UnityServices.InitializeAsync();
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 25;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.AvailableSlots,
                    op: QueryFilter.OpOptions.GT,
                    value: "0")
            };

                    // Order by newest lobbies first
                    options.Order = new List<QueryOrder>()
            {
                new QueryOrder(
                    asc: false,
                    field: QueryOrder.FieldOptions.Created)
            };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
            Debug.Log("Got Lobbies:" + lobbies.Results.Count);

            joinCodeDropDown.options.Clear();
            List<string> rooms = new List<string>();
            foreach (var l in lobbies.Results) {
                Debug.Log(l.Name + ":" + l.Data["relayJoinCode"].Value);
                rooms.Add(l.Name);
                /*
                foreach (var d in l.Data) {
                    Debug.Log(d.Key + ":" + d.Value);
                }*/


            }
            joinCodeDropDown.AddOptions(rooms);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
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
}
