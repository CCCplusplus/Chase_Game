using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScreenSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private static bool isFullscreen;
    private static int currentResolutionIndex;

    void Awake()
    {
        isFullscreen = Screen.fullScreen;
        currentResolutionIndex = FindCurrentResolutionIndex();
    }

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        fullscreenToggle.isOn = isFullscreen;

        resolutionDropdown.onValueChanged.AddListener(delegate { SetResolution(resolutionDropdown.value); });
        fullscreenToggle.onValueChanged.AddListener(delegate { SetFullscreen(fullscreenToggle.isOn); });
    }

    public void SetResolution(int resolutionIndex)
    {
        currentResolutionIndex = resolutionIndex;
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, isFullscreen);
        SyncSettings();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        ScreenSettings.isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
        SyncSettings();
    }

    private void SyncSettings()
    {
        foreach (ScreenSettings screenSettings in FindObjectsOfType<ScreenSettings>())
        {
            if (screenSettings != this)
                screenSettings.UpdateSettings(currentResolutionIndex, isFullscreen);
        }
    }

    public void UpdateSettings(int resolutionIndex, bool isFullscreen)
    {
        resolutionDropdown.value = resolutionIndex;
        resolutionDropdown.RefreshShownValue();
        fullscreenToggle.isOn = isFullscreen;
    }

    private int FindCurrentResolutionIndex()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == Screen.currentResolution.width &&
                Screen.resolutions[i].height == Screen.currentResolution.height)
                return i;
        }
        return 0;
    }
}
