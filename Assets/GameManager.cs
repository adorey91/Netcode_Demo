using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RespawnPlayer(PlayerScript player)
    {
        StartCoroutine(RespawnCoroutine(player));
    }

    private IEnumerator RespawnCoroutine(PlayerScript player)
    {
        yield return new WaitForSeconds(3f); // 3 seconds respawn delay

        if (player != null && player.NetworkObject.IsSpawned)
        {
            Debug.Log($"Respawning {player.OwnerClientId}...");

            // Reset player health
            player.GetComponent<NetworkHealthState>().health.Value = 100;

            // Move player back to correct spawn
            string playerName = ConnectionApprovalHandler.GetPlayerName(player.OwnerClientId);
            if (playerName == "Player 1")
                player.transform.position = new Vector3(-12, 1, 0);
            else
                player.transform.position = new Vector3(12, 1, 0);

            // Call respawn function (syncs across clients)
            player.Respawn();
        }
    }
}