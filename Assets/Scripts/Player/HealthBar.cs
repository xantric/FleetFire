using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HealthSystem healthSystem;
    public Slider healthSlider;

    private void Start()
    {
        healthSystem = null;
    }
    void Update() {

        if(healthSystem != null) healthSlider.value = healthSystem.health.Value /healthSystem.maxHelath;
    }
}
