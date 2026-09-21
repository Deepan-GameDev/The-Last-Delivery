using UnityEngine;

public class DeliveryStartDoor : MonoBehaviour
{
    [Header("Door")]
    [SerializeField] private Transform door;

    [Header("Open Position Offset")]
    [SerializeField] private Vector3 openOffset;

    private Vector3 closedPosition;

    private void Start()
    {
        if (door != null)
        {
            closedPosition = door.localPosition;
        }
    }

    public void OpenDoor()
    {
        if (door == null)
            return;

        door.localPosition = closedPosition + openOffset;
    }

    public void CloseDoor()
    {
        if (door == null)
            return;

        door.localPosition = closedPosition;
    }
}