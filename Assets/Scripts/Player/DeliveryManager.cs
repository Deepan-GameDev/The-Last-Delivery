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

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI coinText;
    [SerializeField] private GameObject deliveryCompletePanel;
    [SerializeField] private TextMeshProUGUI deliveryCompleteText;

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
        CurrentReward = baseReward;

        DeliveryActive = false;

        Debug.Log(
            "New Delivery → Customer: " +
            CurrentCustomer +
            " | Reward: " +
            CurrentReward
        );
    }

    public void AcceptDelivery()
    {
        DeliveryActive = true;

        Debug.Log(
            "Delivery Started → Deliver to: " +
            CurrentCustomer
        );
    }

    public void CompleteDelivery()
    {
        DeliveryActive = false;

        coins += CurrentReward;

        UpdateCoinUI();

        ShowDeliveryComplete();

        Debug.Log(
            "Delivery Completed! +" +
            CurrentReward +
            " Coins | Total Coins: " +
            coins
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
}