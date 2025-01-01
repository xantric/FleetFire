using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    //public float health = 100;
    public NetworkVariable<float> health = new NetworkVariable<float>(100f);
    public float maxHelath = 100;

    AudioManager audioManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            HealthBar _healthBar = FindAnyObjectByType<HealthBar>();
            _healthBar.healthSystem = this;
        }
    }
    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }

    [ServerRpc(RequireOwnership = false)]
    public void reduceHealthServerRpc(float damage)
    {
        //"Damage recieved by rpc:" + damage);
        health.Value -= damage;
        if(health.Value <= 0)
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
