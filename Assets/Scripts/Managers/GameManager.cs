using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action OnGameOver;

    [SerializeField] private float gameOverDelay = 1.5f;
    [SerializeField] private string menuSceneName = "Menu";

    public enum GameState { Playing, GameOver }
    public GameState State { get; private set; } = GameState.Playing;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void OnEnable()  => PlayerHealth.OnDeath += HandlePlayerDeath;
    void OnDisable() => PlayerHealth.OnDeath -= HandlePlayerDeath;

    private void HandlePlayerDeath()
    {
        if (State == GameState.GameOver) return;
        State = GameState.GameOver;
        StartCoroutine(GameOverSequence());
    }

    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);
        Time.timeScale = 0f;
        OnGameOver?.Invoke();
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
