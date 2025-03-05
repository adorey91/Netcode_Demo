using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShootFireball : NetworkBehaviour
{
    [SerializeField] private GameObject fireballPrefab;
    private Gamepad _gamepad;
    private Keyboard _keyboard;

    private void Start()
    {
        if (Gamepad.current != null)
            _gamepad = Gamepad.current;
        _keyboard = Keyboard.current;
    }


    private void Update()
    {
        if (!IsOwner) return;

        if (_gamepad != null)
        {
            if (_gamepad.rightTrigger.wasPressedThisFrame) Shoot();
        }

        if (_keyboard.spaceKey.wasPressedThisFrame) Shoot();
    }

    private void Shoot()
    {
        if (IsOwner) // Only the local player should send the request
        {
            ShootFireballServerRpc(transform.forward);
        }
    }

    [ServerRpc(RequireOwnership = false)] // Allow non-owners to call this ServerRpc
    private void ShootFireballServerRpc(Vector3 direction, ServerRpcParams rpcParams = default)
    {
        ulong ownerClientId = rpcParams.Receive.SenderClientId; // Get the shooter client ID

        // Debug.Log($"Spawning Fireball for owner {ownerClientId}");

        var fireball = Instantiate(fireballPrefab, transform.position, transform.rotation);

        if (!fireball.TryGetComponent(out NetworkObject networkObject)) return;
        networkObject.Spawn();

        if (!fireball.TryGetComponent(out BallAction ballAction)) return;
    
        ballAction.SetOwner(ownerClientId);
        ballAction.SetDirection(direction);
    }

}