using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerCamera : MonoBehaviour
{
    private Camera cam;


    // Start is called before the first frame update
    void Start()
    {
        this.cam = this.GetComponent<Camera>();
        if (UiCamera.Instance != null)
        {
            var cameraData = this.cam.GetUniversalAdditionalCameraData();
            cameraData.cameraStack.Add(UiCamera.Instance.Camera);
        }
    }
}
