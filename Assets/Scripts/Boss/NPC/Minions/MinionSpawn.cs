using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionSpawn : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] enemyPrefabs;   // Heart, Spade, Club, Clover
    public float baseSpawnInterval = 3f;    
    public float randomOffset = 1.5f;       
    public int maxEnemies = 8;

    [Header("Spawn Position")]
    public float spawnXOffset = 2f;        
    public float spawnYMin = -1f;
    public float spawnYMax = 2f;

    private Camera mainCam;
    private List<GameObject> activeEnemies = new List<GameObject>();

    void Start()
    {
        mainCam = Camera.main;
        StartCoroutine(SpawnLoop());
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            activeEnemies.RemoveAll(e => e == null);

            if (activeEnemies.Count < maxEnemies)
                SpawnEnemy();

            float interval = baseSpawnInterval + Random.Range(-randomOffset, randomOffset);
            interval = Mathf.Max(0.5f, interval); 
            yield return new WaitForSeconds(interval);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefabs.Length == 0) return;

        Vector3 rightEdge = mainCam.ViewportToWorldPoint(new Vector3(1, 0.5f, 0));
        float spawnX = rightEdge.x + spawnXOffset;
        float spawnY = Random.Range(spawnYMin, spawnYMax);
        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0);

        GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);

        activeEnemies.Add(enemy);
    }
}
