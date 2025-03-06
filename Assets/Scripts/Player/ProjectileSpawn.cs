using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class ProjectileSpawn : NetworkBehaviour
{
   [SerializeField] private GameObject projectilePrefab;
   [SerializeField] private Transform initialTransform;

  public void Shoot(InputAction.CallbackContext context)
   {
      if(!IsOwner) return;
      
      if (context.performed)
      {
         SpawnProjectileServerRpc(initialTransform.position, initialTransform.rotation);
      }
   }


   [ServerRpc]
   private void SpawnProjectileServerRpc(Vector3 position, Quaternion rotation, ServerRpcParams serverRpcParams = default)
   {
      GameObject obj = Instantiate(projectilePrefab, position, rotation);
      obj.GetComponent<NetworkObject>().SpawnWithOwnership(serverRpcParams.Receive.SenderClientId);
   }
}
