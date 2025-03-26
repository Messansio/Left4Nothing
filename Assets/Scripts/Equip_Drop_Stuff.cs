using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Take_Drop_Stuff : MonoBehaviour
{

    private PlayerInventory pi;
    private GameObject[] piArray;
    private GameObject obj;
    private Transform p_camera;
    private Rigidbody objRb;
    private LayerMask collidingLayerMask;

    private Vector3 weaponHolderPos;
    private Quaternion weaponHolderRot;

    private void Awake()
    {
        collidingLayerMask = LayerMask.GetMask("Default");
        pi = gameObject.GetComponent<PlayerInventory>();
        p_camera = GameObject.FindGameObjectWithTag("MainCamera").transform;
    }

    public void DropCurrentObj()
    {
        piArray = pi.Get_playerInvArray();

        if (piArray[pi.Get_currentSelected()].transform.childCount != 0)
        {
            obj = piArray[pi.Get_currentSelected()].transform.GetChild(0).gameObject;


            obj.transform.GetLocalPositionAndRotation(out weaponHolderPos, out weaponHolderRot);
            


            obj.transform.SetParent(null);
            obj.layer = 0;
            objRb = obj.gameObject.AddComponent<Rigidbody>();
            objRb.includeLayers = collidingLayerMask;
            objRb.interpolation = RigidbodyInterpolation.Interpolate;
            objRb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

    }

    
    public void TakeObj()
    {
        RaycastHit hit;
        if(Physics.Raycast(p_camera.position, p_camera.forward, out hit, 5f))
        {
            Debug.DrawLine(p_camera.position, hit.point);
            Debug.Log(hit.collider.gameObject.tag);
            //check if weapon or item
            if(hit.collider.gameObject.tag == "Weapon")
            {

                hit.transform.SetParent(GameObject.Find("primary_weapon").transform);
                obj.layer = 12;
                Destroy(obj.GetComponent<Rigidbody>());

                obj.transform.localPosition = weaponHolderPos;
                obj.transform.localRotation = weaponHolderRot;
            }
        }
    }

}
