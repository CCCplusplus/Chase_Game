using Mirror;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject pausaCanvas;
    public GameObject settingsCanvas;

    public void MainMenu()
    {
        if (NetworkManager.singleton.isNetworkActive)
        {
            if (NetworkServer.active && NetworkClient.isConnected)
                NetworkManager.singleton.StopHost();
            else if (NetworkClient.isConnected)
                NetworkManager.singleton.StopClient();
        }

        
        if (NetworkManager.singleton != null)
            Destroy(NetworkManager.singleton.gameObject);
        
        SceneManager.LoadSceneAsync("Main Menu");
    }


    public void unpause()
    {
        StartCoroutine(QuitaPausa());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSettings()
    {
        StartCoroutine(ShowSetting());
    }

    public void Turnoff()
    {
        pausaCanvas.SetActive(false);
        settingsCanvas.SetActive(false);
    }

    public void BackToPause()
    {
        StartCoroutine(BackMainMenu());
    }

    private IEnumerator BackMainMenu()
    {
        yield return new WaitForSeconds(0.3f);
        pausaCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    private IEnumerator ShowSetting()
    {
        yield return new WaitForSeconds(0.3f);
        pausaCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    private IEnumerator QuitaPausa()
    {
        yield return new WaitForSeconds(0.3f);
        playerController.MenuPause();
    }

}
