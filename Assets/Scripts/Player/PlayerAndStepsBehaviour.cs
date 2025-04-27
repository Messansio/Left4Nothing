using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;

public class PlayerAndStepsBehaviour : MonoBehaviour
{

    public float maxStepHeight;

    private void Update()
    {
        if (IsNextToStep())
        {
            StepOnSurface();
        }
    }

    public bool enablePASBDebug;
    public GameObject cubeRaycastPointDebugAsset;
    private Transform debugObjects_Transform;

    private void Start()
    {
        debugObjects_Transform = GameObject.Find("DEBUG_Objects").transform;
    }

    private bool IsNextToStep()
    {
        bool getIfNextToStep = false;

        int debugCubeCount = debugObjects_Transform.childCount;

        #region Different Raycastings

        RaycastHit hitInfo;
        bool frwCast = Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hitInfo, 1f, LayerMask.NameToLayer("whatIsGround"));
        
        #endregion

        

        if (frwCast && CheckInverseNormalAngle(hitInfo))
        {


            if (enablePASBDebug)
            {
                Debug.Log(hitInfo.collider);
                //Debug.Log(hitInfo.colliderInstanceID);

                Instantiate(cubeRaycastPointDebugAsset, hitInfo.point, Quaternion.identity, debugObjects_Transform);
                
            }


            getIfNextToStep = true;
        }
    
        if (enablePASBDebug && debugCubeCount > 1)
            Destroy(debugObjects_Transform.GetChild(0).gameObject);


        return getIfNextToStep;
    }

    private void StepOnSurface()
    {
        //Player Body Gets Moved Up the Step's Height
        //Debug.Log("steponsurface");
    }

    private bool CheckInverseNormalAngle(RaycastHit hit)
    {
        bool isAngleVertical = false;

        float angle = Vector3.Angle(Vector3.up, -hit.normal);

        
        if (angle <= 92 && angle >= -92)
            isAngleVertical = true;

        if (enablePASBDebug)
        {
            //Debug.Log(angle);
            //Debug.Log(isAngleVertical);
        }

        return isAngleVertical;
    }

}
