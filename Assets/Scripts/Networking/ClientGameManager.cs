using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
public class ClientGameManager
{
    private JoinAllocation allocation;

    private const string MenuSceneName = "Menu";

    public async Task<bool> InitAsync(string username)
    {
        await UnityServices.InitializeAsync();

        
        AuthState authState = await AuthenticationWrapper.DoAuth(username);

        if (authState == AuthState.Authenticated)
        {
            return true;
        }

        return false;
        
    }

    public void GoToMenu()
    {
        SceneManager.LoadScene(MenuSceneName);
    }

    public async Task StartClientAsync(string joinCode)
    {
        try
        {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartClient();
    }
    public async Task<string> FindLobbyJoinCode(string lobbyName) {
		try { 
            QueryLobbiesOptions options = new QueryLobbiesOptions();
            options.Count = 1;

            // Filter for open lobbies only
            options.Filters = new List<QueryFilter>()
            {
                new QueryFilter(
                    field: QueryFilter.FieldOptions.Name,
                    op: QueryFilter.OpOptions.EQ,
                    value: lobbyName)
            };

            // Order by newest lobbies first
            options.Order = new List<QueryOrder>()
                {
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                };

            QueryResponse lobbies = await LobbyService.Instance.QueryLobbiesAsync(options);
            Debug.Log("LobbyResults:" + lobbies.Results.Count);
            Debug.Log("Lobbyjoincode:" + lobbies.Results[0].Data["relayJoinCode"].Value);
            return lobbies.Results[0].Data["relayJoinCode"].Value;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return "";
        }
    }

}
