using UnityEngine;
using System.Collections;

public class BombSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject bombPrefab;
    public GameObject shadowPrefab;
    public Transform[] platforms; 
    public float spawnHeight = 8f; 
    public float dropSpeed = 8f;   
    public float delayBeforeFall = 1.0f; 

    private bool isSpawning = false;

    public void SpawnBomb()
    {
        if (isSpawning || platforms.Length == 0) return;
        StartCoroutine(SpawnBombRoutine());
    }

    IEnumerator SpawnBombRoutine()
    {
        isSpawning = true;

        int index = Random.Range(0, platforms.Length);
        Transform targetPlatform = platforms[index];
        Vector3 targetPos = targetPlatform.position;

        GameObject shadow = Instantiate(shadowPrefab, targetPos, Quaternion.identity);
        SpriteRenderer sr = shadow.GetComponent<SpriteRenderer>();
        sr.color = new Color(0, 0, 0, 0.4f);

        yield return new WaitForSeconds(delayBeforeFall);

        Vector3 spawnPos = targetPos + Vector3.up * spawnHeight;
        GameObject bomb = Instantiate(bombPrefab, spawnPos, Quaternion.identity);

        Bomb bombScript = bomb.GetComponent<Bomb>();
        if (bombScript != null)
            bombScript.shadowInstance = shadow;

        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.gravityScale = 1f;

        yield return new WaitUntil(() => bomb == null);
        if (shadow != null)
            Destroy(shadow);

        isSpawning = false;
    }
}
