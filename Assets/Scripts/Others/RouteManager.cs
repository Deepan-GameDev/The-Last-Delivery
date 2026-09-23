using UnityEngine;
using TMPro;

public class RouteManager : MonoBehaviour
{
    public enum RouteType
    {
        None,
        Safe,
        Risk
    }

    [Header("Reward")]
    [SerializeField] private DeliveryManager deliveryManager;

    [SerializeField] private TextMeshProUGUI multiplierText;
    [SerializeField] private TextMeshProUGUI rewardPreviewText;

    [Header("UI")]
    [SerializeField] private GameObject routeDecisionPanel;
    [SerializeField] private TextMeshProUGUI routeTitle;
    [SerializeField] private TextMeshProUGUI routeDescription;

    [Header("Door Controllers")]
    [SerializeField] private RouteDoorController safeDoor;
    [SerializeField] private RouteDoorController riskDoor;

    [Header("Enemy Spawner")]
    [SerializeField] private RouteEnemySpawner enemySpawner;

    [Header("Player")]
    [SerializeField] private MonoBehaviour playerMovement;

    private RouteType selectedRoute = RouteType.None;

    public RouteType SelectedRoute => selectedRoute;

    public void ShowRouteDecision(RouteType route)
    {
        selectedRoute = route;

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (route == RouteType.Safe)
        {
            routeTitle.text = "SAFE ROUTE";

            routeDescription.text =
                "Lower enemy risk.\n" +
                "Enter this route?";
        }
        else if (route == RouteType.Risk)
        {
            routeTitle.text = "RISK ROUTE";

            routeDescription.text =
                "Higher enemy danger.\n" +
                "Higher reward.\n" +
                "Enter this route?";
        }

        UpdateRewardPreview();

        routeDecisionPanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void AcceptRoute()
    {
        routeDecisionPanel.SetActive(false);

        if (selectedRoute == RouteType.Safe)
        {
            safeDoor.CloseDoor();

            if (enemySpawner != null)
            {
                enemySpawner.SpawnSafeEnemies();
            }
        }
        else if (selectedRoute == RouteType.Risk)
        {
            riskDoor.CloseDoor();

            if (enemySpawner != null)
            {
                enemySpawner.SpawnRiskEnemies();
            }
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log(
            "Route Accepted: " +
            selectedRoute
        );
    }

    public void CancelRoute()
    {
        selectedRoute = RouteType.None;

        routeDecisionPanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Route Cancelled");
    }

    public void OpenSelectedDoor()
    {
        if (selectedRoute == RouteType.Safe)
        {
            safeDoor.OpenDoor();
        }
        else if (selectedRoute == RouteType.Risk)
        {
            riskDoor.OpenDoor();
        }
    }

    private void UpdateRewardPreview()
    {
        if (deliveryManager == null)
            return;

        float multiplier =
            deliveryManager.CurrentMultiplier;

        if (multiplierText != null)
        {
            multiplierText.text =
                "CURRENT MULTIPLIER: " +
                multiplier.ToString("0.0") +
                "x";
        }

        if (rewardPreviewText != null)
        {
            int baseProfit =
                deliveryManager.CurrentReward;

            int estimatedReward =
                Mathf.RoundToInt(
                    baseProfit * multiplier
                );

            rewardPreviewText.text =
                "EST. REWARD: " +
                estimatedReward +
                " COINS";
        }
    }

        public void ResetRoute()
    {
        selectedRoute = RouteType.None;

        Debug.Log("Route Reset → Ready for next delivery.");
    }
}