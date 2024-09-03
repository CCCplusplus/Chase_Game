using UnityEngine;
using Mirror;
using System.Diagnostics;

public class GoalTrigger : NetworkBehaviour
{
    public GameObject runnerWinsPanel;
    public GameObject chaserLosesPanel;
    public GameObject chaserWinsPanel;
    public GameObject runnerLosesPanel;
    private bool goTime = false;
    private bool runnerWon = false;

    public bool chaserCanTrigger = false;
    private void Start()
    {
        runnerWinsPanel.SetActive(false);
        chaserLosesPanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!chaserCanTrigger)
        {
            if (collision.CompareTag("Runner") ^ collision.CompareTag("Invencible"))
                ShowGameOverScreen();
        }
        else
        {
            if (collision.CompareTag("Runner") ^ collision.CompareTag("Invencible"))
                ShowGameOverScreen();
            else if (collision.CompareTag("Chaser"))
                ShowGameOverScreenChaserWin();
        }
    }

    private void Update()
    {
        if (!goTime) { return; }

        PlayerController playerController = NetworkClient.localPlayer.GetComponent<PlayerController>();

        if (runnerWon)
        {
            if (playerController.playerType == PlayerController.PlayerType.Runner)
                runnerWinsPanel.SetActive(true);
            else
                chaserLosesPanel.SetActive(true);
        }
        else
        {
            if (playerController.playerType == PlayerController.PlayerType.Runner)
                runnerLosesPanel.SetActive(true);
            else
                chaserWinsPanel.SetActive(true);
        }

    }

    [ClientRpc]
    private void RpcShowGameOverScreen()
    {
        PlayerController playerController = NetworkClient.localPlayer.GetComponent<PlayerController>();

        if (playerController.playerType == PlayerController.PlayerType.Runner)
            runnerWinsPanel.SetActive(true);
        
        else
            chaserLosesPanel.SetActive(true);

        goTime = true;

        runnerWon = true;

        // Detener el juego
        //Time.timeScale = 0f;
    }

    [ClientRpc]
    private void RpcShowGameOverScreenChaserWin()
    {
        PlayerController playerController = NetworkClient.localPlayer.GetComponent<PlayerController>();

        if (playerController.playerType == PlayerController.PlayerType.Runner)
            chaserWinsPanel.SetActive(true);

        else
            runnerLosesPanel.SetActive(true);

        goTime = true;

        runnerWon = false;

        // Detener el juego
        //Time.timeScale = 0f;
    }

    private void ShowGameOverScreen()
    {
        RpcShowGameOverScreen();
    }

    private void ShowGameOverScreenChaserWin()
    {
        RpcShowGameOverScreenChaserWin();
    }
}
