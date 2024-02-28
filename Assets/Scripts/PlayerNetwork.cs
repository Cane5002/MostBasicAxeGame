using Unity.Netcode;
using UnityEngine;

public class PlayerNetwork : NetworkBehaviour
{
    private readonly NetworkVariable<Vector2> _netPos = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> _netEmit = new(writePerm: NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> _netHit = new(writePerm: NetworkVariableWritePermission.Server);
    private readonly NetworkVariable<bool> _netParry = new(writePerm: NetworkVariableWritePermission.Owner);

    private TrailRenderer _trail;
    private ParticleSystem _parryEffect;

    public ulong PlayerID;
    public bool IsPlayer;

    private void Awake() {
        _trail = GetComponent<TrailRenderer>();
        _parryEffect = GetComponent<ParticleSystem>();
        _netHit.Value = false;
    }

    private const int ENEMYLAYER = 6;
    public override void OnNetworkSpawn() {
        PlayerID = GetComponent<NetworkObject>().OwnerClientId;
        if (!IsOwner) {
            gameObject.layer = ENEMYLAYER;
            Destroy(GetComponent<PlayerController>());
        }
    }

    void Update()
    {
        if (!IsOwner) {
            transform.position = _netPos.Value;
        }
        else {
            _netPos.Value = transform.position;
        }

        _trail.emitting = _netEmit.Value;
        GetComponent<SpriteRenderer>().enabled = !_netHit.Value;
    }

    public void SetPlayerID(ulong playerID_) {
        PlayerID = playerID_;
    }

    public void SetTrailEmit(bool emit) {
        _netEmit.Value = emit;
    }

    public void SetHit(bool hit) {
        if (IsServer) _netHit.Value = hit;
        else SetHitServerRpc(hit);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetHitServerRpc(bool hit) {
        _netHit.Value = hit;
    }
    public bool GetHit() {
        return _netHit.Value;
    }

    public void SetParry(bool parry) {
        _netParry.Value = parry;
        if (parry) ParryServerRpc();
    }
    public bool GetParry() {
        return _netParry.Value;
    }
    [ServerRpc]
    private void ParryServerRpc() {
        ParryClientRpc();
    }
    [ClientRpc]
    private void ParryClientRpc() {
        _parryEffect.Play();
    }
}
