using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private GameObject inventoryUI;

    private GameObject primaryWeapon;
    private GameObject secondaryWeapon;
    private GameObject grenade;
    private GameObject aidItem;

    private GameObject[] playerInvArray;
    private int currentSelected = 0;

    public GameObject[] Get_playerInvArray()
    {
        return playerInvArray;
    }

    private void Awake()
    {
        primaryWeapon = GameObject.Find("primary_weapon");
        secondaryWeapon = GameObject.Find("secondary_weapon");
        grenade = GameObject.Find("grenade_item");
        aidItem = GameObject.Find("aid_item");

        playerInvArray = new GameObject[4] { primaryWeapon, secondaryWeapon, grenade, aidItem };
        inventoryUI = GameObject.Find("inventoryUI");
    }


    public void EquipNextObj()
    {
        playerInvArray[currentSelected].SetActive(false);
        inventoryUI.transform.GetChild(currentSelected).gameObject.transform.GetChild(0).gameObject.SetActive(false);

        if (currentSelected == playerInvArray.Length - 1)
            currentSelected = 0;
        else
            currentSelected += 1;

        playerInvArray[currentSelected].SetActive(true);
        inventoryUI.transform.GetChild(currentSelected).gameObject.transform.GetChild(0).gameObject.SetActive(true);

        Debug.Log("Current Equipped Slot:\n" + (currentSelected + 1) + "\nObject:\n" + playerInvArray[currentSelected]);

    }

    public void EquipPrecedentObj()
    {
        playerInvArray[currentSelected].SetActive(false);
        inventoryUI.transform.GetChild(currentSelected).gameObject.transform.GetChild(0).gameObject.SetActive(false);

        if (currentSelected == 0)
            currentSelected = playerInvArray.Length - 1;
        else
            currentSelected -= 1;

        playerInvArray[currentSelected].SetActive(true);
        inventoryUI.transform.GetChild(currentSelected).gameObject.transform.GetChild(0).gameObject.SetActive(true);

        Debug.Log("Current Equipped Slot:\n" + (currentSelected + 1) + "\nObject:\n" + playerInvArray[currentSelected]);

    }


}
