using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleUiManager : UiManager
{
    private static TitleUiManager _Instance;
    public static TitleUiManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<TitleUiManager>();
            }
            return _Instance;
        }
    }

    [SerializeField]
    private Button startButton, settingsButton, exitButton;

    [SerializeField]
    private GameObject globalManagerPrefab;

	private void Awake()
	{
        if (GlobalManager.Instance == null)
        {
            Instantiate(this.globalManagerPrefab);
        }
	}

	private new void Start()
	{
        this.startButton.onClick.AddListener(() =>
        {
            this.StartGame();
        });

        this.settingsButton.onClick.AddListener(() =>
        {
            this.OpenSettings();
        });

        this.exitButton.onClick.AddListener(() =>
        {
            this.ExitGame();
        });
	}

    private void StartGame()
    {
        SceneLoader.Instance.LoadScene(1);
        this.startButton.interactable = false;
        this.settingsButton.interactable = false;
        this.exitButton.interactable = false;
    }

    private void OpenSettings()
    {
        base.SetActive(false);
        SettingsUiManager.Instance.SetActive(true);
    }

    private void ExitGame()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }



}
