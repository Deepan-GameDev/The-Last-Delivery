using UnityEngine;
using TMPro;

public class CustomerDelivery : MonoBehaviour
{
    [Header("Customer")]
    [SerializeField] private string customerName;

    [Header("Delivery")]
    [SerializeField] private Transform deliveryPoint;
    [SerializeField] private float deliveryRange = 2.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI deliverText;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Timer")]
    [SerializeField] private DeliveryTimer deliveryTimer;

    private GameObject player;
    private DeliveryManager deliveryManager;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        deliveryManager = FindFirstObjectByType<DeliveryManager>();

        if (deliverText != null)
            deliverText.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (player == null || deliveryManager == null)
            return;

        if (!deliveryManager.DeliveryActive)
        {
            HideDeliverUI();
            return;
        }

        // Only current target customer can be delivered to
        if (!string.Equals(
        deliveryManager.CurrentCustomer.Trim(),
        customerName.Trim(),
        System.StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        float distance = Vector3.Distance(
            player.transform.position,
            deliveryPoint.position
        );

        bool inRange = distance <= deliveryRange;

        if (deliverText != null)
            deliverText.gameObject.SetActive(inRange);

        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            CompleteDelivery();
        }
    }

    private void CompleteDelivery()
    {
        bool wasLate = false;

        if (deliveryTimer != null)
        {
            wasLate = deliveryTimer.IsLate;

            // IMPORTANT:
            // Don't stop timer here.
            // Timer must continue for the next customer.
        }

        string completedCustomer = customerName;

        deliveryManager.CompleteDelivery(wasLate);

        HideDeliverUI();

        // -----------------------------------------
        // NEXT CUSTOMER
        // -----------------------------------------

        if (deliveryManager.DeliveryActive &&
            deliveryManager.ActiveOrderCount > 0)
        {
            if (objectiveText != null)
            {
                objectiveText.text =
                    "DELIVER TO: " +
                    deliveryManager.CurrentCustomer;

                objectiveText.gameObject.SetActive(true);
            }

            Debug.Log(
                completedCustomer +
                " delivered → Next Customer: " +
                deliveryManager.CurrentCustomer
            );
        }
        else
        {
            // All customers completed
            if (objectiveText != null)
                objectiveText.gameObject.SetActive(false);

            // Stop timer ONLY after the final delivery
            if (deliveryTimer != null)
            {
                deliveryTimer.StopTimer();
            }

            Debug.Log(
                completedCustomer +
                " delivered → ALL DELIVERIES COMPLETE!"
            );
        }

        Debug.Log(
            wasLate
            ? completedCustomer +
              " DELIVERED LATE! -25 COINS."
            : completedCustomer +
              " DELIVERED ON TIME!"
        );
    }

    public void SetCustomerName(string newName)
    {
        customerName = newName;
    }

    public void SetDeliveryReferences(
    TextMeshProUGUI newDeliverText,
    TextMeshProUGUI newObjectiveText,
    DeliveryTimer newDeliveryTimer)
    {
        deliverText = newDeliverText;
        objectiveText = newObjectiveText;
        deliveryTimer = newDeliveryTimer;
    }

    private void HideDeliverUI()
    {
        if (deliverText != null)
            deliverText.gameObject.SetActive(false);
    }
}