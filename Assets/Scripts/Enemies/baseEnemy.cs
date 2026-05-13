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
    [SerializeField] protected Sprite enemySprite; // Arrastra el sprite aquí desde Assets

    // --- Estado interno ---
    protected float currentHealth;
    protected bool  isDead = false;

    // Referencia al SpriteRenderer del GameObject
    protected SpriteRenderer spriteRenderer;

    // -------------------------------------------------------
    // Ciclo de vida Unity
    // -------------------------------------------------------

    protected virtual void Awake()
    {
        // Awake se llama antes que Start, ideal para cachear referencias
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // Asigna el sprite si se configuró desde el Inspector
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
        Debug.Log($"{enemyName} ha muerto. XP otorgada: {experienceReward}");
        // Aquí después: animación de muerte, drop de items, etc.
        Destroy(gameObject); // Elimina el GameObject de la escena
    }

    // -------------------------------------------------------
    // Getters (el resto del juego puede consultarlos)
    // -------------------------------------------------------

    // Getters
    public string EnemyName     => enemyName;
    public float  CurrentHealth => currentHealth;
    public float  MaxHealth     => maxHealth;
    public bool   IsDead        => isDead;
    public float  MoveSpeed     => moveSpeed; 
}