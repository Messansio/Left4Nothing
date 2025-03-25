using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SecondaryWeapon : MonoBehaviour
{
    [Header("This Weapon Stats")]
    public string weaponName;
    public GameObject weapon3DModel;
    public int maxAmmo;
    private int ammo;
    public float reloadTime;
    public float fireRate;
    public float fireDistance;
    public float damage;

    [Header("Conditions")]
    public bool hasAmmo = true;
    public bool isNotReloading = true;


    [Header("Important")]
    public GameObject weaponTip;
    public GameObject playerOrientation;
    LayerMask hittableLayer;
    RaycastHit hit;
    private float nextTimeToFire; //created to avoid using the "fireRate" as the "Time.time + 1f/fireRate" container or else it will scale the fire rate each shot
    private zombie_class z;

    [Header("Audio")]
    public AudioClip gunshot;
    public AudioClip riflereload;

    public int getAmmo() {
        return this.ammo;
    }

    private void Start()
    {
        damage = 30f;
        ammo = maxAmmo;
        hittableLayer = LayerMask.GetMask("watIsGround", "CharacterMesh");
    }
    
    public IEnumerator WeaponReload()
    {
        isNotReloading = false;
        Debug.Log("Reloading..");
        hasAmmo = false;
        gameObject.GetComponent<AudioSource>().PlayOneShot(riflereload);
        yield return new WaitForSeconds(reloadTime);
        
        
        ammo = maxAmmo;
        hasAmmo = true;
        Debug.Log("Reloaded Ammos");
        isNotReloading = true;
    }

    public void TryReload()
    {
        // Only allow reload if we're not already reloading and ammo is less than max
        if (isNotReloading && ammo < maxAmmo)
        {
            StartCoroutine(WeaponReload());
        }
        else if (ammo >= maxAmmo)
        {
            Debug.Log("Magazine already full!");
        }
    }

    void CheckForReload()
    {
        
        if (ammo <= 0)
        {
            Debug.Log("Out of Ammo");
            hasAmmo = false;
            StartCoroutine(WeaponReload());

        }
        else
        {
            ammo = ammo - 1;
        }

    }

    public bool hitZombie;
    

    private IEnumerator checkIfShotZombie()
    {
        if (hit.collider.CompareTag("Enemy") == true)
        {
            hitZombie = true;
            z = hit.transform.GetComponentInParent<zombie_class>();
            z.takeDamage(damage);
            yield return new WaitForSeconds(0.05f);
            hitZombie = false;
        }
        
    }

    public void Shoot()
    {
        if (hasAmmo && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            
            if (Physics.Raycast(weaponTip.transform.position, playerOrientation.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity))
            {
                Debug.DrawRay(weaponTip.transform.position, playerOrientation.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                StartCoroutine(checkIfShotZombie());
            }
            else
            {
                Debug.DrawRay(weaponTip.transform.position, playerOrientation.transform.TransformDirection(Vector3.forward) * fireDistance, Color.red);

            }
            
            gameObject.GetComponent<AudioSource>().PlayOneShot(gunshot);
            
            CheckForReload();
            //Debug.Log(ammo);
        }
    }

   

    

}
