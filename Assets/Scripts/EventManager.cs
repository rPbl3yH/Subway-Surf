using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public Action OnPlayerDied;
    public Action OnCoinPickedUp;
    public Action OnGameStarted;

    public void PlayerDied() {
        OnPlayerDied?.Invoke();
        print("Player Died");
    }

    public void CoinPickedUp() {
        OnCoinPickedUp?.Invoke();
        print("Coin++");
    }

    public void GameStarted() {
        OnGameStarted?.Invoke();
    }
}