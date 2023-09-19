using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeController : MonoBehaviour {
    
    public Transform leader;

    private void Update() {
        transform.position = leader.position;
    }
    
    public void AddMember(Transform member) {
        member.transform.parent = transform;
    }
}
