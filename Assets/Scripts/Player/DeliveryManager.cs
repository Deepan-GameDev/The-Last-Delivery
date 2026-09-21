using UnityEngine;

public class DeliveryManager : MonoBehaviour
{
    [Header("Customer Names")]
    [SerializeField] private string[] customerNames =
    {
        "Arun",
        "Priya",
        "Karthik",
        "Harni",
        "Bala",
        "Divya",
        "Vikram",
        "Ananya"
    };

    [Header("Delivery")]
    [SerializeField] private int baseReward = 50;

    public string CurrentCustomer { get; private set; }
    public int CurrentReward { get; private set; }

    public void GenerateDelivery()
    {
        if (customerNames.Length == 0)
            return;

        int randomIndex = Random.Range(0, customerNames.Length);

        CurrentCustomer = customerNames[randomIndex];
        CurrentReward = baseReward;

        Debug.Log(
            "New Delivery → Customer: " +
            CurrentCustomer +
            " | Reward: " +
            CurrentReward
        );
    }
}