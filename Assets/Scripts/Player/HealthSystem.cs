using UnityEngine;
using System;
using System.Collections;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private ushort maxHealthCount = 5;
    public NetworkVariable<ushort> health = new NetworkVariable<ushort>();
    [SerializeField] private ushort currentHealth;
    private Vector3 _spawnLocation;
    public static Action<ushort> OnHealthChanged;
    public static Action<HealthSystem> OnPlayerDeath;

    private void Start()
    {
        if (IsOwner) // Ensure only the local player subscribes to UI updates
        {
            health.OnValueChanged += (oldHealth, newHealth) =>
            {
                OnHealthChanged?.Invoke(newHealth);
            };
        }

        currentHealth = maxHealthCount;
        _spawnLocation = transform.position;
    }


    internal void TakeDamage()
    {
        if (currentHealth == 0) return; // Prevent taking damage if already dead

        PlayerSettingUI.OnDamageTaken?.Invoke();
        
        if (IsServer)
        {
            health.Value--; // Update NetworkVariable on the server
            OnHealthChanged?.Invoke(health.Value);
        }
        else
        {
            TakeDamageServerRpc(); // If not server, request server to handle the health update
        }

        if (health.Value == 0)
        {
            Debug.Log("Player died");
            DieServerRpc();
        }
    }
    
    [ServerRpc(RequireOwnership = false)]
    private void TakeDamageServerRpc()
    {
        if (!IsServer) return;

        health.Value--; 
        OnHealthChanged?.Invoke(health.Value);
    }


    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc()
    {
        // Disable the player on the server
        if (!IsServer) return;

        Debug.Log($"Disabling player {OwnerClientId} on the server.");
        gameObject.SetActive(false); // Disables player on the server

        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        Debug.Log($"Player {OwnerClientId} died.");

        if (IsOwner)
        {
            Debug.Log("Game over. Waiting for respawn...");
            OnPlayerDeath?.Invoke(this); // Trigger UI to show respawn button
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc()
    {
        if (!IsServer) return;

        health.Value = maxHealthCount; // Reset health on the server
        OnHealthChanged?.Invoke(health.Value);

        gameObject.SetActive(true);
        RespawnClientRpc();
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        if (IsOwner)
        {
            Debug.Log("Respawning player...");
            StartNetwork.OnTurnOnClient?.Invoke();
        
            transform.position = _spawnLocation; // Reset position
        }

        gameObject.SetActive(true);
    }

}