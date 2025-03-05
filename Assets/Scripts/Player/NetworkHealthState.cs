using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{
    public NetworkVariable<int> HealthPoint = new();


    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        HealthPoint.Value = 100;
    }
}
