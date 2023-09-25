using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeController : MonoBehaviour {

    public Transform leader;
    public float targetUpdateInterval = 1f;
    
    private List<ParadeMember> members = new List<ParadeMember>();
    private Vector3 targetPosition = Vector3.zero;
    
    public ParadeControllerDebug debug;
    
    private void Start() {
        InvokeRepeating("UpdateMemberPositions", 0, targetUpdateInterval);
    }

    private void FixedUpdate() {
        targetPosition = leader.position;
    }

    public void AddMember(GameObject member) {
        if (!member.CompareTag("ParadeMember")) return;
        
        Debug.Log("Adding member");
        ParadeMember newParadeMember = member.GetComponent<ParadeMember>();
        members.Add(newParadeMember);
        newParadeMember.Init();
        member.transform.SetParent(this.transform);
    }
    
    private void UpdateMemberPositions() {
        for (int i = 0; i < members.Count; i++) {
            members[i].SetTarget(targetPosition);
        }
    }
    
    
    #region Debug
    private void OnDrawGizmos() {
        if (!debug.debugMode) return;
        
        if (debug.showParadeTarget) {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(targetPosition, 0.75f);
        }
    }
    #endregion
    
    
}
[Serializable]
public struct ParadeControllerDebug {
    public bool debugMode;
    public bool showParadeTarget;
}
