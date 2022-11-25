using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [field: SerializeField] public GroundController GroundController { private set; get; }
    [field: SerializeField] public EventManager EventManager { private set; get; }
    [field: SerializeField] public UIController UIController { private set; get; }
    [field: SerializeField] public AudioController AudioController { private set; get; }

    [SerializeField] private PlayerController _playerController;

    public bool IsGameOver;
    public bool IsStarted;

    [SerializeField] private float _scoreMultiplier = 0.1f;

    public int Score, Coins;
    public int BestScore, BestCoins;

    private void Awake() {
        if(Instance == null) {
            Instance = this;
        }
        if (PlayerPrefs.HasKey("Score")) {
            BestScore = PlayerPrefs.GetInt("Score");
        }
        if (PlayerPrefs.HasKey("Coins")) {
            BestCoins = PlayerPrefs.GetInt("Coins");
        }
    }

    private void Start() {
        EventManager.OnCoinPickedUp += OnCoinPickedUp;
        EventManager.OnPlayerDied += OnPlayerDied;
        EventManager.OnGameStarted += OnGameStarted;
        UpdateTexts();
        _playerController.gameObject.SetActive(false);
    }

    private void OnGameStarted() {
        _playerController.gameObject.SetActive(true);
        IsStarted = true;
    }

    private void OnCoinPickedUp() {
        Coins++;
        if(Coins > BestCoins) {
            PlayerPrefs.SetInt("Coins", Coins);
            BestCoins = Coins;
        }
        UIController.UpdateCoinsText(Coins);
    }

    private void Update() {
        if (IsGameOver) return;
        if (IsStarted) {
            Score += (int)(GroundController.Speed * _scoreMultiplier);
            if (Score > BestScore) {
                PlayerPrefs.SetInt("Score", Score);
                BestScore = Score;
            }
            UIController.UpdateScoreText(Score);
        }
    }

    private void OnPlayerDied() {
        
        GameOver();
    }

    private void GameOver() {
        //Time.timeScale = 0f;
        IsGameOver = true;
        UIController.ViewLoseMenu();
    }

    public void RestartGame() {
        GroundController.ResetRoad();
        _playerController.ResetPlayer();
        IsGameOver = false;
        IsStarted = false;
        UpdateTexts();
    }

    private void UpdateTexts() {
        UIController.UpdateCoinsText(Coins);
        UIController.UpdateScoreText(Score);
    }
}
