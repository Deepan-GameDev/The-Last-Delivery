using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("Bag")]
    [SerializeField] private int bagCapacity = 1;

public int BagCapacity => bagCapacity;

    public int CurrentOrderValue { get; private set; }
    public int ShopkeeperShare { get; private set; }
    public int PlayerProfit { get; private set; }

    public bool AwaitingPayment { get; private set; }

    private int coins = 0;

    public string CurrentCustomer { get; private set; }
    public int CurrentReward { get; private set; }
    public bool DeliveryActive { get; private set; }

    private void Start()
    {
        UpdateCoinUI();

        if (deliveryCompletePanel != null)
            deliveryCompletePanel.SetActive(false);
    }

    public void GenerateDelivery()
    {
        if (customerNames.Length == 0)
            return;

        int randomIndex = Random.Range(0, customerNames.Length);

        CurrentCustomer = customerNames[randomIndex];

        CurrentOrderValue = baseOrderValue;

        // Player's profit is 50% of total order value
        ShopkeeperShare = CurrentOrderValue / 2;
        PlayerProfit = CurrentOrderValue - ShopkeeperShare;

        CurrentReward = PlayerProfit;

        DeliveryActive = false;
        AwaitingPayment = false;

        Debug.Log(
            "New Delivery → Customer: " +
            CurrentCustomer +
            " | Order Value: " +
            CurrentOrderValue +
            " | Player Profit: " +
            PlayerProfit
        );
    }

    public void AcceptDelivery()
    {
        DeliveryActive = true;

        if (startDoor != null)
        {
            startDoor.OpenDoor();
        }

        Debug.Log(
            "Delivery Started → Deliver to: " +
            CurrentCustomer
        );
    }

    public void CompleteDelivery()
    {
        DeliveryActive = false;

        // Payment is now waiting at the shopkeeper
        AwaitingPayment = true;

        // Open the selected route door
        if (routeManager != null)
        {
            routeManager.OpenSelectedDoor();
        }

        ShowDeliveryComplete();

        Debug.Log(
            "Delivery Completed! Return to Shopkeeper for payment."
        );
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = "COINS: " + coins;
        }
    }

    private void ShowDeliveryComplete()
    {
        if (deliveryCompletePanel == null)
            return;

        deliveryCompletePanel.SetActive(true);

        if (deliveryCompleteText != null)
        {
            deliveryCompleteText.text =
                "DELIVERY COMPLETE!\n+" +
                CurrentReward +
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

    public void CollectPayment()
    {
        if (!AwaitingPayment)
            return;

        coins += PlayerProfit;

        AwaitingPayment = false;

        UpdateCoinUI();

        Debug.Log(
            "Payment Collected! +" +
            PlayerProfit +
            " Coins | Total Coins: " +
            coins
        );
    }
}