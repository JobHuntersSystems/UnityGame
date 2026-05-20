using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Transform target;
    private float moveSpeed;
    private Vector3 moveDirection;

    [SerializeField] private float damage = 10f;
    private float hitDistance = 0.15f;

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
            Destroy(gameObject);
            return;
        } 

        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void TryDealDamage(Transform hitTarget)
    {
        Enemy enemy = hitTarget.GetComponent<Enemy>();
        if (enemy == null)
        {
            enemy = hitTarget.GetComponentInParent<Enemy>();
        }

        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }

    public void SetTarget(Transform target, float moveSpeed)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
        Debug.Log($"[Projectile] SetTarget → Target={target?.name}, Speed={moveSpeed}, Pos={transform.position}");

        if (target != null)
            moveDirection = (target.position - transform.position).normalized;
    }
}
