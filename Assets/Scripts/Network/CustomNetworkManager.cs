using UnityEngine;
using Mirror;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Networking.Transport.Relay;
using TMPro;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public struct PlayerTypeMessage : NetworkMessage
{
    public string playerType;
}

public class CustomNetworkManager : NetworkManager
{
    public GameObject runnerPrefab;
    public GameObject chaserPrefab;
    public Transform runnerTransform;
    public Transform chaserTransform;

    public TextMeshProUGUI visibleLobbyId;
    private Lobby currentLobby;

    public override void OnStartServer()
    {
        NetworkServer.RegisterHandler<PlayerTypeMessage>(OnCreatePlayer);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("OnClientConnect called");

        string playerType = PlayerPrefs.GetString("PlayerType");
        var msg = new PlayerTypeMessage { playerType = playerType };
        NetworkClient.Send(msg);
    }

    private void OnCreatePlayer(NetworkConnectionToClient conn, PlayerTypeMessage msg)
    {
        Transform start = (msg.playerType == "Runner") ? runnerTransform : chaserTransform;
        if (start == null)
        {
            start = new GameObject("DefaultStart").transform;
            start.position = Vector3.zero;
            start.rotation = Quaternion.identity;
        }

        GameObject player = (msg.playerType == "Runner") ? Instantiate(runnerPrefab, start.position, start.rotation) : Instantiate(chaserPrefab, start.position, start.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);
        Debug.Log("Player created and added to connection");
    }

    public async void StartHostWithRelayAndLobby(string playerType)
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            // Crear asignación de Relay
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            Debug.Log("Join code creado: " + joinCode); // Debug para verificar el joinCode

            var relayServerData = new RelayServerData(allocation, "dtls");

            // Crear lobby
            var lobbyOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { "playerType", new DataObject(DataObject.VisibilityOptions.Public, playerType) },
                    { "joinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode) }
                }
            };

            currentLobby = await LobbyService.Instance.CreateLobbyAsync("LobbyName", 4, lobbyOptions);
            Debug.Log("Lobby creado: " + currentLobby.Id);

            // Mostrar el ID del lobby en la UI
            if (visibleLobbyId != null)
            {
                visibleLobbyId.text = "Lobby ID: " + currentLobby.Id;
            }

            NetworkManager.singleton.transport = GetComponent<Mirror.SimpleWeb.SimpleWebTransport>();
            NetworkManager.singleton.StartHost();
            Debug.Log("Host started");
        }
        catch (Exception e)
        {
            Debug.LogError("Error in StartHostWithRelayAndLobby: " + e.Message);
        }
    }

    public async void ListLobbiesAndJoin()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            var queryResponse = await LobbyService.Instance.QueryLobbiesAsync();
            foreach (var lobby in queryResponse.Results)
            {
                // Aquí puedes mostrar una lista de lobbies disponibles en la UI
                // Por simplicidad, uniremos al primer lobby encontrado
                if (lobby.Data["playerType"].Value == "Runner" && PlayerPrefs.GetString("PlayerType") == "Chaser" ||
                    lobby.Data["playerType"].Value == "Chaser" && PlayerPrefs.GetString("PlayerType") == "Runner")
                {
                    Debug.Log("Lobby encontrado: " + lobby.Id); // Debug para verificar el lobby encontrado
                    await JoinLobbyAndRelay(lobby.Id);
                    break;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error in ListLobbiesAndJoin: " + e.Message);
        }
    }

    public async Task JoinLobbyAndRelay(string lobbyId)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);
            string joinCode = currentLobby.Data["joinCode"].Value;

            Debug.Log("Join code from lobby: " + joinCode);

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("JoinAllocation obtenido: " + joinAllocation.AllocationId); // Verificar joinAllocation

            var relayServerData = new RelayServerData(joinAllocation, "dtls");

            NetworkManager.singleton.transport = GetComponent<Mirror.SimpleWeb.SimpleWebTransport>();
            NetworkManager.singleton.StartClient();
            Debug.Log("Client started and trying to connect");
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("RelayServiceException in JoinLobbyAndRelay: " + e.Message);
        }
        catch (AuthenticationException e)
        {
            Debug.LogError("AuthenticationException in JoinLobbyAndRelay: " + e.Message);
        }
        catch (Exception e)
        {
            Debug.LogError("Error in JoinLobbyAndRelay: " + e.Message);
        }
    }
}
