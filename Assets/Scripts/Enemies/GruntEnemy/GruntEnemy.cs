using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class GruntEnemy : Enemy
{
    [Header("Grunt Data")]
    [SerializeField] private GruntEnemyData data;

    private bool isEnraged = false;
    private bool isCharging = false;
    private float chargeEndTime = 0f;

    private Coroutine damageFlashCoroutine;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        isEnraged = false;
        isCharging = false;
        chargeEndTime = 0f;
        damageFlashCoroutine = null;

        if (data != null)
        {
            enemyName = data.enemyName;
            maxHealth = data.maxHealth;
            moveSpeed = data.moveSpeed;
            attackDamage = data.attackDamage;
            attackRange = data.attackRange;
            minExperienceReward = data.minExperienceReward;
            maxExperienceReward = data.maxExperienceReward;
            enemySprite = data.sprite;
        }

        base.OnEnable();

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        if (UpgradeManager.Instance != null)
            moveSpeed *= UpgradeManager.Instance.EnemySpeedMultiplier;
    }

    protected override void Start()
    {
        if (data != null)
        {
            enemyName = data.enemyName;
            maxHealth = data.maxHealth;
            moveSpeed = data.moveSpeed;
            attackDamage = data.attackDamage;
            attackRange = data.attackRange;
            minExperienceReward = data.minExperienceReward;
            maxExperienceReward = data.maxExperienceReward;
            enemySprite = data.sprite;
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] No tiene GruntEnemyData asignado!");
        }

        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (isCharging && Time.time >= chargeEndTime)
            ResetCharge();
    }

    public void Attack(PlayerHealth target)
    {
        if (isDead || target == null || data == null) return;

        int damage = Mathf.RoundToInt(isCharging
            ? attackDamage * data.chargeMultiplier
            : attackDamage);

        target.TakeDamage(damage);
        Debug.Log($"{enemyName} golpea por {damage}{(isCharging ? " ⚡ carga!" : "")}");
    }

    public override void Attack()
    {
    }

    public override void UseSpecialAbility()
    {
        if (isDead || isCharging || data == null) return;

        isCharging = true;
        chargeEndTime = Time.time + data.chargeDuration;

        Debug.Log($"{enemyName} está cargando durante {data.chargeDuration}s!");

        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }

    protected override void OnDamaged(float amount)
    {
        base.OnDamaged(amount);

        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine);

        damageFlashCoroutine = StartCoroutine(DamageFlash());

        if (data != null && !isEnraged && currentHealth < maxHealth * data.enrageTreshold)
        {
            isEnraged = true;
            OnEnrage();
        }
    }

    private IEnumerator DamageFlash()
    {
        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 0.7f);

        yield return new WaitForSeconds(0.12f);

        if (spriteRenderer != null)
            spriteRenderer.color = isEnraged ? new Color(1f, 0.4f, 0f) : Color.white;
    }

    protected override void Die()
    {
        isCharging = false;
        isEnraged = false;

        base.Die();
    }

    private void OnEnrage()
    {
        moveSpeed *= 1.5f;
        attackDamage *= 1.3f;

        Debug.Log($"{enemyName} ESTÁ ENFURECIDO. Velocidad y daño aumentados.");

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0.4f, 0f);
    }

    private void ResetCharge()
    {
        isCharging = false;

        Debug.Log($"{enemyName} ha terminado la carga.");

        if (spriteRenderer != null)
            spriteRenderer.color = isEnraged ? new Color(1f, 0.4f, 0f) : Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 center = sr != null ? sr.bounds.center : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange * transform.localScale.x);
    }
}