using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeController : MonoBehaviour {

    public Transform leader;

    private void Update() {
        transform.position = leader.position;
    }

    public void AddMember(GameObject member) {
        if (!member.CompareTag("ParadeMember")) return;
        
        Debug.Log("Adding member");
        ParadeMember newParadeMember = member.GetComponent<ParadeMember>();
        newParadeMember.Init();
    }
}
