using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalManager : MonoBehaviour
{

	private static GlobalManager _Instance;
	public static GlobalManager Instance
	{
		get
		{
			if (_Instance == null)
			{
				_Instance = FindObjectOfType<GlobalManager>();
			}
			return _Instance;
		}
	}

	private void Awake()
	{
		if (GlobalManager.Instance != null && GlobalManager.Instance != this)
		{
			Destroy(this.gameObject);
		}
	}
}
