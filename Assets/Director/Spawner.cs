using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class playerSpawner : MonoBehaviour
{
    [Header("Game Objects")]
    public GameObject player;
    public GameObject spawnpoint;
    public GameObject cameraOrientation;
    public GameObject zombieAsset;

    private Vector3 sp_pos;

    private void Start()
    {
        sp_pos = spawnpoint.transform.position.ConvertTo<Vector3>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
            StartCoroutine(spawnPlayer());
        if (Input.GetKeyDown(KeyCode.Z))
            spawnZombieToView();
    }
    
    IEnumerator spawnPlayer()
    {
        Debug.Log("teleporting...");
        yield return new WaitForSeconds(0.1f);
        player.transform.position = sp_pos;
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
