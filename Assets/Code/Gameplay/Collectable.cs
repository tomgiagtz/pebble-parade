using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Collectable : MonoBehaviour {
    
    public UnityEvent onCollect;

    private void OnTriggerEnter(Collider other) {   
        Debug.Log("collect!");
        if (other.gameObject.CompareTag("Player")) {
            Destroy(gameObject);
            onCollect.Invoke();
        }
    }


}
