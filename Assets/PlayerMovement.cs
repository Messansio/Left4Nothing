using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    [Header("Player Stuff")]
    public CapsuleCollider playerCollision;
    public GameObject playerFlashlight;

    [Header("Movement")]
    public float moveSpeed;
    public float runningSpeed;
    public float crouchSpeed;

    private bool isCrouching;
    private bool isFlashlightOn;

    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode flashlightKey = KeyCode.F;
    private bool readyToJump;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    [Header("Tweaks")]
    public float crouchHeightMultiplier = 0.5f;
    public float crouchSpeedMultiplier = 0.5f;
    public bool enableFlashlightDebugLogs = true;

    public Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        // Initialize movement speeds
        runningSpeed = moveSpeed;
        // Replace hard-coded values with multipliers
        crouchSpeed = moveSpeed * crouchSpeedMultiplier;

        // Setup rigidbody
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        // Initialize states
        readyToJump = true;
        isCrouching = false;
        isFlashlightOn = false;
    }

    private void Update()
    {
        // Check if player is grounded
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        // Handle input and speed
        MyInput();
        SpeedControl();

        // Apply drag when grounded
        rb.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    #endregion

    #region Input Handling
    private void MyInput()
    {
        // Get movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Handle jumping
        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Handle flashlight toggle
        if (Input.GetKeyDown(flashlightKey))
        {
            isFlashlightOn = !isFlashlightOn;
            playerFlashlight.SetActive(isFlashlightOn);

            if(enableFlashlightDebugLogs)
                Debug.Log(isFlashlightOn ? "Flashlight ON" : "Flashlight OFF");
        }

        // Handle crouching
        if (!isCrouching && Input.GetKeyDown(crouchKey))
        {
            playerCollision.height *= crouchHeightMultiplier;
            isCrouching = true;
            moveSpeed = crouchSpeed;
        }

        if (isCrouching && Input.GetKeyUp(crouchKey))
        {
            playerCollision.height /= crouchHeightMultiplier;
            isCrouching = false;
            moveSpeed = runningSpeed;
        }

        // Update player height based on collider
        playerHeight = playerCollision.height;
    }
    #endregion

    #region Movement Methods
    private void MovePlayer()
    {
        // Calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // Apply forces based on ground state
        float force = moveSpeed * 10f * (grounded ? 1f : airMultiplier);
        rb.AddForce(moveDirection.normalized * force, ForceMode.Force);
    }

    private void SpeedControl()
    {
        // Limit horizontal velocity
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // Reset vertical velocity and apply jump force
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    #endregion
}
