using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DasherEnemyMover : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private DasherEnemy dasher;
    [SerializeField] private DasherEnemyData data;

    [Header("Movimiento")]
    [SerializeField] private float stopDistanceFromPlayer = 0.3f;

    [Header("Separación entre enemigos")]
    [SerializeField] private float separationRadius = 0.45f;
    [SerializeField] private float separationForce  = 0.7f;

    [Header("Visual")]
    [SerializeField] private float flipThreshold = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform playerTransform;
    private PlayerHealth playerHealth;

    private float lastAttackTime  = -Mathf.Infinity;
    private float zigzagTimer     = 0f;
    private float nextDashTime    = 0f;
    private float stateEndTime    = 0f;
    private Vector2 dashDirection;
    private Vector2 randomSeparationDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (dasher == null)
            dasher = GetComponent<DasherEnemy>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        GenerateRandomSeparationDir();
    }

    void OnEnable()
    {
        lastAttackTime = -Mathf.Infinity;
        zigzagTimer    = 0f;
        GenerateRandomSeparationDir();
        ScheduleNextDash();

        // Arranca en estado normal (caminando)
        if (dasher != null)
            dasher.SetDashState(DasherEnemy.DashState.Normal);
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth    = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("[DasherMover] No se encontró GameObject con tag 'Player'");
        }

        ScheduleNextDash();
    }

    void FixedUpdate()
    {
        if (dasher == null || dasher.IsDead || playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        zigzagTimer += Time.fixedDeltaTime;

        switch (dasher.CurrentDashState)
        {
            case DasherEnemy.DashState.Normal:    UpdateNormal();    break;
            case DasherEnemy.DashState.Preparing: UpdatePreparing(); break;
            case DasherEnemy.DashState.Dashing:   UpdateDashing();   break;
        }
    }

    // ── Estados ──────────────────────────────────────────────────────────

    void UpdateNormal()
    {
        MoveZigzag();

        if (Time.time >= nextDashTime)
            BeginPrep();
    }

    void UpdatePreparing()
    {
        rb.linearVelocity = Vector2.zero;

        if (data == null) { EndDash(); return; }

        if (Time.time >= stateEndTime)
        {
            stateEndTime = Time.time + data.dashDuration;
            dasher.SetDashState(DasherEnemy.DashState.Dashing);
        }
    }

    void UpdateDashing()
    {
        if (data == null) { EndDash(); return; }

        if (Time.time >= stateEndTime)
        {
            EndDash();
            return;
        }

        rb.MovePosition(rb.position + dashDirection * data.dashSpeed * Time.fixedDeltaTime);
        UpdateFlip(dashDirection);
    }

    // ── Transiciones de estado ────────────────────────────────────────────

    void BeginPrep()
    {
        if (data == null) return;

        rb.linearVelocity = Vector2.zero;
        dashDirection = ((Vector2)playerTransform.position - rb.position).normalized;
        stateEndTime = Time.time + data.dashPrepTime;
        dasher.SetDashState(DasherEnemy.DashState.Preparing);
    }

    void EndDash()
    {
        rb.linearVelocity = Vector2.zero;
        dasher.SetDashState(DasherEnemy.DashState.Normal);
        ScheduleNextDash();
    }

    void ScheduleNextDash()
    {
        if (data != null)
            nextDashTime = Time.time + Random.Range(data.dashCooldownMin, data.dashCooldownMax);
    }

    // ── Movimiento normal (zigzag) ────────────────────────────────────────

    void MoveZigzag()
    {
        Vector2 enemyPos  = rb.position;
        Vector2 playerPos = playerTransform.position;
        Vector2 toPlayer  = playerPos - enemyPos;
        float dist        = toPlayer.magnitude;

        Vector2 moveDir = dist > stopDistanceFromPlayer ? toPlayer.normalized : Vector2.zero;

        if (data != null && moveDir.sqrMagnitude > 0f)
        {
            Vector2 perp = new Vector2(-moveDir.y, moveDir.x);
            moveDir += perp * Mathf.Sin(zigzagTimer * data.zigzagFrequency) * data.zigzagAmplitude * 0.25f;
        }

        Vector2 separation = GetSeparationDirection();
        Vector2 final = moveDir + separation * separationForce;

        if (final.sqrMagnitude > 0.001f)
            final.Normalize();

        rb.MovePosition(enemyPos + final * dasher.MoveSpeed * Time.fixedDeltaTime);
        UpdateFlip(final);
    }

    // ── Separación ────────────────────────────────────────────────────────

    Vector2 GetSeparationDirection()
    {
        Collider2D[] nearby = Physics2D.OverlapCircleAll(rb.position, separationRadius);
        Vector2 sep = Vector2.zero;

        foreach (Collider2D col in nearby)
        {
            if (col.gameObject == gameObject) continue;
            Enemy other = col.GetComponent<Enemy>();
            if (other == null || other.IsDead) continue;

            Vector2 away = rb.position - (Vector2)col.transform.position;
            float d = away.magnitude;

            sep += d < 0.03f ? randomSeparationDirection * 0.4f : away.normalized / d;
        }

        return sep.normalized;
    }

    // ── Ataque por contacto ───────────────────────────────────────────────

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TryAttack();
    }

    void TryAttack()
    {
        if (data == null || playerHealth == null) return;
        if (Time.time - lastAttackTime < data.attackCooldown) return;

        lastAttackTime = Time.time;
        bool isDashing = dasher.CurrentDashState == DasherEnemy.DashState.Dashing;
        dasher.Attack(playerHealth, isDashing);
    }

    // ── Helpers ───────────────────────────────────────────────────────────

    void UpdateFlip(Vector2 dir)
    {
        if (sr == null) return;
        if (dir.x > flipThreshold)  sr.flipX = false;
        else if (dir.x < -flipThreshold) sr.flipX = true;
    }

    void GenerateRandomSeparationDir()
    {
        randomSeparationDirection = Random.insideUnitCircle.normalized;
        if (randomSeparationDirection == Vector2.zero)
            randomSeparationDirection = Vector2.right;
    }

    void OnDrawGizmosSelected()
    {
        SpriteRenderer currentSr = GetComponent<SpriteRenderer>();
        Vector3 center = currentSr != null ? currentSr.bounds.center : transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, dasher != null ? dasher.AttackRange : 0.3f);

        Gizmos.color = new Color(0f, 0.8f, 1f, 0.4f);
        Gizmos.DrawWireSphere(center, separationRadius);
    }
}
