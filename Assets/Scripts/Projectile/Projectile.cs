using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float moveSpeed;
    private Vector3 moveDirection;

    [SerializeField] private float damage = 10f;
    [SerializeField] private float lifetime = 5f;
    private float hitDistance = 0.15f;

    void OnEnable()
    {
        target = null;
        moveDirection = Vector3.zero;
        CancelInvoke();
        Invoke(nameof(ReturnToPool), lifetime);
    }

    void Update()
    {
        if (target != null)
        {
            moveDirection = (target.position - transform.position).normalized;

            if (Vector3.Distance(transform.position, target.position) <= hitDistance)
            {
                TryDealDamage(target);
                return;
            }
        }
        else if (moveDirection == Vector3.zero)
        {
            ReturnToPool();
            return;
        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void TryDealDamage(Transform hitTarget)
    {
        Enemy enemy = hitTarget.GetComponent<Enemy>();
        if (enemy == null)
            enemy = hitTarget.GetComponentInParent<Enemy>();

        if (enemy != null)
            enemy.TakeDamage(damage);

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        CancelInvoke();
        if (ObjectPooler.Instance != null)
            ObjectPooler.Instance.ReturnToPool("Projectile", gameObject);
        else
            Destroy(gameObject);
    }

    public void SetTarget(Transform target, float moveSpeed)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;

        if (target != null)
            moveDirection = (target.position - transform.position).normalized;
    }
}
