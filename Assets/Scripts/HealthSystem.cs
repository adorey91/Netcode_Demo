using UnityEngine;
using System;
using Unity.Netcode;

public class HealthSystem : NetworkBehaviour
{
    [SerializeField] private ushort healthCount = 5;

    public static Action <ushort> OnHealthChanged;

    internal void TakeDamage()
    {
        healthCount--;
        if(healthCount == 0)
        {
            Debug.Log("player died");
        }
        OnHealthChanged?.Invoke(healthCount);
    }

    internal void Heal()
    {
        if (healthCount == 5) return;
        healthCount++;

        OnHealthChanged?.Invoke(healthCount);
    }
}
