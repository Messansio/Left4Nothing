using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public float sensX;
    public float sensY;

    private PlayerMovement pMov;
    public Transform orientation;
    private playerSpawner pSpawner;

    float xRotation;
    float yRotation;
    // Start is called before the first frame update
    void Start()
    {
        pSpawner = GameObject.Find("game_director").GetComponent<playerSpawner>();

        if (AlphaCheckIfPlayerController())
        {
            AssignCamToPlayerController();
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pMov = orientation.parent.GetComponent<PlayerMovement>();
    }

    private bool AlphaCheckIfPlayerController()
    {

        return pSpawner.usePlayerController;

    }

    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensX;
        float mouseY = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensY;

        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);

        if (!AlphaCheckIfPlayerController())
        {

            if (pMov.isPlayerMoving)
                gameObject.GetComponent<Animator>().SetBool("IsOwningPlayerMoving", true);
            else
                gameObject.GetComponent<Animator>().SetBool("IsOwningPlayerMoving", false);
            if (pMov.GetIsCrouching() || pMov.isWalking)
                gameObject.GetComponent<Animator>().SetLayerWeight(1, 0);
            else
                gameObject.GetComponent<Animator>().SetLayerWeight(1, 1f);
            
        }
        

    }


    private void AssignCamToPlayerController()
    {
        orientation = GameObject.Find("PlayerController").transform.Find("Orientation").transform;
    }
}
