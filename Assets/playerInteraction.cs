using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class playerInteraction : MonoBehaviour
{

    public GameObject weapon;
    private weaponBehaviour wb;

    // Start is called before the first frame update
    void Start()
    {
        wb = weapon.GetComponent<weaponBehaviour>();
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButton(0) && wb.hasAmmo)
            wb.Shoot();

    }
}
