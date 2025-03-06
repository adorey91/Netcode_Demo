using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;


public class StartNetwork : MonoBehaviour
{
    [SerializeField] private Canvas startNetworkCanvas;
    [SerializeField] private Canvas serverCanvas;
    [SerializeField] private Canvas clientCanvas;
    [SerializeField] private TMP_Text isHost;
    [SerializeField] private TMP_Text networkText;
    [SerializeField] private TMP_InputField networkInput;
    [SerializeField] private TMP_Text serverText;
    [SerializeField] private TMP_Text errorText;
    private UnityTransport _transport;
    public int port = 7777;

    public static Action<string> OnNetworkError;
    public static Action OnSendToMainMenu;
    public static Action OnTurnOffClient;
    public static Action OnTurnOnClient;

    private void Start()
    {
        _transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        if (_transport == null)
        {
            Debug.LogError("UnityTransport component missing from NetworkManager.");
            return;
        }

        SetConnectedText(true);
        networkInput.onEndEdit.AddListener(ValidIpInput);
    }

    private void OnEnable()
    {
        OnNetworkError +=  SetErrorText;
        OnSendToMainMenu += SentToMainMenu;
        OnTurnOffClient += TurnOffClient;
        OnTurnOnClient += TurnOnClient;
    }

    private void OnDisable()
    {
        OnNetworkError -= SetErrorText;
        OnSendToMainMenu -= SentToMainMenu;
        OnTurnOffClient -= TurnOffClient;
        OnTurnOnClient -= TurnOnClient;
    }

    private void OnDestroy()
    {
        OnNetworkError -= SetErrorText;
        OnSendToMainMenu -= SentToMainMenu;
        OnTurnOffClient -= TurnOffClient;
        OnTurnOnClient -= TurnOnClient;
    }

    private IEnumerator SetError(string message)
    {
        errorText.enabled = true;
        errorText.text = message;
        yield return new WaitForSeconds(3f);
        errorText.enabled = false;
    }
    
    private void SetErrorText(string errorMessage)
    {
        StartCoroutine(SetError(errorMessage));
    }
    
    private void TurnOffClient() => clientCanvas.enabled = false;
    
    private void TurnOnClient() => clientCanvas.enabled = true;
    
    private void SetConnectedText(bool isValid)
    {
        if (isValid)
            networkText.text = $"Enter Server IP:";
        else
            networkText.text = $"Enter Server IP:\n Invalid Input";
    }

    #region Setting Ip Address

    private string GetLocalIPAddress()
    {
        try
        {
            using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
            {
                socket.Connect("8.8.8.8", 65530); // Connect to an external address (Google DNS)
                IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                return endPoint?.Address.ToString();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Unable to get IP Address: " + ex.Message);
            return "127.0.0.1"; // Return loopback as a fallback
        }
    }

    private void ValidIpInput(string ip)
    {
        if (IsValidIPAddress(ip))
        {
            SetIPAddress(ip);
            StartClient();
        }
        else
        {
            SetConnectedText(false);
        }
    }

    private bool IsValidIPAddress(string ip)
    {
        string pattern = @"^(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\."
                         + @"(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\."
                         + @"(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])\."
                         + @"(25[0-5]|2[0-4][0-9]|1[0-9]{2}|[1-9]?[0-9])$";
        return Regex.IsMatch(ip, pattern);
    }

    private void SetIPAddress(string ip)
    {
        _transport.ConnectionData.Address = ip;
        _transport.ConnectionData.Port = 7777;
    }

    #endregion

    private void SentToMainMenu()
    {
        SetActiveCanvas(startNetworkCanvas);
    }
    public void StartServer()
    {
        string localIP = GetLocalIPAddress();

        if (!string.IsNullOrEmpty(localIP))
        {
            _transport.SetConnectionData("0.0.0.0", (ushort)port);
            Debug.Log("Starting server on " + localIP + ":" + port);
        }
        else
        {
            SetActiveCanvas(startNetworkCanvas);
            errorText.text = "Unable to find IP";
            Debug.LogError("Unable to get IP Address");
            return;
        }

        serverText.text = $"-Server-\n{GetLocalIPAddress()}";

        NetworkManager.Singleton.StartServer();
        SetActiveCanvas(serverCanvas);
    }

    private void SetActiveCanvas(Canvas canvas)
    {
        startNetworkCanvas.enabled = false;
        serverCanvas.enabled = false;
        clientCanvas.enabled = false;

        canvas.enabled = true;
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
        SetActiveCanvas(clientCanvas);
        isHost.enabled = false;
    }

    public void StartHost()
    {
        string localIP = GetLocalIPAddress();

        if (!string.IsNullOrEmpty(localIP))
        {
            _transport.SetConnectionData("0.0.0.0", (ushort)port);
            Debug.Log("Starting host on " + localIP + ":" + port);
        }
        else
        {
            SetActiveCanvas(startNetworkCanvas);
            errorText.text = "Unable to get IP Address";
            Debug.LogError("Unable to get IP Address");
            return;
        }

        isHost.text = $"Hosting on Server Address {GetLocalIPAddress()}";
        NetworkManager.Singleton.StartHost();
        SetActiveCanvas(clientCanvas);
        isHost.enabled = true;
    }
    
    public void Quit() => Application.Quit();
}