using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.Rendering.Universal;

public class ThrowController : MonoBehaviour
{
    [Header("Aim Objects")]
    public DecalProjector projector;

    [Header("Aim Params")]
    public LayerMask hitLayer;
    public float playerOffset = 1f;
    public float maxAimDistance = 5f;

    

    bool isAiming;
    float projSize = 0.8f;

    Vector3 aimPoint;
    Vector3 aimNormal;
    Vector3 aimNormalRotate1;
    Vector3 aimNormalRotate2;
    Vector3[] projPoints;
    public void OnToggleAim(InputAction.CallbackContext context)
    {
        if (context.started) ToggleAimOn();
        else if (context.canceled) ToggleAimOff();
    }

    void ToggleAimOn()
    {
        isAiming = true;
        projector.gameObject.SetActive(true);
        Debug.Log("Aiming On");
    }
    void ToggleAimOff()
    {
        isAiming = false;
        projector.gameObject.SetActive(false);
        Debug.Log("Aiming Off");
    }

    private void Update()
    {
        if(isAiming)
        {
            RaycastHit hit;
            Camera cam = Camera.main;
            Vector3 direction = ((gameObject.transform.position + cam.transform.up * playerOffset) - cam.transform.position);
            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, Mathf.Infinity, hitLayer))
            {
                aimPoint = hit.point;
                aimNormal = hit.normal;
            }
        }
    }

    private void FixedUpdate()
    {
        if(isAiming)
        {
            projector.transform.position = aimPoint;
            projector.transform.forward = CalcAvgNormal();
        }
    }

    Vector3 CalcAvgNormal()
    {
        Camera cam = Camera.main;
        Vector3 cross = Vector3.Cross(aimNormal, Vector3.zero).normalized;
        Vector3 vertVec = (Quaternion.AngleAxis(90, Vector3.forward) * aimNormal).normalized;
        Vector3 horizVec = (Quaternion.AngleAxis(90, Vector3.right) * aimNormal).normalized;
        if(Vector3.Dot(aimNormal, vertVec) >= 0.95f)
        {
            vertVec = (Quaternion.AngleAxis(90, Vector3.up) * aimNormal).normalized;
            horizVec = (Quaternion.AngleAxis(90, Vector3.right) * aimNormal).normalized;
        }
        else if(Vector3.Dot(aimNormal, horizVec) >= 0.95f)
        {
            vertVec = (Quaternion.AngleAxis(90, Vector3.up) * aimNormal).normalized;
            horizVec = (Quaternion.AngleAxis(90, Vector3.forward) * aimNormal).normalized;        
        }

        aimNormalRotate1 = vertVec;
        aimNormalRotate2 = horizVec;
        Vector3 p1 = aimPoint - (horizVec * projSize) + (vertVec * projSize) + (aimNormal * projSize);
        Vector3 p2 = aimPoint + (horizVec * projSize) + (vertVec * projSize) + (aimNormal * projSize);
        Vector3 p3 = aimPoint - (horizVec * projSize) - (vertVec * projSize) + (aimNormal * projSize);
        Vector3 p4 = aimPoint + (horizVec * projSize) - (vertVec * projSize) + (aimNormal * projSize);

        Vector3 p5 = aimPoint - (horizVec * projSize) + (vertVec * projSize) - (aimNormal * projSize);
        Vector3 p6 = aimPoint + (horizVec * projSize) + (vertVec * projSize) - (aimNormal * projSize);
        Vector3 p7 = aimPoint - (horizVec * projSize) - (vertVec * projSize) - (aimNormal * projSize);
        Vector3 p8 = aimPoint + (horizVec * projSize) - (vertVec * projSize) - (aimNormal * projSize);
        projPoints = new Vector3[] {p1, p2, p3, p4, p5, p6, p7, p8};

        Vector3 direction = cam.transform.forward;
        Vector3 normalAvg = Vector3.zero;
        foreach(Vector3 point in projPoints)
        {
            RaycastHit hit;
            if (Physics.Raycast(point, direction, out hit, 5f, hitLayer))
            {
                normalAvg += hit.normal;
            }
        }
        return -(normalAvg * (1f / projPoints.Length));
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!isAiming || !context.started) return;
        Debug.Log("Throwing");


    }

    private void OnDrawGizmos()
    {
        if(isAiming)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(aimPoint, 0.1f);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(aimPoint, aimNormal);
            Gizmos.color = Color.red;
            Gizmos.DrawRay(aimPoint, aimNormalRotate1);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(aimPoint, aimNormalRotate2);
        }

        if(projector.gameObject.activeSelf)
        {
            Gizmos.color = Color.red;
            foreach (Vector3 point in projPoints)
            {
                Gizmos.DrawSphere(point, 0.1f);
            }
        }
    }
}
