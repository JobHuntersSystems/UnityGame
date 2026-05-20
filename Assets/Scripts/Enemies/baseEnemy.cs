using UnityEngine;

/// <summary>
/// Clase base para todos los enemigos del juego.
/// Adjunta este script (o sus hijos) a un GameObject con SpriteRenderer.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    // --- Propiedades configurables desde el Inspector de Unity ---
    [Header("Stats")]
    [SerializeField] protected string enemyName = "Enemy";
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float moveSpeed  = 2f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackRange  = 1.5f;
    [SerializeField] protected int   experienceReward = 50;

    [Header("Visuals")]
    [SerializeField] protected Sprite enemySprite;

    // --- Estado interno ---
    protected float currentHealth;
    protected bool  isDead = false;

    protected SpriteRenderer spriteRenderer;
    private string poolTag;

    // -------------------------------------------------------
    // Ciclo de vida Unity
    // -------------------------------------------------------

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        poolTag = gameObject.name.Replace("(Clone)", "").Trim();
    }

    protected virtual void OnEnable()
    {
        currentHealth = maxHealth;
        isDead = false;
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        if (spriteRenderer != null && enemySprite != null)
            spriteRenderer.sprite = enemySprite;
    }

    protected virtual void Update()
    {
        // Por ahora vacío — aquí irá la IA cuando la necesites
    }

    // -------------------------------------------------------
    // Métodos públicos (interfaz común de todo enemigo)
    // -------------------------------------------------------

    public virtual void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        OnDamaged(amount);

        if (currentHealth <= 0)
            Die();
    }

    public virtual void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }

    // -------------------------------------------------------
    // Métodos abstractos — cada enemigo DEBE implementarlos
    // -------------------------------------------------------

    /// <summary>Lógica de ataque única por tipo de enemigo.</summary>
    public abstract void Attack();

    /// <summary>Habilidad especial única por tipo.</summary>
    public abstract void UseSpecialAbility();

    // -------------------------------------------------------
    // Métodos virtuales — pueden sobreescribirse o no
    // -------------------------------------------------------

    protected virtual void OnDamaged(float amount)
    {
        Debug.Log($"{enemyName} recibió {amount} de daño. HP: {currentHealth}/{maxHealth}");
        // Aquí después: animación de daño, flash de color, etc.
    }

    protected virtual void Die()
    {
        isDead = true;
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
        else
            Destroy(gameObject);
    }

    // -------------------------------------------------------
    // Getters (el resto del juego puede consultarlos)
    // -------------------------------------------------------

    // Getters
    public void ApplySlow(float multiplier) => moveSpeed *= multiplier;

    public string EnemyName     => enemyName;
    public float  CurrentHealth => currentHealth;
    public float  MaxHealth     => maxHealth;
    public bool   IsDead        => isDead;
    public float  MoveSpeed     => moveSpeed;
    public float  AttackRange   => attackRange;
}