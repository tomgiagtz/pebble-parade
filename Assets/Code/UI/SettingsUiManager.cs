using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using TMPro;
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

    private bool ttsEnabled = false;

    public bool TtsEnabled
    {
        get
        {
            return this.ttsEnabled;
        } set
        {
            this.ttsEnabled = value;
        }
    }

    public SettingsUiManager.Toggle descriptionsToggle, highContrastToggle, aimAssistToggle, ttsToggle;

    public TMP_Dropdown resolutionDropdown, fullscreenDropdown, qualityDropdown, textureDropdown, aaDropdown;

    public Button exitButton;

    public List<Resolution> resolutions;
    
    [System.Serializable]
    public class Toggle
    {
        public UnityEngine.UI.Toggle toggle;

        public TextMeshProUGUI on, off;

        public Image onImage, offImage;

        public void ToggleSwap(bool toggle)
        {
            this.on.color = ((toggle) ? SettingsUiManager.Instance.textOnColor : SettingsUiManager.Instance.textOffColor);
            this.off.color = ((!toggle) ? SettingsUiManager.Instance.textOnColor : SettingsUiManager.Instance.textOffColor);
            this.onImage.color = ((toggle) ? SettingsUiManager.Instance.buttonOnColor : SettingsUiManager.Instance.buttonOffColor);
            this.offImage.color = ((!toggle) ? SettingsUiManager.Instance.buttonOnColor : SettingsUiManager.Instance.buttonOffColor);
        }
    }

    [SerializeField]
    private Color32 buttonOnColor, buttonOffColor, textOnColor, textOffColor;

	private new void Start()
	{
        this.resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        
        var resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct();

        this.resolutions = resolutions.ToList();

        int currentResolutionIndex = 0;

        for (int i = 0; i < this.resolutions.Count; i++) {
            string option = this.resolutions[i].width + " x " + this.resolutions[i].height;
         options.Add(option);
     }


        currentResolutionIndex = (int)options.Count - 1;

        this.resolutionDropdown.AddOptions(options);
        this.resolutionDropdown.RefreshShownValue();
        this.LoadSettings(currentResolutionIndex);

        base.SetActive(false);
	}

    public void ToggleDescriptions(bool toggle)
    {
        this.descriptionsEnabled = toggle;
        this.descriptionsToggle.ToggleSwap(toggle);
    }

    public void ToggleHighContrast(bool toggle)
    {
        this.highContrastEnabled = toggle;
        this.highContrastToggle.ToggleSwap(toggle);
    }

    public void ToggleAimAssist(bool toggle)
    {
        this.AimAssistEnabled = toggle;
        this.aimAssistToggle.ToggleSwap(toggle);
    }

    public void ToggleTextToSpeech(bool toggle)
    {
        this.ttsEnabled = toggle;
        this.ttsToggle.ToggleSwap(toggle);
    }

    public void SetVolume(float volume)
    {

    }

    public void SetFullscreen(int isFullscreen)
    {
        Screen.fullScreen = (isFullscreen == 0) ? true : false;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = this.resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        qualityDropdown.value = qualityIndex;
    }

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("DescripionsToggle",
            Convert.ToInt32(this.descriptionsToggle.toggle.isOn));
        PlayerPrefs.SetInt("HighContrastToggle",
            Convert.ToInt32(this.highContrastToggle.toggle.isOn));
    //    PlayerPrefs.SetInt("HighContrastMode",
    //        Convert.ToInt32(this.highContrastMode.));
        PlayerPrefs.SetInt("AimAsisstToggle",
            Convert.ToInt32(this.aimAssistToggle.toggle.isOn));
        PlayerPrefs.SetInt("TtsToggle",
            Convert.ToInt32(this.ttsToggle.toggle.isOn));

        PlayerPrefs.SetInt("QualitySettingPreference",
            qualityDropdown.value);
        PlayerPrefs.SetInt("ResolutionPreference",
            resolutionDropdown.value);
        PlayerPrefs.SetInt("FullscreenPreference",
            Convert.ToInt32(Screen.fullScreen));
    }

    public void LoadSettings(int currentResolutionIndex)
    {

        this.descriptionsToggle.toggle.isOn = ((PlayerPrefs.HasKey("DescriptionsToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("DescriptionsToggle")) : false);
        this.highContrastToggle.toggle.isOn = ((PlayerPrefs.HasKey("HighContrastToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("HighContrastToggle")) : false);
        this.aimAssistToggle.toggle.isOn = ((PlayerPrefs.HasKey("AimAsisstToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("AimAsisstToggle")) : false);
        this.ttsToggle.toggle.isOn = ((PlayerPrefs.HasKey("TtsToggle")) ? Convert.ToBoolean(PlayerPrefs.GetInt("TtsToggle")) : false);

        this.ToggleAimAssist(this.descriptionsToggle.toggle.isOn);
        this.ToggleHighContrast(this.highContrastToggle.toggle.isOn);
        this.ToggleAimAssist(this.aimAssistToggle.toggle.isOn);
        this.ToggleTextToSpeech(this.ttsToggle.toggle.isOn);

        this.qualityDropdown.value = ((PlayerPrefs.HasKey("QualitySettingsPreference")) ? PlayerPrefs.GetInt("QualitySettingsPreference") : 3);
        this.resolutionDropdown.value = ((PlayerPrefs.HasKey("ResolutionPreference")) ? PlayerPrefs.GetInt("ResolutionPreference") : currentResolutionIndex);
        Screen.fullScreen = ((PlayerPrefs.HasKey("FullscreenPreference")) ? Convert.ToBoolean(PlayerPrefs.GetInt("FullscreenPreference")) : true);
    }

    public void ExitSettings()
    {
        this.SaveSettings();
        if (TitleUiManager.Instance != null)
        {
            TitleUiManager.Instance.SetActive(true);
            base.SetActive(false);
            return;
        }
        if (PauseUiManager.Instance != null)
        {
            PauseUiManager.Instance.SetActive(true);
            base.SetActive(false);
            return;
        }
    }
}
