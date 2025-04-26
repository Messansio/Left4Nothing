using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    private PlayerMovement pmov;
    private Rigidbody prb;

    public bool isPlayerClimbing;

    private float climbingSpeedMultiplier;
    


    private void Start()
    {
        prb = GetComponent<Rigidbody>();    
        pmov = GetComponent<PlayerMovement>();
        climbingSpeedMultiplier = 400f;
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
                
                //transform.GetComponent<Rigidbody>().AddForce(-transform.forward * pmov.jumpForce, ForceMode.Impulse);
            }
            if (Input.GetKeyDown(pmov.crouchKey))
            {
                //lock player to its current position and now can pursue shooting
            }

            if(pmov.verticalInput > 0 && pmov.horizontalInput == 0)
            {
                Debug.Log("Moving Up");
                prb.AddForce(Vector3.up * climbingSpeedMultiplier, ForceMode.Force);
            }
            if(pmov.verticalInput < 0 && pmov.horizontalInput == 0)
            {
                Debug.Log("Moving Down");
                prb.AddForce(Vector3.down * climbingSpeedMultiplier, ForceMode.Force);
            }

        }
        else
        {
            pmov.changeToClimbingMovement = false;
        }
        
    }


}
