using Unity.Netcode;
using UnityEngine;
using System;

public class NetworkPlayer : NetworkBehaviour
{
    public static Action<GameObject> OnPlayerSpawn;
    public static Action<GameObject> OnPlayerDespawn;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        OnPlayerSpawn?.Invoke(gameObject);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();
        OnPlayerDespawn?.Invoke(gameObject);
    }
}
