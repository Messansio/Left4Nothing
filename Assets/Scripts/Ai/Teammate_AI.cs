using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teammate_AI : MonoBehaviour
{
    private float health = 100f;
    private NavMeshAgent TM_AI_NavMeshAgent;
    private GameObject[] allPlayers;
    private GameObject closestPlayer;
    private float distanceToPlayer = 0f;
    private float oldRefDistance = 0f;


    public void takeDamage(float incomingDamage)
    {
        health = health - incomingDamage;
    }


    private void lookForClosestPlayer()
    {
        allPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in allPlayers) {
            distanceToPlayer = Vector3.Distance(gameObject.transform.position, player.transform.position);
            
            if(distanceToPlayer < oldRefDistance)
            {
                closestPlayer = player;
            }
            oldRefDistance = distanceToPlayer;
        }
    }


    private void checkIfClose()
    {
        moveToPlayer();
    }

    

    private void moveToPlayer()
    {
        TM_AI_NavMeshAgent.SetDestination(closestPlayer.transform.position);
    }

    private GameObject[] allEnemies;


/*
    private void LookAtClosestEnemy()
    {

        allEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        
        foreach (var enemy in allEnemies)
        {
            if (allEnemies == null)
                break;
            
                distanceToPlayer = Vector3.Distance(gameObject.transform.position, enemy.transform.position);

            if (distanceToPlayer < oldRefDistance)
            {
                closestPlayer = enemy;
            }
            oldRefDistance = distanceToPlayer;
            gameObject.transform.LookAt(enemy.transform);
        }

        
    }
*/
    private void Start()
    {
        TM_AI_NavMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    
    private void Update()
    {
        lookForClosestPlayer();
        checkIfClose();
        //LookAtClosestEnemy();
    }
}
