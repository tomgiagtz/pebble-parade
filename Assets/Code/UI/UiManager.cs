using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
	
public class UiManager : MonoBehaviour {
	
	public new GameObject gameObject;
	
	public RectTransform rect;
	
	public virtual void Start() {
		this.SetActive(false);
	}
	
	public virtual void SetActive(bool toggle) {
		this.gameObject.SetActive(toggle);
	}
}