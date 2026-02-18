using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public GameObject objectPrefab;
    
    public int spawnAmount = 10;
    
    public float xMin = -10f;
    public float xMax = 10f;
    public float zMin = -10f;
    public float zMax = 10f;
    public float spawnY = 0.5f;

    void Start()
    {
        SpawnObjects();
    }

    void SpawnObjects()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Vector3 spawnPosition = new Vector3(
                Random.Range(xMin, xMax),
                spawnY,
                Random.Range(zMin, zMax)
            );

            Instantiate(objectPrefab, spawnPosition, Quaternion.identity);
        }
    }
}