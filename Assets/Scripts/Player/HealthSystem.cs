using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    public float health = 100;
    public float maxHelath = 100;

    AudioManager audioManager;

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void reduceHealthServerRpc(float damage)
    {
        health -= damage;
        Debug.Log("current Health: " + health);
        if(health <= 0)
        {
            Die();
        }
    }
    void Die()
    {
        audioManager.PlaySFX(audioManager.death);
        Destroy(gameObject);
    }
    
}
