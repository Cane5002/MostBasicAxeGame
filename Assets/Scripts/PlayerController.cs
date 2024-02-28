using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Net;

public class PlayerController : MonoBehaviour
{
    private PlayerNetwork _playerNet;

    [SerializeField]
    private Rigidbody2D _rb;
    
    public int Velocity = 80;

    private const int ENEMYLAYER = 6;

    public int MaxDashDistance = 10;
    public float DashCooldown = 2;
    private float _lastDashTime;
    private bool _canDash = true;
    [SerializeField]
    private TrailRenderer _trail;
    public float TrailTime = 0.5f;

    public float ParryCooldown = 3;
    public float ParryDuration = 0.2f;
    private float _lastParryTime;
    private bool _canParry = true;
    

    public float RespawnTime = 4;
    private float _respawnTimer = 0;

    private MouseTracker _mouse;

    // public ulong PlayerID;

    private void Awake() {
        _mouse = GameObject.FindGameObjectsWithTag("Mouse")[0].GetComponent<MouseTracker>();
        _playerNet = GetComponent<PlayerNetwork>();
    }

    private void Update() {
        if (_playerNet.GetHit()) {
            _respawnTimer += Time.deltaTime;
            if (_respawnTimer >= RespawnTime) {
                _respawnTimer = 0;
                _playerNet.SetHit(false);
                GetComponent<SpriteRenderer>().enabled = true;
            }
            else return;
        }

        if (_canDash) {
            if (Input.GetButtonDown("Fire1")) Dash();
        }
        else {
            _lastDashTime += Time.deltaTime;
            if (_lastDashTime >= DashCooldown) {
                _canDash = true;
                _lastDashTime = DashCooldown;
            }
            else if (_lastDashTime >= TrailTime) {
                _playerNet.SetTrailEmit(false);
            }
            _mouse.SetCooldown(_lastDashTime / DashCooldown);
        }

        if (_canParry) {
            if (Input.GetButtonDown("Fire2")) Parry();
        }
        else
        {
            _lastParryTime += Time.deltaTime;
            if (_lastParryTime >= ParryCooldown) {
                _canParry = true;
                _lastParryTime = ParryCooldown;
            }
            else if (_lastParryTime >= ParryDuration) {
                _playerNet.SetParry(false);
            }
        }
    }

    void FixedUpdate()
    {
        float verticalMovement = Input.GetAxis("Vertical");
        float horizontalMovement = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(horizontalMovement, verticalMovement, 0);
        if (movement.magnitude > 1) movement.Normalize();
        if (movement.magnitude > 0) Debug.Log("Moving");

        _rb.velocity = movement * Velocity * Time.deltaTime;
    }

    void Dash() {
        Vector3 direction = _mouse.GetPosition() - transform.position;
        if (direction.magnitude > MaxDashDistance) direction = direction.normalized * MaxDashDistance;

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        if (Physics2D.Raycast(transform.position, direction, new ContactFilter2D(), hits, direction.magnitude) > 0) {
            Debug.DrawRay(transform.position, direction, Color.blue, 1);
            foreach (var h in hits) {
                if (h.collider.gameObject.layer != ENEMYLAYER) continue;
                Debug.DrawRay(transform.position, h.transform.position - transform.position, Color.green, 1);
                Debug.Log("Hit");
                Debug.Log(h.rigidbody.name);
                
                PlayerNetwork p = h.rigidbody.GetComponent<PlayerNetwork>();
                if(p != null) {
                    if (p.GetParry()) _playerNet.SetHit(true);
                    else p.SetHit(true);
                }
            }
        }
        else {
            Debug.DrawRay(transform.position, direction, Color.red, 1);
            Debug.Log("Miss");
        }   

        transform.Translate(direction);

        _canDash = false;
        _lastDashTime = 0;
        _playerNet.SetTrailEmit(true);
    }

    void Parry() {
        Debug.Log("Parry");
        _canParry = false;
        _lastParryTime = 0;
        _playerNet.SetParry(true);
    }

}
