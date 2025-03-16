using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

public class PlayerNetworkData : NetworkBehaviour
{
    public NetworkVariable<FixedString64Bytes> playerName = new NetworkVariable<FixedString64Bytes>();

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string localName = LobbyManager.Instance.playerName;
            SetPlayerNameServerRpc(localName);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerNameServerRpc(string newName)
    {
        playerName.Value = newName;
    }
}
