using UnityEngine;
using System;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private ushort maxHealthCount = 5;
    private ushort _currentHealth;
    public static Action<ushort> OnHealthChanged;
    public static Action<HealthSystem> OnPlayerDeath;
    private Vector3 _spawnLocation;

    private void Start()
    {
        _currentHealth = maxHealthCount;
        _spawnLocation = transform.position;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    internal void TakeDamage()
    {
        if (_currentHealth == 0) return; // Prevent taking damage if already dead

        _currentHealth--;

        OnHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth == 0)
        {
            Debug.Log("Player died");
            DieServerRpc();
        }
    }

    internal void Heal()
    {
        if (_currentHealth == maxHealthCount) return;
        _currentHealth++;
        OnHealthChanged?.Invoke(_currentHealth);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc()
    {
        DieClientRpc();
    }

    [ClientRpc]
    private void DieClientRpc()
    {
        if (IsOwner)
        {
            Debug.Log("Game over. Waiting for respawn...");
            OnPlayerDeath?.Invoke(this); // Trigger UI to show respawn button
            gameObject.SetActive(false); // Disable player temporarily
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnServerRpc()
    {
        RespawnClientRpc();
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        Debug.Log("Respawning player");
        if (IsOwner)
        {
            Debug.Log("Respawning player...");
            StartNetwork.OnTurnOnClient?.Invoke();
            _currentHealth = maxHealthCount;
            OnHealthChanged?.Invoke(_currentHealth);

            transform.position = _spawnLocation; // Reset position
            gameObject.SetActive(true); // Re-enable player
        }
    }
}