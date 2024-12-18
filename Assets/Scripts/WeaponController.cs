using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponController : MonoBehaviour
{
    public Transform muzzle;
    public RectTransform crossHair;
    public Camera mainCamera;
    public float range;
    bool somethingHit;
    public LayerMask ignoreLayer;
    RaycastHit hitInfo;
    public GameObject bullet;
    public float speed;
    public float scaleFactor =1f;
    public float damage;

    void SetCrossHair() {

        somethingHit = Physics.Raycast(muzzle.position, muzzle.forward, out hitInfo, range, ~ignoreLayer);
        if (somethingHit) {
            crossHair.gameObject.SetActive(true);
            Vector3 hitPoint = hitInfo.point;
            Vector3 positionInCamera = mainCamera.WorldToScreenPoint(hitPoint);
            crossHair.anchoredPosition = new Vector2 (positionInCamera.x, positionInCamera.y);
            crossHair.localScale = new Vector3(1,1,1) * scaleFactor/hitInfo.distance;
        }
        else {
            crossHair.gameObject.SetActive(false);
        }
    }

    private void Shoot() {
        GameObject bulletClone = Instantiate(bullet, muzzle.position,Quaternion.identity);
        bulletClone.GetComponent<BulletBehaviuor>().SetVelocity(speed * muzzle.forward.normalized);
        bulletClone.GetComponent<BulletBehaviuor>().SetDamage(damage);
    }

    void Update()
    {
        SetCrossHair();
        if (Input.GetMouseButtonDown(0) && crossHair.gameObject.activeSelf) {
            Shoot();
        }
    }
}
