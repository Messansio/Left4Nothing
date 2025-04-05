using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{

    public bool hasPlayerInteracted = false;
    public bool isDoorOpen = false;
    private float rotSpeedPerSecond = 3f;

    private Vector3 defRotation;
    private Vector3 maxRotation;
    private Vector3 inaccuracy;

    public AudioClip door_open;
    public AudioClip door_close;


    private void Awake()
    {
        defRotation = transform.rotation.eulerAngles;
        maxRotation = new Vector3(0, 90, 0);
        inaccuracy = new Vector3(0, 4, 0);
        
    }

    public void PlayOpenDoorSound()
    {
        gameObject.GetComponentInChildren<AudioSource>().clip = door_open;
        gameObject.GetComponentInChildren<AudioSource>().Play();
    }

    private void PlayCloseDoorSound()
    {
        gameObject.GetComponentInChildren<AudioSource>().PlayOneShot(door_close);
    }

    private void FixedUpdate()
    {

        if (hasPlayerInteracted)
        {
            
            if (isDoorOpen)
                Close();
            else
                Open();
            
        }

    }

    private void Open()
    {
        if(transform.rotation.eulerAngles.y < ((defRotation.y + maxRotation.y - inaccuracy.y)))
        {
            transform.Rotate(maxRotation * rotSpeedPerSecond * Time.fixedDeltaTime);
        }
        else
        {
            hasPlayerInteracted = false;
            isDoorOpen = true;
        }
        
    }

    private void Close()
    {
        if (transform.rotation.eulerAngles.y > (defRotation.y + inaccuracy.y))
            transform.Rotate(rotSpeedPerSecond * Time.fixedDeltaTime * -maxRotation);
        else
        {
            //PlayCloseDoorSound();
            hasPlayerInteracted = false;
            isDoorOpen = false;
        }
    }
}
