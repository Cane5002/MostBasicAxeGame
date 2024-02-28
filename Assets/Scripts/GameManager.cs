using System.Collections.Generic;
using System.Net;
using System.Resources;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public GameObject ScoreTracker;
    public GameObject StartButton;
    public List<Transform> SpawnPoints;

    private List<ulong> _activePlayers = new List<ulong>();

    private readonly NetworkVariable<GameState> _state = new();

    #region Singleton
    public static GameManager Singleton { get; private set; }
    public void Awake() {
        if (Singleton != null) Destroy(gameObject);
        Singleton = this;
    }

    public void OnApplicationQuit() {
        Singleton = null;
        Destroy(gameObject);
    }
    #endregion
/*
    public void Update() {
        
        switch(_state.Value) {
            case GameState.PreGame:
                if (NetworkManager.Singleton.ConnectedClientsIds.Count == 2) SetState(GameState.Ready);
                break;
            case GameState.Ready:
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().enabled = false;

                SpawnPoints.Shuffle();
                _activePlayers.Clear();
                int i = 0;
                foreach (var player in NetworkManager.Singleton.ConnectedClients) {
                    PlayerController p = player.Value.PlayerObject.GetComponent<PlayerController>();
                    p.gameObject.SetActive(true);
                    if (NetworkManager.Singleton.IsHost) p.transform.position = SpawnPoints[i++].position;
                    _activePlayers.Add(player.Key);
                }

                StartButton.SetActive(true);
                SetState(GameState.Standby);
                break;
            case GameState.Standby:
                break;
            case GameState.Start:
                StartButton.SetActive(false);
                NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerController>().enabled = false;

                SetState(GameState.Fight);
                break;
            case GameState.Fight:
                if (_activePlayers.Count == 1) {
                    SetState(GameState.Winner);
                }
                break;
            case GameState.Winner:
                // ScoreTracker.AddPoint(_activePlayers[0]);
                SetState(GameState.Ready);
                break;
        }
    }
*/
    public void SetState(GameState state_) {
        Debug.Log("Change State: " + state_);
        _state.Value = state_;
    }
    public void StartButtonClick() => SetState(GameState.Start);

    public void PlayerHit(ulong playerID) {
        _activePlayers.Remove(playerID);
    }
}

public enum GameState
{
    PreGame = 0,
    Ready = 1,
    Start = 2,
    Fight = 3,
    Winner = 4,
    Standby = 5
}