using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
   private WeaponControl weaponControl;
   private HealthSystem healthSystem;
   void OnTriggerEnter(Collider other){
    weaponControl = GameObject.Find("Pistol").GetComponent<WeaponControl>();
    healthSystem = gameObject.GetComponent<HealthSystem>();
    if(other.tag == "CollectibleAmmo"){
        weaponControl.ClipCount++;
        Destroy(other.gameObject);
    }
    if(other.tag == "CollectibleHealth"){
        if(healthSystem.health.Value >80 && healthSystem.health.Value <100){
            healthSystem.health.Value =100;
        }
         else if (healthSystem.health.Value >0 && healthSystem.health.Value <80)
        {
            healthSystem.health.Value += 20;
        }
        Destroy(other.gameObject);
    }
   }
   
}
