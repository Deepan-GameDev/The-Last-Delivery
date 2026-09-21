using UnityEngine;

public class RouteDoorController : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform door;

    [Header("Closed Position Offset")]
    [SerializeField] private Vector3 closedOffset;

    private Vector3 openPosition;

    private void Start()
    {
        if (door != null)
        {
            openPosition = door.localPosition;
        }
    }

    public void CloseDoor()
    {
        if (door == null)
            return;

        door.localPosition = openPosition + closedOffset;
    }

    public void OpenDoor()
    {
        if (door == null)
            return;

        door.localPosition = openPosition;
    }
}