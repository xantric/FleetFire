using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SetPistolCrosshair : NetworkBehaviour
{
    RectTransform rectTransform;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Crosshair _crosshair = FindObjectOfType<Crosshair>();
        if (_crosshair != null)
        {
            rectTransform = _crosshair.gameObject.GetComponentInChildren<RectTransform>();
        }
        
        if (IsOwner)
        {
            WeaponControl _weaponControl = GetComponentInChildren<WeaponControl>();
            _weaponControl.crossHair = rectTransform;
        }
    }
}
