using UnityEngine;

/// <summary>
/// Asset de datos del Grunt. Crear desde: clic derecho en Assets
/// → Create → Enemies → Grunt Data
/// </summary>
[CreateAssetMenu(fileName = "GruntData", menuName = "Enemies/Grunt Data")]
public class GruntEnemyData : ScriptableObject
{
    [Header("Stats base")]
    public string enemyName = "Grunt";
    public float maxHealth = 150f;
    public float moveSpeed = 3f;
    public float attackDamage = 20f;
    public float attackRange = 0.25f;

    [Header("XP")]
    public int minExperienceReward = 20;
    public int maxExperienceReward = 40;

    [Header("Ataque")]
    public float attackCooldown = 1f;

    [Header("Config especial Grunt")]
    public float chargeMultiplier = 2.5f;
    public float chargeDuration = 3f;
    public float enrageTreshold = 0.25f;

    [Header("Visuals")]
    public Sprite sprite;
}