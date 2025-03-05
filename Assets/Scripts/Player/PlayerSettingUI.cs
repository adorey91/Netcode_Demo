using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerSettingUI : NetworkBehaviour
{
    public static Action OnDamageTaken;
    [SerializeField] private MeshRenderer meshRenderer;
    
    [SerializeField] private TextMeshProUGUI playerNameText;
    private NetworkVariable<FixedString128Bytes> _networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone);

    [SerializeField] public List<Color> playerColors;

    [SerializeField] private RectTransform healthBar;

    private NetworkHealthState netHealth;
    

    public override void OnNetworkSpawn()
    {
        netHealth = GetComponent<NetworkHealthState>();
        _networkPlayerName.Value = $"Player: {OwnerClientId + 1}";
        playerNameText.text = _networkPlayerName.Value.ToString();
        meshRenderer.material.color = playerColors[(int)OwnerClientId];
    }


    private void OnEnable()
    {
        netHealth.HealthPoint.OnValueChanged += HealthChanged;
    }

    private void OnDisable()
    {
        netHealth.HealthPoint.OnValueChanged -= HealthChanged;
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        healthBar.transform.localScale = new Vector3((newValue / 100f), 1, 1);
    }
}
