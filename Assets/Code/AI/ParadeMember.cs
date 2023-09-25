using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParadeMember : MonoBehaviour {
    
    NavMeshAgent agent;
    Rigidbody rb;

    [Header("Follow")]
    public float minDistance = 2f;
    public float maxDistance = 20f;
    
    Vector3 currTarget;
    private Vector3 currDirection;
    ParadeMemberState state = ParadeMemberState.Moving;
    
    
    
    
    public ParadeMemberDebug debug;
    private void Start() {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        if (state == ParadeMemberState.Teleporting) {
            rb.velocity = Vector3.zero;
            rb.MovePosition(currTarget);
            rb.AddForce(Vector3.up * 30f, ForceMode.Force);
            state = ParadeMemberState.Moving;
        }
        
        float distanceFromTarget = Vector3.Distance(this.transform.position, currTarget);
        if (distanceFromTarget < minDistance) {
            state = ParadeMemberState.Idle;
        } else if (distanceFromTarget < maxDistance) {
            state = ParadeMemberState.Moving;
        } else {
            state = ParadeMemberState.Teleporting;
        }
        
        if (state == ParadeMemberState.Moving) {
               RotateTowardsTarget();
               rb.AddTorque(torque * 25000f, ForceMode.Force);
        }

        
    }
    Vector3 torque;
    private void RotateTowardsTarget() {
        currDirection = (currTarget - transform.position);
        currDirection.y = 0;
        currDirection.Normalize();

        torque = new Vector3(currDirection.z, 0, -currDirection.x);
    }

    public void Init(Mesh _mesh) {
        MeshCollider meshCollider = GetComponent<MeshCollider>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshCollider.sharedMesh = _mesh;
        meshFilter.mesh = _mesh;

        state = ParadeMemberState.Teleporting;
    }

    private void OnDrawGizmos() {
        if (!debug.debugMode) return;
        
        if (debug.showMemberTarget) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currTarget, 0.75f);
        }
        
        if (debug.showForwardVector) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + currDirection * 2f);
        }
        
        if (debug.showTorqueVector) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + torque * 2f);
        }
    }

    public void SetTarget(Vector3 targetPosition) {
        currTarget = targetPosition;
        // if (!agent.SetDestination(currTarget)) {
        //     Debug.LogWarning("Failed to set destination");
        // }
    }
}

enum ParadeMemberState {
    Idle,
    Moving,
    Teleporting
}

[Serializable]
public struct ParadeMemberDebug {
    public bool debugMode;
    public bool showMemberTarget;
    public bool showForwardVector;
    public bool showTorqueVector;
}