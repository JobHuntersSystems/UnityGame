using UnityEngine;
using System.Collections.Generic;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager Instance { get; private set; }

    [Header("Mejoras disponibles")]
    public UpgradeData[] allUpgrades;

    public float EnemySpeedMultiplier { get; private set; } = 1f;
    public float XPAttractionBonus    { get; private set; } = 0f;

    private PlayerMovement playerMovement;
    private PlayerHealth   playerHealth;
    private Shooter        shooter;
    private UpgradeData[]  currentChoices;

    void Awake() => Instance = this;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        playerHealth   = player.GetComponent<PlayerHealth>();
        shooter        = player.GetComponentInChildren<Shooter>();
        if (shooter == null) shooter = FindAnyObjectByType<Shooter>();
    }

    public UpgradeData[] PickRandomUpgrades(int count = 3)
    {
        List<UpgradeData> pool = new List<UpgradeData>(allUpgrades);
        int take = Mathf.Min(count, pool.Count);
        currentChoices = new UpgradeData[take];
        for (int i = 0; i < take; i++)
        {
            int idx = Random.Range(0, pool.Count);
            currentChoices[i] = pool[idx];
            pool.RemoveAt(idx);
        }
        return currentChoices;
    }

    public void ApplyUpgrade(int choiceIndex)
    {
        if (currentChoices == null || choiceIndex >= currentChoices.Length) return;
        Apply(currentChoices[choiceIndex]);
    }

    private void Apply(UpgradeData u)
    {
        switch (u.type)
        {
            case UpgradeType.ShootRate:       shooter.UpgradeShootRate(u.value);              break;
            case UpgradeType.Damage:          shooter.UpgradeDamage(u.value);                 break;
            case UpgradeType.ProjectileSpeed: shooter.UpgradeProjectileSpeed(u.value);        break;
            case UpgradeType.Penetration:     shooter.UpgradePenetration();                   break;
            case UpgradeType.MultiShot:       shooter.UpgradeMultiShot();                     break;
            case UpgradeType.PlayerSpeed:     playerMovement.moveSpeed += u.value;            break;
            case UpgradeType.MaxHP:           playerHealth.IncreaseMaxHP(Mathf.RoundToInt(u.value)); break;
            case UpgradeType.Regen:           playerHealth.AddRegen(u.value);                 break;
            case UpgradeType.SlowEnemies:     ApplySlowToEnemies(u.value);                    break;
            case UpgradeType.XPAttraction:    ApplyXPAttraction(u.value);                     break;
        }
    }

    private void ApplySlowToEnemies(float slowAmount)
    {
        EnemySpeedMultiplier *= (1f - slowAmount);
        foreach (Enemy e in FindObjectsByType<Enemy>())
            e.ApplySlow(1f - slowAmount);
    }

    private void ApplyXPAttraction(float bonus)
    {
        XPAttractionBonus += bonus;
        foreach (XPOrb orb in FindObjectsByType<XPOrb>())
            orb.attractionRange += bonus;
    }
}
