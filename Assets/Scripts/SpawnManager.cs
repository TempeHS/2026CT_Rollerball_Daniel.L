using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject objectPrefab;
    public GameObject powerUpPrefab; // maybe later
    public Transform pickupParent;

    [Header("Amounts")]
    public int spawnAmount = 19;
    public int powerUpAmount = 4;

    [Header("Spawn Area")]
    public float xMin = -9.5f;
    public float xMax = 9.5f;
    public float zMin = -9.5f;
    public float zMax = 9.5f;
    public float spawnY = 0.5f;

    [Header("Collision Check")]
    [Min(0.05f)] public float spawnCheckRadius = 0.45f;
    [Min(1)] public int maxSpawnAttempts = 30;
    public LayerMask blockedSpawnLayers = ~0;

    private void Start()
    {
        SpawnPickups();
        SpawnPowerUps();
    }

    private void SpawnPickups()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            if (!TryGetValidSpawnPosition(out Vector3 spawnPosition))
            {
                Debug.LogWarning("SpawnManager: Could not find free position for pickup.");
                continue;
            }

            GameObject obj = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
            if (pickupParent != null)
            {
                obj.transform.SetParent(pickupParent);
            }
        }
    }

    private void SpawnPowerUps()
    {
        if (powerUpPrefab == null)
        {
            return;
        }

        for (int i = 0; i < powerUpAmount; i++)
        {
            if (!TryGetValidSpawnPosition(out Vector3 spawnPosition))
            {
                Debug.LogWarning("SpawnManager: Could not find free position for power-up.");
                continue;
            }

            Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
        }
    }

    private bool TryGetValidSpawnPosition(out Vector3 spawnPosition)
    {
        for (int attempt = 0; attempt < maxSpawnAttempts; attempt++)
        {
            Vector3 candidate = GetRandomPosition();

            bool blocked = Physics.CheckSphere(
                candidate,
                spawnCheckRadius,
                blockedSpawnLayers,
                QueryTriggerInteraction.Ignore
            );

            if (!blocked)
            {
                spawnPosition = candidate;
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private Vector3 GetRandomPosition()
    {
        return new Vector3(
            Random.Range(xMin, xMax),
            spawnY,
            Random.Range(zMin, zMax)
        );
    }
}