using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject objectPrefab;
    public GameObject powerUpPrefab;
    public GameObject dynamicObjectPrefab;
    public Transform pickupParent;
    public Transform objectsParent;

    [Header("Spawn Volume")]
    public int spawnAmount = 19;
    public int powerUpAmount = 4;
    public int dynamicObjectAmount = 4;
    public float xMin = -9.5f, xMax = 9.5f, zMin = -9.5f, zMax = 9.5f, spawnY = 0.5f;

    [Header("Collision Check")]
    [Min(0.05f)] public float spawnCheckRadius = 0.45f;
    [Min(1)] public int maxSpawnAttempts = 30;
    public LayerMask blockedSpawnLayers = ~0;

    private void Start()
    {
        SpawnObjects(objectPrefab, spawnAmount, pickupParent);
        SpawnObjects(powerUpPrefab, powerUpAmount, pickupParent);
        SpawnObjects(dynamicObjectPrefab, dynamicObjectAmount, objectsParent);
    }

    private void SpawnObjects(GameObject prefab, int count, Transform parent)
    {
        if (prefab == null)
            return;

        for (int i = 0; i < count; i++)
        {
            if (!TryGetValidSpawnPosition(out Vector3 pos))
            {
                Debug.LogWarning($"SpawnManager: Could not spawn {prefab.name} (attempt {i + 1}/{count})");
                continue;
            }

            GameObject obj = Instantiate(prefab, pos, Quaternion.identity);
            if (parent != null)
                obj.transform.SetParent(parent);
        }
    }

    private bool TryGetValidSpawnPosition(out Vector3 spawnPosition)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 candidate = GetRandomPosition();
            if (!Physics.CheckSphere(candidate, spawnCheckRadius, blockedSpawnLayers, QueryTriggerInteraction.Ignore))
            {
                spawnPosition = candidate;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private Vector3 GetRandomPosition() => new(Random.Range(xMin, xMax), spawnY, Random.Range(zMin, zMax));
}