using UnityEngine;
using System;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private ushort healthCount = 5;

    public static Action<ushort> OnHealthChanged;
    public static Action OnPlayerDeath;
    private Vector3 _spawnLocation;

    private void Start()
    {
        _spawnLocation = transform.position;
        OnHealthChanged?.Invoke(healthCount);
    }

    internal void TakeDamage()
    {
        healthCount--;
        if (healthCount == 0)
        {
            Debug.Log("player died");
            GameOverClientRpc();
        }

        OnHealthChanged?.Invoke(healthCount);
    }

    internal void Heal()
    {
        if (healthCount == 5) return;
        healthCount++;

        OnHealthChanged?.Invoke(healthCount);
    }

    [ClientRpc]
    private void GameOverClientRpc()
    {
        if (!IsOwner) return;
        Debug.Log("game over");
        OnPlayerDeath?.Invoke();
        NetworkManager.Singleton.Shutdown();
    }

    [ServerRpc(RequireOwnership = false)]
    internal void RespawnServerRpc()
    {
        healthCount = 5;
        OnHealthChanged?.Invoke(healthCount);

        if (NetworkObject.IsSpawned != false) return;

        NetworkObject.Spawn();
        transform.position = _spawnLocation;
    }
}