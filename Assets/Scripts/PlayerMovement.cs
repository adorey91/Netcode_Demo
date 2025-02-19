using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 5f;
    [SerializeField] private float rotationSpeed = 180f;
    [SerializeField] private GameObject ballPrefab;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Shoot();
        
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Debug.DrawRay(transform.position, forward, Color.green);
    }

    private void FixedUpdate()
    {
        Jump();

        Move(InputManager.Movement);
    }

    private void Move(Vector3 moveInput)
    {
        if(moveInput.x != 0)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveInput, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, (rotationSpeed * 10) * Time.fixedDeltaTime);
        }

        //apply multipliers
        moveInput *= playerSpeed * Time.fixedDeltaTime;
        moveInput.z = 0;
        moveInput.y = 0;

        //apply movement
        rb.MovePosition(rb.position + moveInput);
    }

    private void Jump()
    {
        if(InputManager.JumpWasPressed)
        {

        }
    }

    private void Shoot()
    {
        if(InputManager.ShootWasPressed)
        {
            Debug.Log("shoot ball");
            GameObject fireball = Instantiate(ballPrefab, Vector3.forward, Quaternion.identity);
        }
    }
}
