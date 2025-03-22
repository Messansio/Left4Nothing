using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zombie_class : MonoBehaviour
{
    [Header("Zombie Stats")]
    private float health = 100;
    public void takeDamage(float incomingDamage)
    {
        health = health - incomingDamage;
    }

    private void checkToDestroySelf()
    {
        if(health <= 0)
            Destroy(gameObject);

    }

    private void Update()
    {
        checkToDestroySelf();
    }
}
