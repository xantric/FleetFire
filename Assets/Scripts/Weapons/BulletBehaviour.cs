using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour
{
    Vector3 direction;
    float speed;
    RaycastHit hitInfo;
    float damage;
    bool somethingHit;
    public LayerMask _layerMask;
    public void SetVelocity(Vector3 velocity)
    {
        GetComponent<Rigidbody>().velocity = velocity;
        direction = velocity.normalized;
        speed = velocity.magnitude;
    }
    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Hit object :"+other.gameObject.name);
        if(other.gameObject.CompareTag("Enemy") || other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Damage given to rpc:" + damage);
            other.gameObject.GetComponent<HealthSystem>().reduceHealthServerRpc(damage);
            //Debug.Log(other.gameObject.GetComponent<HealthSystem>().health);
        }
        Destroy(gameObject);
    }
    public void SetDamage(float Damage)
    {
        damage = Damage;
    }
    void Update()
    {
        if(direction != null)
        {
            somethingHit = Physics.Raycast(transform.position, -direction, out hitInfo, speed * Time.deltaTime, _layerMask);
            if(somethingHit)
            {
                
                Debug.Log("RayCast Hit" + " " + hitInfo.collider.gameObject.name);
                if (hitInfo.collider.gameObject.CompareTag("Player"))
                {
                    hitInfo.collider.gameObject.GetComponent<HealthSystem>().reduceHealthServerRpc(damage);
                    //Debug.Log(hitInfo.collider.gameObject.GetComponent<HealthSystem>().health);
                }
                Destroy(gameObject);
            }
        }
    }
}
