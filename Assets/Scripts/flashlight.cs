using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flashlight : MonoBehaviour
{
    [Header("Variables")]
    public GameObject playerFlashlight;
    private bool isFlashlightOn;
    public bool enableFlashlightDebugLogs = true;

    [Header("Keybind")]
    public KeyCode flashlightKey = KeyCode.F;

    // Start is called before the first frame update
    void Start()
    {
        isFlashlightOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        // Flashlight toggle
        if (Input.GetKeyDown(flashlightKey))
        {
            isFlashlightOn = !isFlashlightOn;
            playerFlashlight.SetActive(isFlashlightOn);

            if (enableFlashlightDebugLogs)
                Debug.Log(isFlashlightOn ? "Flashlight ON" : "Flashlight OFF");
        }
    }
}
