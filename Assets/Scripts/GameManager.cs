using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public CustomNetworkManager customNetworkManager;

    private void Start()
    {
        if (customNetworkManager == null)
        {
            Debug.LogError("CustomNetworkManager is not assigned in the inspector.");
            return;
        }

        StartCoroutine(InitializeNetworkManager());
    }

    private IEnumerator InitializeNetworkManager()
    {
        yield return new WaitForSeconds(0.1f);

        string playerType = PlayerPrefs.GetString("PlayerType");
        string connectionType = PlayerPrefs.GetString("ConnectionType");

        if (string.IsNullOrEmpty(playerType))
        {
            Debug.LogError("PlayerType is not configured. Make sure to set PlayerType in PlayerPrefs.");
            yield break;
        }

        if (connectionType == "Host")
        {
            customNetworkManager.StartHostWithRelayAndLobby(playerType);
        }
        else if (connectionType == "Client")
        {
            customNetworkManager.ListLobbiesAndJoin();
        }
        else
        {
            Debug.LogError("Unknown ConnectionType. Make sure ConnectionType is 'Host' or 'Client'.");
        }
    }
}
