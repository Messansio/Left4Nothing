using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class weaponBehaviour : MonoBehaviour
{
    [Header("This Weapon Stats")]
    public string weaponName;
    public GameObject weapon3DModel;
    public int maxAmmo;
    private int ammo;
    public float reloadTime;
    public float fireRate;
    public float fireDistance;

    [Header("Conditions")]
    bool hasAmmo = true;


    [Header("Important")]
    public GameObject weaponTip;
    public GameObject playerOrientation;
    LayerMask hittableLayer;

    IEnumerator WeaponReload()
    {
        Debug.Log("Reloading..");
        yield return new WaitForSeconds(reloadTime);
        
        
        ammo = maxAmmo;
        hasAmmo = true;
        Debug.Log("Reloaded Ammos");
    }

    IEnumerator ShootingTimeForEachBullet()
    {
        
        if (ammo == 0)
        {
            Debug.Log("Out of Ammo");
            hasAmmo = false;
            StartCoroutine(WeaponReload());

        }
        else
        {
            FixedUpdate();

            yield return new WaitForSeconds(fireRate);
            
            ammo = ammo - 1;
        }

    }
    


    private void Start()
    {
        ammo = maxAmmo;
        hittableLayer = LayerMask.GetMask("watIsGround","CharacterMesh");
    }

    
    
    public void Shooting()
    {
        if (hasAmmo)
        {
            StartCoroutine(ShootingTimeForEachBullet());
            
            Debug.Log(ammo);
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(weaponTip.transform.position, playerOrientation.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
        {
            Debug.DrawRay(weaponTip.transform.position, playerOrientation.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            
            
        }
        else
        {
            Debug.DrawRay(weaponTip.transform.position, playerOrientation.transform.TransformDirection(Vector3.forward) * fireDistance, Color.red);
            
        }
    }
}
