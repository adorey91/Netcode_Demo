using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : NetworkBehaviour
{
    // Variables
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 10f;
[SerializeField] private float jumpForce = 10f;
    private Vector3 _inputMovement;
    private Rigidbody _rb;
    private bool _jumpRequest;
    
    private NetworkCommunication _networkCommunication;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _networkCommunication = GetComponent<NetworkCommunication>();
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
        if(!IsOwner) return;
        
        //send movement input to server
        _networkCommunication.MovePlayerServerRpc(_inputMovement, playerSpeed);

        if (_jumpRequest)
        {
            _networkCommunication.PlayerJumpServerRpc(jumpForce);
            _jumpRequest = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Should only be called by the server
        if(!IsServer) return;
        
        // checks for projectile and to make sure it's not owned by themselves.
        if (other.GetComponent<Projectile>() && GetComponent<NetworkObject>().OwnerClientId != other.GetComponent<NetworkObject>().OwnerClientId)
        {
            GetComponent<NetworkHealthState>().health.Value -= 10;
        }
    }
}