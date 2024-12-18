using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    float health = 100f;
    public GameObject player;

    public void ReduceHealth(float damage) {

        health -= damage;
    }

    private void Update() {
        if (health <= 0) { 
            Destroy(player);
        }
    }
}
