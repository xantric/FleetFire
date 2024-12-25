using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float health = 100;
    public float maxHelath = 100;

    AudioManager audioManager;

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    }
    public void reduceHealth(float damage)
    {
        health -= damage;
    }
    void Update()
    {
        if(health <= 0)
        {
            audioManager.PlaySFX(audioManager.death);
            Destroy(gameObject);
        }
    }
}
