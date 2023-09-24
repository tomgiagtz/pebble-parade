using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCamera : MonoBehaviour {

	private static UiCamera _Instance;
	public static UiCamera Instance {
		get {
			if (_Instance == null) {
				_Instance = FindObjectOfType<UiCamera>();
			}
			return _Instance;
		}
	}
	
	private new Camera camera;
	
	public Camera Camera {
		get {
			return this.camera;
		}
	}
	
	private void Awake() {
		this.camera = this.GetComponent<Camera>();
	}
}
