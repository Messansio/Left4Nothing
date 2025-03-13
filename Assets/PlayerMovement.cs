/*
    PlayerMovement.cs
    -------------------------------------------------------
    This script handles movement-based functionalities of a player character:
    - Walking, crouching, and jumping
    - Applying physics-based forces
    - Toggling the player's flashlight
    - Detecting ground collisions
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

/*
    CLASS: PlayerMovement
    -------------------------------------------------------
    Inherits from MonoBehaviour; controls primary player input
    and movement mechanics. Also manages character states such 
    as isCrouching, isFlashlightOn, and readyToJump.
*/
public class PlayerMovement : MonoBehaviour
{
    #region Variables

    /* 
       [HEADER: Player Components]
       -------------------------------------------------------
       - playerCollision is the BoxCollider or CapsuleCollider
         used for collision detection.
       - playerFlashlight references the player’s flashlight GameObject 
         that can be toggled on/off.
    */
    [Header("Player Stuff")]
    public CapsuleCollider playerCollision;
    public GameObject playerFlashlight;

    /*
        [HEADER: Movement]
        -------------------------------------------------------
        - moveSpeed/runningSpeed/crouchSpeed: controls overall
          walking, running, and crouch movement speed.
        - isCrouching tracks current crouch state.
        - isFlashlightOn tracks whether flashlight is active.
        - groundDrag, jumpForce, jumpCooldown, airMultiplier: 
          control physical properties influencing the player's
          movement behavior.
    */
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

    /*
        [HEADER: Keybinds]
        -------------------------------------------------------
        Defines default input keys for the player's jump, crouch,
        and flashlight toggle. readyToJump ensures there is a delay
        between jumps (controlled by jumpCooldown).
    */
    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode flashlightKey = KeyCode.F;
    private bool readyToJump;

    /*
        [HEADER: Ground Check]
        -------------------------------------------------------
        - playerHeight tracks the current height of the player's collider.
        - whatIsGround sets the LayerMask for valid ground collisions.
        - grounded indicates if the player is currently in contact
          with the ground.
    */
    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    private bool grounded;

    /*
        [HEADER: Tweaks]
        -------------------------------------------------------
        - crouchHeightMultiplier scales the collider height
          during a crouch.
        - crouchSpeedMultiplier lowers the movement speed
          while crouched.
        - enableFlashlightDebugLogs toggles debug messages.
    */
    [Header("Tweaks")]
    public float crouchHeightMultiplier = 0.5f;
    public float crouchSpeedMultiplier = 0.5f;
    public bool enableFlashlightDebugLogs = true;

    // Reference to the camera orientation, typically parented under the player
    public Transform orientation;

    // Private variables to store input values and references
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;

    #endregion

    #region Unity Lifecycle Methods

    /*
        Called once at startup. Assigns movement speeds, initializes
        crouched speed, sets up the Rigidbody, and resets player states.
    */
    private void Start()
    {
        runningSpeed = moveSpeed;
        crouchSpeed = moveSpeed * crouchSpeedMultiplier;

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        isCrouching = false;
        isFlashlightOn = false;
    }

    /*
        Called every frame. 
        - Updates grounded state using a Raycast.
        - Checks for input and adjusts movement speeds.
        - Applies drag only if grounded.
    */
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        rb.drag = grounded ? groundDrag : 0;
    }

    /*
        Called on a fixed interval. Updates player's movement
        by applying physics forces in MovePlayer().
    */
    private void FixedUpdate()
    {
        MovePlayer();
    }

    #endregion

    #region Input Handling

    /*
        Captures primary user inputs for horizontal/vertical movement,
        jumping, flashlight toggling, and crouching.
    */
    private void MyInput()
    {
        // Get movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump logic: checks for key press, readyToJump state, and ground contact
        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Flashlight toggle
        if (Input.GetKeyDown(flashlightKey))
        {
            isFlashlightOn = !isFlashlightOn;
            playerFlashlight.SetActive(isFlashlightOn);

            if(enableFlashlightDebugLogs)
                Debug.Log(isFlashlightOn ? "Flashlight ON" : "Flashlight OFF");
        }

        // Crouch logic: modifies collider height and reduces movement speed
        if (!isCrouching && Input.GetKeyDown(crouchKey))
        {
            playerCollision.height *= crouchHeightMultiplier;
            isCrouching = true;
            moveSpeed = crouchSpeed;
        }
        else if (isCrouching && Input.GetKeyUp(crouchKey))
        {
            playerCollision.height /= crouchHeightMultiplier;
            isCrouching = false;
            moveSpeed = runningSpeed;
        }

        // Update stored playerHeight using the current collision height
        playerHeight = playerCollision.height;
    }

    #endregion

    #region Movement Methods

    /*
        Moves the player along X-Z plane based on input direction
        and applies a force relative to whether the player is in-air.
    */
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        float force = moveSpeed * 10f * (grounded ? 1f : airMultiplier);
        rb.AddForce(moveDirection.normalized * force, ForceMode.Force);
    }

    /*
        Caps the player's horizontal velocity to the current moveSpeed,
        preventing exploits or uncontrolled acceleration.
    */
    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    /*
        Resets the player's vertical velocity and applies a jump impulse.
        This prevents the jump force from stacking if the player is
        already moving upward.
    */
    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    /*
        Delays the player's ability to jump again
        until jumpCooldown has passed.
    */
    private void ResetJump()
    {
        readyToJump = true;
    }

    #endregion
}
