using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Netcode;
using Michsky.MUIP;

public class UiPlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerText;
    [SerializeField] private Canvas deathPanel;
    [SerializeField] private TextMeshProUGUI deathText;
    [SerializeField] private ButtonManager[] quitButton;
    private HealthSystem _playerHealth;

    private void Start()
    {
        deathPanel.enabled = false;
        foreach (var button in quitButton)
        {
            button.onClick.AddListener(QuitGame);
        }
    }

    #region Events

    private void OnEnable()
    {
        HealthSystem.OnHealthChanged += ChangeText;
        HealthSystem.OnPlayerDeath += PlayerDeath;
    }

    private void OnDisable()
    {
        HealthSystem.OnHealthChanged -= ChangeText;
        HealthSystem.OnPlayerDeath -= PlayerDeath;
    }

    private void OnDestroy()
    {
        HealthSystem.OnHealthChanged -= ChangeText;
        HealthSystem.OnPlayerDeath -= PlayerDeath;
    }

    #endregion

    private void Respawn()
    {
        deathPanel.enabled = false;

        var player = NetworkManager.Singleton.LocalClient.PlayerObject.gameObject;
        player.GetComponent<HealthSystem>().RespawnServerRpc();
    }

    private void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
    }

    private void ChangeText(ushort health)
    {
        if (!NetworkManager.Singleton.LocalClient.PlayerObject.TryGetComponent(out HealthSystem localHealth)) return;

        if (_playerHealth == null || _playerHealth != localHealth) return; // Only update the correct player UI
    
        playerText.text = $"Player Health: {health}";
    }


    private void PlayerDeath(HealthSystem player)
    {
        StartNetwork.OnTurnOffClient?.Invoke();
        deathPanel.enabled = true;
        _playerHealth = player;
    }

    public void RespawnPlayer()
    {
        _playerHealth.RespawnServerRpc();
        deathPanel.enabled = false;
    }
}