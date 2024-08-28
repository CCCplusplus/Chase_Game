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

    private const string ResolutionPrefKey = "resolutionIndex";
    private const string FullscreenPrefKey = "isFullscreen";

    void Awake()
    {
        // Load saved preferences, if any
        isFullscreen = PlayerPrefs.GetInt(FullscreenPrefKey, Screen.fullScreen ? 1 : 0) == 1;
        currentResolutionIndex = PlayerPrefs.GetInt(ResolutionPrefKey, FindCurrentResolutionIndex());
    }

    void Start()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        HashSet<string> uniqueResolutions = new HashSet<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            if (uniqueResolutions.Add(option)) // Only add unique resolutions
            {
                options.Add(option);
            }
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
        SaveSettings();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        ScreenSettings.isFullscreen = isFullscreen;
        Screen.fullScreen = isFullscreen;
        SaveSettings();
    }

    private void SaveSettings()
    {
        // Save preferences
        PlayerPrefs.SetInt(ResolutionPrefKey, currentResolutionIndex);
        PlayerPrefs.SetInt(FullscreenPrefKey, isFullscreen ? 1 : 0);
        PlayerPrefs.Save();

        // Sync with other instances if needed
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
        // Find the current resolution index in the available resolutions
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].width == Screen.currentResolution.width &&
                Screen.resolutions[i].height == Screen.currentResolution.height)
                return i;
        }
        return 0; // Default to the first resolution if not found
    }
}
