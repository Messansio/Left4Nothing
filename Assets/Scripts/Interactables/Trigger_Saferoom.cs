using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Saferoom : MonoBehaviour
{
    private GameObject saferoom_door;
    private Collider playerCollider;

    private int numberOfPlayers;
    //private int numberOfAlivePlayers;
    private int numberOfPlayersInsideSafe = 0;

    private void Awake()
    {
        numberOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length;
        saferoom_door = GameObject.Find("SaferoomDoor");
        playerCollider = GameObject.FindWithTag("Player").GetComponent<CapsuleCollider>();
    }

    private IEnumerator UpdateCheckOnPlayers()
    {
        numberOfPlayers = GameObject.FindGameObjectsWithTag("Player").Length;

        yield return new WaitForSeconds(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == playerCollider)
            numberOfPlayersInsideSafe++;
    }

    private void OnTriggerExit(Collider other)
    {
        if(other == playerCollider)
            numberOfPlayersInsideSafe--;
    }

    private void CheckIfAllPlayersAreInSafe()
    {
        if (numberOfPlayersInsideSafe == numberOfPlayers && !saferoom_door.transform.parent.GetComponent<Door>().isDoorOpen)
        {
            Debug.Log("All Players are inside the Saferoom");
            //LEVEL COMPLETED...CHANGE LEVEL
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == playerCollider)
            CheckIfAllPlayersAreInSafe();
    }

    
}
