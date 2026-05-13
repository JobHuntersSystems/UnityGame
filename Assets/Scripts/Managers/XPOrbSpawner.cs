using UnityEngine;

public class XPOrbSpawner : MonoBehaviour
{
    [Header("Prefabs de Orbes")]
    public GameObject orbBasicPrefab;
    public GameObject orbEnhancedPrefab;
    public GameObject orbSuperiorPrefab;

    [Header("Spawn Inicial")]
    public int initialOrbCount = 30;
    public Vector2 mapMin = new Vector2(-20f, -20f);
    public Vector2 mapMax = new Vector2(20f, 20f);

    [Header("Spawn Continuo (offscreen)")]
    public int maxTotalOrbs = 60;
    public float spawnInterval = 2f;
    public int orbsPerWave = 3;
    public float maxSpawnRadius = 20f;

    [Header("Probabilidades (deben sumar 100)")]
    [Range(0, 100)] public int chanceBasic = 70;
    [Range(0, 100)] public int chanceEnhanced = 25;
    // Superior = 100 - chanceBasic - chanceEnhanced

    private Transform playerTransform;
    private Camera mainCamera;
    private float spawnTimer;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player")?.transform;
        mainCamera = Camera.main;
        SpawnInitial();
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

    void SpawnInitial()
    {
        for (int i = 0; i < initialOrbCount; i++)
        {
            Vector2 pos = new Vector2(
                Random.Range(mapMin.x, mapMax.x),
                Random.Range(mapMin.y, mapMax.y)
            );
            SpawnOrbAt(pos);
        }
    }

    void TrySpawnWave()
    {
        int current = GameObject.FindGameObjectsWithTag("XPOrb").Length;
        if (current >= maxTotalOrbs) return;

        int toSpawn = Mathf.Min(orbsPerWave, maxTotalOrbs - current);
        for (int i = 0; i < toSpawn; i++)
        {
            if (TryGetOffscreenPosition(out Vector2 pos))
                SpawnOrbAt(pos);
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

    void SpawnOrbAt(Vector2 position)
    {
        GameObject prefab = PickPrefab();
        if (prefab != null)
            Instantiate(prefab, position, Quaternion.identity);
    }

    GameObject PickPrefab()
    {
        int roll = Random.Range(0, 100);
        if (roll < chanceBasic) return orbBasicPrefab;
        if (roll < chanceBasic + chanceEnhanced) return orbEnhancedPrefab;
        return orbSuperiorPrefab;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3((mapMin.x + mapMax.x) / 2f, (mapMin.y + mapMax.y) / 2f);
        Vector3 size   = new Vector3(mapMax.x - mapMin.x, mapMax.y - mapMin.y);
        Gizmos.DrawWireCube(center, size);

        if (Application.isPlaying && playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(playerTransform.position, maxSpawnRadius);
        }
    }
}
