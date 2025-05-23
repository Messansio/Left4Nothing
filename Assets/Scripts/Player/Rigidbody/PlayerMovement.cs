/*
    PlayerMovement.cs
    -------------------------------------------------------
    This script handles movement-based functionalities of a player character:
    - Walking, crouching, and jumping
    - Applying physics-based forces
   
    - Detecting ground collisions
*/

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Burst.CompilerServices;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;
using UnityEngine.Timeline;
using UnityEngine.UI;

/*
    CLASS: PlayerMovement
    -------------------------------------------------------
    Inherits from MonoBehaviour; controls primary player input
    and movement mechanics. Also manages character states such 
    as isCrouching, and readyToJump.
*/
public class PlayerMovement : MonoBehaviour
{
    #region Inspector Variables

    [Header("Player Components")]
    public bool useBoxCollider;
    private BoxCollider boxCollider;
    public CapsuleCollider playerCollision;
    
    [Header("Movement Settings")]
    public float moveSpeed;
    public float runningSpeed;
    public float walkSpeed;  // New variable for walking speed
    public float crouchSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public float maxSlopeDegree = 40;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode walkKey = KeyCode.LeftShift;  // New keybind for walking
    
    [Header("Ground & Crouch Settings")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public float crouchHeightMultiplier = 0.5f;
    public float crouchSpeedMultiplier = 0.5f;

    [Header("Orientation")]
    public Transform orientation;

    #endregion

    #region Variables

    public bool changeToClimbingMovement;
    private bool isCrouching;
    public bool isWalking;  // New variable to track walking state
    private bool readyToJump;
    private bool grounded;
    public float horizontalInput;
    public float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    public bool isPlayerMoving;


    public bool GetIsCrouching() { return isCrouching; }
    public bool GetIsGrounded() { return grounded; }
    public bool SetIsGrounded(bool y) { return grounded = y; }
    #endregion

    #region Unity Lifecycle

    public List<ContactPoint> contactPoints = new List<ContactPoint>();
    private void OnCollisionEnter(Collision collision)
    {
        contactPoints.AddRange(collision.contacts);
    }
    private void OnCollisionStay(Collision collision)
    {
        contactPoints.AddRange(collision.contacts);
    }

    /*
        Called once at startup. Assigns movement speeds, initializes
        crouched speed, sets up the Rigidbody, and resets player states.
    */
    private void Start()
    {
        // Initialize movement speeds
        runningSpeed = moveSpeed;
        walkSpeed = moveSpeed * 0.6f;  // Set walking speed to 60% of running speed
        crouchSpeed = moveSpeed * crouchSpeedMultiplier;

        boxCollider = transform.Find("PlayerCollisionBox").GetComponent<BoxCollider>();

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        
        //playerRoot = transform.Find("PlayerRoot");
        //halfBoxDim = new Vector3(0.5f, 0.1f, 0.5f);

        readyToJump = true;
        isCrouching = false;
        isWalking = false;  // Initialize walking state
        isPlayerMoving = false;
    }

    /*
        Called every frame. 
        - Updates grounded state using a Raycast.
        - Checks for input and adjusts movement speeds.
        - Applies drag only if grounded.
    */
    private void Update()
    {
        // Get movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (!changeToClimbingMovement)  //if not climbing player has those movement inputs and methods
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f); //removed  whatIsGround

            HandleInput();
            ControlSpeed();

            rb.drag = grounded ? groundDrag : 0;
        }
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

    

    #region Input & Movement

    /*
        Captures primary user inputs for horizontal/vertical movement,
        jumping, flashlight toggling, and crouching.
    */
    private void HandleInput()
    {
        

        #region Check for Animation Input

        if (horizontalInput != 0 || verticalInput != 0)
            isPlayerMoving = true;
        else
            isPlayerMoving = false;

        #endregion

        
        // Jump logic: checks for key press, readyToJump state, and ground contact
        if (Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {

            readyToJump = false;

            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Walk logic: reduce speed when shift is pressed
        if (!isCrouching)
        {
            if (Input.GetKeyDown(walkKey))
            {
                isWalking = true;
                moveSpeed = walkSpeed;
            }
            else if (Input.GetKeyUp(walkKey))
            {
                isWalking = false;
                moveSpeed = runningSpeed;
            }
        }

        // Crouch logic: modifies collider height and reduces movement speed
        if (!isCrouching && Input.GetKeyDown(crouchKey))
        {
            if (useBoxCollider)
                boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y * crouchHeightMultiplier, boxCollider.size.z);
            else
                playerCollision.height *= crouchHeightMultiplier;


            isCrouching = true;
            moveSpeed = crouchSpeed;
            isWalking = false;  // Cancel walking state when crouching
        }
        else if (isCrouching && Input.GetKeyUp(crouchKey))
        {
            if (useBoxCollider)
                boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y / crouchHeightMultiplier, boxCollider.size.z);
            else
                playerCollision.height /= crouchHeightMultiplier; ;


            isCrouching = false;
            moveSpeed = isWalking ? walkSpeed : runningSpeed;  // Restore appropriate speed
        }

        // Update stored playerHeight using the current collision height
        if (useBoxCollider)
            playerHeight = boxCollider.size.y;
        else
            playerHeight = playerCollision.height;

        
    }

    /*
        Moves the player along X-Z plane based on input direction
        and applies a force relative to whether the player is in-air.
    */
    private void MovePlayer()
    {
        IsPlayerOnSlope();

        if (isOnWalkableSlope)
        {
            moveDirection = Vector3.Cross(groundCollision.normal, -orientation.right * verticalInput) + Vector3.Cross(groundCollision.normal, orientation.forward * horizontalInput);
        }
        else
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        float force = moveSpeed * 10f * (grounded ? 1f : airMultiplier);
        rb.AddForce(moveDirection.normalized * force, ForceMode.Force);

        //Debug.Log("> Player Speed:  " + rb.velocity.magnitude.ConvertTo<float>().ToShortString(4));
    }

    /*
        Caps the player's horizontal velocity to the current moveSpeed,
        preventing exploits or uncontrolled acceleration.
    */
    private void ControlSpeed()
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
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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


    private float currentSlopeAngle;
    public bool isOnWalkableSlope = false;
    private RaycastHit groundCollision;
    private void IsPlayerOnSlope()      //ERRORI DA RISOLVERE QUIQUIQUIQ
    {
        

        Physics.Raycast(transform.position, Vector3.down, out groundCollision);
        currentSlopeAngle = Vector3.Angle(Vector3.up, groundCollision.normal);
        /*Debug.Log(groundCollision.collider);
        Debug.Log(currentSlopeAngle);
        Debug.Log(isOnWalkableSlope);*/
        if (currentSlopeAngle != 0 && currentSlopeAngle <= maxSlopeDegree)
            isOnWalkableSlope = true;
        else
            isOnWalkableSlope = false;
    }


}
