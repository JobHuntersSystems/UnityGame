using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GruntEnemyMover : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private GruntEnemy grunt;

    private Rigidbody2D  rb;
    private SpriteRenderer sr;
    private Transform    playerTransform;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        rb.gravityScale   = 0f;
        rb.freezeRotation = true;
    }

    void Start()
    {
        // Busca al jugador automáticamente por tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning("[GruntMover] No se encontró GameObject con tag 'Player'");
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

    void MoveTowardsPlayer()
    {
        Vector2 direction = ((Vector2)playerTransform.position - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * grunt.MoveSpeed;

        // Flip horizontal según dirección
        if (direction.x != 0)
            sr.flipX = direction.x < 0;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}