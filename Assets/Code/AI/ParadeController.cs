using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeController : MonoBehaviour {

    public Transform leader;
    public float targetUpdateInterval = 1f;
    
    public List<Mesh> possibleMeshes = new List<Mesh>();
    private List<Mesh> usedMeshes = new List<Mesh>();
    
    private List<ParadeMember> members = new List<ParadeMember>();
    private Vector3 targetPosition = Vector3.zero;
    private Vector3 prevTargetPosition = Vector3.zero;
    
    public ParadeControllerDebug debug;
    
    private void Start() {
        InvokeRepeating("UpdateMemberPositions", 0, targetUpdateInterval);
    }

    private void FixedUpdate() {
        
    }

    public void AddMember(GameObject member) {
        if (!member.CompareTag("ParadeMember")) return;

        Mesh newMesh = GetRandomMesh();
        
        
        // Debug.Log("Adding member");
        ParadeMember newParadeMember = member.GetComponent<ParadeMember>();
        members.Add(newParadeMember);
        newParadeMember.Init(newMesh);
        member.transform.SetParent(this.transform);
    }

    private Mesh GetRandomMesh() {
        if (possibleMeshes.Count == 0) {
            possibleMeshes.AddRange(usedMeshes.ToArray());
            usedMeshes.Clear();
            
        }
        
        //get a new mesh
        int newMeshIndex = UnityEngine.Random.Range(0, possibleMeshes.Count);
        Mesh newMesh = possibleMeshes[newMeshIndex];
        // add it to used meshes, remove it from possible meshes
        usedMeshes.Add(newMesh);
        possibleMeshes.RemoveAt(newMeshIndex);

        return newMesh;
    }

    private void UpdateMemberPositions() {
        prevTargetPosition = targetPosition;
        targetPosition = leader.position;
        
        // stop updating positions when player not moving
        if (Vector3.Distance(prevTargetPosition, targetPosition) < 1f)  return;
        
        for (int i = 0; i < members.Count; i++) {
            Vector3 randomDirection = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
            Vector3 randomPosition = targetPosition + randomDirection * UnityEngine.Random.Range(2f, 5f);
            
            members[i].SetTarget(randomPosition);
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
