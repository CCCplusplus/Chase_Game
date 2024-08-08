using UnityEngine;
using Mirror;
using System.Diagnostics;

public class GoalTrigger : NetworkBehaviour
{
    public GameObject runnerWinsPanel;
    public GameObject chaserLosesPanel;

    private void Start()
    {
        runnerWinsPanel.SetActive(false);
        chaserLosesPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Runner") ^ collision.CompareTag("Invencible"))
        {
            ShowGameOverScreen();
        }
    }

    [ClientRpc]
    private void RpcShowGameOverScreen()
    {
        PlayerController playerController = NetworkClient.localPlayer.GetComponent<PlayerController>();

        if (playerController.playerType == PlayerController.PlayerType.Runner)
        {
            runnerWinsPanel.SetActive(true);
            UnityEngine.Debug.Log("it works");
        }
        else
        {
            chaserLosesPanel.SetActive(true);
        }

        // Detener el juego
        //Time.timeScale = 0f;
    }

    private void ShowGameOverScreen()
    {
        RpcShowGameOverScreen();
    }
}
