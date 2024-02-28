using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Net.Security;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using TMPro;
using Unity.Services.Authentication;

public class SimpleMatchmaking : MonoBehaviour
{
    [SerializeField] private TMP_Text _joinCodeText;
    [SerializeField] private TMP_InputField _joinInput;
    [SerializeField] private GameObject _buttons;

    private UnityTransport _transport;
    private const int MaxPlayers = 5;

    private async void Awake() {
        _transport = FindAnyObjectByType<UnityTransport>();

        _buttons.SetActive(false);

        await Authenticate();

        _buttons.SetActive(true);
    }

    private static async Task Authenticate() {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateGame() {
        _buttons.SetActive(false);

        Allocation a = await RelayService.Instance.CreateAllocationAsync(MaxPlayers);
        _joinCodeText.text = await RelayService.Instance.GetJoinCodeAsync(a.AllocationId);

        _transport.SetHostRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData);

        NetworkManager.Singleton.StartHost();
    }

    public async void JoinGame() {
        _buttons.SetActive(false);
        JoinAllocation a = await RelayService.Instance.JoinAllocationAsync(_joinInput.text);

        _transport.SetClientRelayData(a.RelayServer.IpV4, (ushort)a.RelayServer.Port, a.AllocationIdBytes, a.Key, a.ConnectionData, a.HostConnectionData);

        NetworkManager.Singleton.StartClient();
    }
}