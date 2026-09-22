using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class CustomerSpawner : MonoBehaviour
{
    [Header("Customer")]
    [SerializeField] private GameObject customerPrefab;

    [Header("Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI deliverText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Timer")]
    [SerializeField] private DeliveryTimer deliveryTimer;

    private List<Transform> usedSpawnPoints =
        new List<Transform>();

    private List<GameObject> spawnedCustomers =
        new List<GameObject>();

    public void SpawnCustomers(List<string> customerNames)
    {
        if (customerPrefab == null)
        {
            Debug.LogWarning(
                "CustomerSpawner: Customer Prefab is not assigned!"
            );
            return;
        }

        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning(
                "CustomerSpawner: No Spawn Points assigned!"
            );
            return;
        }

        ClearSpawnedCustomers();

        usedSpawnPoints.Clear();

        foreach (string customerName in customerNames)
        {
            Transform spawnPoint = GetRandomFreeSpawnPoint();

            if (spawnPoint == null)
            {
                Debug.LogWarning(
                    "No free customer spawn point available!"
                );
                break;
            }

            GameObject customer =
                Instantiate(
                    customerPrefab,
                    spawnPoint.position,
                    spawnPoint.rotation
                );

            CustomerDelivery customerDelivery =
                customer.GetComponent<CustomerDelivery>();

            if (customerDelivery != null)
            {
                customerDelivery.SetCustomerName(
                    customerName
                );

                customerDelivery.SetDeliveryReferences(
                    deliverText,
                    objectiveText,
                    deliveryTimer
                );
}

            spawnedCustomers.Add(customer);
        }
    }

    private Transform GetRandomFreeSpawnPoint()
    {
        List<Transform> availablePoints =
            new List<Transform>();

        foreach (Transform point in spawnPoints)
        {
            if (!usedSpawnPoints.Contains(point))
            {
                availablePoints.Add(point);
            }
        }

        if (availablePoints.Count == 0)
            return null;

        int randomIndex =
            Random.Range(0, availablePoints.Count);

        Transform selectedPoint =
            availablePoints[randomIndex];

        usedSpawnPoints.Add(selectedPoint);

        return selectedPoint;
    }

    public void ClearSpawnedCustomers()
    {
        foreach (GameObject customer in spawnedCustomers)
        {
            if (customer != null)
                Destroy(customer);
        }

        spawnedCustomers.Clear();
        usedSpawnPoints.Clear();
    }
}