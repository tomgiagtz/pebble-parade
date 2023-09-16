using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PebbleController : MonoBehaviour {
    public PebbleControllerDebug debug;

    [Header("Input")]
    // public InputActionAsset inputActions;
    public InputAction jumpAction;

    public Vector2 moveInput;

    [Header("Movement")]
    public float torqueStrength = 50f;

    public float strafeStrength = 5f;

    [Header("Jump")]
    public float groundedRadius = 2f;

    public float jumpHeight = 6f;
    private float timeToApex = 0.5f;
    private float gravity = -30f;
    private float fastFallGravity = -40f;
    private const float DEFAULT_GRAVITY = -30f;
    private float v0 = 0f;


    private bool isGrounded;
    private Rigidbody rb;

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

        Quaternion CameraRotation = Camera.main.transform.rotation;
        Vector3 torqueInput = new Vector3(moveInput.y, 0, -moveInput.x);
        Vector3 torqueVector = CameraRotation * torqueInput;
        rb.AddTorque(torqueVector * torqueStrength, ForceMode.Force);

        //strafe forces
        if (isGrounded) {
            Vector3 movementInput = new Vector3(moveInput.x, 0, moveInput.y);
            Vector3 movementVector = CameraRotation * movementInput;
            rb.AddTorque(movementVector * strafeStrength, ForceMode.Force);
        }
        // Debug.Log("move mag: " + rb.velocity.magnitude);
    }

    public void OnMove(InputAction.CallbackContext context) {
        moveInput = context.ReadValue<Vector2>();
    }
    // public void OnMove(InputValue _inputVec) {
    //     moveInput = _inputVec.Get<Vector2>();
    // }

    float timeSinceJump = 0f;
    private bool isJumping = false;

    private void OnJump(InputAction.CallbackContext _context) {
        Debug.Log("yump");
        if (isGrounded) {
            isJumping = true;
            timeSinceJump = 0f;
            // rb.AddForce(Vector3.up * 100f, ForceMode.Impulse);
            UpdateJumpValues();
            rb.AddForce(Vector3.up * v0 * rb.mass, ForceMode.Impulse);
        }
    }

    private void OnReleaseJump(InputAction.CallbackContext _context) {
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
        // isGrounded = Physics.Raycast(prevContactPoint + Vector3.up * (0.5f * groundedRadius), Vector3.down,
        //     groundedRadius);

        foreach (Vector3 point in contactPoints) {
            isGrounded = Physics.Raycast(point + transform.position, Vector3.down,
                groundedRadius);
            if (isGrounded) break;
        }
        // isGrounded = Physics.SphereCast(transform.position, groundedRadius,
        //     Vector3.down, out RaycastHit hit, groundedRadius * 0.5f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        if (debug.debugMode && debug.showGroundedRadius) Gizmos.DrawSphere(transform.position, groundedRadius);

        if (debug.debugMode && debug.showPrevContactPoint) {
            contactPoints.ForEach(_point => Gizmos.DrawSphere(transform.TransformPoint(_point), 0.1f));
        }
    }

    private void UpdateJumpValues() {
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        v0 = 2.1f * jumpHeight / timeToApex;
    }

    List<Vector3> contactPoints = new List<Vector3>();

    public void OnCollisionStay(Collision _other) {
        List<ContactPoint> contacts = new List<ContactPoint>();
        contacts.Clear();
        _other.GetContacts(contacts);
        contacts.Capacity = 6;

        Debug.Log("colliding with " + contacts.Count + " points");
        float maxSlopeAngle = 60f;
        float maxSlopeCos = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        contactPoints = contacts.Select(_point => transform.InverseTransformPoint(_point.point)).ToList();
    }
}


[Serializable]
public struct PebbleControllerDebug {
    public bool debugMode;
    public bool showGroundedRadius;
    public bool showPrevContactPoint;
}