using UnityEngine;

public class SpawnManager : MonoBehaviour
{
public GameObject objectPrefab;
public GameObject powerUpPrefab;
public Transform pickupParent;

public int spawnAmount = 19;
public int powerUpAmount = 4;

public float xMin = -9.5f;
public float xMax = 9.5f;
public float zMin = -9.5f;
public float zMax = 9.5f;
public float spawnY = 0.5f;

void Start()
{
SpawnPickups();
SpawnPowerUps();
}

void SpawnPickups()
{
for (int i = 0; i < spawnAmount; i++)
{
Vector3 spawnPosition = GetRandomPosition();
GameObject obj = Instantiate(objectPrefab, spawnPosition, Quaternion.identity);

if (pickupParent != null)
{
obj.transform.SetParent(pickupParent);
}
}
}

void SpawnPowerUps()
{
if (powerUpPrefab == null)
{
return;
}

for (int i = 0; i < powerUpAmount; i++)
{
Vector3 spawnPosition = GetRandomPosition();
Instantiate(powerUpPrefab, spawnPosition, Quaternion.identity);
}
}

Vector3 GetRandomPosition()
{
return new Vector3(
Random.Range(xMin, xMax),
spawnY,
Random.Range(zMin, zMax)
);
}
}