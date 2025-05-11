using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectibles : MonoBehaviour
{
    [SerializeField]
    private HealthSystem healthSystem;
    [SerializeField]
    private assault57 _assault57;
    
    private float HealthRecharge;
    private int bullets;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "CollectibleHealth")
        {
            if(healthSystem != null) healthSystem.ResetHealthServerRpc();
        }
        else if(collision.gameObject.tag == "CollectibleAmmo")
        {
            if(_assault57 != null)
            {
                _assault57.stored_bullets = 120;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "CollectibleHealth")
        {
            if (healthSystem != null) healthSystem.ResetHealthServerRpc();
            if(other.gameObject != null) Destroy(other.gameObject);
        }
        else if (other.gameObject.tag == "CollectibleAmmo")
        {
            if (_assault57 != null)
            {
                _assault57.stored_bullets = 120;
            }
            if (other.gameObject != null) Destroy(other.gameObject);
        }
    }

}
