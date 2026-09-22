using UnityEngine;
using TMPro;
using System.Collections;

public class BagUpgradeManager : MonoBehaviour
{
    [Header("Bag")]
    [SerializeField] private int currentBagLevel = 1;
    [SerializeField] private int bagCapacity = 1;

    [Header("Upgrade")]
    [SerializeField] private int upgradeCost = 250;
    [SerializeField] private int nextCapacity = 2;

    [Header("References")]
    [SerializeField] private DeliveryManager deliveryManager;

    [Header("UI")]
    [SerializeField] private GameObject upgradePanel;
    [SerializeField] private TextMeshProUGUI currentBagText;
    [SerializeField] private TextMeshProUGUI capacityText;
    [SerializeField] private TextMeshProUGUI upgradeCostText;
    [SerializeField] private TextMeshProUGUI upgradeCapacityText;

    [Header("Messages")]
    [SerializeField] private TextMeshProUGUI messageText;

    public int CurrentBagLevel => currentBagLevel;
    public int BagCapacity => bagCapacity;

    private void Start()
    {
        UpdateUpgradeUI();

        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        if (messageText != null)
            messageText.gameObject.SetActive(false);
    }

    public void OpenUpgradePanel()
    {
        UpdateUpgradeUI();

        if (upgradePanel != null)
        {
            upgradePanel.SetActive(true);

            StopAllCoroutines();
            StartCoroutine(AutoClosePanel());
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseUpgradePanel()
    {
        if (upgradePanel != null)
            upgradePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UpgradeBag()
    {
        if (deliveryManager == null)
        {
            Debug.LogWarning("DeliveryManager is not assigned!");
            return;
        }

        if (currentBagLevel >= 2)
        {
            Debug.Log("Bag is already upgraded!");
            return;
        }

        if (!deliveryManager.HasEnoughCoins(upgradeCost))
        {
            ShowMessage("NOT ENOUGH COINS!");
            return;
        }

        deliveryManager.SpendCoins(upgradeCost);

        currentBagLevel++;
        bagCapacity = nextCapacity;

        UpdateUpgradeUI();

        Debug.Log(
            "Bag Upgraded! Level: " +
            currentBagLevel +
            " | Capacity: " +
            bagCapacity
        );
    }

    private void UpdateUpgradeUI()
    {
        if (currentBagText != null)
        {
            currentBagText.text =
                "Bag Level: " + currentBagLevel;
        }

        if (capacityText != null)
        {
            capacityText.text =
                "Capacity: " + bagCapacity + " Orders";
        }

        if (upgradeCapacityText != null)
        {
            if (currentBagLevel >= 2)
            {
                upgradeCapacityText.text = "MAX LEVEL";
            }
            else
            {
                upgradeCapacityText.text =
                    "Upgrade → Capacity: " + nextCapacity + " Orders";
            }
        }

        if (upgradeCostText != null)
        {
            if (currentBagLevel >= 2)
                upgradeCostText.text = "MAX LEVEL";
            else
                upgradeCostText.text =
                    "Upgrade Cost: " + upgradeCost + " Coins";
        }
    }

    private void ShowMessage(string message)
    {
        if (messageText == null)
            return;

        messageText.text = message;
        messageText.gameObject.SetActive(true);
    }

    private IEnumerator AutoClosePanel()
    {
        yield return new WaitForSeconds(5f);

        CloseUpgradePanel();
    }
}