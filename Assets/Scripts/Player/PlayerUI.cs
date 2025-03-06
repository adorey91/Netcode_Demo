using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerUI : NetworkBehaviour
{
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private TextMeshProUGUI nameText;
private PlayerData _playerData;
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        _playerData = GetComponent<PlayerData>();
        
        if (_playerData != null)
        {
            // **Set initial name**
            nameText.text = _playerData.Name.Value.ToString();

            // **Listen for changes to the name**
            _playerData.Name.OnValueChanged += (oldValue, newValue) =>
            {
                nameText.text = newValue.ToString();
            };
        }
    }
    
    private void OnEnable()
    {
        GetComponent<NetworkHealthState>().health.OnValueChanged += HealthChanged;
    }

    private void OnDisable()
    {
        GetComponent<NetworkHealthState>().health.OnValueChanged -= HealthChanged;
    }

    private void HealthChanged(int previousValue, int newValue)
    {
        healthBar.transform.localScale = new Vector3(Mathf.Clamp(newValue / 100f, 0, 1), 1, 1);
    }

}