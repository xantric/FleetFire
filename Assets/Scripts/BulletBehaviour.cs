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
        Debug.Log(other.gameObject.name);
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HealthSystem>().reduceHealth(damage);
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
                    hitInfo.collider.gameObject.GetComponent<HealthSystem>().reduceHealth(damage);
                }
                Destroy(gameObject);
            }
        }
    }
}
