using UnityEngine;
using Mirror;

public class ChaserTrigger : NetworkBehaviour
{
    public GameObject chaserWinsPanel;
    public GameObject runnerLosesPanel;
    private bool goTime = false;
    private void Start()
    {
        chaserWinsPanel.SetActive(false);
        runnerLosesPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Runner"))
            ShowGameOverScreen();
    }

    private void Update()
    {
        if (!goTime) { return; }

        PlayerController playerController = NetworkClient.localPlayer.GetComponent<PlayerController>();

        if (playerController.playerType == PlayerController.PlayerType.Chaser)
            chaserWinsPanel.SetActive(true);
        
        else
            runnerLosesPanel.SetActive(true);
        
    }

    [ClientRpc]
    private void RpcShowGameOverScreen()
    {
        PlayerController playerController = NetworkClient.localPlayer.GetComponent<PlayerController>();

        if (playerController.playerType == PlayerController.PlayerType.Chaser)
            chaserWinsPanel.SetActive(true);
        else
            runnerLosesPanel.SetActive(true);
        goTime = true;
    }

    private void ShowGameOverScreen()
    {
        RpcShowGameOverScreen();
    }
}
