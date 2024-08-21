using UnityEngine;
using System.Collections;
using Network;

public class GameManager : MonoBehaviour
{
    public CustomNetworkManager customNetworkManager;
    public string joinCode;

    private void Start()
    {
        if (customNetworkManager == null)
        {
            Debug.LogError("CustomNetworkManager is not assigned in the inspector.");
            return;
        }

        joinCode = PlayerPrefs.GetString("RelayJoinCode");

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
            
            customNetworkManager.StartHostWithRelay();
        }
        else if (connectionType == "Client")
        {
            if (!string.IsNullOrEmpty(joinCode))
            {
                customNetworkManager.JoinRelay(joinCode);
            }
            else
            {
                Debug.LogError("Join code is empty or null. Cannot join relay.");
            }
        }
        else
        {
            Debug.LogError("Unknown ConnectionType. Make sure ConnectionType is 'Host' or 'Client'.");
        }
    }
}
