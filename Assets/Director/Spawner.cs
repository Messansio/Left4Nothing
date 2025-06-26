using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class playerSpawner : MonoBehaviour
{
    [Header("Game Objects")]
    private GameObject player;
    private GameObject playerController;
    public GameObject spawnpoint;
    public GameObject cameraOrientation;
    public GameObject zombieAsset;

    private Vector3 sp_pos;

    public bool usePlayerController = false;


    private void Start()
    {
        player = GameObject.Find("Player");
        playerController = GameObject.Find("PlayerController");

        if (usePlayerController)
        {
            Destroy(player);
            player = playerController;
        }
        else
        {
            Destroy(playerController);
        }


            sp_pos = spawnpoint.transform.position.ConvertTo<Vector3>();
    }
    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if(usePlayerController)
                Invoke(nameof(spawnPlayer), 0.2f);
        }
            
        if (Input.GetKeyDown(KeyCode.Z))
            spawnZombieToView();
    }
    
    private void spawnPlayer()
    {
        Debug.Log("teleporting...");
        player.transform.position = sp_pos;
        player.GetComponent<Rigidbody>().MovePosition(spawnpoint.transform.position);
    }


    private RaycastHit placeToSpawn;
    private void spawnZombieToView()
    {
        if (Physics.Raycast(player.transform.position, cameraOrientation.transform.TransformDirection(Vector3.forward), out placeToSpawn, Mathf.Infinity))
        {
            Instantiate(zombieAsset, placeToSpawn.point,Quaternion.identity);
        }
    }
}
