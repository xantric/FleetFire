using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    float health = 100;
    public GameObject player;
    public void reduceHealth(float damage)
    {
        health -= damage;
    }
    void Update()
    {
        if(health <= 0)
        {
            Destroy(player);
        }
    }
}
