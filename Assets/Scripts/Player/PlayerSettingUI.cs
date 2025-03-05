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
    
    [SerializeField] private GameObject healthText;
    [SerializeField] private TextMeshProUGUI playerNameText;
    private NetworkVariable<FixedString128Bytes> _networkPlayerName = new NetworkVariable<FixedString128Bytes>("Player: 0", NetworkVariableReadPermission.Everyone);

    [SerializeField] public List<Color> playerColors;
    
    #region Events
    private void OnEnable()
    {
        OnDamageTaken += ShowDamageText;
    }

    private void OnDisable()
    {
        OnDamageTaken -= ShowDamageText;
    }

    private void OnDestroy()
    {
        OnDamageTaken -= ShowDamageText;
    }
    #endregion

    public override void OnNetworkSpawn()
    {
        _networkPlayerName.Value = $"Player: {OwnerClientId + 1}";
        playerNameText.text = _networkPlayerName.Value.ToString();
        meshRenderer.material.color = playerColors[(int)OwnerClientId];
        healthText.SetActive(false);
    }
    
    private void ShowDamageText()
    {
        Debug.Log("ShowDamageText called");
        if (IsOwner)
        {
            Debug.Log("Calling ShowDamageTextServerRpc");
            ShowDamageTextServerRpc();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void ShowDamageTextServerRpc()
    {
        Debug.Log("Showing damage text");
        healthText.SetActive(true);
        StartCoroutine(HideDamageTextAfterDelay());
    }

    private IEnumerator HideDamageTextAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        healthText.SetActive(false);
    }

}
