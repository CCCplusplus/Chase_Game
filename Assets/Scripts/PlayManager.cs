using System.Collections;
using UnityEngine;
using Mirror;
using TMPro;

public class PlayManager : NetworkBehaviour
{
    public GameObject sharedObject;            // The GameObject to be activated
    public TMP_Text countdownText;             // Reference to the countdown text in the middle of the screen
    public TMP_Text topCountdownText;          // Reference to the top countdown text in the screen
    public TMP_Text topLeftTimerText;          // Reference to the top left corner timer text
    public Transform runnerSpawn;              // Transform for the runner's initial spawn position
    public Transform chaserSpawn;              // Transform for the chaser's initial spawn position
    public Transform newRunnerSpawn;           // New Transform for the runner's updated spawn position
    public Transform newChaserSpawn;           // New Transform for the chaser's updated spawn position
    public GameObject goalTriggerObject;       // The GameObject containing the GoalTrigger script
    public GameObject cage;

    [SyncVar] private bool chaserCanTrigger = false;

    // This method will be called to start the sequence
    public void StartSequence()
    {
        RpcStartSequence();
    }

    [ClientRpc]
    private void RpcStartSequence()
    {
        StartCoroutine(SequenceRoutine());
    }

    private IEnumerator SequenceRoutine()
    {
        // 1. Activate the shared object for both players
        sharedObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        // Update runner and chaser spawn positions
        runnerSpawn.position = newRunnerSpawn.position;
        chaserSpawn.position = newChaserSpawn.position;

        runnerSpawn = newRunnerSpawn;
        chaserSpawn = newChaserSpawn;

        // 2. Start the countdown from 5 to 0 in the middle of the screen
        countdownText.gameObject.SetActive(true); // Ensure the text is visible
        for (int i = 5; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        countdownText.gameObject.SetActive(false);

        // 3. Move players to their specific positions
        if (isServer)
        {
            RpcMovePlayers(runnerSpawn.position, chaserSpawn.position);
        }

        yield return new WaitForSeconds(1f);

        // 4. Show the top countdown "The Zombie will be released in..." from 10 to 0
        topCountdownText.gameObject.SetActive(true);
        for (int i = 10; i > 0; i--)
        {
            topCountdownText.text = $"The Zombie will be released in {i}";
            yield return new WaitForSeconds(1f);
        }
        topCountdownText.gameObject.SetActive(false);

        // 5. Deactivate the specific GameObject after the countdown
        if (isServer)
        {
            RpcDeactivateObject(cage);
        }

        // 6. Start the 2-minute countdown at the top left corner of the screen
        topLeftTimerText.gameObject.SetActive(true);
        int totalTime = 120; // 2 minutes in seconds
        while (totalTime > 0)
        {
            totalTime--;
            int minutes = totalTime / 60;
            int seconds = totalTime % 60;
            topLeftTimerText.text = $"{minutes:00}:{seconds:00}";
            yield return new WaitForSeconds(1f);
        }
        topLeftTimerText.gameObject.SetActive(false);

        // 7. Allow the chaser to destroy the goal
        if (isServer)
        {
            chaserCanTrigger = true;
            RpcEnableChaserCanTrigger();
            RpcFlashMessage("Chaser can Destroy the Goal");
        }
    }

    [ClientRpc]
    private void RpcMovePlayers(Vector3 runnerPos, Vector3 chaserPos)
    {
        // Find and move the players
        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.playerType == "Runner")
            {
                player.transform.position = runnerPos;
            }
            else if (player.playerType == "Chaser")
            {
                player.transform.position = chaserPos;
            }
        }
    }

    [ClientRpc]
    private void RpcDeactivateObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    [ClientRpc]
    private void RpcEnableChaserCanTrigger()
    {
        GoalTrigger goalTrigger = goalTriggerObject.GetComponent<GoalTrigger>();
        goalTrigger.chaserCanTrigger = true;
    }

    [ClientRpc]
    private void RpcFlashMessage(string message)
    {
        StartCoroutine(FlashMessageRoutine(message));
    }

    private IEnumerator FlashMessageRoutine(string message)
    {
        countdownText.gameObject.SetActive(true);
        countdownText.text = message;
        countdownText.color = Color.red; // Make it stand out

        for (int i = 0; i < 5; i++) // Flash message a few times
        {
            countdownText.enabled = !countdownText.enabled;
            yield return new WaitForSeconds(0.5f);
        }

        countdownText.gameObject.SetActive(false);
    }
}
