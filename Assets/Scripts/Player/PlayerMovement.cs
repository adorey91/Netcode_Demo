using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : NetworkBehaviour
{
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 5f;

    private Rigidbody _rb;
    private Vector2 _movement;
    private bool _isGrounded;
    private Keyboard _keyboard;
    private Gamepad _gamepad;

    [SerializeField] private LayerMask layerMask;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        _keyboard = Keyboard.current;
        
        if(Gamepad.current != null)
            _gamepad = Gamepad.current;
    }

    private void Update()
    {
        if(!IsOwner) return;
        
        GroundCheck(); // Ensure _isGrounded is updated

        if (_gamepad != null)
        {
            if (_gamepad.aButton.wasPressedThisFrame && _isGrounded) Jump();
        }
    
        if ((_keyboard.upArrowKey.wasPressedThisFrame || _keyboard.wKey.wasPressedThisFrame) && _isGrounded) Jump();
    }

    private void GroundCheck()
    {
        // Adjust based on collider size
        float rayLength = GetComponent<Collider>().bounds.extents.y + 0.1f;
        _isGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength, layerMask);
    }


    private void FixedUpdate()
    {
        if(!IsOwner) return;
       
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

    private void Jump()
    {
        if (_isGrounded)
            _rb.AddForce(Vector3.up * 10, ForceMode.Impulse);
    }
}