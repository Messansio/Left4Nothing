using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


public class playerInteraction : MonoBehaviour
{

    public GameObject weapon;
    private weaponBehaviour wb;

    [Header("Keybinds")]
    public KeyCode reloadKey = KeyCode.R;

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
        if (Input.GetKeyDown(reloadKey))
            StartCoroutine(wb.WeaponReload());
        if (Input.mouseScrollDelta.y > 0)
            Debug.Log("Scroll Up");
        if (Input.mouseScrollDelta.y < 0)
            Debug.Log("Scroll Down");
    }
}
