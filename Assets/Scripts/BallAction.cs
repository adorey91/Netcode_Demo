using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class BallAction : NetworkBehaviour
{    
    private PlayerMovement _player;
    private ulong _ownerClientId;
    [SerializeField] private float lifetime = 5f;
    private float _timer;
    private Rigidbody _rb;
    [ReadOnly] private float _speed = 10f;
    private Vector3 _direction;

    private void Start()
    {
        _timer = 0;
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = false;
            _rb.velocity = _direction * _speed;
    }

    internal void SetOwner(ulong owner)
    {
        _ownerClientId = owner;
    }

    internal void SetDirection(Vector3 shootDirection)
    {
        _direction = shootDirection.normalized;
    }

    private void Update()
    {
        if (_timer >= lifetime)
        {
            if (IsServer) // Only the server should despawn objects
                DespawnBallServerRPC();
        }
        else
            _timer += Time.deltaTime;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var hitPlayerNetObj = collision.gameObject.GetComponent<NetworkObject>();

            // Ensure we are checking against a valid NetworkObject
            if (hitPlayerNetObj == null) return;

            // Check if the hit player is NOT the owner
            if (hitPlayerNetObj.OwnerClientId == _ownerClientId) return;
            collision.gameObject.GetComponent<HealthSystem>()?.TakeDamage();
            DespawnBallServerRPC();
        }
        else
        {
            DespawnBallServerRPC();
        }
    }


    [ServerRpc(RequireOwnership = false)]
    private void DespawnBallServerRPC()
    {
        if(TryGetComponent<NetworkObject>(out var ball))    
            ball.Despawn();
    }
}
