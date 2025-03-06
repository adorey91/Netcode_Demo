using System;
using Unity.Netcode;
using UnityEngine;

public class NetworkCommunication : NetworkBehaviour
{
   private Rigidbody _rb;   
   [SerializeField] private LayerMask groundLayer;
   private bool _isGrounded;
   
   private void Awake()
   {
      _rb = GetComponent<Rigidbody>();
   }

   private void FixedUpdate()
   {
      if(!IsServer)  return;
      GroundCheck();
   }
   
   [ServerRpc]
   public void MovePlayerServerRpc(Vector2 input, float moveSpeed)
   {
      if (!IsServer) return;

      Vector3 moveDirection = new Vector3(input.x, 0, 0);
      _rb.velocity = new Vector3(moveDirection.x * moveSpeed, _rb.velocity.y, _rb.velocity.z);
        
      // Handle rotation
      if (input.x < 0)
         transform.rotation = Quaternion.Euler(0, -90, 0);
      else if (input.x > 0)
         transform.rotation = Quaternion.Euler(0, 90, 0);
   }

   [ServerRpc]
   public void PlayerJumpServerRpc(float jumpForce)
   {
      if (!IsServer || !_isGrounded) return;

      _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
   }

   private void GroundCheck()
   {
      float rayLength = GetComponent<Collider>().bounds.extents.y + 0.1f;
      _isGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength, groundLayer);
   }
}


