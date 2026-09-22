using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DeliveryManager : MonoBehaviour
{
    [Header("Customer Names")]
    [SerializeField] private string[] customerNames =
    {
        "Arun",
        "Priya",
        "Karthik",
        "Meena",
        "Rahul",
        "Divya",
        "Vikram",
        "Ananya"
    };

    [Header("Delivery")]
    [SerializeField] private int baseReward = 50;

    [Header("Start Door")]
    [SerializeField] private DeliveryStartDoor startDoor;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject deliveryCompletePanel;
    [SerializeField] private TextMeshProUGUI deliveryCompleteText;

    [Header("Route")]
    [SerializeField] private RouteManager routeManager;

    [Header("Payment")]
    [SerializeField] private int baseOrderValue = 100;
    [SerializeField] private int failurePenalty = 25;

    [Header("Bag")]
    [SerializeField] private int bagCapacity = 1;

    [Header("Customer Spawner")]
    [SerializeField] private CustomerSpawner customerSpawner;

    public int BagCapacity => bagCapacity;

    public void SetBagCapacity(int newCapacity)
    {
        bagCapacity = Mathf.Max(1, newCapacity);

        Debug.Log(
            "Delivery Bag Capacity Updated → " +
            bagCapacity
        );
    }

    public bool DeliveryFailedState { get; private set; }
    public int FailurePenalty => failurePenalty;

    // Current active customers
    private List<string> activeCustomers = new List<string>();

    // Number of customers already delivered
    private int deliveredCount = 0;

    public string CurrentCustomer
    {
        get
        {
            if (activeCustomers.Count > 0)
                return activeCustomers[0];

            return "";
        }
    }

    public int CurrentOrderValue { get; private set; }
    public int ShopkeeperShare { get; private set; }
    public int PlayerProfit { get; private set; }

    public int CurrentReward { get; private set; }

    public bool AwaitingPayment { get; private set; }

    public bool DeliveryActive { get; private set; }

    public int ActiveOrderCount => activeCustomers.Count;

    public string GetActiveCustomersText()
    {
        if (activeCustomers.Count == 0)
            return "";

        string result = "";

        for (int i = 0; i < activeCustomers.Count; i++)
        {
            result +=
                "Customer " +
                (i + 1) +
                ": " +
                activeCustomers[i];

            if (i < activeCustomers.Count - 1)
                result += "\n";
        }

        return result;
    }

    public List<string> GetActiveCustomerNames()
    {
        return new List<string>(activeCustomers);
    }

    private int coins = 0;

    private void Start()
    {
        UpdateCoinUI();

        if (deliveryCompletePanel != null)
            deliveryCompletePanel.SetActive(false);
    }

    // --------------------------------------------------
    // GENERATE FIRST ORDER
    // --------------------------------------------------

    public void GenerateDelivery()
    {
        if (customerNames.Length == 0)
            return;

        if (activeCustomers.Count >= bagCapacity)
        {
            Debug.Log("Bag is full!");
            return;
        }

        string newCustomer = GetRandomCustomer();

        if (newCustomer == "")
            return;

        activeCustomers.Add(newCustomer);

        CurrentOrderValue = baseOrderValue;

        UpdatePaymentValues();

        DeliveryActive = false;
        AwaitingPayment = false;
        deliveredCount = 0;

        // Spawn all currently active customers
        if (customerSpawner != null)
        {
            customerSpawner.SpawnCustomers(
                activeCustomers
            );
        }

        Debug.Log(
            "Order Added → Customer: " +
            newCustomer +
            " | Orders in Bag: " +
            activeCustomers.Count +
            "/" +
            bagCapacity
        );
    }

    // --------------------------------------------------
    // RANDOM CUSTOMER
    // --------------------------------------------------

    private string GetRandomCustomer()
    {
        List<string> availableCustomers = new List<string>();

        foreach (string customer in customerNames)
        {
            if (!activeCustomers.Contains(customer))
            {
                availableCustomers.Add(customer);
            }
        }

        if (availableCustomers.Count == 0)
            return "";

        int randomIndex =
            Random.Range(0, availableCustomers.Count);

        return availableCustomers[randomIndex];
    }

    // --------------------------------------------------
    // PAYMENT CALCULATION
    // --------------------------------------------------

    private void UpdatePaymentValues()
    {
        CurrentOrderValue =
            baseOrderValue * activeCustomers.Count;

        ShopkeeperShare =
            CurrentOrderValue / 2;

        PlayerProfit =
            CurrentOrderValue - ShopkeeperShare;

        CurrentReward = PlayerProfit;
    }

    // --------------------------------------------------
    // ACCEPT DELIVERY
    // --------------------------------------------------

    public void AcceptDelivery()
    {
        if (activeCustomers.Count == 0)
            return;

        // Don't start the actual delivery yet.
        // Delivery starts only after the player
        // finishes taking all available orders.

        Debug.Log(
            "Order Accepted → " +
            CurrentCustomer +
            " | Orders: " +
            activeCustomers.Count +
            "/" +
            bagCapacity
        );
    }
    // --------------------------------------------------
    // CUSTOMER DELIVERED
    // --------------------------------------------------

    public void CompleteDelivery(bool wasLate)
{
    if (!DeliveryActive)
        return;

    if (activeCustomers.Count == 0)
        return;

    string completedCustomer =
        activeCustomers[0];

    // Remove delivered customer
    activeCustomers.RemoveAt(0);

    deliveredCount++;

    // ---------------------------------------------
    // LATE PENALTY
    // ---------------------------------------------

    if (wasLate)
    {
        PlayerProfit = Mathf.Max(
            0,
            PlayerProfit - failurePenalty
        );

        Debug.Log(
            completedCustomer +
            " delivered late. Penalty: -" +
            failurePenalty +
            " Coins"
        );
    }

    // Route door opens after actual delivery
    if (routeManager != null)
    {
        routeManager.OpenSelectedDoor();
    }

    // ---------------------------------------------
    // MORE CUSTOMERS REMAIN
    // ---------------------------------------------

    if (activeCustomers.Count > 0)
    {
        Debug.Log(
            "Delivered: " +
            completedCustomer +
            " | Next Customer: " +
            CurrentCustomer +
            " | Remaining Profit: " +
            PlayerProfit
        );

        ShowNextCustomer();

        return;
    }

    // ---------------------------------------------
    // ALL CUSTOMERS DELIVERED
    // ---------------------------------------------

    DeliveryActive = false;
    AwaitingPayment = true;

    // IMPORTANT:
    // Do NOT call UpdatePaymentValues() here.
    // The active customer list is now empty,
    // but PlayerProfit must keep the earned amount.

    ShowDeliveryComplete();

    Debug.Log(
        "All Deliveries Completed! " +
        "Final Player Profit: " +
        PlayerProfit +
        " Coins. Return to Shopkeeper."
    );
}

    // --------------------------------------------------
    // NEXT CUSTOMER
    // --------------------------------------------------

    private void ShowNextCustomer()
    {
        Debug.Log(
            "NEXT CUSTOMER → " +
            CurrentCustomer
        );
    }

    // --------------------------------------------------
    // DELIVERY COMPLETE UI
    // --------------------------------------------------

    private void ShowDeliveryComplete()
    {
        if (deliveryCompletePanel == null)
            return;

        deliveryCompletePanel.SetActive(true);

        if (deliveryCompleteText != null)
        {
            deliveryCompleteText.text =
                "ALL DELIVERIES COMPLETE!\n+" +
                PlayerProfit +
                " COINS";
        }

        StartCoroutine(HideCompletePanel());
    }

    private IEnumerator HideCompletePanel()
    {
        yield return new WaitForSeconds(2.5f);

        if (deliveryCompletePanel != null)
            deliveryCompletePanel.SetActive(false);
    }

    // --------------------------------------------------
    // PAYMENT
    // --------------------------------------------------

    public void CollectPayment()
    {
        if (!AwaitingPayment)
            return;

        coins += PlayerProfit;

        AwaitingPayment = false;

        UpdateCoinUI();

        if (startDoor != null)
        {
            startDoor.CloseDoor();
        }

        if (customerSpawner != null)
        {
            customerSpawner.ClearSpawnedCustomers();
        }

        Debug.Log(
            "Payment Collected! +" +
            PlayerProfit +
            " Coins | Total Coins: " +
            coins
        );

        // Reset delivery data
        activeCustomers.Clear();
        deliveredCount = 0;
        CurrentOrderValue = 0;
        ShopkeeperShare = 0;
        PlayerProfit = 0;
        CurrentReward = 0;
    }

    // --------------------------------------------------
    // FAILED DELIVERY
    // --------------------------------------------------

    public void DeliveryFailed()
    {
        if (!DeliveryActive)
            return;

        DeliveryActive = false;
        AwaitingPayment = true;
        DeliveryFailedState = true;

        if (routeManager != null)
        {
            routeManager.OpenSelectedDoor();
        }

        Debug.Log(
            "DELIVERY FAILED! Penalty: " +
            failurePenalty +
            " Coins. Return to Shopkeeper."
        );
    }

    // --------------------------------------------------
    // COINS
    // --------------------------------------------------

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text =
                "COINS: " + coins;
        }
    }

    public bool HasEnoughCoins(int amount)
    {
        return coins >= amount;
    }

    public void SpendCoins(int amount)
    {
        if (amount <= 0)
            return;

        if (coins < amount)
            return;

        coins -= amount;

        UpdateCoinUI();

        Debug.Log(
            "Spent " +
            amount +
            " Coins | Remaining: " +
            coins
        );
    }

    public void StartDelivery()
    {
        if (activeCustomers.Count == 0)
            return;

        DeliveryActive = true;
        AwaitingPayment = false;

        if (startDoor != null)
        {
            startDoor.OpenDoor();
        }

        Debug.Log(
            "DELIVERY STARTED → " +
            "First Customer: " +
            CurrentCustomer +
            " | Total Orders: " +
            activeCustomers.Count
        );
    }
}