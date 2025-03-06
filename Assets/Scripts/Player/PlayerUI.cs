using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private RectTransform healthBar;

    private void OnEnable()
    {
        GetComponent<NetworkHealthState>().health.OnValueChanged += HealthChanged;
    }

    private void OnDisable()
    {
        GetComponent<NetworkHealthState>().health.OnValueChanged -= HealthChanged;
    }

    private void HealthChanged(int previousvalue, int newvalue)
    {
        healthBar.transform.localScale = new Vector3(newvalue / 100f, 1, 1);
    }
}