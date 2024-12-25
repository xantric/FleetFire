using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HealthSystem healthSystem;
    public Slider healthSlider;
    
    void Update() {

        healthSlider.value = healthSystem.health/healthSystem.maxHelath;
    }
}
