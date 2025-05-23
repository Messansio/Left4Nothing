
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAndStepsBehaviour : MonoBehaviour
{

    public float maxStepHeight;

    public bool enablePASBDebug;
    public GameObject cubeRaycastPointDebugAsset;
    private Transform debugObjects_Transform;
    public PlayerMovement plrmov;

    private void Start()
    {
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

        foreach (ContactPoint contact in plrmov.contactPoints)
        {
            if (contact.normal != Vector3.up && !plrmov.isOnWalkableSlope)
            {
                //Debug.Log(contact.normal);
                isStep = true;
                CP = contact;
            }
                
        }
        return isStep;
    }

    private void FixedUpdate()
    {
        if (CheckIfMoving() && CheckIfStep())
        {
            //Debug.Log(CheckIfStep());
            if (CanPlayerStepOn(CP))
            {
                StepOnSurface(CP);
            }
            plrmov.contactPoints.Clear();
        }
        

    }

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

    private bool CanPlayerStepOn(ContactPoint stepCP)
    {
        bool result = false;

        int debugCubeCount = debugObjects_Transform.childCount;


        origin = new Vector3(stepCP.point.x, transform.position.y, stepCP.point.z);
        Vector3 direction = Vector3.down;
        Vector3 stepTestDir = new Vector3(origin.x + ( - (stepCP.normal.x/10)), origin.y + maxStepHeight, origin.z + ( - (stepCP.normal.z/10)));

        //Debug.Log(origin);

        if (Physics.Raycast(new Ray(stepTestDir, direction), out hitInfo, maxStepHeight))
        {
            result = true;
            if (enablePASBDebug && debugCubeCount > 1)
                Destroy(debugObjects_Transform.GetChild(0).gameObject);
            if (enablePASBDebug)
                Instantiate(cubeRaycastPointDebugAsset, hitInfo.point, Quaternion.identity, debugObjects_Transform);
            
        }
        

        //Debug.Log(result);
        return result;
    }

    /// Offsets Player position based on Player origin and latest Raycast hit height point
    /// ( requires a RaycastHit )
    /// 
    /// \offsetY    ;no desc.
    /// \pos        ;no desc.
    /// 
    private void StepOnSurface(ContactPoint stepCP)
    {
        
        Rigidbody posBody = GetComponentInParent<Rigidbody>();
        Vector3 velocity = posBody.velocity;
        Vector3 lastVelocity = velocity;
        float offsetY =  hitInfo.point.y - origin.y;
        Vector3 offsetVector = new(0, offsetY, 0);

        posBody.MovePosition(posBody.position + offsetVector);
        posBody.velocity = lastVelocity;


        Debug.Log(offsetY);
        Debug.Log(posBody.position.y);
    }

    /*
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
    */
}
