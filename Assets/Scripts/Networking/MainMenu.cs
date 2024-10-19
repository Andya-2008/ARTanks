using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private TMP_Dropdown joinCodeDropDown;
    private float lastCheck = 0;
    public async void StartHost()
    {
        await HostSingleton.Instance.GameManager.StartHostAsync();
    }

    public async void StartClient()
    {
        string joincode;
        if (joinCodeField.text != "")
        {
            joincode = joinCodeField.text;
        }
        else {
            joincode = joinCodeDropDown.options[joinCodeDropDown.value].text;
        }

        await ClientSingleton.Instance.GameManager.StartClientAsync(joincode);
    }

	private void Update()
	{

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
                Debug.Log(l.Name + ":" + l.LobbyCode);
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
}
