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

    [Header("Drone Prefab")]
    [SerializeField] private GameObject dronePrefab;

    [Header("Drone Spawn")]
    [SerializeField] private Transform[] droneSpawnPoints;

    [Header("Drone Patrol")]
    [SerializeField] private Transform[] dronePatrolPoints;

    [Header("Drone Settings")]
    [SerializeField] private int riskDroneCount = 1;

    private List<GameObject> spawnedEnemies =
        new List<GameObject>();

    private List<GameObject> spawnedDrones =
        new List<GameObject>();


    // =========================================================
    // SAFE ROUTE
    // =========================================================

    public void SpawnSafeEnemies()
    {
        ClearEnemiesAndDrones();

        SpawnEnemies(
            safeSpawnPoints,
            safeEnemyCount
        );

        Debug.Log(
            "SAFE ROUTE → " +
            safeEnemyCount +
            " enemies spawned. No drone."
        );
    }


    // =========================================================
    // RISK ROUTE
    // =========================================================

    public void SpawnRiskEnemies()
    {
        ClearEnemiesAndDrones();

        SpawnEnemies(
            riskSpawnPoints,
            riskEnemyCount
        );

        SpawnDrones(
            riskDroneCount
        );

        Debug.Log(
            "RISK ROUTE → " +
            riskEnemyCount +
            " enemies + " +
            riskDroneCount +
            " drone spawned."
        );
    }


    // =========================================================
    // ENEMY SPAWNING
    // =========================================================

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
                "RouteEnemySpawner: Enemy spawn points are not assigned!"
            );

            return;
        }

        int count =
            Mathf.Min(
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

            availablePoints.RemoveAt(
                randomIndex
            );
        }
    }


    // =========================================================
    // DRONE SPAWNING
    // =========================================================

    private void SpawnDrones(int droneCount)
    {
        if (dronePrefab == null)
        {
            Debug.LogWarning(
                "RouteEnemySpawner: Drone Prefab is not assigned!"
            );

            return;
        }

        if (droneSpawnPoints == null ||
            droneSpawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "RouteEnemySpawner: Drone spawn points are not assigned!"
            );

            return;
        }

        int count =
            Mathf.Min(
                droneCount,
                droneSpawnPoints.Length
            );

        List<Transform> availablePoints =
            new List<Transform>(
                droneSpawnPoints
            );

        for (int i = 0; i < count; i++)
        {
            int randomIndex =
                Random.Range(
                    0,
                    availablePoints.Count
                );

            Transform spawnPoint =
                availablePoints[randomIndex];

            GameObject drone =
                Instantiate(
                    dronePrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            // Give patrol points to the spawned drone
            DroneAI droneAI =
                drone.GetComponent<DroneAI>();

            if (droneAI != null)
            {
                droneAI.SetPatrolPoints(
                    dronePatrolPoints
                );
            }
            else
            {
                Debug.LogWarning(
                    "RouteEnemySpawner: " +
                    "Drone prefab does not contain DroneAI!"
                );
            }

            spawnedDrones.Add(drone);

            availablePoints.RemoveAt(
                randomIndex
            );
        }
    }


    // =========================================================
    // CLEANUP
    // =========================================================

    public void ClearEnemiesAndDrones()
    {
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();


        foreach (GameObject drone in spawnedDrones)
        {
            if (drone != null)
            {
                Destroy(drone);
            }
        }

        spawnedDrones.Clear();

        Debug.Log(
            "Route enemies and drones cleared."
        );
    }
}