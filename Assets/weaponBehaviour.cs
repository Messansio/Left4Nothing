using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class weaponBehaviour : MonoBehaviour
{
    [Header("This Weapon Stats")]

    public string weaponName;
    public GameObject weapon3DModel;

    public float firingSpeed;
    public float reloadSpeed;

    [Header("Extra Stats")]

    private bool isMeleeWeapon;         //if this is a melee weapon
    public bool isPlayerShooting;       //if the player is shooting while it's holding this weapon





    // Start is called before the first frame update
    void Start()
    {
        isPlayerShooting = false;
        isMeleeWeapon = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
