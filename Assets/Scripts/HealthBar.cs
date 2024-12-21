using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public HealthSystem healthSystem;
    public Slider healthSlider;
    // Start is called before the first frame update


    // Update is called once per frame
    void Update()
    {
        healthSlider.value = healthSystem.health/healthSystem.maxHeath;
    }
}
