using UnityEngine;

/// <summary>
/// Asset de datos del Dasher. Crear desde: clic derecho en Assets
/// → Create → Enemies → Dasher Data
/// </summary>
[CreateAssetMenu(fileName = "DasherData", menuName = "Enemies/Dasher Data")]
public class DasherEnemyData : ScriptableObject
{
    [Header("Stats base")]
    public string enemyName = "Dasher";
    public float maxHealth = 60f;
    public float moveSpeed = 6f;
    public float attackDamage = 15f;
    public float attackRange = 0.3f;

    [Header("XP")]
    public int minExperienceReward = 15;
    public int maxExperienceReward = 25;

    [Header("Ataque")]
    public float attackCooldown = 0.8f;

    [Header("Dash")]
    public float dashSpeed = 20f;
    public float dashDuration = 0.25f;
    public float dashDamageMultiplier = 2f;
    public float dashPrepTime = 0.5f;
    public float dashCooldownMin = 3f;
    public float dashCooldownMax = 5f;

    [Header("Zigzag")]
    public float zigzagFrequency = 2f;
    public float zigzagAmplitude = 1.5f;

    [Header("Visuals")]
    public Sprite sprite;
}
