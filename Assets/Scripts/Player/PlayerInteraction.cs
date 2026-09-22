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

    [Header("Timer")]
    [SerializeField] private DeliveryTimer deliveryTimer;

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

        // -----------------------------------------
        // SHOPKEEPER TALK UI
        // -----------------------------------------

        if (interactText != null)
        {
            bool panelOpen =
                deliveryOrderPanel != null &&
                deliveryOrderPanel.activeSelf;

            bool paymentOpen =
                paymentPanel != null &&
                paymentPanel.activeSelf;

            // Can talk when:
            // - No delivery currently running
            // - Payment is not waiting
            bool canTalkToShopkeeper =
                !deliveryManager.DeliveryActive &&
                !deliveryManager.AwaitingPayment;

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
            // Payment pending
            if (deliveryManager.AwaitingPayment)
            {
                OpenPaymentPanel();
                return;
            }

            // Delivery currently active
            if (deliveryManager.DeliveryActive)
            {
                return;
            }

            // Open next order
            OpenDeliveryOrder();
        }
    }

    // =========================================
    // OPEN DELIVERY ORDER
    // =========================================

    private void OpenDeliveryOrder()
    {
        if (deliveryManager.ActiveOrderCount >=
            deliveryManager.BagCapacity)
        {
            Debug.Log("BAG FULL!");
            return;
        }

        deliveryManager.GenerateDelivery();

        UpdateOrderUI();

        deliveryOrderPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    // =========================================
    // UPDATE ORDER UI
    // =========================================

    private void UpdateOrderUI()
    {
        if (customerText != null)
        {
            customerText.text =
                deliveryManager.GetActiveCustomersText();
        }

        if (rewardText != null)
        {
            rewardText.text =
                "Reward: " +
                deliveryManager.CurrentReward +
                " Coins";
        }
    }
    // =========================================
    // ACCEPT DELIVERY
    // =========================================

    public void AcceptDelivery()
    {
        if (deliveryOrderPanel != null)
            deliveryOrderPanel.SetActive(false);

        if (interactText != null)
            interactText.gameObject.SetActive(false);

        deliveryManager.AcceptDelivery();

        int currentOrders =
            deliveryManager.ActiveOrderCount;

        // Bag is full → start delivery
        if (currentOrders >= deliveryManager.BagCapacity)
        {
            deliveryManager.StartDelivery();

            if (deliveryTimer != null)
            {
                if (currentOrders == 1)
                {
                    deliveryTimer.StartTimer();
                }
                else
                {
                    deliveryTimer.StartTimer();

                    for (int i = 1; i < currentOrders; i++)
                    {
                        deliveryTimer.AddExtraTime();
                    }
                }
            }

            UpdateObjective();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            return;
        }

        // Bag still has space
        // Keep player at shopkeeper so another
        // order can be collected.
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "Order stored in bag. " +
            "Orders: " +
            currentOrders +
            "/" +
            deliveryManager.BagCapacity
        );
    }

    // =========================================
    // OBJECTIVE
    // =========================================

    private void UpdateObjective()
    {
        if (objectiveText == null)
            return;

        if (deliveryManager.ActiveOrderCount > 0)
        {
            objectiveText.text =
                "DELIVER TO: " +
                deliveryManager.CurrentCustomer;

            objectiveText.gameObject.SetActive(true);
        }
        else
        {
            objectiveText.gameObject.SetActive(false);
        }
    }

    // =========================================
    // PAYMENT PANEL
    // =========================================

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

    // =========================================
    // COLLECT PAYMENT
    // =========================================

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