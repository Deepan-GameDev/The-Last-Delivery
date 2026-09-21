using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRange = 2.5f;

    [Header("Order UI")]
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private GameObject deliveryOrderPanel;
    [SerializeField] private TextMeshProUGUI customerText;
    [SerializeField] private TextMeshProUGUI rewardText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Payment UI")]
    [SerializeField] private GameObject paymentPanel;
    [SerializeField] private TextMeshProUGUI orderAmountText;
    [SerializeField] private TextMeshProUGUI shopkeeperAmountText;
    [SerializeField] private TextMeshProUGUI profitText;

    [Header("Delivery")]
    [SerializeField] private DeliveryManager deliveryManager;

    private bool canInteract;

    private void Update()
    {
        if (interactionPoint == null || deliveryManager == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            interactionPoint.position
        );

        canInteract = distance <= interactionRange;

        // Show TALK only when near Shopkeeper
        // and no panel is currently open.
        if (interactText != null)
        {
            bool panelOpen =
                deliveryOrderPanel != null &&
                deliveryOrderPanel.activeSelf;

            bool paymentOpen =
                paymentPanel != null &&
                paymentPanel.activeSelf;

            // Shopkeeper interaction is allowed when:
            // 1. No delivery is currently active
            // OR
            // 2. Payment is waiting to be collected
            bool canTalkToShopkeeper =
                !deliveryManager.DeliveryActive;

            interactText.gameObject.SetActive(
                canInteract &&
                canTalkToShopkeeper &&
                !panelOpen &&
                !paymentOpen
            );
        }

        if (!canInteract)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            // 1. Payment is pending
            if (deliveryManager.AwaitingPayment)
            {
                OpenPaymentPanel();
                return;
            }

            // 2. Delivery is currently active
            // Don't allow a new order
            if (deliveryManager.DeliveryActive)
            {
                return;
            }

            // 3. No active delivery and no payment pending
            // Start a new order
            OpenDeliveryOrder();
        }
    }

    private void OpenDeliveryOrder()
    {
        deliveryManager.GenerateDelivery();

        if (customerText != null)
        {
            customerText.text =
                "Customer: " +
                deliveryManager.CurrentCustomer;
        }

        if (rewardText != null)
        {
            rewardText.text =
                "Reward: " +
                deliveryManager.CurrentReward +
                " Coins";
        }

        deliveryOrderPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OpenPaymentPanel()
    {
        if (paymentPanel == null)
            return;

        if (orderAmountText != null)
        {
            orderAmountText.text =
                "Order Value: " +
                deliveryManager.CurrentOrderValue +
                " Coins";
        }

        if (shopkeeperAmountText != null)
        {
            shopkeeperAmountText.text =
                "Shopkeeper: " +
                deliveryManager.ShopkeeperShare +
                " Coins";
        }

        if (profitText != null)
        {
            profitText.text =
                "YOUR PROFIT: " +
                deliveryManager.PlayerProfit +
                " Coins";
        }

        paymentPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AcceptDelivery()
    {
        deliveryOrderPanel.SetActive(false);

        if (interactText != null)
            interactText.gameObject.SetActive(false);

        deliveryManager.AcceptDelivery();

        if (objectiveText != null)
        {
            objectiveText.text =
                "DELIVER TO: " +
                deliveryManager.CurrentCustomer;

            objectiveText.gameObject.SetActive(true);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void CollectPayment()
    {
        deliveryManager.CollectPayment();

        if (paymentPanel != null)
            paymentPanel.SetActive(false);

        if (objectiveText != null)
            objectiveText.gameObject.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}