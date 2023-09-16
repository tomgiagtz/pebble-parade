using System;
using System.Collections.Generic;
using System.Linq;
using Tweens;
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
    public float torqueStrength = 250f;

    public float strafeStrength = 5f;

    [Header("Jump")]
    public float jumpHeight = 3f;

    [Range(0, 1)]
    public float timeToApex = 0.35f;

    [Range(0, 180)]
    public float maxSlopeAngle = 20f;

    public float coyoteTime = 0.1f;
    public EaseType strafeEaseType = EaseType.ExpoOut;

    private bool isGrounded;
    private float maxSlopeCos;

    private float gravity = -30f;
    private float fastFallGravity = -40f;
    private float jumpGravity = -20f;
    private float initialJumpVel = 0f;

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

    Vector3 cameraBasedInput = Vector3.zero;

    private void FixedUpdate() {
        CheckGrounded();

        if (isJumping) {
            timeSinceJump += Time.deltaTime;
            if (timeSinceJump >= timeToApex) {
                gravity = fastFallGravity;
            } else {
                gravity = jumpGravity;
            }
        }

        //gravity
        if (!isGrounded) {
            timeSinceGrounded += Time.deltaTime;
        }

        if (isGrounded) {
            gravity = DEFAULT_GRAVITY;
        }

        //gravity force
        rb.AddForce(Vector3.up * gravity, ForceMode.Acceleration);

        //torque forces
        Quaternion cameraRotation = Camera.main.transform.rotation;
        Vector3 torqueInput = new Vector3(moveInput.y, 0, -moveInput.x);
        Vector3 torqueVector = cameraRotation * torqueInput;

        rb.AddTorque(torqueVector * torqueStrength, ForceMode.Force);

        //calc strafe forces
        Vector3 strafeInput = new Vector3(moveInput.x, 0, moveInput.y).normalized;
        cameraBasedInput = cameraRotation * (strafeInput);
        cameraBasedInput.y = 0;
        cameraBasedInput.Normalize();

        //strafe forces
        if (!isGrounded) {
            rb.AddForce(cameraBasedInput * strafeMovementStrength, ForceMode.Acceleration);
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
        if (isGrounded) {
            isJumping = true;
            timeSinceJump = 0f;
            StartStrafeTween();
            // rb.AddForce(Vector3.up * initialJumpVel * rb.mass, ForceMode.Impulse);
            rb.velocity = new Vector3(rb.velocity.x, initialJumpVel, rb.velocity.z);
        }
    }

    private float strafeMovementStrength = 1f;

    // [Tooltip("Starts on jump, ends at end of flat ground jump")]
    // public AnimationCurve strafeCurve = AnimationCurve.Linear(0, 0, 1, 1);


    private void StartStrafeTween() {
        gameObject.CancelTweens();
        strafeMovementStrength = 0;
        FloatTween strafeTween = new FloatTween {
            from = 0,
            to = strafeStrength,
            duration = 2 * timeToApex,
            easeType = strafeEaseType,
            onUpdate = (_, value) => { strafeMovementStrength = value; }
        };
        gameObject.AddTween(strafeTween);
    }

    private void OnReleaseJump(InputAction.CallbackContext _context) {
        if (isGrounded && !(rb.velocity.y > 0)) return;
        gravity = fastFallGravity;
    }

    private float timeSinceGrounded = 0f;

    private void CheckGrounded() {
        // prevent early grounded checks just after jumping
        if (isJumping && timeSinceJump < 0.3f) {
            isGrounded = false;
            return;
        }

        if (timeSinceGrounded < coyoteTime) {
            isGrounded = true;
            return;
        }

        isJumping = false;
        isGrounded = contactLocations.Count != 0;

        // reset coyote time
        if (isGrounded) {
            timeSinceGrounded = 0f;
        }
    }


    private void UpdateJumpValues() {
        jumpGravity = -2 * jumpHeight / (timeToApex * timeToApex);
        initialJumpVel = 2f * jumpHeight / timeToApex;
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
            Gizmos.DrawLine(position, position + cameraBasedInput * 3f);
        }
    }
}


[Serializable]
public struct PebbleControllerDebug {
    public bool debugMode;
    public bool showPrevContactPoint;
    public bool showContactNormals;
    public bool showInputVector;
}