using System;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Utp;
using System.Threading.Tasks;

public struct PlayerTypeMessage : NetworkMessage
{
    public string playerType;
}


namespace Network
{
    public class CustomNetworkManager : RelayNetworkManager
    {
        public GameObject runnerPrefab;
        public GameObject chaserPrefab;
        public Transform runnerTransform;
        public Transform chaserTransform;

        public Player localPlayer;
        private string m_SessionId = "";
        private string m_Username;
        private string m_UserId;

        public bool isLoggedIn = false;
        private List<Player> m_Players;

        public override void Awake()
        {
            base.Awake();
            m_Players = new List<Player>();
            m_Username = SystemInfo.deviceName;

            utpTransport.OnClientConnected = OnClientConnected;
            utpTransport.OnClientDataReceived = OnClientDataReceived;
            utpTransport.OnClientDisconnected = OnClientDisconnected;
            utpTransport.OnServerConnected = OnServerConnected;
            utpTransport.OnServerDataReceived = OnServerDataReceived;
            utpTransport.OnServerDisconnected = OnServerDisconnected;
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<PlayerTypeMessage>(OnPlayerTypeReceived);

            // Manually create and add a player for the host
            NetworkConnectionToClient conn = NetworkServer.localConnection;
            if (conn != null)
                AddPlayerForConnection(conn, PlayerPrefs.GetString("PlayerType", "Runner"));
            else
                Debug.LogError("Host connection not found!");
        }

        private void AddPlayerForConnection(NetworkConnectionToClient conn, string playerType)
        {
            GameObject playerPrefab = playerType == "Runner" ? runnerPrefab : chaserPrefab;
            Transform spawnTransform = playerType == "Runner" ? runnerTransform : chaserTransform;

            GameObject player = Instantiate(playerPrefab, spawnTransform.position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);

            Player comp = player.GetComponent<Player>();
            if (comp != null)
            {
                comp.sessionId = m_SessionId;
                comp.playerType = playerType;
                m_Players.Add(comp);
                Debug.Log($"Player added to the list: {comp.playerType}");
            }
            else
                Debug.LogError("The instantiated player prefab does not have a Player component!");
        }


        public override void OnStartClient()
        {
            base.OnStartClient();
            
        }


        private void OnPlayerTypeReceived(NetworkConnectionToClient conn, PlayerTypeMessage message)
        {
            string playerType = message.playerType;
            if (!CanAddPlayerOfType(playerType))
            {
                Debug.LogError("Cannot add more players, game is full or both player types are selected.");
                conn.Disconnect();
                return;
            }

            GameObject playerPrefab = playerType == "Runner" ? runnerPrefab : chaserPrefab;
            Transform spawnTransform = playerType == "Runner" ? runnerTransform : chaserTransform;

            GameObject player = Instantiate(playerPrefab, spawnTransform.position, Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player);

            Player comp = player.GetComponent<Player>();
            if (comp != null)
            {
                comp.sessionId = m_SessionId;
                comp.playerType = playerType;
                m_Players.Add(comp);
                Debug.Log($"Player added to the list: {comp.playerType}");
            }
            else
            {
                Debug.LogError("The instantiated player prefab does not have a Player component!");
            }
        }




        private async void OnClientConnected()
        {
            Debug.Log("Client connected successfully.");
            await Task.Delay(1000);
            string playerType = PlayerPrefs.GetString("PlayerType", "Runner");
            PlayerTypeMessage message = new PlayerTypeMessage { playerType = playerType };
            NetworkClient.Send(message);
        }



        private void OnClientDataReceived(ArraySegment<byte> data, int channelId)
        {
            Debug.Log("Data received from server.");
            // Here you can process the data received, such as updating player states, handling game events, etc.
        }

        private void OnClientDisconnected()
        {
            Debug.Log("Client disconnected from server.");
            
            if (localPlayer != null)
            {
                m_Players.Remove(localPlayer);
                localPlayer = null;
            }
            // Optionally, trigger a reconnect or go back to the main menu
        }

        private void OnServerConnected(int connectionId)
        {
            Debug.Log($"Client connected with connectionId: {connectionId}");
            // Aquí puedes añadir cualquier lógica que necesites cuando un cliente se conecta al servidor
        }

        private void OnServerDataReceived(int connectionId, ArraySegment<byte> data, int channelId)
        {
            Debug.Log($"Data received from client {connectionId} on channel {channelId}. Data length: {data.Count}");
            // Aquí puedes añadir lógica para procesar los datos recibidos del cliente
        }

        private void OnServerDisconnected(int connectionId)
        {
            Debug.Log($"Client disconnected with connectionId: {connectionId}");
            // Aquí puedes añadir lógica para manejar la desconexión del cliente, como limpiar datos asociados a la conexión
        }


        public async void UnityLogin()
        {
            try
            {
                await UnityServices.InitializeAsync();
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
                isLoggedIn = true;
            }
            catch (Exception e)
            {
                isLoggedIn = false;
                Debug.Log(e);
            }
        }

        public async void StartHostWithRelay()
        {
            Debug.Log("Starting host with Relay...");
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
                isLoggedIn = true;
            }
            StartRelayHost(2, null);
        }

        public async void JoinRelay(string joinCode)
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
                await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("Logged into Unity, player ID: " + AuthenticationService.Instance.PlayerId);
                isLoggedIn = true;
            }
            JoinRelayServer(joinCode);
            
        }

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            string playerType = PlayerPrefs.GetString("PlayerType", "Runner"); // Default to Runner if not set
            Debug.Log($"OnServerAddPlayer called. PlayerType: {playerType}");

            if (!CanAddPlayerOfType(playerType))
            {
                Debug.LogError("Cannot add more players, game is full or both player types are selected.");
                conn.Disconnect();
                return;
            }

            AddPlayerForConnection(conn, playerType);
        }

        private bool CanAddPlayerOfType(string playerType)
        {
            int runnerCount = 0;
            int chaserCount = 0;

            foreach (var player in m_Players)
            {
                if (player.playerType == "Runner") runnerCount++;
                if (player.playerType == "Chaser") chaserCount++;
            }

            if (playerType == "Runner" && runnerCount >= 1) return false;
            if (playerType == "Chaser" && chaserCount >= 1) return false;

            return true;
        }
    }
}
