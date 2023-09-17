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
    public float playerOffset = 2f;
    public float maxAimDistance = 30f;

    

    bool isAiming;
    float projSize = 0.5f;

    Vector3 aimPoint;
    Vector3 aimNormal;
    Vector3 aimNormalRotate1;
    Vector3 aimNormalRotate2;
    Vector3[] projPoints;

    private void Update()
    {
        if (isAiming)
        {
            RaycastHit hit;
            Camera cam = Camera.main;
            Vector3 direction = ((gameObject.transform.position + cam.transform.up * playerOffset) - cam.transform.position);
            if (Physics.Raycast(Camera.main.transform.position, direction, out hit, maxAimDistance, hitLayer))
            {
                aimPoint = hit.point;
                aimNormal = hit.normal;
            }
        }
    }
    private void FixedUpdate()
    {
        if (isAiming)
        {
            projector.transform.position = aimPoint;
            projector.transform.forward = CalcAvgNormal();
        }
    }
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

    Vector3 CalcAvgNormal()
    {


        Camera cam = Camera.main;
        Vector3 normalVec = -cam.transform.forward;
        Vector3 rightVec = cam.transform.right;
        Vector3 upVec = cam.transform.up;

        aimNormalRotate1 = rightVec;
        aimNormalRotate2 = upVec;
        Vector3 p1 = aimPoint - (rightVec * projSize) + (upVec * projSize) + (normalVec * projSize);
        Vector3 p2 = aimPoint + (rightVec * projSize) + (upVec * projSize) + (normalVec * projSize);
        Vector3 p3 = aimPoint - (rightVec * projSize) - (upVec * projSize) + (normalVec * projSize);
        Vector3 p4 = aimPoint + (rightVec * projSize) - (upVec * projSize) + (normalVec * projSize);

        projPoints = new Vector3[] {p1, p2, p3, p4};

        Vector3 normalAvg = Vector3.zero;
        int avgCount = 0;
        foreach(Vector3 point in projPoints)
        {
            RaycastHit hit;
            Vector3 direction = (point - cam.transform.position);
            if (Physics.Raycast(cam.transform.position, direction, out hit, maxAimDistance * 1.5f, hitLayer))
            {
                normalAvg += hit.normal;
                avgCount++;
            }
        }
        return -(normalAvg * (1f / avgCount));
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

        if(projector.gameObject.activeSelf && projPoints != null)
        {
            Gizmos.color = Color.red;
            foreach (Vector3 point in projPoints)
            {
                Gizmos.DrawSphere(point, 0.1f);
                Gizmos.DrawRay(Camera.main.transform.position, (point - Camera.main.transform.position).normalized * 100f);
            }
        }
    }
}
