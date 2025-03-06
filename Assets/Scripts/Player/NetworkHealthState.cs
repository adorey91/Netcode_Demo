using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<int> health = new();

    public void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        health.Value = 100;
    }
}
