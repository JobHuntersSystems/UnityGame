using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs de Enemigos")]
    public GameObject[] enemyPrefabs;

    [Header("Configuracion")]
    public int maxEnemies = 20;
    public float spawnInterval = 3f;
    public int enemiesPerWave = 3;
    public float maxSpawnRadius = 20f;

    private Transform playerTransform;
    private Camera mainCamera;
    private float spawnTimer;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (playerTransform == null) return;

        spawnTimer += Time.deltaTime;
        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            TrySpawnWave();
        }
    }

    void TrySpawnWave()
    {
        int current = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (current >= maxEnemies) return;

        int toSpawn = Mathf.Min(enemiesPerWave, maxEnemies - current);
        for (int i = 0; i < toSpawn; i++)
        {
            if (TryGetOffscreenPosition(out Vector2 pos))
                SpawnEnemyAt(pos);
        }
    }

    bool TryGetOffscreenPosition(out Vector2 result)
    {
        for (int attempt = 0; attempt < 20; attempt++)
        {
            Vector2 candidate = (Vector2)playerTransform.position
                              + Random.insideUnitCircle.normalized * Random.Range(5f, maxSpawnRadius);

            Vector3 vp = mainCamera.WorldToViewportPoint(candidate);
            bool offscreen = vp.x < 0f || vp.x > 1f || vp.y < 0f || vp.y > 1f;

            if (offscreen)
            {
                result = candidate;
                return true;
            }
        }
        result = Vector2.zero;
        return false;
    }

    void SpawnEnemyAt(Vector2 position)
    {
        if (enemyPrefabs.Length == 0) return;
        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        Instantiate(prefab, position, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || playerTransform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerTransform.position, maxSpawnRadius);
    }
}
