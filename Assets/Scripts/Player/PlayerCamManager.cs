using UnityEngine;
using Unity.Netcode;

public class PlayerCamManager : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if(Camera.main.gameObject.activeSelf) Camera.main.gameObject.SetActive(false);
    }
}
