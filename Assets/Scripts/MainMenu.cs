using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TMP_Dropdown connectionTypeDropdown;
    public TMP_InputField inputField;
    public GameObject mainMenuCanvas;
    public GameObject settingsCanvas;

    public Button RunnerButton;
    public Button ChaserButton;

    public GameObject warningText;

    private void Start()
    {
        mainMenuCanvas.SetActive(true);
        settingsCanvas.SetActive(false);

        RunnerButton.interactable = false;
        ChaserButton.interactable = false;

        warningText.SetActive(false);

        inputField.onValueChanged.AddListener(OnInputFieldChanged);
        connectionTypeDropdown.onValueChanged.AddListener(OnDropdownChanged);

        UpdateButtonInteractivity();
    }

    private void OnInputFieldChanged(string input)
    {
        UpdateButtonInteractivity();
    }

    private void OnDropdownChanged(int index)
    {
        UpdateButtonInteractivity();
    }

    private void UpdateButtonInteractivity()
    {
        int dropdownIndex = connectionTypeDropdown.value;

        if (dropdownIndex == 0)
        {
            RunnerButton.interactable = true;
            ChaserButton.interactable = true;
            warningText.SetActive(false);
        }
        else if (dropdownIndex == 1)
        {
            bool isInputNotEmpty = !string.IsNullOrEmpty(inputField.text);
            RunnerButton.interactable = isInputNotEmpty;
            ChaserButton.interactable = isInputNotEmpty;
            warningText.SetActive(!isInputNotEmpty);
        }
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

    public void RunnerTutorial()
    {
        PlayerPrefs.SetString("ConnectionType", "Host");
        PlayerPrefs.SetString("PlayerType", "Runner");
        SceneManager.LoadScene("Tutorial Runner");
    }

    public void ChaserTutorial()
    {
        PlayerPrefs.SetString("ConnectionType", "Host");
        PlayerPrefs.SetString("PlayerType", "Chaser");
        SceneManager.LoadScene("Tutorial Chaser");
    }

    private void SetConnectionType()
    {
        string connectionType = connectionTypeDropdown.options[connectionTypeDropdown.value].text;
        PlayerPrefs.SetString("ConnectionType", connectionType);

        if (!string.IsNullOrEmpty(inputField.text))
            PlayerPrefs.SetString("RelayJoinCode", inputField.text);
    }
}
