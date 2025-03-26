using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class SecondaryWeapon : MonoBehaviour
{
    [Header("Weapon Inputs")]
    public KeyCode reloadKey = KeyCode.R;

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
    private bool resetReload = false;

    [Header("Important")]
    private GameObject weaponTip;
    private GameObject playerOrientation;
    LayerMask hittableLayer;
    RaycastHit hit;
    private float nextTimeToFire; //created to avoid using the "fireRate" as the "Time.time + 1f/fireRate" container or else it will scale the fire rate each shot
    private zombie_class z;
    private Shooting_Trail st;

    [Header("Audio")]
    public AudioClip gunshot;
    public AudioClip riflereload;

    public int getAmmo() {
        return this.ammo;
    }

    public void forceReload()
    {
        hasAmmo = true;
        isNotReloading = true;

        if(ammo <= 0)
            resetReload = true;

    }

    private void Start()
    {

        playerOrientation = GameObject.FindGameObjectWithTag("MainCamera");
        ammo = maxAmmo;
        hittableLayer = LayerMask.GetMask("watIsGround", "CharacterMesh");
        weaponTip = gameObject.transform.Find("w_tip").gameObject;
        st = weaponTip.GetComponent<Shooting_Trail>();

    }


    private void Update()
    {
        if(gameObject.transform.parent != null && gameObject.transform.parent.name == "primary_weapon")
        {
            
            if (Input.GetMouseButtonDown(0) && hasAmmo)
                Shoot();
            if (Input.GetKeyDown(reloadKey) && isNotReloading && getAmmo() < maxAmmo)
                StartCoroutine(WeaponReload());
            if (resetReload) {
                StartCoroutine(WeaponReload());
                resetReload = false;
            }
                

        }

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
            
            if (!resetReload)
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

                //StartCoroutine(st.CreateTrail(weaponTip.transform.position, hit, 0.2f));

                gameObject.GetComponent<AudioSource>().PlayOneShot(gunshot);

            }
            

            CheckForReload();
        }
    }


}

