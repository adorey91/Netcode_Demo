using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PlayerMovement : NetworkBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private GameObject ballPrefab;

    private HealthSystem healthSystem;

    private Rigidbody rb;
    private Vector3 movement;
    private bool isGrounded = false;

    private bool isJumping = false;
    private Vector3 forward;

    private float moveX;

    [SerializeField] private LayerMask layerMask;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        if (IsHost || NetworkObjectId == 1)
            transform.SetLocalPositionAndRotation(new Vector3(14, 1, 0), Quaternion.Euler(0, -90, 0));
        else
            transform.SetLocalPositionAndRotation(new Vector3(-14, 1, 0), Quaternion.Euler(0, 90, 0));
    }

    private void Start()
    {
        healthSystem = GetComponent<HealthSystem>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isJumping && rb.velocity.y == 0)
            isJumping = false;

        forward = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, forward, Color.green);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    public void Move(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            moveX = context.ReadValue<Vector2>().x;
        }
    }

    private void Movement()
    {
        Turn(moveX);
        moveX *= playerSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + new Vector3(moveX, 0, 0));
    }

    private void Turn(float moveInput)
    {
        if (moveInput < 0)
            transform.rotation = Quaternion.Euler(0, -90, 0);
        else if (moveInput > 0)
            transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (Physics.Raycast(transform.position, Vector3.down, 1.1f, layerMask))
            {
                isGrounded = true;
            }

            if (!isJumping & isGrounded)
            {
                isJumping = true;
                rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            }
            else
            {
                Debug.Log("Already jumping");
            }
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("shoot ball");
            GameObject fireball = Instantiate(ballPrefab, transform.position + forward, transform.rotation);
            fireball.GetComponent<BallAction>().SetOwner(gameObject);
            fireball.GetComponent<Rigidbody>().isKinematic = false;
            fireball.GetComponent<Rigidbody>().velocity = transform.right * 10;
        }
    }
}
