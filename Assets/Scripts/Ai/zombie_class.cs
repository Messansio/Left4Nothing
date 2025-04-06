using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombie_class : MonoBehaviour
{
    [Header("Zombie Type")]
    public bool isBoss = false;
    public bool isCommon = true;    //Common by default with 100 hp

    [Header("Zombie Stats")]
    private float health = 100;
    public AudioClip zDeath;

    private void Awake()
    {
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
        checkToDestroySelf();
    }
}
