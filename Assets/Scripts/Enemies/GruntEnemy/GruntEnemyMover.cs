using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GruntEnemyMover : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GruntEnemy grunt;
    [SerializeField] private GruntEnemyData data;

    [Header("Movimiento")]
    [SerializeField] private float stopDistanceFromPlayer = 0.35f;

    [Header("Separación entre enemigos")]
    [SerializeField] private float separationRadius = 0.45f;
    [SerializeField] private float separationForce = 0.9f;

    [Header("Visual")]
    [SerializeField] private float flipThreshold = 0.15f;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Transform playerTransform;
    private PlayerHealth playerHealth;

    private float lastAttackTime = -Mathf.Infinity;
    private Vector2 randomSeparationDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        if (grunt == null)
            grunt = GetComponent<GruntEnemy>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        GenerateRandomSeparationDirection();
    }

    void OnEnable()
    {
        lastAttackTime = -Mathf.Infinity;
        GenerateRandomSeparationDirection();
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth = player.GetComponent<PlayerHealth>();
        }
        else
        {
            Debug.LogWarning("[GruntMover] No se encontró GameObject con tag 'Player'");
        }
    }

    void FixedUpdate()
    {
        if (grunt == null || grunt.IsDead || playerTransform == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        MoveTowardsPlayer();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            TryAttack();
    }

    void MoveTowardsPlayer()
    {
        Vector2 enemyPosition = rb.position;
        Vector2 playerPosition = playerTransform.position;

        Vector2 toPlayer = playerPosition - enemyPosition;
        float distanceToPlayer = toPlayer.magnitude;

        Vector2 directionToPlayer = Vector2.zero;

        if (distanceToPlayer > stopDistanceFromPlayer)
            directionToPlayer = toPlayer.normalized;

        Vector2 separationDirection = GetSeparationDirection();

        Vector2 finalDirection = directionToPlayer + separationDirection * separationForce;

        if (finalDirection.sqrMagnitude > 0.001f)
            finalDirection.Normalize();

        Vector2 newPos = enemyPosition + finalDirection * grunt.MoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        UpdateFlip(finalDirection);
    }

    Vector2 GetSeparationDirection()
    {
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(
            rb.position,
            separationRadius
        );

        Vector2 separationDirection = Vector2.zero;

        foreach (Collider2D col in nearbyColliders)
        {
            if (col.gameObject == gameObject) continue;

            Enemy otherEnemy = col.GetComponent<Enemy>();

            if (otherEnemy == null) continue;
            if (otherEnemy.IsDead) continue;

            Vector2 awayFromEnemy = rb.position - (Vector2)col.transform.position;
            float distance = awayFromEnemy.magnitude;

            if (distance < 0.03f)
            {
                separationDirection += randomSeparationDirection * 0.4f;
            }
            else
            {
                separationDirection += awayFromEnemy.normalized / distance;
            }
        }

        return separationDirection.normalized;
    }

    void UpdateFlip(Vector2 direction)
    {
        if (sr == null) return;

        if (direction.x > flipThreshold)
            sr.flipX = false;
        else if (direction.x < -flipThreshold)
            sr.flipX = true;
    }

    void GenerateRandomSeparationDirection()
    {
        randomSeparationDirection = Random.insideUnitCircle.normalized;

        if (randomSeparationDirection == Vector2.zero)
            randomSeparationDirection = Vector2.right;
    }

    void TryAttack()
    {
        if (data == null) return;
        if (playerHealth == null) return;

        if (Time.time - lastAttackTime < data.attackCooldown) return;

        lastAttackTime = Time.time;
        grunt.Attack(playerHealth);
    }

    void OnDrawGizmosSelected()
    {
        SpriteRenderer currentSr = GetComponent<SpriteRenderer>();
        Vector3 center = currentSr != null ? currentSr.bounds.center : transform.position;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, grunt != null ? grunt.AttackRange : 0.5f);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, separationRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(center, stopDistanceFromPlayer);
    }
}