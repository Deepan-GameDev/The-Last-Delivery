using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRange = 2.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private GameObject deliveryOrderPanel;
    [SerializeField] private TextMeshProUGUI customerText;
    [SerializeField] private TextMeshProUGUI rewardText;

    [Header("Delivery")]
    [SerializeField] private DeliveryManager deliveryManager;

    private bool canInteract;

    private void Update()
    {
        if (interactionPoint == null)
            return;

        float distance = Vector3.Distance(
            transform.position,
            interactionPoint.position
        );

        canInteract = distance <= interactionRange;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(
                canInteract && !deliveryOrderPanel.activeSelf
            );
        }

        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            deliveryManager.GenerateDelivery();

            // Update order UI with generated delivery
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
    }

    public void AcceptDelivery()
    {
        deliveryOrderPanel.SetActive(false);

        if (interactText != null)
        {
            interactText.gameObject.SetActive(false);
        }

        Debug.Log(
            "Delivery Accepted! Customer: " +
            deliveryManager.CurrentCustomer
        );

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}