using Mirror;
using UnityEngine;

public class Player : NetworkBehaviour
{
    /// <summary>
    /// The Sessions ID for the current server.
    /// </summary>
    [SyncVar]
    public string sessionId = "";

    /// <summary>
    /// The type of player (Runner or Chaser).
    /// </summary>
    [SyncVar]
    public string playerType;

    private void Awake()
    {
        // Any initialization specific to networking
    }

    private void Start()
    {
        // Additional setup can be done here if necessary
    }

    public override void OnStartServer()
    {
        Debug.Log("Player has been spawned on the server!");
    }
}
