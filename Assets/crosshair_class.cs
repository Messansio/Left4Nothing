using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class crosshair_class : MonoBehaviour
{
    public Sprite hitCrosshair;
    private Sprite defCrosshair;

    public GameObject weapon;
    private weaponBehaviour wb;

    private void Start()
    {
        wb = weapon.GetComponent<weaponBehaviour>();
        defCrosshair = gameObject.GetComponent<Image>().sprite;
    }
    private void Update()
    {

        if(wb.hitZombie == true)
        {
            Debug.Log("Zombie Hit!");
            gameObject.GetComponent<Image>().sprite = hitCrosshair;

        }
        else
        {
            gameObject.GetComponent<Image>().sprite = defCrosshair;
        }

    }
}
