using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class GameOverMenu : MonoBehaviour
{
    public GameObject gameOverMenuUI;

    private void Start()
    {
        gameOverMenuUI.SetActive(false);
    }

    public void ShowGameOver()
    {
        gameOverMenuUI.SetActive(true);
        Time.timeScale = 0f; // Pausar el juego
    }

    public void MainMenu()
    {
        Time.timeScale = 1f; // Restablecer el tiempo antes de cambiar de escena
        if (NetworkManager.singleton.isNetworkActive)
        {
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopHost();
            }
            else if (NetworkClient.isConnected)
            {
                NetworkManager.singleton.StopClient();
            }
        }
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
    }
}

