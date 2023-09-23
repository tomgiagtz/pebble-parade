using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseUiManager : UiManager
{
    private static PauseUiManager _Instance;
    public static PauseUiManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<PauseUiManager>();
            }
            return _Instance;
        }
    }

    [SerializeField]
    private Button continueButton, settingsButton, quitButton;

	private new void Start()
	{
        this.continueButton.onClick.AddListener(() =>
        {
            this.Continue();
        });

        this.settingsButton.onClick.AddListener(() =>
        {
            this.OpenSettings();
        });

        this.quitButton.onClick.AddListener(() =>
        {
            this.Quit();
        });

        this.SetActive(false);
	}

    public new void SetActive(bool toggle)
    {
        Time.timeScale = ((toggle) ? 0f : 1f);
        base.SetActive(toggle);
    }

	private void Update()
	{
        if (SceneLoader.Instance.IsLoading || TitleUiManager.Instance != null)
        {
            return;
        }

        this.HandlePause();
	}

    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            this.SetActive(true);
        }
    }

	private void Continue()
    {
        this.SetActive(false);
    }

    private void OpenSettings()
    {
        base.SetActive(false);
        SettingsUiManager.Instance.SetActive(true);
    }

	private void Quit()
	{
        SceneLoader.Instance.LoadScene(0);
	}
}
