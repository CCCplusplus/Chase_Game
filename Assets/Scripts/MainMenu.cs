using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown connectionTypeDropdown;
    public TMP_InputField inputField;
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;

    private void Start()
    {
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void PlayAsRunner()
    {
        PlayerPrefs.SetString("PlayerType", "Runner");
        SetConnectionType();
        //SceneManager.LoadSceneAsync("Game");
    }

    public void PlayAsChaser()
    {
        PlayerPrefs.SetString("PlayerType", "Chaser");
        SetConnectionType();
        //SceneManager.LoadSceneAsync("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ShowSettings()
    {
        mainMenuCanvas.SetActive(false);
        settingsCanvas.SetActive(true);
    }

    public void BackToMainMenu()
    {
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void Creditos()
    {
        SceneManager.LoadScene("Credits");
    }

    private void SetConnectionType()
    {
        string connectionType = connectionTypeDropdown.options[connectionTypeDropdown.value].text;
        PlayerPrefs.SetString("ConnectionType", connectionType);

        if (!string.IsNullOrEmpty(inputField.text))
            PlayerPrefs.SetString("RelayJoinCode", inputField.text);
    }
}
