using UnityEngine;

public class Shooter : MonoBehaviour
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootRate = 0.5f;
    [SerializeField] private float projectileSpeed = 8f;

    private float damageBonus    = 0f;
    private int   multiShotCount = 1;
    private float multiShotSpread = 20f;
    private bool  penetrating    = false;
    private float shootTimer;

    void Start()
    {
        if (ObjectPooler.Instance == null)
            Debug.LogWarning("[Shooter] No hay ObjectPooler en la escena.", this);
    }

    void Update()
    {
        if (shootRate <= 0f) return;
        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f) return;

        Transform target = GetClosestEnemy();
        if (target == null) { shootTimer = 0.2f; return; }

        shootTimer = shootRate;
        FireAt(target);
    }

    void FireAt(Transform target)
    {
        if (multiShotCount == 1)
        {
            SpawnProjectile(target, null);
            return;
        }

        Vector2 baseDir = ((Vector2)(target.position - transform.position)).normalized;
        for (int i = 0; i < multiShotCount; i++)
        {
            float angle = (i - (multiShotCount - 1) / 2f) * multiShotSpread;
            SpawnProjectile(null, RotateVector(baseDir, angle));
        }
    }

    void SpawnProjectile(Transform target, Vector2? dir)
    {
        GameObject obj = ObjectPooler.Instance != null
            ? ObjectPooler.Instance.GetFromPool("Projectile", transform.position, Quaternion.identity)
            : Instantiate(projectilePrefab, transform.position, Quaternion.identity);

        if (obj == null) return;
        Projectile p = obj.GetComponent<Projectile>();
        if (p == null) return;

        if (target != null)
            p.Initialize(target, projectileSpeed, damageBonus, penetrating);
        else
            p.InitializeDirection(dir.Value, projectileSpeed, damageBonus, penetrating);
    }

    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float rad = degrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }

    Transform GetClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Transform best = null;
        float closest = float.MaxValue;
        foreach (GameObject e in enemies)
        {
            float d = (e.transform.position - transform.position).sqrMagnitude;
            if (d < closest) { closest = d; best = e.transform; }
        }
        return best;
    }

    public void UpgradeShootRate(float reduction)     => shootRate = Mathf.Max(0.1f, shootRate - reduction);
    public void UpgradeDamage(float bonus)            => damageBonus += bonus;
    public void UpgradeProjectileSpeed(float bonus)   => projectileSpeed += bonus;
    public void UpgradePenetration()                  => penetrating = true;
    public void UpgradeMultiShot()                    => multiShotCount++;
}
