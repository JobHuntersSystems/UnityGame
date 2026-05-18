using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configuracion")]
    public float freezeDelay = 2f;

    [Header("Musica")]
    public AudioClip ambientMusic;
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    public float fadeOutDuration = 1.5f;

    public bool IsGameOver { get; private set; }

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.loop = true;
        audioSource.volume = musicVolume;
        audioSource.playOnAwake = false;
    }

    void Start()
    {
        if (ambientMusic != null)
        {
            audioSource.clip = ambientMusic;
            audioSource.Play();
        }
    }

    void OnEnable() => PlayerHealth.OnDeath += HandleGameOver;
    void OnDisable() => PlayerHealth.OnDeath -= HandleGameOver;

    private void HandleGameOver()
    {
        if (IsGameOver) return;
        IsGameOver = true;

        var enemySpawner = FindObjectOfType<EnemySpawner>();
        var orbSpawner = FindObjectOfType<XPOrbSpawner>();
        if (enemySpawner != null) enemySpawner.enabled = false;
        if (orbSpawner != null) orbSpawner.enabled = false;

        StartCoroutine(FadeOutMusic());
        StartCoroutine(FreezeAfterDelay());
    }

    private IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        audioSource.Stop();
    }

    private IEnumerator FreezeAfterDelay()
    {
        yield return new WaitForSecondsRealtime(freezeDelay);
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
