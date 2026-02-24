using UnityEngine;
using System.Collections.Generic;

public class SpawnManager : MonoBehaviour
{
    public GameObject objectPrefab;
    
    public int spawnAmount = 19;
    
    public float xMin = -9.5f;
    public float xMax = 9.5f;
    public float zMin = -9.5f;
    public float zMax = 9.5f;
    public float spawnY = 0.5f;

void Awake() { Debug.Log("SpawnManager Awake on: " + gameObject.name); } 
void Start() { Debug.Log("SpawnManager Start on: " + gameObject.name); SpawnObjects(); }

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