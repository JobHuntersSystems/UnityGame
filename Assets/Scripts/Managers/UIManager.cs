using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [Header("XP")]
    public Slider xpBar;
    public TextMeshProUGUI levelText;

    [Header("Timer")]
    public TextMeshProUGUI timerText;

    [Header("Game Over")]
    public GameObject gameOverPanel;
    public TextMeshProUGUI statLevelText;
    public TextMeshProUGUI statXPText;
    public TextMeshProUGUI statTimeText;

    [Header("Upgrades")]
    public GameObject upgradePanel;
    public Button[] upgradeButtons;
    public TextMeshProUGUI[] upgradeLabels;

    private PlayerHealth playerHealth;
    private PlayerXP playerXP;
    private float elapsedTime;
    private bool gameRunning = true;

    void Awake()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        playerXP = player.GetComponent<PlayerXP>();

        gameOverPanel.SetActive(false);
        upgradePanel.SetActive(false);
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

        xpBar.value = (float)playerXP.currentXP / playerXP.XPToNextLevel;
        levelText.text = "Nv. " + playerXP.currentLevel;

        elapsedTime += Time.deltaTime;
        int min = Mathf.FloorToInt(elapsedTime / 60f);
        int sec = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = $"{min:00}:{sec:00}";
    }

    private void ShowUpgradePanel(int level)
    {
        upgradePanel.SetActive(true);
    }

    public void SelectUpgrade(int index)
    {
        upgradePanel.SetActive(false);
        playerXP.ResumeAfterUpgrade();
    }

    private void ShowGameOver()
    {
        gameRunning = false;

        int min = Mathf.FloorToInt(elapsedTime / 60f);
        int sec = Mathf.FloorToInt(elapsedTime % 60f);

        statLevelText.text = "Nivel: " + playerXP.currentLevel;
        statXPText.text = "XP total: " + playerXP.totalXP;
        statTimeText.text = $"Tiempo: {min:00}:{sec:00}";

        gameOverPanel.SetActive(true);
    }
}
