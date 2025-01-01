using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerWeapon : NetworkBehaviour
{
    public GameObject BulletPrefab;
    [ServerRpc]
    public void SpawnBulletServerRpc(Vector3 position, Vector3 direction, float damage, float speed)
    {
        //"Bullet Spawned");
        GameObject bulletClone = Instantiate(BulletPrefab, position, Quaternion.identity);
        bulletClone.GetComponent<NetworkObject>().Spawn(true);

        bulletClone.GetComponent<BulletBehaviour>().SetDamage(damage);
        bulletClone.GetComponent<BulletBehaviour>().SetVelocity(speed * direction);
    }
}
