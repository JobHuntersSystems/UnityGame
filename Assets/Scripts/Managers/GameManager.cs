using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action OnGameOver;
    public static event Action OnPause;
    public static event Action OnResume;

    [SerializeField] private float gameOverDelay = 1.5f;
    [SerializeField] private string menuSceneName = "Menu";

    public enum GameState { Playing, Paused, GameOver }
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

    public void TogglePause()
    {
        if (State == GameState.GameOver) return;
        // Si el upgrade panel ha congelado el tiempo, no permitir pausar
        if (State == GameState.Playing && Time.timeScale == 0f) return;

        if (State == GameState.Playing) PauseGame();
        else if (State == GameState.Paused) ResumeGame();
    }

    public void PauseGame()
    {
        State = GameState.Paused;
        Time.timeScale = 0f;
        OnPause?.Invoke();
    }

    public void ResumeGame()
    {
        State = GameState.Playing;
        Time.timeScale = 1f;
        OnResume?.Invoke();
    }

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
