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
    [SerializeField] private Button respawnButton;
    [SerializeField] private ButtonManager[] quitButton;

    private void Start()
    {
        deathPanel.enabled = false;
        // respawnButton.onClick.AddListener(Respawn);
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
        playerText.text = $"Player Health: {health.ToString()}";
    }

    private void PlayerDeath()
    {
        deathPanel.enabled = true;
        deathText.text = "You died \n Play Again?";
    }
}