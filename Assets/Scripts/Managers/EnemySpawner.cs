using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs de Enemigos")]
    public GameObject[] enemyPrefabs;

    [Header("Configuracion inicial")]
    public int   maxEnemies     = 20;
    public float spawnInterval  = 3f;
    public int   enemiesPerWave = 3;
    public float maxSpawnRadius = 20f;

    [Header("Escalado con el tiempo")]
    public float intervalReductionPerMinute = 0.3f;
    public float minSpawnInterval           = 0.5f;
    public int   waveIncreasePerMinute      = 1;
    public int   maxEnemiesIncreasePerMinute = 5;
    public int   absoluteMaxEnemies        = 80;

    private Transform playerTransform;
    private Camera    mainCamera;
    private float     spawnTimer;
    private float     elapsedTime;

    private float CurrentInterval  => Mathf.Max(minSpawnInterval, spawnInterval  - intervalReductionPerMinute  * (elapsedTime / 60f));
    private int   CurrentWaveSize  =>             Mathf.RoundToInt(enemiesPerWave + waveIncreasePerMinute       * (elapsedTime / 60f));
    private int   CurrentMaxEnemies => Mathf.Min(absoluteMaxEnemies, maxEnemies  + maxEnemiesIncreasePerMinute * Mathf.FloorToInt(elapsedTime / 60f));

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (playerTransform == null) return;

        elapsedTime += Time.deltaTime;
        spawnTimer  += Time.deltaTime;

        if (spawnTimer >= CurrentInterval)
        {
            spawnTimer = 0f;
            TrySpawnWave();
        }
    }

    void TrySpawnWave()
    {
        int current = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (current >= CurrentMaxEnemies) return;

        int toSpawn = Mathf.Min(CurrentWaveSize, CurrentMaxEnemies - current);
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

        GameObject enemy = null;
        if (ObjectPooler.Instance != null)
            enemy = ObjectPooler.Instance.GetFromPool(prefab.name, position, Quaternion.identity);

        if (enemy == null)
            Instantiate(prefab, position, Quaternion.identity);
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying || playerTransform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerTransform.position, maxSpawnRadius);
    }
}
