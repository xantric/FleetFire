using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
   private WeaponControl weaponControl;
   private HealthSystem healthSystem;
   void OnTriggerEnter(Collider other){
    weaponControl = GameObject.Find("Pistol").GetComponent<WeaponControl>();
    healthSystem = GameObject.Find("Player").GetComponent<HealthSystem>();
    if(other.tag == "CollectibleAmmo"){
        weaponControl.ClipCount++;
        Destroy(other.gameObject);
    }
    if(other.tag == "CollectibleHealth"){
        if(healthSystem.health>80 && healthSystem.health<100){
            healthSystem.health =100;
        }
         else if (healthSystem.health>0 && healthSystem.health<80)
        {
            healthSystem.health += 20;
        }
        Destroy(other.gameObject);
    }
   }
   
}
