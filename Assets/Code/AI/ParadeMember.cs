using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParadeMember : MonoBehaviour {
    
    NavMeshAgent agent;
    Rigidbody rb;
    
    Vector3 currentTarget;
    private Vector3 currDirection;
    ParadeMemberState state = ParadeMemberState.Moving;
    
    
    public ParadeMemberDebug debug;
    private void Start() {
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        
        agent.updatePosition = false;
        agent.updateRotation = false;
    }

    private void FixedUpdate() {
        // if (state == ParadeMemberState.Moving) {
        //     rb.AddTorque(transform.right * 2500f, ForceMode.Force);
        // }
        // agent.transform.position = rb.position;
        // if (!agent.isOnNavMesh) {
        //     // agent.Warp(agent.nextPosition);
        //     Debug.LogWarning("Agent not on navmesh");
        // }
        // agent.velocity = rb.velocity;
        // if (agent.hasPath) {
        //     // Get the next position on the path
        //     Vector3 nextPosition = agent.steeringTarget;
        //
        //     // Calculate the direction to the next position
        //     Vector3 moveDirection = (nextPosition - transform.position).normalized;
        //     currDirection = moveDirection;
        //     currDirection.y = 0f;
        //     currDirection = currDirection.normalized;
        //     
        //     
        //     rb.AddTorque(moveDirection * 10f, ForceMode.Impulse);
        //     // Move the Rigidbody towards the next position
        //     // rb.AddTorque(transform.right * Quaternion., ForceMode.Force);
        //      agent.Warp(rb.position);
        // } else {
        //     // rb.position = agent.nextPosition;
        // }
        
        if (state == ParadeMemberState.Moving) {
               rb.AddTorque(transform.right * 2500f, ForceMode.Force);
        }
    }

    public void Init() {
        
    }

    private void OnDrawGizmos() {
        if (!debug.debugMode) return;
        
        if (debug.showMemberTarget) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(currentTarget, 0.75f);
        }
        
        if (debug.showForwardVector) {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + currDirection * 2f);
        }
    }

    public void SetTarget(Vector3 targetPosition) {
        currentTarget = targetPosition;
        if (!agent.SetDestination(currentTarget)) {
            Debug.LogWarning("Failed to set destination");
        }
    }
}

enum ParadeMemberState {
    Idle,
    Moving,
}

[Serializable]
public struct ParadeMemberDebug {
    public bool debugMode;
    public bool showMemberTarget;
    public bool showForwardVector;
}