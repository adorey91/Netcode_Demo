using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    // Variables
    [Header("Movement")]
    public GameObject playerObject;

    public Canvas playerCanvas;
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private float jumpForce = 10f;
    private Vector3 _inputMovement;
    private Rigidbody _rb;
    private bool _jumpRequest;

    private NetworkVariable<bool> isDead = new(false, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    private NetworkHealthState _healthState;
    private NetworkCommunication _networkCommunication;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _networkCommunication = GetComponent<NetworkCommunication>();
        _healthState = GetComponent<NetworkHealthState>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (!IsOwner) this.enabled = false;
    }

    public void Move(InputAction.CallbackContext context)
    {
        _inputMovement = context.ReadValue<Vector2>();
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jumpRequest = true;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner) return;
        if (isDead.Value) return;

        //send movement input to server
        _networkCommunication.MovePlayerServerRpc(_inputMovement, playerSpeed);

        if (_jumpRequest)
        {
            _networkCommunication.PlayerJumpServerRpc(jumpForce);
            _jumpRequest = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        if (collision.gameObject.GetComponent<Projectile>() && GetComponent<NetworkObject>().OwnerClientId !=
            collision.gameObject.GetComponent<NetworkObject>().OwnerClientId)
        {
            Debug.Log("Collision Enter");
            var networkObject = collision.gameObject.GetComponent<NetworkObject>();
            _healthState.health.Value -= 10;

            if (_healthState.health.Value <= 0)
            {
                DieServerRpc();
            }

            networkObject.Despawn();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void DieServerRpc()
    {
        if (!IsServer) return;

        Debug.Log($"Player {OwnerClientId} died. Respawning...");
        isDead.Value = true; // **Syncs across all clients**

        // **Disable physics and rendering**
        _rb.isKinematic = true;
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        playerObject.SetActive(false);
        playerCanvas.enabled = false;

        // **Notify the GameManager to handle respawn**
        GameManager.Instance.RespawnPlayer(this);
    }

    [ClientRpc]
    private void RespawnClientRpc()
    {
        // **Re-enable everything on the client**
        playerObject.SetActive(true);
        playerCanvas.enabled = true;
        _rb.isKinematic = false;
    }


    public void Respawn()
    {
        isDead.Value = false; // **Syncs across clients**
        RespawnClientRpc();
    }
}