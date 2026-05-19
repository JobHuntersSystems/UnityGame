using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GruntEnemyMover : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GruntEnemy      grunt;
    [SerializeField] private GruntEnemyData  data; 

    private Rigidbody2D    rb;
    private SpriteRenderer sr;
    private Transform      playerTransform;
    private PlayerHealth   playerHealth;

    // Control del ataque
    private float lastAttackTime = -Mathf.Infinity;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale   = 0f;
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous; // ← añade
    }

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHealth    = player.GetComponent<PlayerHealth>(); // referencia directa
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
            rb.MovePosition(rb.position);
            return;
        }

        MoveTowardsPlayer();
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            TryAttack();
        }
    }
    void MoveTowardsPlayer()
    {
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        Vector2 newPos = rb.position + direction * grunt.MoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPos);

        if (direction.x != 0)
            sr.flipX = direction.x < 0;
    }

    void TryAttack()
    {
        if (Time.time - lastAttackTime < data.attackCooldown) return;
        lastAttackTime = Time.time;
        grunt.Attack(playerHealth);
    }

    void OnDrawGizmosSelected()
    {
        // Rojo = rango de ataque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, grunt != null ? grunt.AttackRange : 1.2f);

        // Amarillo = rango de detección (para referencia visual)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 6f);
    }
}