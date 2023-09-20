using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParadeMember : MonoBehaviour {
    
    Rigidbody rb;
    MeshCollider coll;
    private void Start() {
        rb = GetComponent<Rigidbody>();
        coll = GetComponent<MeshCollider>();
        coll.isTrigger = true;
        rb.isKinematic = true;
    }

    public void Init() {
        Debug.Log("Init Parade Member");
        Destroy(gameObject.GetComponent<Collectable>());
        coll.isTrigger = false;
        rb.isKinematic = false;
    }
}
