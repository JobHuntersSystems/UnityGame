using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))] // Unity añade SpriteRenderer automático si falta
public class GruntEnemy : Enemy
{
    [Header("Grunt Data")]
    [SerializeField] private GruntEnemyData data; // Arrastra el .asset aquí

    // Estado interno del grunt
    private bool isEnraged      = false;
    private bool isCharging     = false;
    private float chargeEndTime = 0f;

    // -------------------------------------------------------
    // Ciclo de vida
    // -------------------------------------------------------

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnEnable()
    {
        isEnraged     = false;
        isCharging    = false;
        chargeEndTime = 0f;
        damageFlashCoroutine = null;

        if (data != null)
        {
            moveSpeed    = data.moveSpeed;
            attackDamage = data.attackDamage;
            maxHealth    = data.maxHealth;
        }

        base.OnEnable();
        if (spriteRenderer != null) spriteRenderer.color = Color.white;

        if (UpgradeManager.Instance != null)
            moveSpeed *= UpgradeManager.Instance.EnemySpeedMultiplier;
    }

    protected override void Start()
    {
        // Carga los datos del ScriptableObject a la clase base
        if (data != null)
        {
            enemyName      = data.enemyName;
            maxHealth      = data.maxHealth;
            moveSpeed      = data.moveSpeed;
            attackDamage   = data.attackDamage;
            attackRange    = data.attackRange;
            experienceReward = data.experienceReward;
            enemySprite    = data.sprite;
        }
        else
        {
            Debug.LogWarning($"[{gameObject.name}] No tiene GruntEnemyData asignado!");
        }

        base.Start(); // Aplica el sprite y pone currentHealth = maxHealth
    }

    protected override void Update()
    {
        base.Update();

        // Resetear carga cuando expira el tiempo
        if (isCharging && Time.time >= chargeEndTime)
            ResetCharge();
    }

    // -------------------------------------------------------
    // Implementación de abstractos
    // -------------------------------------------------------

    // Cambia la firma del Attack para recibir al jugador
    public void Attack(PlayerHealth target)
    {
        if (isDead || target == null || data == null) return;

        int damage = Mathf.RoundToInt(isCharging
            ? attackDamage * data.chargeMultiplier
            : attackDamage);

        target.TakeDamage(damage);
        Debug.Log($"{enemyName} golpea por {damage}{(isCharging ? " ⚡ carga!" : "")}");
    }

    // La versión abstracta del padre hay que mantenerla — sobrescríbela así:
    public override void Attack()
    {
        // Versión sin target — no se usa directamente en el Grunt
        // El Mover llama a Attack(PlayerHealth) directamente
    }

    public override void UseSpecialAbility()
    {
        if (isDead || isCharging || data == null) return;

        isCharging   = true;
        chargeEndTime = Time.time + data.chargeDuration;

        Debug.Log($"{enemyName} ¡está cargando durante {data.chargeDuration}s!");

        // Visual feedback: ponlo rojo mientras carga
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }

    // -------------------------------------------------------
    // Overrides de comportamiento
    // -------------------------------------------------------

    protected override void OnDamaged(float amount)
    {
        base.OnDamaged(amount);

        // Flash de daño
        if (damageFlashCoroutine != null)
            StopCoroutine(damageFlashCoroutine); // Evita que se pisen si recibe daño seguido
        damageFlashCoroutine = StartCoroutine(DamageFlash());

        // Enrabiarse con poco HP
        if (!isEnraged && currentHealth < maxHealth * data.enrageTreshold)
        {
            isEnraged = true;
            OnEnrage();
        }
    }

    private Coroutine damageFlashCoroutine;

    private IEnumerator DamageFlash()
    {
        // Color rojo semi-transparente — ajusta el alpha a tu gusto (0.7 = bastante visible)
        spriteRenderer.color = new Color(1f, 0.2f, 0.2f, 0.7f);
        yield return new WaitForSeconds(0.12f); // Duración del flash
        spriteRenderer.color = isEnraged ? new Color(1f, 0.4f, 0f) : Color.white;
    }

    protected override void Die()
    {
        // Limpieza de estado antes de destruir
        isCharging = false;
        isEnraged  = false;
        base.Die();
    }

    // -------------------------------------------------------
    // Helpers privados
    // -------------------------------------------------------

    private void OnEnrage()
    {
        moveSpeed    *= 1.5f; // Se mueve más rápido enfurecido
        attackDamage *= 1.3f;
        Debug.Log($"{enemyName} ¡ESTÁ ENFURECIDO! Velocidad y daño aumentados.");

        if (spriteRenderer != null)
            spriteRenderer.color = new Color(1f, 0.4f, 0f); // Naranja
    }

    private void ResetCharge()
    {
        isCharging = false;
        Debug.Log($"{enemyName} ha terminado la carga.");

        // Restaurar color (naranja si enraged, blanco si normal)
        if (spriteRenderer != null)
            spriteRenderer.color = isEnraged ? new Color(1f, 0.4f, 0f) : Color.white;
    }

    // -------------------------------------------------------
    // Gizmo — dibuja el rango de ataque en el Editor
    // -------------------------------------------------------
    private void OnDrawGizmosSelected()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Vector3 center = (sr != null) ? sr.bounds.center : transform.position;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRange * transform.localScale.x);
    }
}