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
        
        // **Get player name from the connection handler**
        string assignedName = ConnectionApprovalHandler.GetPlayerName(OwnerClientId);
        Name.Value = assignedName; // **Now it syncs automatically**
    }
}