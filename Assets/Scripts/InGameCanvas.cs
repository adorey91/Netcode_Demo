using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    [SerializeField] private Transform scoreboard;
    [SerializeField] private GameObject playerScorePrefab;

    private void OnEnable()
    {
        NetworkPlayer.OnPlayerSpawn += OnPlayerSpawned;
    }

    private void OnDisable()
    {
        NetworkPlayer.OnPlayerSpawn -= OnPlayerSpawned;
    }

    private void OnPlayerSpawned(GameObject player)
    {
        var playerUI = Instantiate(playerScorePrefab, scoreboard);
        playerUI.GetComponent<PlayerScore>().TrackPlayer(player);
    }
}