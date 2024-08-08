using UnityEngine;
using Mirror;

public class ChaserTrigger : NetworkBehaviour
{
    public GameObject chaserWinsPanel;
    public GameObject runnerLosesPanel;

    private void Start()
    {
        chaserWinsPanel.SetActive(false);
        runnerLosesPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Runner"))
        {
            ShowGameOverScreen();
        }
    }

    [ClientRpc]
    private void RpcShowGameOverScreen()
    {
        PlayerController playerController = NetworkClient.localPlayer.GetComponent<PlayerController>();

        if (playerController.playerType == PlayerController.PlayerType.Chaser)
        {
            chaserWinsPanel.SetActive(true);
        }
        else
        {
            runnerLosesPanel.SetActive(true);
        }

        // Detener el juego
        //Time.timeScale = 0f;
    }

    private void ShowGameOverScreen()
    {
        RpcShowGameOverScreen();
    }
}
