using Unity.Netcode;
using UnityEngine;

public class PlayerColourManager : NetworkBehaviour
{
    private Renderer _renderer;
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        Color.white, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            SetRandomColourServerRpc();
        }

        playerColor.OnValueChanged += (oldColor, newColor) =>
        {
            _renderer.material.color = newColor;
        };
    }

    [ServerRpc]
    private void SetRandomColourServerRpc()
    {
        playerColor.Value = new Color(Random.value, Random.value, Random.value);
    }
}