using UnityEngine;

/// <summary>
/// Clase base para todos los enemigos del juego.
/// Adjunta este script o sus hijos a un GameObject con SpriteRenderer.
/// </summary>
public abstract class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] protected string enemyName = "Enemy";
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float moveSpeed = 2f;
    [SerializeField] protected float attackDamage = 10f;
    [SerializeField] protected float attackRange = 1.5f;

    [Header("XP")]
    [SerializeField] protected int minExperienceReward = 20;
    [SerializeField] protected int maxExperienceReward = 40;

    [Header("Visuals")]
    [SerializeField] protected Sprite enemySprite;

    protected float currentHealth;
    protected bool isDead = false;

    protected SpriteRenderer spriteRenderer;
    private string poolTag;

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
    }

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

    public abstract void Attack();

    public abstract void UseSpecialAbility();

    protected virtual int GetExperienceReward()
    {
        return Random.Range(minExperienceReward, maxExperienceReward + 1);
    }

    protected virtual void OnDamaged(float amount)
    {
        Debug.Log($"{enemyName} recibió {amount} de daño. HP: {currentHealth}/{maxHealth}");
    }

    protected virtual void Die()
    {
        isDead = true;

        PlayerXP playerXP = FindFirstObjectByType<PlayerXP>();

        if (playerXP != null)
        {
            int xpGained = GetExperienceReward();
            playerXP.AddXP(xpGained);
            Debug.Log($"{enemyName} murió y dio {xpGained} XP");
        }

        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
        else
            Destroy(gameObject);
    }

    public void ApplySlow(float multiplier)
    {
        moveSpeed *= multiplier;
    }

    public string EnemyName => enemyName;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsDead => isDead;
    public float MoveSpeed => moveSpeed;
    public float AttackRange => attackRange;
}