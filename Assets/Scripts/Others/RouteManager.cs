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

    [Header("UI")]
    [SerializeField] private GameObject routeDecisionPanel;
    [SerializeField] private TextMeshProUGUI routeTitle;
    [SerializeField] private TextMeshProUGUI routeDescription;

    [Header("Door Controllers")]
    [SerializeField] private RouteDoorController safeDoor;
    [SerializeField] private RouteDoorController riskDoor;

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
                "Lower enemy risk.\nEnter this route?";
        }
        else if (route == RouteType.Risk)
        {
            routeTitle.text = "RISK ROUTE";
            routeDescription.text =
                "Higher enemy danger.\nHigher reward.\nEnter this route?";
        }

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
        }
        else if (selectedRoute == RouteType.Risk)
        {
            riskDoor.CloseDoor();
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Route Accepted: " + selectedRoute);
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

        selectedRoute = RouteType.None;
    }
}