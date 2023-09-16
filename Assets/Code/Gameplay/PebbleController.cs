using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PebbleController : MonoBehaviour {
    // Constants
    private const float DEFAULT_GRAVITY = -30f;
    private const int MAX_CONTACTS = 5;


    public Vector2 moveInput;

    [Header("Movement")]
    public float torqueStrength = 50f;

    public float strafeStrength = 5f;

    [Range(0, 180)]
    public float maxSlopeAngle = 20f;

    private float maxSlopeCos;

    [Header("Jump")]
    public float jumpHeight = 6f;

    private float timeToApex = 0.5f;
    private float gravity = -30f;
    private float fastFallGravity = -40f;
    private float v0 = 0f;


    private bool isGrounded;

    // collisions for grounded check
    private List<Vector3> contactLocations = new List<Vector3>();
    private List<ContactPoint> contactPoints = new List<ContactPoint>();

    // components
    [Header("Input")]
    // public InputActionAsset inputActions;
    public InputAction jumpAction;

    private Rigidbody rb;

    [Space(10)]
    public PebbleControllerDebug debug;

    private void OnEnable() {
        jumpAction.Enable();
    }

    private void OnDisable() {
        jumpAction.Disable();
    }

    private void OnValidate() {
        maxSlopeCos = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
        UpdateJumpValues();
    }

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        jumpAction = new InputAction("JumpAction", binding: "<Keyboard>/space", interactions: "press(behavior=1)");
        jumpAction.AddBinding("<Gamepad>/buttonSouth");

        // Add performed callback
        jumpAction.started += OnJump;
        jumpAction.canceled += OnReleaseJump;

        UpdateJumpValues();
        maxSlopeCos = Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad);
    }

    Vector3 transformedInput = Vector3.zero;
    Vector3 strafeDirection = Vector3.zero;

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

        Quaternion cameraRotation = Camera.main.transform.rotation;
        Vector3 torqueInput = new Vector3(moveInput.y, 0, -moveInput.x);

        Vector3 torqueVector = cameraRotation * torqueInput;

        transformedInput = torqueVector;
        rb.AddTorque(torqueVector * torqueStrength, ForceMode.Force);

        //strafe forces
        if (!isGrounded) {
            Vector3 strafeInput = new Vector3(moveInput.x, 0, moveInput.y).normalized;

            strafeDirection = cameraRotation * (strafeInput);
            strafeDirection.y = 0;
            strafeDirection.Normalize();

            rb.AddForce(strafeDirection * strafeStrength, ForceMode.Acceleration);
        }
        // Debug.Log("move mag: " + rb.velocity.magnitude);
    }


    public void OnMove(InputValue _inputVec) {
        moveInput = _inputVec.Get<Vector2>();
    }

    float timeSinceJump = 0f;
    private bool isJumping = false;

    private void OnJump(InputAction.CallbackContext _context) {
        if (isGrounded) {
            isJumping = true;
            timeSinceJump = 0f;
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

        isGrounded = contactLocations.Count != 0;
    }


    private void UpdateJumpValues() {
        gravity = -2 * jumpHeight / (timeToApex * timeToApex);
        v0 = 2.1f * jumpHeight / timeToApex;
    }


    private void OnCollisionStay(Collision _other) {
        _other.GetContacts(contactPoints);

        //prevent too many collision points
        if (contactPoints.Count > MAX_CONTACTS) {
            contactPoints.RemoveRange(MAX_CONTACTS - 1, contactPoints.Count - MAX_CONTACTS);
        }


        // filter out points that are too steep
        List<ContactPoint> upwardFacingPoints = contactPoints.Where(_point => {
            float dot = Vector3.Dot(_point.normal.normalized, Vector3.up);
            if (dot < maxSlopeCos) {
                // Debug.Log("Angle too steep: " + dot + " vs " + maxSlopeCos);
            }

            return dot >= maxSlopeCos;
        }).ToList();

        // Debug.Log("Num valid points: " + upwardFacingPoints.Count);

        contactPoints = upwardFacingPoints;
        contactLocations = upwardFacingPoints.Select(_point => _point.point).ToList();
    }

    private void OnCollisionExit(Collision other) {
        contactPoints.Clear();
        contactLocations.Clear();
    }

    // DEBUG
    private void OnDrawGizmos() {
        if (!debug.debugMode) return;


        if (debug.showPrevContactPoint) {
            Gizmos.color = Color.red;
            contactLocations.ForEach(_point => Gizmos.DrawSphere(_point, 0.1f));
        }

        Vector3 position = transform.position;

        if (debug.showContactNormals) {
            Gizmos.color = Color.red;
            contactPoints.ForEach(_point => Gizmos.DrawLine(_point.point,
                _point.point + _point.normal * 3f));
        }

        if (debug.showInputVector) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + transformedInput * 3f);
        }

        if (debug.showStrafeForce) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(position, position + strafeDirection * 3f);
        }
    }
}


[Serializable]
public struct PebbleControllerDebug {
    public bool debugMode;
    public bool showPrevContactPoint;
    public bool showContactNormals;
    public bool showInputVector;
    public bool showStrafeForce;
}