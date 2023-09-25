using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

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

    public InputAction pauseAction;

	private void Awake()
    {
        this.pauseAction = new InputAction("PauseAction", binding: "<Keyboard>/escape", interactions: "press (behavior=1)");
        this.pauseAction.AddBinding("<Gamepad>/startButton");

        this.pauseAction.started += this.HandlePause;
        this.pauseAction.Enable();
    }

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

    private void HandlePause(InputAction.CallbackContext _context)
    {
        Debug.Log("Attempting to handle pause");
        if (SceneLoader.Instance.IsLoading || TitleUiManager.Instance != null)
        {
            Debug.Log("Cannot do pause");
            return;
        }
        bool flag = this.gameObject.activeSelf;
        this.SetActive(!flag);
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
