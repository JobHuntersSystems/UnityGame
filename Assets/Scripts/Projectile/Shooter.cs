using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootRate = 0.5f;
    [SerializeField] private float projectileSpeed = 8f;
    private float shootTimer;

    void Start()
    {
        if (ObjectPooler.Instance == null)
            Debug.LogWarning("[Shooter] No hay ObjectPooler en la escena. Se usará Instantiate como fallback.", this);
    }

    void Update()
    {
        if (shootRate <= 0f) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f) return;

        Transform target = GetClosestEnemy();

        if (target == null)
        {
            shootTimer = 0.2f;
            return;
        }

        shootTimer = shootRate;

        GameObject projectileObj = ObjectPooler.Instance != null
            ? ObjectPooler.Instance.GetFromPool("Projectile", transform.position, Quaternion.identity)
            : Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        if (projectileObj == null) return;

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        if (projectile != null)
            projectile.SetTarget(target, projectileSpeed);
    }

    Transform GetClosestEnemy()
    {
        // 1. Busca los enemigos directamente usando el Tag
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        
        Transform bestTarget = null;
        float closestDistanceSqr = float.MaxValue;
        Vector3 currentPosition = transform.position;

        // 2. Compara las distancias directamente desde el arreglo original
        foreach (GameObject enemy in enemies)
        {
            Vector3 differenceToTarget = enemy.transform.position - currentPosition;
            float distanceToTargetSqr = differenceToTarget.sqrMagnitude;

            if (distanceToTargetSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceToTargetSqr;
                bestTarget = enemy.transform;
            }
        }

        return bestTarget;
    }
}
