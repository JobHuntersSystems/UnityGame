using UnityEngine;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float baseDamage = 10f;
    [SerializeField] private float lifetime = 5f;

    private float damage;
    private float moveSpeed;
    private Vector3 moveDirection;
    private Transform target;
    private bool isPenetrating;
    private readonly HashSet<Enemy> hitEnemies = new HashSet<Enemy>();
    private const float hitRadius = 0.2f;

    void OnEnable()
    {
        damage        = baseDamage;
        moveSpeed     = 0f;
        moveDirection = Vector3.zero;
        target        = null;
        isPenetrating = false;
        hitEnemies.Clear();
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifetime);
    }

    void Update()
    {
        if (target != null)
            moveDirection = (target.position - transform.position).normalized;
        else if (moveDirection == Vector3.zero)
        {
            ReturnToPool();
            return;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
        foreach (Collider2D col in hits)
        {
            if (!col.CompareTag("Enemy")) continue;
            Enemy enemy = col.GetComponent<Enemy>() ?? col.GetComponentInParent<Enemy>();
            if (enemy == null || hitEnemies.Contains(enemy)) continue;

            hitEnemies.Add(enemy);
            enemy.TakeDamage(damage);

            if (!isPenetrating)
            {
                ReturnToPool();
                return;
            }

            if (target != null && col.transform == target)
                target = null;
        }
    }

    public void Initialize(Transform t, float speed, float damageBonus, bool penetrating)
    {
        target        = t;
        moveSpeed     = speed;
        damage        = baseDamage + damageBonus;
        isPenetrating = penetrating;
        if (t != null)
            moveDirection = (t.position - transform.position).normalized;
    }

    public void InitializeDirection(Vector2 dir, float speed, float damageBonus, bool penetrating)
    {
        target        = null;
        moveDirection = dir.normalized;
        moveSpeed     = speed;
        damage        = baseDamage + damageBonus;
        isPenetrating = penetrating;
    }

    public void SetTarget(Transform t, float speed) => Initialize(t, speed, 0f, false);

    private void ReturnToPool()
    {
        CancelInvoke();
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.ReturnToPool("Projectile", gameObject);
        else
            Destroy(gameObject);
    }
}
