using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Climbable : MonoBehaviour
{
    private PlayerClimbing pclimb;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("climbable trigger");

            pclimb = other.GetComponent<PlayerClimbing>();
            pclimb.isPlayerClimbing = true;
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
            other.GetComponent<Rigidbody>().useGravity = false;
        }
    }

    
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            pclimb.isPlayerClimbing = false;
            Debug.Log("player exited the climbable trigger");
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
            other.GetComponent<Rigidbody>().useGravity = true;
            
        }
    }
}
