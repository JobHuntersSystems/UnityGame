using UnityEngine;

/// <summary>
/// Asset de datos del Grunt. Crear desde: clic derecho en Assets
/// → Create → Enemies → Grunt Data
/// </summary>
[CreateAssetMenu(fileName = "GruntData", menuName = "Enemies/Grunt Data")]
public class GruntEnemyData : ScriptableObject
{
    [Header("Stats base")]
    public string enemyName       = "Grunt";
    public float  maxHealth       = 150f;   // Más HP que los otros, es el tanque
    public float  moveSpeed       = 3f;   // Lento
    public float  attackDamage    = 20f;
    public float  attackRange     = 0.25f;
    public int    experienceReward = 30;

    [Header("Ataque")]
    public float attackCooldown = 1f; 
      
    [Header("Config especial Grunt")]
    public float chargeMultiplier    = 2.5f;  // Multiplicador de daño al cargar
    public float chargeDuration      = 3f;    // Segundos que dura la carga
    public float enrageTreshold      = 0.25f; // Se enfurece por debajo del 25% HP

    [Header("Visuals")]
    public Sprite sprite; // Arrastra el sprite aquí cuando te lo pasen
}