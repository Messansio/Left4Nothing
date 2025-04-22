using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    private PlayerMovement pmov;

    public bool isPlayerClimbing;

    private float climbingSpeedMultiplier;
    


    private void Start()
    {
        pmov = GetComponent<PlayerMovement>();
        climbingSpeedMultiplier = 8f;
    }

    
    private void Update()
    {
        if (isPlayerClimbing)//if touching a climb trigger
        {
            //disable normal movement to enable new climb movement
            pmov.isPlayerMoving = false;
            pmov.changeToClimbingMovement = true;

            if (Input.GetKeyDown(pmov.jumpKey))
            {
                //jump away from trigger;
                transform.GetComponent<Rigidbody>().AddForce(-transform.forward * pmov.jumpForce, ForceMode.Impulse);
            }
            if (Input.GetKeyDown(pmov.crouchKey))
            {
                //lock player to its current position and now can pursue shooting
            }

            if(pmov.verticalInput > 0 && pmov.horizontalInput == 0)
            {
                Debug.Log("Moving Up");
                transform.GetComponent<Rigidbody>().AddForce(Vector3.up * climbingSpeedMultiplier, ForceMode.Acceleration);
            }
            if(pmov.verticalInput < 0 && pmov.horizontalInput == 0)
            {
                Debug.Log("Moving Down");
                transform.GetComponent<Rigidbody>().AddForce(Vector3.down * climbingSpeedMultiplier, ForceMode.Acceleration);
            }
            /*
            if(pmov.verticalInput == 0 && pmov.horizontalInput > 0)
            {
                Debug.Log("Moving Right");
                transform.GetComponent<Rigidbody>().AddForce(Vector3.right * climbingSpeedMultiplier, ForceMode.Acceleration);
            }
            if (pmov.verticalInput == 0 && pmov.horizontalInput < 0)
            {
                Debug.Log("Moving Left");
                transform.GetComponent<Rigidbody>().AddForce(Vector3.left * climbingSpeedMultiplier, ForceMode.Acceleration);
            }*/
        }
        
    }


}
