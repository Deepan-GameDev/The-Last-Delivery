using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [SerializeField] private Transform target;

    [Header("Camera")]
    [SerializeField] private float distance = 5f;
    [SerializeField] private float height = 2.5f;

    [Header("Mouse")]
    [SerializeField] private float sensitivity = 2.5f;
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 60f;

    [Header("Follow")]
    [SerializeField] private float followSmooth = 12f;

    [Header("Collision")]
    [SerializeField] private float collisionRadius = 0.25f;
    [SerializeField] private float collisionOffset = 0.15f;
    [SerializeField] private LayerMask collisionLayers;

    private float yaw;
    private float pitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        // Mouse rotation
        yaw += Input.GetAxis("Mouse X") * sensitivity;
        pitch -= Input.GetAxis("Mouse Y") * sensitivity;

        pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

        Quaternion rotation =
            Quaternion.Euler(pitch, yaw, 0f);

        Vector3 targetPosition =
            target.position + Vector3.up * height;

        // Normal camera position
        Vector3 direction =
            -(rotation * Vector3.forward);

        float currentDistance = distance;

        // Camera collision check
        if (Physics.SphereCast(
            targetPosition,
            collisionRadius,
            direction,
            out RaycastHit hit,
            distance,
            collisionLayers,
            QueryTriggerInteraction.Ignore))
        {
            currentDistance =
                Mathf.Max(0.5f, hit.distance - collisionOffset);
        }

        Vector3 desiredPosition =
            targetPosition + direction * currentDistance;

        // Smooth follow
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            followSmooth * Time.deltaTime
        );

        transform.rotation = rotation;
    }
}