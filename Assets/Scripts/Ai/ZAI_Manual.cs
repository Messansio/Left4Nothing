using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZAI_Manual : MonoBehaviour
{
    //Get ZAI Pathfinding Conditions ref
    private ZAI_Pathfind_Conditions ZAI_PFC;


    //Zombie Status

    private int roamingType;

    private void Awake()
    {
        ZAI_PFC = gameObject.GetComponent<ZAI_Pathfind_Conditions>();
        roamingType = Random.Range(0, 2); 
    }

    private bool sawPlayer = false;

    private void FixedUpdate()
    {
        Roam();
    }

    private float maxZombieVisualRange;
    private bool SeePlayer()   //check if the zombie saw the player
    {
        /*if( distance between player and zombie is less equal to maxZombieVisualRange )
            sawPlayer = true;
          else
            keep roaming
         */

        return sawPlayer;
    }

    private void Roam() //set by default as the zombie has not seen the player yet
    {
        switch (roamingType)
        {
            case 0:
                //Laying down
                break;
            case 1:
                //Random wandering
                break;
            case 2:
                //Follow the closest Zombie
                break;
        }

        if(sawPlayer)
            ChasePlayer();

    }

    

    private void ChasePlayer()  //triggered after the zombie saw the player
    {
        UnderstandPlayerPos();
    }

    private float minAttackDistance;

    private void UnderstandPlayerPos()  //finds Player position and if so initiates the attack
    {
        /*if( distance between player and zombie is less than minAttackDistance )
            StartCoroutine(StopAndAttack());
          else
            keep chasing player
        */
    }

    private IEnumerator StopAndAttack()    //triggered after the zombie reached the minimum distance to attack
    {
        // Each Attack applies slow movement speed to the player

        // 1 - Stop Zombie movement

        // 2 - Damage Target Player
        // 2.5 - Apply Slow per hit

        /*if ( target player is dead )
            Roam(); 
          else
            keep damaging the target player
        */
        yield return new WaitForSeconds(0.9f);
        StartCoroutine(StopAndAttack());
    }

}
