using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HostGameManager
{
    private Allocation allocation;
    public string joinCode;

    private const int MaxConnections = 20;
    private const string GameSceneName = "Game";


    // Lobby data key used to lookup each players' name as displayed in the lobby.
    public const string k_PlayerNameKey = "playerName";

    // Lobby data key used to check if each player has clicked the [Ready] button.
    public const string k_IsReadyKey = "isReady";

    // Lobby data for host name.
    public const string k_HostNameKey = "hostName";

    // Lobby data for host's Relay Join Code. Used to allow all players to initialize Relay so NGO
    // (Netcode for GameObjects) can synchronize multiplayer game play between players.
    public const string k_RelayJoinCodeKey = "relayJoinCode";


    public Lobby activeLobby { get; private set; }

    public async Task StartHostAsync()
    {
        try
        {
            allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }

        try
        {
            joinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log(joinCode);
        }
        catch (Exception e)
        {
            Debug.Log(e);
            return;
        }



        var options = new CreateLobbyOptions();
        options.IsPrivate = false;
        options.Data = new Dictionary<string, DataObject>
                {
                    { k_HostNameKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) },
                    { k_RelayJoinCodeKey, new DataObject(DataObject.VisibilityOptions.Public, joinCode) },
                };

        options.Player = CreatePlayerData();

        string lobbyName = joinCode;
        int maxPlayers = 4;

        activeLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);

        UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
        transport.SetRelayServerData(relayServerData);

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
        GameObject.Find("RoomCodeCanvas").GetComponent<RoomCodeText>().RoomCode(joinCode);
    }



    Player CreatePlayerData()
    {
        var player = new Player();
        player.Data = CreatePlayerDictionary();

        return player;
    }

    Dictionary<string, PlayerDataObject> CreatePlayerDictionary()
    {
        var playerDictionary = new Dictionary<string, PlayerDataObject>
            {
                { k_PlayerNameKey,  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "Playername") },
                { k_IsReadyKey,  new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, "True") },
            };

        return playerDictionary;
    }
}

