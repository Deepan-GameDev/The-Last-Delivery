using UnityEngine;
using System.Collections.Generic;

public class RouteEnemySpawner : MonoBehaviour
{
    [Header("Enemy Prefab")]
    [SerializeField] private GameObject enemyPrefab;

    [Header("Safe Route")]
    [SerializeField] private Transform[] safeSpawnPoints;
    [SerializeField] private int safeEnemyCount = 2;

    [Header("Risk Route")]
    [SerializeField] private Transform[] riskSpawnPoints;
    [SerializeField] private int riskEnemyCount = 4;

    private List<GameObject> spawnedEnemies =
        new List<GameObject>();

    public void SpawnSafeEnemies()
    {
        ClearEnemies();

        SpawnEnemies(
            safeSpawnPoints,
            safeEnemyCount
        );

        Debug.Log(
            "SAFE ROUTE → " +
            safeEnemyCount +
            " enemies spawned."
        );
    }

    public void SpawnRiskEnemies()
    {
        ClearEnemies();

        SpawnEnemies(
            riskSpawnPoints,
            riskEnemyCount
        );

        Debug.Log(
            "RISK ROUTE → " +
            riskEnemyCount +
            " enemies spawned."
        );
    }

    private void SpawnEnemies(
        Transform[] spawnPoints,
        int enemyCount)
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning(
                "RouteEnemySpawner: Enemy Prefab is not assigned!"
            );
            return;
        }

        if (spawnPoints == null ||
            spawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "RouteEnemySpawner: No spawn points assigned!"
            );
            return;
        }

        int count = Mathf.Min(
            enemyCount,
            spawnPoints.Length
        );

        List<Transform> availablePoints =
            new List<Transform>(spawnPoints);

        for (int i = 0; i < count; i++)
        {
            int randomIndex =
                Random.Range(
                    0,
                    availablePoints.Count
                );

            Transform spawnPoint =
                availablePoints[randomIndex];

            GameObject enemy =
                Instantiate(
                    enemyPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            spawnedEnemies.Add(enemy);

            availablePoints.RemoveAt(randomIndex);
        }
    }

    public void ClearEnemies()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }

        spawnedEnemies.Clear();
    }
}