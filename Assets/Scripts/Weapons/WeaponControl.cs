using System.Collections;
using System.Collections.Generic;
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

    public int MaxAmmo=10;
    public int CurrentAmmo;
    public float reloadTime=1f;
    private bool isReloading = false;
    public int ClipCount=2;

    public PauseMenu pauseMenu;
    public PlayerWeapon _playerWeapon;
    AudioManager audioManager;

    private void Awake() {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        crossHair = null;
    }

    void Start()
    {
        CurrentAmmo = MaxAmmo;
    }
    void SetCrossHair()
    {
        somethingHit = Physics.Raycast(muzzle.position, muzzle.forward, out hitInfo, range, ~ignoreLayer);
        if(somethingHit)
        {
            if ( crossHair != null )
            {
                crossHair.gameObject.SetActive(true);
                Vector3 hitPoint = hitInfo.point;
                Vector3 positionInCamera = mainCamera.WorldToScreenPoint(hitPoint);
                crossHair.anchoredPosition = new Vector2(positionInCamera.x, positionInCamera.y);
                crossHair.localScale = new Vector3(1, 1, 1) * scaleFactor / hitInfo.distance;
            } 
        }
        else
        {
            if (crossHair != null) crossHair.gameObject.SetActive(false);
        }
    }
    void Shoot()
    {
        /*if(pauseMenu.isPaused == true) {
            return;
        }*/
        //GameObject bulletClone = Instantiate(bullet, muzzle.position, Quaternion.identity);
        audioManager.PlaySFX(audioManager.shooting);
       // bulletClone.GetComponent<BulletBehaviour>().SetVelocity(speed * muzzle.forward.normalized);
        //bulletClone.GetComponent<BulletBehaviour>().SetDamage(Damage);
        CurrentAmmo--;
        _playerWeapon.SpawnBulletServerRpc(muzzle.position, muzzle.forward.normalized, Damage, speed);
    }

    IEnumerator Reload()
    {
        isReloading = true;
        audioManager.PlaySFX(audioManager.reload);
        yield return new WaitForSeconds(reloadTime);
        CurrentAmmo = MaxAmmo;
        isReloading = false;
        ClipCount--;
    }
    void Update()
    {
        if(isReloading)
        {
            return;
        }
        SetCrossHair();
        if (crossHair != null )
        {
            if (Input.GetMouseButtonDown(0) && crossHair.gameObject.activeSelf && CurrentAmmo > 0)
            {
                Shoot();
            }
        }
        if(Input.GetKeyDown(KeyCode.R) && CurrentAmmo < MaxAmmo && ClipCount > 0) 
        {
            StartCoroutine(Reload());
        }
    }
}
