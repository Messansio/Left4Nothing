using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    public float defaultSpeed = 8f;
    public float jumpHeight = 2f;
    public float gravity = -2f;

    private float y_velocity;
    private Transform orientation;

    public KeyCode jumpKey = KeyCode.Space;

    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        y_velocity = controller.velocity.y;
        orientation = transform.Find("Orientation");
    }

    private void Update()
    {
        Move();
        //Jump();
        ApplyGravity();
    }

    private void Move()
    {
        Vector3 move = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
        move = Vector3.ClampMagnitude(move, 1f);
        move = (move * defaultSpeed) + (y_velocity * Vector3.up);
        controller.Move(move * Time.deltaTime);

    }

    private void Jump()
    {
        if (Input.GetKeyDown(jumpKey) && controller.isGrounded)
        {

            y_velocity = Mathf.Sqrt(jumpHeight * gravity);

        }
    }

    private void ApplyGravity()
    {
        y_velocity += gravity * Time.deltaTime;
    }

}
