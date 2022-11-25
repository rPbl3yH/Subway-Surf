using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField] private TMP_Text _textCoins;
    [SerializeField] private TMP_Text _textScore;
    [SerializeField] private TMP_Text _textBestCoins;
    [SerializeField] private TMP_Text _textBestScore;
    [SerializeField] private TMP_Text _textEndCoins;
    [SerializeField] private TMP_Text _textEndScore;


    [SerializeField] private GameObject _loseMenu;
    [SerializeField] private GameObject _startMenu;
    [SerializeField] private GameObject _gameMenu;

    [SerializeField] private Image _fadePanel;
    [SerializeField] private float _timeRateToFade = 0.05f;
    [SerializeField] private float _fadeRate = 0.05f;

    private void Start() {
        InitializeFadePanel();
        _startMenu.SetActive(true);

        GameManager.Instance.EventManager.OnPlayerDied += OnPlayerDied;
    }

    private void OnPlayerDied() {
        _textEndCoins.text = "Your coins " + GameManager.Instance.Coins.ToString("0000000");
        _textEndScore.text = "Your score " + GameManager.Instance.Score.ToString("0000000");
    }

    public void UpdateCoinsText(int value) {
        _textCoins.text = "Coins " + value.ToString("0000000");
        _textBestCoins.text = "Best coins " + GameManager.Instance.BestCoins.ToString("0000000");
    }

    public void UpdateScoreText(int value) {
        _textScore.text = "Score " + value.ToString("0000000");
        _textBestScore.text = "Best score " + GameManager.Instance.BestScore.ToString("0000000");
    }

    public void ViewLoseMenu() {
        _loseMenu.SetActive(true);
    }

    private void InitializeFadePanel() {
        var fadeColor = _fadePanel.color;
        _fadePanel.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
    }

    IEnumerator FadeOut(bool IsStartingGame) {
        var fadeColor = _fadePanel.color;
        print("fade A " + fadeColor.a);
        while (fadeColor.a < 1f) {
            yield return new WaitForSeconds(_timeRateToFade);
            fadeColor.a = _fadePanel.color.a + _fadeRate;
            _fadePanel.color = fadeColor;
        }


        _startMenu.SetActive(!IsStartingGame);
        _gameMenu.SetActive(IsStartingGame);

        yield return new WaitForSeconds(1f);

        if (IsStartingGame) {
            GameManager.Instance.EventManager.GameStarted();
        }
        else {
            _loseMenu.SetActive(false);
            GameManager.Instance.RestartGame();
        }

        while (fadeColor.a > 0f) {
            yield return new WaitForSeconds(_timeRateToFade);
            fadeColor.a = _fadePanel.color.a - _fadeRate;
            _fadePanel.color = fadeColor;
        }
    }

    public void GoToMenyAfterLose() {
        StartCoroutine(FadeOut(false));
    }

    public void StartGame() {
        StartCoroutine(FadeOut(true));
    }
}