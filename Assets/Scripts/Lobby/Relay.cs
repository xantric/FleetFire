using System;
using Unity.Services.Relay;
using UnityEngine;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;

public class Relay : MonoBehaviour
{
    void Update()
    {
        if(NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host");
            gameObject.SetActive(false);
        }
        else if(NetworkManager.Singleton.IsClient)
        {
            Debug.Log("Client");
            gameObject.SetActive(false);
        }
    }

    public async Task<string> CreateRelay(int MaxPlayers)
    {
        if(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient) 
            return null;
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(MaxPlayers - 1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                 (ushort) allocation.RelayServer.Port,
                 allocation.AllocationIdBytes,
                 allocation.Key,
                 allocation.ConnectionData
            );
            NetworkManager.Singleton.StartHost();
            return joinCode;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
        return null;
    }

    public async void JoinRelay(string joinCode)
    {
        if(NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            return;
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort) joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );
            NetworkManager.Singleton.StartClient();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
