using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviuor : MonoBehaviour
{
    private Vector3 direction;
    private float speed;

    bool somethingHit;
    RaycastHit hitInfo;
    float damage;

    public void SetVelocity(Vector3 velocity) {
        GetComponent<Rigidbody>().velocity = velocity;
        direction = velocity.normalized;
        speed = velocity.magnitude;


    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Player")){

            other.gameObject.GetComponent<HealthSystem>().ReduceHealth(damage);
        }
        Destroy(gameObject);
    }

    void Update()
    {
        if (direction != null) {
            somethingHit = Physics.Raycast(transform.position, -direction, out hitInfo, speed * Time.deltaTime);
            if (somethingHit) {

                if (hitInfo.collider.gameObject.CompareTag("Player")) {

                    //reduce health
                }
                Destroy(gameObject);
            }
        }
    }
    public void SetDamage(float damage) { 
        this.damage = damage;
    }
}
