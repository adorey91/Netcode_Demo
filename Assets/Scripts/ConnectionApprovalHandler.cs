using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ConnectionApprovalHandler : NetworkBehaviour
{
    private const int MaxPlayers = 2;
    private bool _hostConnected;
    private ulong _firstClientId = ulong.MaxValue;

    private void Start()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        _hostConnected = false;
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request,
        NetworkManager.ConnectionApprovalResponse response)
    {
        var clientId = request.ClientNetworkId;
        bool isHost = NetworkManager.Singleton.IsHost;
        bool isServer = NetworkManager.Singleton.IsServer;
        Debug.Log($"Client ID: {clientId}");

        response.Approved = true;
        response.CreatePlayerObject = true;
        response.PlayerPrefabHash = null;
        
        if (NetworkManager.Singleton.ConnectedClients.Count >= MaxPlayers)
        {
            response.Approved = false;
            response.Reason = "Server is full";
            StartNetwork.OnSendToMainMenu?.Invoke();
            StartNetwork.OnNetworkError?.Invoke("Server is full");
        }
        
        if (clientId == 0) // Host case
        {
            _hostConnected = true;
            response.Position = new Vector3(-12, 1, 0); // Left side
            response.Rotation = Quaternion.Euler(0, 90, 0);
        }
        else
        {
            // **If it's the first client (1) in dedicated server mode, they take the left side**
            if (!_hostConnected && _firstClientId == ulong.MaxValue)
            {
                _firstClientId = clientId;
                response.Position = new Vector3(-12, 1, 0); // Left side
                response.Rotation = Quaternion.Euler(0, 90, 0);
            }
            // **The second player (or first client when there's a host) goes to the right**
            else
            {
                response.Position = new Vector3(12, 1, 0); // Right side
                response.Rotation = Quaternion.Euler(0, -90, 0);
            }
        }

        Debug.Log($"Client {clientId} assigned position {response.Position}");
        response.Pending = false;
    }
}
