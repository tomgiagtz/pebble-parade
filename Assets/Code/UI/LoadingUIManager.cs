using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LoadingUIManager : UiManager
{
	private static LoadingUIManager _Instance;
	public static LoadingUIManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = FindObjectOfType<LoadingUIManager>();
			}
			return _Instance;
		}
	}

	[SerializeField]
	private Animator animator;

	[SerializeField]
	private TextMeshProUGUI loading;

	private new void Start()
	{
		this.SetActive(true);
	}

	public void PlayAnimation(string anim)
	{
		int hash = Animator.StringToHash(anim);
		this.animator.Play(hash, -1, 0f);
	}

	public IEnumerator DoLoadText()
	{
		while (SceneLoader.Instance.IsLoading)
		{
			yield return AdvancedUtil.WaitForRealSeconds(0.5f);
			this.loading.SetText("Loading...");
			yield return AdvancedUtil.WaitForRealSeconds(0.5f);
			this.loading.SetText("Loading.");
			yield return AdvancedUtil.WaitForRealSeconds(0.5f);
			this.loading.SetText("Loading..");

			yield return null;
		}
	}
}
