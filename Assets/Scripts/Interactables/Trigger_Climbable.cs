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
            //other.transform.position = new Vector3(this.transform.position.x, other.transform.position.y, this.transform.position.z);
            //other.GetComponent<PlayerMovement>().SetIsGrounded(true);
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
            Debug.Log("jump away from climbable trigger");
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            other.GetComponent<Rigidbody>().useGravity = true;
            pclimb.isPlayerClimbing = false;
            

        }
    }
}
