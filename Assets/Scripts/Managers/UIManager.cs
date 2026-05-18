using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System;

public class UIManager : MonoBehaviour
{
    [Header("XP")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;
    public Image xpBarFill;

    [Header("XP Bar Animacion")]
    public float xpLerpSpeed = 4f;
    public Color xpColorNormal = new Color(0.1f, 0.4f, 1f);

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI statLevelText;
    public TextMeshProUGUI statXPText;
    public TextMeshProUGUI statTimeText;
    public Button restartButton;
    public float gameOverDelay = 1.5f;

    [Header("Upgrades")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public TextMeshProUGUI[] upgradeLabels;

    private PlayerHealth playerHealth;
    private PlayerXP playerXP;
    private float elapsedTime;
    private bool gameRunning = true;
    private float xpBarTarget;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerXP = player.GetComponent<PlayerXP>();

        gameOverPanel.SetActive(false);
        upgradePanel.SetActive(false);
        restartButton?.onClick.AddListener(() => GameManager.Instance.RestartGame());
    }

    void OnEnable()
    {
        PlayerXP.OnLevelUp += ShowUpgradePanel;
        PlayerHealth.OnDeath += ShowGameOver;
    }

    void OnDisable()
    {
        PlayerXP.OnLevelUp -= ShowUpgradePanel;
        PlayerHealth.OnDeath -= ShowGameOver;
    }

    void Update()
    {
        if (!gameRunning) return;

        float newTarget = (float)playerXP.currentXP / playerXP.XPToNextLevel;

        xpBarTarget = newTarget;
        xpBar.value = Mathf.Lerp(xpBar.value, xpBarTarget, Time.unscaledDeltaTime * xpLerpSpeed);
        levelText.text = "Nv. " + playerXP.currentLevel;

        if (xpBarFill != null)
            xpBarFill.color = xpColorNormal;

        elapsedTime += Time.deltaTime;
        int min = Mathf.FloorToInt(elapsedTime / 60f);
        int sec = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{min:00}:{sec:00}";
    }

    private void ShowUpgradePanel(int level)
    {
        StartCoroutine(FillThenShowPanel());
    }

    private IEnumerator FillThenShowPanel()
    {
        xpBarTarget = 1f;
        xpBar.value = 1f;
        yield return new WaitForSecondsRealtime(0.3f);
        upgradePanel.SetActive(true);
    }

    public void SelectUpgrade(int index)
    {
        upgradePanel.SetActive(false);
        xpBar.value = 0f;
        xpBarTarget = 0f;
        playerXP.ResumeAfterUpgrade();
    }

    private void ShowGameOver()
    {
        gameRunning = false;
        StartCoroutine(ShowGameOverDelayed());
    }

    private IEnumerator ShowGameOverDelayed()
    {
        yield return new WaitForSecondsRealtime(gameOverDelay);

        int min = Mathf.FloorToInt(elapsedTime / 60f);
        int sec = Mathf.FloorToInt(elapsedTime % 60f);

        statLevelText.text = "Nivel: " + playerXP.currentLevel;
        statXPText.text = "XP total: " + playerXP.totalXP;
        statTimeText.text = $"Tiempo: {min:00}:{sec:00}";

        gameOverPanel.SetActive(true);
    }
}
