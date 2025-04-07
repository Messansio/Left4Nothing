using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class zombie_class : MonoBehaviour
{
    [Header("Zombie Type")]
    public bool isBoss = false;
    public bool isCommon = true;    //Common by default with 100 hp

    [Header("Zombie Stats")]
    private float health = 100;
    public AudioClip zDeath;
    private bool isdead;
    public bool isZombieDead
    {
        get { return isdead; }
    }
    LayerMask excludeRagdollCollisions;
    private void Awake()
    {
        excludeRagdollCollisions = LayerMask.GetMask("whatIsPlayer");
        isdead = false;
        if(isBoss)
            health = 800;

    }

    public void takeDamage(float incomingDamage)
    {
        health = health - incomingDamage;
        gameObject.GetComponent<AudioSource>().PlayOneShot(zDeath);
    }
    
    private void checkToDestroySelf()
    {
        if(health <= 0)
        {
            
            Destroy(gameObject);
        }
            

    }

    private void Update()
    {
        //checkToDestroySelf();
        RagdollOnDeath();
    }


    private Vector3 simulateTrippingOnDeath;
    

    private void RagdollOnDeath()
    {
        if (health <= 0 && !isdead)
        {
            gameObject.GetComponent<Rigidbody>().freezeRotation = false;
            gameObject.GetComponent<CapsuleCollider>().excludeLayers = excludeRagdollCollisions;
            if(gameObject.GetComponent<NavMeshAgent>() != null)
                simulateTrippingOnDeath = gameObject.GetComponent<NavMeshAgent>().velocity;

            Destroy(gameObject.GetComponent<ScriptMachine>());
            Destroy(gameObject.GetComponent<NavMeshAgent>());
            gameObject.GetComponent<Rigidbody>().AddForce(simulateTrippingOnDeath, ForceMode.VelocityChange);
            

            isdead = true;
        }
    }

    
}
