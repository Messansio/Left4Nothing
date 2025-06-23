/*
    PlayerMovement.cs
    -------------------------------------------------------
    This script handles movement-based functionalities of a player character:
    - Walking, crouching, and jumping
    - Applying physics-based forces
   
    - Detecting ground collisions
*/


using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

/*
    CLASS: PlayerMovement
    -------------------------------------------------------
    Inherits from MonoBehaviour; controls primary player input
    and movement mechanics. Also manages character states such 
    as isCrouching, and readyToJump.
*/
public class PlayerMovement : MonoBehaviour
{

    [Header("Important")]
    private playerSpawner pSpawner;

    [Header("Player Components")]
    public bool useBoxCollider;
    private BoxCollider boxCollider;
    public CapsuleCollider playerCollision;
    private CharacterController _controller;

    [Header("Movement Settings")]
    public float moveSpeed;
    public float runningSpeed;
    public float walkSpeed;  // New variable for walking speed
    public float crouchSpeed;
    public float groundDrag;
    public float jumpForce;
    public float airMultiplier;
    public float maxSlopeDegree = 40;
    public bool isFalling = false;

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

    public bool changeToClimbingMovement;
    private bool isCrouching;
    public bool isWalking;  // New variable to track walking state
    private bool grounded;
    public float horizontalInput;
    public float verticalInput;
    private Vector3 moveDirection;
    private Rigidbody rb;
    public bool isPlayerMoving;


    public bool GetIsCrouching() { return isCrouching; }
    public bool GetIsGrounded() { return grounded; }
    public bool SetIsGrounded(bool y) { return grounded = y; }


    #region RIGIDBODY CYCLE
    private void RigidBodyStarter()
    {
        // Initialize movement speeds
        runningSpeed = moveSpeed;
        walkSpeed = moveSpeed * 0.6f;  // Set walking speed to 60% of running speed
        crouchSpeed = moveSpeed * crouchSpeedMultiplier;

        if (useBoxCollider)
        {
            boxCollider = transform.Find("PlayerCollisionBox").GetComponent<BoxCollider>();
            boxCollider.gameObject.SetActive(true);
            playerCollision.enabled = false;
        }

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;



        isCrouching = false;
        isWalking = false;  // Initialize walking state
        isPlayerMoving = false;
    }
    private void RigidBodyUpdate()
    {
        // Get movement input
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (!changeToClimbingMovement)  //if not climbing player has those movement inputs and methods
        {

            CheckIfGrounded();
            HandleInput();

            hasContactWithFloor = false;
            hasContactWithWall = false;

            //Apply exceptions
            PhysicsAndTechExceptions();

            rb.drag = grounded ? groundDrag : 0;

        }
    }
    private void RigidBodyFixedUpdate()
    {
        // Jump logic: check for key press and ground contact
        if (Input.GetKey(jumpKey) && !isFalling)
        {

            Jump();

        }
        ControlSpeed();
        MovePlayer();
    }

    #endregion


    #region PLAYERCONTROLLER CYCLE

    private void PlayerControllerStarter()
    {
        
    }
    private void PlayerControllerUpdate()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        
        //_controller.SimpleMove(moveSpeed);
    }
    #endregion


    private void Start()
    {
        
        pSpawner = GameObject.Find("game_director").GetComponent<playerSpawner>();

        if (!pSpawner.usePlayerController)
        {
            RigidBodyStarter();
        }
        else
        {
            _controller = GetComponent<CharacterController>();
        }
    }
    private void Update()
    {
        if (!pSpawner.usePlayerController)
        {
            RigidBodyUpdate();
        }
        else
        {
            PlayerControllerUpdate();
        }
    }
    private void FixedUpdate()
    {
        if (!pSpawner.usePlayerController)
        {
            RigidBodyFixedUpdate();
        }
        else
        {

        }
    }


    /// SCRIPT REGION IF RIGIDBODY PLAYER
    // 
    #region RIGIDBODY CODE

    #region ContactPoints and Collisions

    public List<ContactPoint> contactPoints = new List<ContactPoint>();
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Entered " + collision);
        contactPoints.AddRange(collision.contacts);
    }
    private void OnCollisionStay(Collision collision)
    {

        contactPoints.AddRange(collision.contacts);
    }
    private void OnCollisionExit(Collision collision)
    {
        //Debug.Log("Exited " + collision);

        contactPoints.Clear();
        //isOnWalkableSlope = false;
        //hasContactWithFloor = false;
        //hasContactWithWall = false;

    }
    #endregion


    public GameObject debugBox;
    bool isTouchingBothWallAndFloor;
    bool hasContactWithFloor;
    bool hasContactWithWall;
    ContactPoint CPFloor = default;
    ContactPoint CPWall = default;
    

    private void GroundBoxCast(Transform playerRoot)
    {
        float fixedBoundZ = boxCollider.bounds.extents.z - 0.1f;
        float fixedBoundX = boxCollider.bounds.extents.x - 0.1f;
        Vector3 boxExtents = new Vector3(fixedBoundX, 0.1f, fixedBoundZ);
        //Vector3 halfExtents = boxCollider.bounds.extents;

        Physics.BoxCast(playerRoot.position + Vector3.up, boxExtents, Vector3.down, out RaycastHit boxHit, Quaternion.identity, math.INFINITY);
        //debugBox.transform.localScale = boxExtents;
        //GameObject.Instantiate(debugBox, boxHit.point, Quaternion.identity);


        double rootDistance = 1.0f;

        if (boxHit.distance > rootDistance)
        {
            isFalling = true;
            hasContactWithFloor = false;
        }
        else
            isFalling = false;

        //Debug.Log("boxHit Distance: " + boxHit.distance);

    }


    private void CheckIfGrounded()
    {

        float CPAngle = 0;

        if (contactPoints.Count == 0)
        {
            isOnWalkableSlope = false;
            rb.useGravity = true;
            grounded = false;
            hasContactWithWall = false;
            hasContactWithFloor = false;
            isTouchingBothWallAndFloor = false;
            CPAngle = 0;
            currentSlopeAngle = 0;
            isFalling = true;
            return;
        }

        hasContactWithFloor = false;
        hasContactWithWall = false;

        Transform playerRoot = transform.Find("PlayerRoot");
        
        
        

        if (contactPoints.Count > 0)
        {
            foreach (ContactPoint contact in contactPoints)
            {

                CPAngle = Vector3.Angle(contact.normal, Vector3.up);
                

                if (CPAngle >= 0 && CPAngle <= maxSlopeDegree)
                {
                    CPFloor = contact;
                    
                    hasContactWithFloor = true;

                }
                //condizione per sapere se l'oggetto è un muro
                else if (CPAngle > maxSlopeDegree)
                {
                    CPWall = contact;
                    hasContactWithWall = true;
                }

                
            }

        }

        GroundBoxCast(playerRoot);

        isTouchingBothWallAndFloor = hasContactWithFloor && hasContactWithWall;
        /*
        Transform debugObjects_Transform = GameObject.Find("DEBUG_Objects").transform;
        int debugCubeCount = debugObjects_Transform.childCount;

        if (debugCubeCount > 5 || contactPoints.Count == 0)
            Destroy(debugObjects_Transform.GetChild(0).gameObject);
        else
        {
            GameObject.Instantiate(debugBox, CPFloor.point, Quaternion.identity, debugObjects_Transform);
            GameObject.Instantiate(debugBox, CPWall.point, Quaternion.identity, debugObjects_Transform);
        }
        */

        
        //Debug.Log("Last ContactPoint Angle: " + CPAngle);
        //Debug.Log("Angle between CPFloor and Vector3.up: " + Vector3.Angle(CPFloor.normal, Vector3.up));
        //Debug.Log("Angle between CPWall and Vector3.up: " + Vector3.Angle(CPWall.normal, Vector3.up));



        ResolveContactsForGroundedCheck();

    }


    
    void ResolveContactsForGroundedCheck() 
    {

        
        if (hasContactWithFloor)
        {
            grounded = true;
            return;
        }
        if (hasContactWithWall)
        {
            grounded = false;
            return;
        }
        if (isTouchingBothWallAndFloor)
        {
            grounded = true;
            return;
        }
    }




    private float currentSlopeAngle;
    public bool isOnWalkableSlope = false;
    private RaycastHit groundCollision;
    ContactPoint CP = default;


    private void IsPlayerOnSlope()      
    {

        foreach (ContactPoint contact in contactPoints)
        {
            CP = contact;
        }
        currentSlopeAngle = Vector3.Angle(Vector3.up, CP.normal);
        //Debug.Log(currentSlopeAngle);

        if (currentSlopeAngle > 0 && currentSlopeAngle <= maxSlopeDegree)
            isOnWalkableSlope = true;
        else
            isOnWalkableSlope = false;
    }



    private void PhysicsAndTechExceptions()
    {
        if (isOnWalkableSlope)
            rb.useGravity = false;
        else
        {
            if (!isTouchingBothWallAndFloor)
            {
                rb.useGravity = true;
            }

        }
    }



    private void HandleInput()
    {
        

        #region Check for Animation Input

        if (horizontalInput != 0 || verticalInput != 0)
            isPlayerMoving = true;
        else
            isPlayerMoving = false;

        #endregion

        
        

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
        CrouchMethod();

        
    }

    /*
        Moves the player along X-Z plane based on input direction
        and applies a force relative to whether the player is in-air.
    */

    private void CrouchMethod()
    {
        Transform playerRoot = transform.Find("PlayerRoot");

        if (!isCrouching && Input.GetKeyDown(crouchKey))
        {
            if (useBoxCollider)
                boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y * crouchHeightMultiplier, boxCollider.size.z);

            playerRoot.position = new Vector3(playerRoot.position.x, playerRoot.position.y + 0.5f, playerRoot.position.z);


            isCrouching = true;
            moveSpeed = crouchSpeed;
            isWalking = false;  // Cancel walking state when crouching
        }
        else if (isCrouching && Input.GetKeyUp(crouchKey))
        {
            if (useBoxCollider)
                boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y / crouchHeightMultiplier, boxCollider.size.z);
                
            playerRoot.position = new Vector3(playerRoot.position.x, playerRoot.position.y - 0.5f, playerRoot.position.z);

            isCrouching = false;
            moveSpeed = isWalking ? walkSpeed : runningSpeed;  // Restore appropriate speed
        }

        // Update stored playerHeight using the current collision height
        if (useBoxCollider)
            playerHeight = boxCollider.size.y;
    }

    private void MovePlayer()
    {
        IsPlayerOnSlope();

        if (isOnWalkableSlope)
        {
            moveDirection = Vector3.Cross(CP.normal, -orientation.right * verticalInput) + Vector3.Cross(CP.normal, orientation.forward * horizontalInput);
            //moveDirection = Vector3.Cross(groundCollision.normal, -orientation.right * verticalInput) + Vector3.Cross(groundCollision.normal, orientation.forward * horizontalInput);
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
        rb.useGravity = true;
        grounded = false;
        isTouchingBothWallAndFloor = false;
        isOnWalkableSlope = false;
    }

    #endregion


    /// SCRIPT REGION IF PLAYERCONTROLLER PLAYER
    //
    #region PLAYERCONTROLLER CODE

    public float gravity = -9.81f;
    private float gravityForce;
    private void AddGravity()
    {
        //_controller.velocity.y += gravity * Time.fixedDeltaTime;
        
    }

    #endregion
}
