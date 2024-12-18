using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class WeaponControl : MonoBehaviour
{
    public Transform muzzle;
    public RectTransform crossHair;
    public Camera mainCamera;
    public float range;
    bool somethingHit;
    RaycastHit hitInfo;
    public LayerMask ignoreLayer;
    public GameObject bullet;
    public float speed;
    public float scaleFactor = 1;
    public float Damage;
    void SetCrossHair()
    {
        somethingHit = Physics.Raycast(muzzle.position, muzzle.forward, out hitInfo, range, ~ignoreLayer);
        if(somethingHit)
        {
            crossHair.gameObject.SetActive(true);
            Vector3 hitPoint = hitInfo.point;
            Vector3 positionInCamera = mainCamera.WorldToScreenPoint(hitPoint);
            crossHair.anchoredPosition = new Vector2(positionInCamera.x, positionInCamera.y);
            crossHair.localScale = new Vector3(1,1,1) * scaleFactor/hitInfo.distance;
        }
        else
        {
            crossHair.gameObject.SetActive(false);
        }
    }
    void Shoot()
    {
        GameObject bulletClone = Instantiate(bullet, muzzle.position, Quaternion.identity);
        bulletClone.GetComponent<BulletBehaviour>().SetVelocity(speed * muzzle.forward.normalized);
        bulletClone.GetComponent<BulletBehaviour>().SetDamage(Damage);
    }
    void Update()
    {
        SetCrossHair();
        if(Input.GetMouseButtonDown(0) && crossHair.gameObject.activeSelf)
        {
            Shoot();
        }
    }
}
