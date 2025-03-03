using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 5f;

    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private GameObject ballPrefab;

    [Header("Health")]
    [SerializeField] private int maxHealth = 5;

    private int _currentHealth;

    private Rigidbody _rb;
    private Vector2 _movement;
    private bool _isGrounded;
    private Vector3 _forward;
    private Keyboard _keyboard;
    private Gamepad _gamepad;

    [SerializeField] private LayerMask layerMask;
    public static Action<int> OnHealthChanged;
    public static Action OnPlayerDeath;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }
    }

    private void Start()
    {
        _currentHealth = maxHealth;
        _rb = GetComponent<Rigidbody>();
        _keyboard = Keyboard.current;
        
        if(Gamepad.current != null)
            _gamepad = Gamepad.current;
        
        OnHealthChanged?.Invoke(maxHealth);
    }

    private void Update()
    {
        if (_keyboard.spaceKey.wasPressedThisFrame || _gamepad.rightTrigger.wasPressedThisFrame) Shoot();

        if (_keyboard.upArrowKey.wasPressedThisFrame || _keyboard.wKey.wasPressedThisFrame || _gamepad.aButton.wasPressedThisFrame) Jump();
    }

    private void FixedUpdate()
    {
        _forward = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, _forward, Color.green);
        Movement();
    }

    public void Move(InputAction.CallbackContext context)
    {
        _movement = context.ReadValue<Vector2>();
    }

    private void Movement()
    {
        if (_movement.x != 0)
        {
            Turn(_movement.x);
            _rb.velocity = new Vector3(_movement.x * playerSpeed, _rb.velocity.y, 0);
        }
        else
        {
            _rb.velocity = new Vector3(0, _rb.velocity.y, 0); // Stop when no input
        }
    }


    private void Turn(float moveInput)
    {
        transform.rotation = moveInput switch
        {
            < 0 => Quaternion.Euler(0, -90, 0),
            > 0 => Quaternion.Euler(0, 90, 0),
            _ => transform.rotation
        };
    }

    public void Jump()
    {
        // if (Physics.Raycast(transform.position, Vector3.down, 1.1f, layerMask))
        //     _isGrounded = true;
        //
        // if (_isGrounded)
            _rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
        // else
        //     Debug.Log("Already jumping");
    }

    public void Shoot()
    {
        ShootServerRPC();
        //Debug.Log("shoot ball");
        //GameObject fireball = Instantiate(ballPrefab, transform.position + forward, transform.rotation);
        //BallAction ballAction = fireball.GetComponent<BallAction>();
        //ballAction.SetOwner(gameObject);
        //ballAction.SetDirection(forward);
    }

    [ServerRpc]
    private void ShootServerRPC(ServerRpcParams rpcParams = default)
    {
        var fireball = Instantiate(ballPrefab, transform.position + transform.forward * 1.5f, transform.rotation);

        if (!fireball.TryGetComponent<NetworkObject>(out var fireballNetObj)) return;

        fireballNetObj.Spawn(true);

        // Ensure owner is set properly
        var ballAction = fireball.GetComponent<BallAction>();
        ballAction.SetOwner(rpcParams.Receive.SenderClientId); // Use SenderClientId to track the correct owner
        ballAction.SetDirection(_forward);
    }
}