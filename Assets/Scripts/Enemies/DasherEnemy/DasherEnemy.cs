using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class DasherEnemy : Enemy
{
    [Header("Dasher Data")]
    [SerializeField] private DasherEnemyData data;

    public enum DashState { Normal, Preparing, Dashing }

    private DashState dashState = DashState.Normal;
    private Coroutine damageFlashCoroutine;
    private Animator animator;

    // Hashes de parámetros del Animator (más eficiente que strings)
    static readonly int HashIsMoving   = Animator.StringToHash("isMoving");
    static readonly int HashIsPreparing = Animator.StringToHash("isPreparing");
    static readonly int HashIsDashing  = Animator.StringToHash("isDashing");
    static readonly int HashIsDead     = Animator.StringToHash("isDead");

    public DashState CurrentDashState => dashState;
    public DasherEnemyData Data => data;

    // Colores del Dasher
    static readonly Color ColorNormal   = Color.white;
    static readonly Color ColorPrep     = new Color(0.4f, 0.9f, 1f);       // cian: telegrafía
    static readonly Color ColorDashing  = new Color(0.1f, 0.4f, 1f);       // azul intenso
    static readonly Color ColorDamage   = new Color(1f, 0.2f, 0.2f, 0.7f); // flash rojo

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        dashState = DashState.Normal;
        damageFlashCoroutine = null;

        if (data != null)
            LoadData();

        base.OnEnable();

        if (spriteRenderer != null)
            spriteRenderer.color = ColorNormal;

        if (UpgradeManager.Instance != null)
            moveSpeed *= UpgradeManager.Instance.EnemySpeedMultiplier;
    }

    protected override void Start()
    {
        if (data != null)
            LoadData();
        else
            Debug.LogWarning($"[{gameObject.name}] No tiene DasherEnemyData asignado!");

        base.Start();
    }

    private void LoadData()
    {
        enemyName              = data.enemyName;
        maxHealth              = data.maxHealth;
        moveSpeed              = data.moveSpeed;
        attackDamage           = data.attackDamage;
        attackRange            = data.attackRange;
        minExperienceReward    = data.minExperienceReward;
        maxExperienceReward    = data.maxExperienceReward;
        enemySprite            = data.sprite;
    }

    public void SetDashState(DashState state)
    {
        dashState = state;

        if (spriteRenderer != null)
            spriteRenderer.color = state switch
            {
                DashState.Preparing => ColorPrep,
                DashState.Dashing   => ColorDashing,
                _                   => ColorNormal
            };

        if (animator != null)
        {
            animator.SetBool(HashIsPreparing, state == DashState.Preparing);
            animator.SetBool(HashIsDashing,   state == DashState.Dashing);
            animator.SetBool(HashIsMoving,    state == DashState.Normal);
        }
    }

    public void Attack(PlayerHealth target, bool isDashing)
    {
        if (isDead || target == null || data == null) return;

        int damage = Mathf.RoundToInt(isDashing
            ? attackDamage * data.dashDamageMultiplier
            : attackDamage);

        target.TakeDamage(damage);
        Debug.Log($"{enemyName} golpea por {damage}{(isDashing ? " ⚡ dash!" : "")}");
    }

    public override void Attack() { }

    public override void UseSpecialAbility() { }

    protected override void OnDamaged(float amount)
    {
        base.OnDamaged(amount);

        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine);

        damageFlashCoroutine = StartCoroutine(DamageFlash());
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = ColorDamage;

        yield return new WaitForSeconds(0.12f);

        if (spriteRenderer != null)
            spriteRenderer.color = dashState switch
            {
                DashState.Preparing => ColorPrep,
                DashState.Dashing   => ColorDashing,
                _                   => ColorNormal
            };
    }

    protected override void Die()
    {
        dashState = DashState.Normal;

        if (animator != null)
        {
            animator.SetBool(HashIsMoving,    false);
            animator.SetBool(HashIsPreparing, false);
            animator.SetBool(HashIsDashing,   false);
            animator.SetBool(HashIsDead,      true);
        }

        base.Die();
    }

    private void OnDrawGizmosSelected()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 center = sr != null ? sr.bounds.center : transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, attackRange * transform.localScale.x);
    }
}
