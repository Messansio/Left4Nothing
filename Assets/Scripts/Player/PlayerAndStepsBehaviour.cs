using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class PlayerAndStepsBehaviour : MonoBehaviour
{

    private void Update()
    {
        if (IsNextToStep())
        {
            StepOnSurface();
        }
    }

    public bool enablePASBDebug;

    private bool IsNextToStep()
    {
        bool getIfNextToStep = false;

        RaycastHit hitInfo;

        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, 0.6f, LayerMask.NameToLayer("whatIsPlayer")))
        {
            if (enablePASBDebug)
            {
                Debug.Log(hitInfo.collider);
                Debug.Log(hitInfo.colliderInstanceID);
            }
            
            getIfNextToStep = true;
        }

        return getIfNextToStep;
    }

    private void StepOnSurface()
    {
        //Player Body Gets Moved Up the Step's Height
    }
}
