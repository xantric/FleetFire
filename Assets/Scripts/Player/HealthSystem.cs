using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class HealthSystem : NetworkBehaviour
{
    //public float health = 100;
    public static Dictionary<ulong, HealthSystem> playerHealths = new Dictionary<ulong, HealthSystem>();
    public NetworkVariable<float> health = new NetworkVariable<float>(100f);
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false);
    public float maxHelath = 100;

    AudioManager audioManager;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerHealths.Add(OwnerClientId, this);
        if (IsOwner)
        {
            HealthBar _healthBar = FindAnyObjectByType<HealthBar>();
            _healthBar.healthSystem = this;
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        playerHealths.Remove(OwnerClientId);
    }
    private void Awake()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    [ServerRpc(RequireOwnership = false)]
    public void ResetHealthServerRpc()
    {
        health.Value = maxHelath;
        isDead.Value = false;
        Debug.LogWarning("Health Reset");
    }
    [ServerRpc(RequireOwnership = false)]
    public void reduceHealthServerRpc(float damage, ulong attackerID)
    {
        //"Damage recieved by rpc:" + damage);
        health.Value -= damage;
        if (health.Value <= 0 && !isDead.Value)
        {
            isDead.Value = true;
            Die(attackerID);
            return;
        }
    }
    void Die(ulong attackerID)
    {
        audioManager.PlaySFX(audioManager.death);
        //Destroy(gameObject);
        polygon_fps_controller.TogglePlayer(OwnerClientId, false);
        GameManager.PlayerDied(OwnerClientId, attackerID);
    }

}