using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{

	private static SceneLoader _Instance;
	public static SceneLoader Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = FindObjectOfType<SceneLoader>();
			}
			return _Instance;
		}
	}

	private AsyncOperation asyncLoad, asyncUnloaded;

	private int currentScene = -1;

	public int CurrentScene
	{
		get
		{
			return this.currentScene;
		}
	}

	private bool isLoading = false;
	public bool IsLoading
	{
		get
		{
			return this.isLoading;
		}
	}

	private Coroutine routine;

	private void Start()
	{
#if !UNITY_EDITOR
		QualitySettings.vSyncCount = 1;
#endif
	}

	public void LoadScene(int scene)
	{
		this.ClearRoutine();
		this.routine = this.StartCoroutine(this.StartLoadRoutine(scene));
		this.currentScene = scene;
	}

	private void ClearRoutine()
	{
		this.routine = null;
	}

	private IEnumerator StartLoadRoutine(int scene)
	{
		Time.timeScale = 0f;
		this.isLoading = true;
		// DO START TRANSITION
		yield return AdvancedUtil.WaitForRealSeconds(3.33333f);
		this.asyncLoad = SceneManager.LoadSceneAsync(scene);
		this.asyncLoad.allowSceneActivation = false;

		while (this.asyncLoad.progress < 0.9f)
		{
			yield return null;
		}
		if (!this.asyncLoad.allowSceneActivation)
		{
			this.routine = this.StartCoroutine(this.FinishLoadRoutine());
		}
	}

	private IEnumerator FinishLoadRoutine()
	{
		yield return AdvancedUtil.WaitForRealSeconds(1f);
		this.asyncLoad.allowSceneActivation = true;
		while (!asyncLoad.isDone)
		{
			yield return null;
		}
		yield return AdvancedUtil.WaitForRealSeconds(1f);
		Time.timeScale = 1f;
		// DO END TRANSITION
		yield return AdvancedUtil.WaitForRealSeconds(1f);
		yield return AdvancedUtil.WaitForRealSeconds(0.05f);
		this.isLoading = false;
		this.asyncLoad = null;
		this.routine = null;
	}

	public float GetLoadProgress()
	{
		if (this.asyncLoad == null)
		{
			return 0f;
		}
		else
		{
			return this.asyncLoad.progress;
		}
	}

	public int GetActiveScene()
	{
		return SceneManager.GetActiveScene().buildIndex;
	}
}
