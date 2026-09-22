using UnityEngine;
using TMPro;

public class BagShopInteraction : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionRange = 2.5f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactText;
    [SerializeField] private BagUpgradeManager bagUpgradeManager;

    private bool canInteract;

    private void Update()
    {
        if (interactionPoint == null || bagUpgradeManager == null)
            return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null)
            return;

        float distance = Vector3.Distance(
            player.transform.position,
            interactionPoint.position
        );

        canInteract = distance <= interactionRange;

        if (interactText != null)
        {
            interactText.gameObject.SetActive(canInteract);
        }

        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            bagUpgradeManager.OpenUpgradePanel();

            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }
}