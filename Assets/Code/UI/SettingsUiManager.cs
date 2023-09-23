using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsUiManager : UiManager
{
    private static SettingsUiManager _Instance;
    public static SettingsUiManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<SettingsUiManager>();
            }
            return _Instance;
        }
    }

    private bool descriptionsEnabled = false;

    public bool DescriptionsEnabled
    {
        get
        {
            return this.descriptionsEnabled;
        }
        set
        {
            this.descriptionsEnabled = value;
        }
    }

    private bool highContrastEnabled = false;

    public bool HighContrastEnabled
    {
        get
        {
            return this.highContrastEnabled;
        }
        set
        {
            this.highContrastEnabled = value;
        }
    }

    private int highContrastMode = 0;

    public int HighContrastMode
    {
        get
        {
            return this.highContrastMode;
        }
        set
        {
            this.highContrastMode = Mathf.Clamp(value, 0, 3);
        }
    }

    private bool aimAssistEnabled = false;

    public bool AimAssistEnabled
    {
        get
        {
            return this.aimAssistEnabled;
        }
        set
        {
            this.aimAssistEnabled = value;
        }
    }

    public Toggle descriptionsToggle, highContrastToggle, aimAssistToggle, ttsToggle;

    public Dropdown resolutionDropdown, qualityDropdown, textureDropdown, aaDropdown;

    public Button exitButton;

    private Resolution[] resolutions;

	private new void Start()
	{
        this.resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        resolutions = Screen.resolutions;
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);
            if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        this.resolutionDropdown.AddOptions(options);
        this.resolutionDropdown.RefreshShownValue();
        this.LoadSettings(currentResolutionIndex);
	}

    public void SetVolume(float volume)
    {

    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        if (qualityIndex != 6)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        switch (qualityIndex)
        {
            case 0: // quality level - very low
                textureDropdown.value = 3;
                aaDropdown.value = 0;
                break;
            case 1: // quality level - low
                textureDropdown.value = 2;
                aaDropdown.value = 0;
                break;
            case 2: // quality level - medium
                textureDropdown.value = 1;
                aaDropdown.value = 0;
                break;
            case 3: // quality level - high
                textureDropdown.value = 0;
                aaDropdown.value = 0;
                break;
            case 4: // quality level - very high
                textureDropdown.value = 0;
                aaDropdown.value = 1;
                break;
            case 5: // quality level - ultra
                textureDropdown.value = 0;
                aaDropdown.value = 2;
                break;
        }

        qualityDropdown.value = qualityIndex;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("DescripionsToggle",
            Convert.ToInt32(this.descriptionsToggle.isOn));
        PlayerPrefs.SetInt("HighContrastToggle",
            Convert.ToInt32(this.highContrastToggle.isOn));
    //    PlayerPrefs.SetInt("HighContrastMode",
    //        Convert.ToInt32(this.highContrastMode.));
        PlayerPrefs.SetInt("AimAsisstToggle",
            Convert.ToInt32(this.aimAssistToggle.isOn));
        PlayerPrefs.SetInt("TtsToggle",
            Convert.ToInt32(this.ttsToggle.isOn));

        PlayerPrefs.SetInt("QualitySettingPreference",
            qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference",
            resolutionDropdown.value);
        PlayerPrefs.SetInt("TextureQualityPreference",
            textureDropdown.value);
        PlayerPrefs.SetInt("AntiAliasingPreference",
            aaDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference",
            Convert.ToInt32(Screen.fullScreen));
    }

    public void LoadSettings(int currentResolutionIndex)
    {
        this.descriptionsToggle.isOn = ((PlayerPrefs.HasKey("DescriptionsToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("DescriptionsToggle")) : false);
        this.highContrastToggle.isOn = ((PlayerPrefs.HasKey("HighContrastToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("HighContrastToggle")) : false);
        this.aimAssistToggle.isOn = ((PlayerPrefs.HasKey("AimAsisstToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("AimAsisstToggle")) : false);
        this.ttsToggle.isOn = ((PlayerPrefs.HasKey("TtsToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("TtsToggle")) : false);


        this.qualityDropdown.value = ((PlayerPrefs.HasKey("QualitySettingsPreference")) ? PlayerPrefs.GetInt("QualitySettingsPreference") : 3);
        this.resolutionDropdown.value = ((PlayerPrefs.HasKey("ResolutionPreference")) ? PlayerPrefs.GetInt("ResolutionPreference") : currentResolutionIndex);
        this.textureDropdown.value = ((PlayerPrefs.HasKey("TextureQualityPreference")) ? PlayerPrefs.GetInt("TextureQualityPreference") : 0);
        this.aaDropdown.value = ((PlayerPrefs.HasKey("AntiAliasingPreference")) ? PlayerPrefs.GetInt("AntiAliasingPreference") : 1);
        Screen.fullScreen = ((PlayerPrefs.HasKey("FullscreenPreference")) ? Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference")) : true);
    }

    public void ExitSettings()
    {
        if (TitleUiManager.Instance != null)
        {

            return;
        } 
    }



}
