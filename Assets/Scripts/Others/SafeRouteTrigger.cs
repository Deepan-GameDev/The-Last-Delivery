using UnityEngine;

public class SafeRouteTrigger : MonoBehaviour
{
    [SerializeField] private RouteManager routeManager;
    [SerializeField] private DeliveryManager deliveryManager;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Route selection is allowed only during an active delivery
        if (!deliveryManager.DeliveryActive)
            return;

        // Don't show route panel if a route is already selected
        if (routeManager.SelectedRoute != RouteManager.RouteType.None)
            return;

        routeManager.ShowRouteDecision(
            RouteManager.RouteType.Safe
        );
    }
}