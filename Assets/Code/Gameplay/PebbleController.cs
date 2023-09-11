using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PebbleController : MonoBehaviour {
    private Rigidbody rb;
    private Vector3 torqueInput;
    private void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    public void OnMove(InputValue inputVec) {
        Vector2 vec = inputVec.Get<Vector2>();
        if (vec.magnitude == 0) {
            torqueInput = Vector3.zero;
            return;
        }
        Debug.Log("move: " + vec);
        // Vector3 forceVec = new Vector3(vec.x, 0, vec.y);
        // rb.AddForce(forceVec * 100, ForceMode.Acceleration);
        torqueInput = new Vector3(vec.y, 0, -vec.x); // This will rotate the object around its Y-axis.
        // torqueVector = Camera.main.transform.rotation * torque;
        
    }

    public void OnJump() {
        Debug.Log("yump");
    }

    private void FixedUpdate() {
        Vector3 torqueVector = Camera.main.transform.rotation * torqueInput;
        rb.AddTorque(torqueVector * 100, ForceMode.Force);
        
        //gravity
        rb.AddForce(Vector3.down * 50, ForceMode.Acceleration);
    }
}
