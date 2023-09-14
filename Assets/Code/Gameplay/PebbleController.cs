using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PebbleController : MonoBehaviour {
    public PebbleControllerDebug debug;

    [Header("Input")]
    // public InputActionAsset inputActions;
    public InputAction jumpAction;

    [Header("Movement")]
    public float torqueStrength = 50f;

    [Header("Jump")]
    public float groundedRadius = 2f;

    public float jumpHeight = 6f;
    private float timeToApex = 0.5f;
    private float gravity = -50f;
    private float fastFallGravity = -100f;
    private const float DEFAULT_GRAVITY = -50f;
    private float v0 = 0f;


    private bool isGrounded;
    private Rigidbody rb;
    private Vector3 torqueInput;

    private void OnEnable() {
        jumpAction.Enable();
    }

    private void OnDisable() {
        jumpAction.Disable();
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        jumpAction = new InputAction("JumpAction", binding: "<Keyboard>/space", interactions: "press(behavior=1)");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");


        // Add performed callback
        jumpAction.started += OnJump;
        jumpAction.canceled += OnReleaseJump;
    }

    private void FixedUpdate() {
        Vector3 torqueVector = Camera.main.transform.rotation * torqueInput;
        rb.AddTorque(torqueVector * torqueStrength, ForceMode.Force);
        CheckGrounded();

        if (isJumping) {
            timeSinceJump += Time.deltaTime;
            if (timeSinceJump >= timeToApex) {
                gravity = fastFallGravity;
            }
        }

        //gravity
        if (isGrounded) {
            gravity = DEFAULT_GRAVITY;
        }

        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);

        // Debug.Log("move mag: " + rb.velocity.magnitude);
    }


    public void OnMove(InputValue inputVec) {
        Vector2 vec = inputVec.Get<Vector2>();
        if (vec.magnitude == 0) {
            torqueInput = Vector3.zero;
            return;
        }

        // Vector3 forceVec = new Vector3(vec.x, 0, vec.y);
        // rb.AddForce(forceVec * 100, ForceMode.Acceleration);
        torqueInput = new Vector3(vec.y, 0, -vec.x); // This will rotate the object around its Y-axis.

        // torqueVector = Camera.main.transform.rotation * torque;
    }

    float timeSinceJump = 0f;
    private bool isJumping = false;

    private void OnJump(InputAction.CallbackContext context) {
        Debug.Log("yump");
        if (isGrounded) {
            isJumping = true;
            timeSinceJump = 0f;
            // rb.AddForce(Vector3.up * 100f, ForceMode.Impulse);
            UpdateJumpValues();
            rb.AddForce(Vector3.up * v0 * rb.mass, ForceMode.Impulse);
        }
    }

    private void OnReleaseJump(InputAction.CallbackContext context) {
        if (isGrounded && !(rb.velocity.y > 0)) return;

        gravity = fastFallGravity;
    }

    private void CheckGrounded() {
        // prevent early grounded checks just after jumping
        if (isJumping && timeSinceJump < 0.3f) {
            isGrounded = false;
            return;
        }

        timeSinceJump = 0;
        isJumping = false;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 2f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (debug.debugMode && debug.showGroundedRadius) Gizmos.DrawSphere(transform.position, groundedRadius);
    }

    private void UpdateJumpValues() {
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        v0 = 2.1f * jumpHeight / timeToApex;
    }
}


[Serializable]
public struct PebbleControllerDebug {
    public bool debugMode;
    public bool showGroundedRadius;
}