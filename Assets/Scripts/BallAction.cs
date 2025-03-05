using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class BallAction : NetworkBehaviour
{    
    private PlayerMovement _player;
    [SerializeField] private float lifetime = 5f;
    private float _timer;
    [SerializeField] private Rigidbody rb;
    [ReadOnly] private float _speed = 10f;
    private Vector3 _direction;
    
    private NetworkVariable<ulong> _ownerClientId = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private void Start()
    {
        _timer = 0;
        rb.isKinematic = false;
        rb.WakeUp();

        if (_direction != Vector3.zero)
            rb.velocity = _direction * _speed;
    }


    internal void SetOwner(ulong owner)
    {
        if(!IsServer) return;
        
        _ownerClientId.Value = owner;
        Debug.Log($"Fireball Owner Set: {_ownerClientId.Value}");
    }

    internal void SetDirection(Vector3 shootDirection)
    {
        _direction = shootDirection.normalized;
        if (rb)
        {
            rb.velocity = _direction * _speed;
        }

        if (IsOwner) // Apply instant movement on the client who shot it
        {
            rb.velocity = _direction * _speed;
        }

        // Let the server take control of physics afterward
        if (IsServer)
        {
            rb.velocity = _direction * _speed;
        }
    }


    private void Update()
    {
        if (IsServer) // Ensure this runs only on the server
        {
            if (!IsOwnerAlive()) 
            {
                DespawnBallServerRPC();
                return;
            }
        }

        if (_timer >= lifetime)
        {
            if (IsServer) // Only the server should despawn objects
                DespawnBallServerRPC();
        }
        else
            _timer += Time.deltaTime;
    }

    private bool IsOwnerAlive()
    {
        if (!IsServer) return true;

        return NetworkManager.Singleton.ConnectedClients.ContainsKey(_ownerClientId.Value) &&
               NetworkManager.Singleton.ConnectedClients[_ownerClientId.Value].PlayerObject != null &&
               NetworkManager.Singleton.ConnectedClients[_ownerClientId.Value].PlayerObject.gameObject.activeInHierarchy;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var hitPlayerNetObj = collision.gameObject.GetComponent<NetworkObject>();
        if (hitPlayerNetObj == null) return;

        Debug.Log($"Fireball hit {hitPlayerNetObj.OwnerClientId}, Fireball owner: {_ownerClientId}");

        if (hitPlayerNetObj.OwnerClientId == _ownerClientId.Value) 
        {
            Debug.Log("Ignoring hit on owner.");
            return;
        }

        collision.gameObject.GetComponent<HealthSystem>()?.TakeDamage();
        DespawnBallServerRPC();
    }

    private bool _isDespawning = false;

    [ServerRpc(RequireOwnership = false)]
    private void DespawnBallServerRPC()
    {
        if (_isDespawning) return; // Prevent multiple calls
        _isDespawning = true;
    
        if (TryGetComponent<NetworkObject>(out var ball))
            ball.Despawn();
    }

}
