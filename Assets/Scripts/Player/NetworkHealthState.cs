using Unity.Netcode;
using UnityEngine;

public class NetworkHealthState : NetworkBehaviour
{
    [HideInInspector]
    public NetworkVariable<int> health = new(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn() // Ensure override is here
    {
        base.OnNetworkSpawn();
        
        if (IsServer) // Ensure only the server initializes the health
        {
            health.Value = 100;
        }
    }
}