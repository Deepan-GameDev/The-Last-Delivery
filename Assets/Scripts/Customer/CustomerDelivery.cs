using UnityEngine;

public class CustomerDelivery : MonoBehaviour
{
    [SerializeField] private Transform deliveryPoint;
    [SerializeField] private float deliveryRange = 2.5f;

    [Header("UI")]
    [SerializeField] private TMPro.TextMeshProUGUI deliverText;

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
            return;

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
        deliveryManager.CompleteDelivery();

        if (deliverText != null)
            deliverText.gameObject.SetActive(false);

        Debug.Log("DELIVERY COMPLETED!");
    }
}