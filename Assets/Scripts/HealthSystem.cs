using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    [SerializeField] private int healthCount = 5;


    internal void TakeDamage()
    {
        healthCount--;
        if(healthCount == 0)
        {
            Debug.Log("player died");
        }
    }

    internal void Heal()
    {
        if (healthCount == 5) return;
        healthCount++;
    }
}
