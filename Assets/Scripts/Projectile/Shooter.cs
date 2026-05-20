using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootRate = 0.5f;
    [SerializeField] private float projectileSpeed = 8f;
    private float shootTimer;

    void Start()
    {
        if (projectilePrefab == null)
            Debug.LogError("[Shooter] Falta asignar el projectilePrefab en el Inspector.", this);
        else if (projectilePrefab.GetComponent<Projectile>() == null)
            Debug.LogWarning("[Shooter] El prefab no tiene el script Projectile.", this);
        else
            Debug.Log("[Shooter] Configurado correctamente.", this);
    }

    void Update()
    {
        if (projectilePrefab == null || shootRate <= 0f) return;

        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f) return;

        Transform target = GetClosestEnemy(); 
        
        if (target == null)
        {
            shootTimer = 0.2f;
            Debug.LogWarning("[Shooter] No se encontró ningún enemigo con tag 'Enemy'.", this);
            return;
        }

        shootTimer = shootRate;
        Debug.Log($"[Shooter] Disparando a {target.name}", this);

        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();

        if (projectile == null)
        {
            Debug.LogWarning("[Shooter] El prefab no tiene el script Projectile. Añadiéndolo automáticamente.");
            projectile = projectileObj.AddComponent<Projectile>();
        }

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
