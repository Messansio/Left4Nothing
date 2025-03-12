using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerMovement : MonoBehaviour
{

    [Header("Player Stuff")]
    public CapsuleCollider playerCollision;
    public GameObject playerFlashlight;

    [Header("Movement")]
    public float moveSpeed;

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
    bool readyToJump;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    // Start is call\ed before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        isCrouching = false;
        isFlashlightOn = false;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();

        if(grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKeyDown(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        //FLASHLIGHT

        if (Input.GetKeyDown(flashlightKey))
        {
            if (!isFlashlightOn)
            {
                playerFlashlight.SetActive(true);
                isFlashlightOn = true;
                Debug.Log("Flashlight ON");
            }
            /*if (isFlashlightOn)
            {
                playerFlashlight.SetActive(false);
                isFlashlightOn = false;
                Debug.Log("Flashlight OFF");
            }*/
        }


        //CROUCHING MECHANIC

        if (!isCrouching)
        {
            if (Input.GetKeyDown(crouchKey))
            {
                playerCollision.height = playerCollision.height / 2;
                
                isCrouching = true;

                moveSpeed = moveSpeed / 2;
            }
        }

        if (isCrouching)
        {
            if (Input.GetKeyUp(crouchKey))
            {
                playerCollision.height = playerCollision.height * 2;

                isCrouching = false;

                moveSpeed = moveSpeed * 2;
            }
        }

        playerHeight = playerCollision.height;

    }

        

    private void MovePlayer()
    {
        
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}
