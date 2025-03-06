using Unity.Collections;
using Unity.Netcode;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<int> Score = new();
    public NetworkVariable<FixedString128Bytes> Name = new();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsServer) return;
        Score.Value = 0;
       
        GetNameClientRpc(new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] {GetComponent<NetworkObject>().OwnerClientId}
            }
        });
    }

    [ClientRpc]
    public void GetNameClientRpc(ClientRpcParams clientRpcParams = default)
    {
        GetNameServerRpc(OwnerClientId.ToString());
    }

    [ServerRpc]
    private void GetNameServerRpc(string Name)
    {
        this.Name.Value = "Player: " + Name;
    }
}