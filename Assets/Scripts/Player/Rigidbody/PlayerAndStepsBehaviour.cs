
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.ProBuilder;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UI;

public class PlayerAndStepsBehaviour : MonoBehaviour
{

    public float maxStepHeight;

    public bool enablePASBDebug;
    public GameObject cubeRaycastPointDebugAsset;
    private Transform debugObjects_Transform;
    public PlayerMovement plrmov;
    Rigidbody posBody;
    Vector3 velocity;
    Vector3 lastVelocity;
    public BoxCollider boxCol;
    private LayerMask boxCastIgnoreMask;

    private void Start()
    {
        posBody = GetComponentInParent<Rigidbody>();
        debugObjects_Transform = GameObject.Find("DEBUG_Objects").transform;
        
    }

    private bool CheckIfMoving()
    {
        bool isPlayerMoving = plrmov.isPlayerMoving;
        return isPlayerMoving;
    }

    private ContactPoint CP;
    private bool CheckIfStep()
    {
        bool isStep = false;
        /*
        foreach (ContactPoint contact in plrmov.contactPoints)
        {
            if (contact.normal != Vector3.up)
            {
                isStep = true;
                CP = contact;
            }
                
        }*/

        isStep = GetIfSteppable();


        if (plrmov.isOnWalkableSlope || !plrmov.GetIsGrounded())
        {
            isStep = false;
        }

        return isStep;
    }

    

    private void FixedUpdate()
    {
        RaycastFloorMatrix();

        //velocity = new(posBody.velocity.x, 0, posBody.velocity.z);
        if (CheckIfStep())
        {
            if (GetIfSteppable())
            {
                ForceGroundedCondition();
                StepOnSurface();
            }
        }
        //lastVelocity = velocity;
    }





    ///                    DEPRECATED

    /// Checks if Player can step on touched obstacle/step
    /// ( requires a ContactPoint )
    /// 
    /// \hitInfo        raycast to cast after stepTestDir
    /// \origin         lower position of the obstacle/step, based on player lower position and contact point
    /// \direction      ;no desc.
    /// \stepTestDir    ;no desc.
    /// 
    /// Returns true if Player can step on touched obstacle/step
    /// 

    RaycastHit hitInfo;
    Vector3 origin;
    bool result;
    int debugCubeCount;
    Vector3 stepTestDir;

    private bool CanPlayerStepOn(ContactPoint stepCP)
    {

        result = false;
        debugCubeCount = debugObjects_Transform.childCount;

        origin = new Vector3(stepCP.point.x, transform.position.y, stepCP.point.z);
        Vector3 direction = Vector3.down;
        stepTestDir = new Vector3(origin.x + ( - (stepCP.normal.x/10)), origin.y + maxStepHeight, origin.z + ( - (stepCP.normal.z/10)));


        if (Physics.Raycast(new Ray(stepTestDir, direction), out hitInfo, maxStepHeight))
        {
            result = true;
            if (enablePASBDebug)
                Debug.DrawLine(stepTestDir, hitInfo.point, Color.yellow, 2.5f);
        }
        return result;
    }


    






    public int RaysOnSingleAxis = 5;
    public float matrixSizeAddition = 0.1f;

    private void RaycastFloorMatrix()
    {
        result = false;
        RaycastHit sbam, oldSbam = default;
        

        float maxMatrixDistance = boxCol.size.x + matrixSizeAddition;
        float distanceBetweenRays = maxMatrixDistance / ( RaysOnSingleAxis - 1 );
        float x_matrixDist = 0;
        float z_matrixDist = 0;
        float fixedDistance = 0.5f; // SERVE PER EVITARE CHE IL RAYCAST VADA SOTTO IL PAVIMENTO IN ALCUNI CASI

        Vector3 objectFoot = transform.position - new Vector3(maxMatrixDistance/2, 0, maxMatrixDistance/2);

        

        for (x_matrixDist = 0; x_matrixDist <= maxMatrixDistance; x_matrixDist += distanceBetweenRays)
        {

            for (z_matrixDist = 0; z_matrixDist <= maxMatrixDistance; z_matrixDist += distanceBetweenRays)
            {
                if (Physics.Raycast(objectFoot + new Vector3(x_matrixDist, fixedDistance, z_matrixDist), Vector3.down, out sbam))
                {
                    Debug.DrawLine(objectFoot + new Vector3(x_matrixDist, fixedDistance, z_matrixDist), sbam.point, Color.blue);


                    if (x_matrixDist == 0 || z_matrixDist == 0)
                        oldSbam = sbam;
                    else
                        GetShortestRay(sbam, oldSbam);

                }
                
            }
        }
    }

    float height;
    float currentRayDistance;
    private void GetShortestRay(RaycastHit rayHit, RaycastHit oldRayHit)
    {
        //get ray distance and store it
        float storedRayDistance = oldRayHit.distance;
        currentRayDistance = rayHit.distance;


        if (storedRayDistance <= currentRayDistance)
        {
            currentRayDistance = storedRayDistance;
            height = oldRayHit.point.y - transform.position.y;
        }else
            height = rayHit.point.y - transform.position.y;


    }

    private bool GetIfSteppable()
    {
        if (height > transform.position.y && height <= maxStepHeight)
            result = true;
        else
            result = false;

        Debug.Log("currentRayDistance " + currentRayDistance);
        Debug.Log("playerRoot Y " + transform.position.y);
        Debug.Log("height " + height);
        Debug.Log("is steppable? " + result);
        return result;
    }


    private void ForceGroundedCondition()
    {
         plrmov.SetIsGrounded(true);
    }



    /// Offsets Player position based on Player origin and latest Raycast hit height point
    /// ( requires a RaycastHit )
    /// 
    /// \offsetY    ;no desc.
    /// \pos        ;no desc.
    /// 
    private void StepOnSurface()
    {
        
        float offsetY = height + 0.0001f;
        Vector3 offsetVector = new(0, offsetY, 0);
        

        posBody.position += offsetVector;

    }
}
