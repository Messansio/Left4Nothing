using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;
    public playerSpawner pSpawner;

    // Update is called once per frame
    void Update()
    {
        if (pSpawner.usePlayerController)
            cameraPosition = GameObject.Find("PlayerController").transform.Find("CameraPos").transform;
        transform.position = cameraPosition.position;
    }
}
