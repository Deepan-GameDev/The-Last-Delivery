using UnityEngine;

public class DroneAI : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 5f;
    [SerializeField] private float waypointReachDistance = 1f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Detection")]
    [SerializeField] private float detectionRange = 15f;
    [SerializeField] private float fieldOfView = 90f;
    [SerializeField] private LayerMask detectionLayers;

    [Header("Chase")]
    [SerializeField] private float chaseSpeed = 7f;

    private Transform[] patrolPoints;
    private Transform player;

    private int currentWaypoint = 0;
    private bool playerDetected = false;

    public bool PlayerDetected => playerDetected;

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogWarning(
                "DroneAI: Player with tag 'Player' not found!"
            );
        }
    }

    private void Update()
    {
        if (player == null)
            return;

        DetectPlayer();

        if (playerDetected)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    // Called by RouteEnemySpawner after the drone is spawned
    public void SetPatrolPoints(Transform[] points)
    {
        patrolPoints = points;

        currentWaypoint = 0;
    }

    private void Patrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        Transform target =
            patrolPoints[currentWaypoint];

        MoveTowards(
            target.position,
            patrolSpeed
        );

        float distance =
            Vector3.Distance(
                transform.position,
                target.position
            );

        if (distance <= waypointReachDistance)
        {
            currentWaypoint++;

            if (currentWaypoint >= patrolPoints.Length)
            {
                currentWaypoint = 0;
            }
        }
    }

    private void ChasePlayer()
    {
        MoveTowards(
            player.position,
            chaseSpeed
        );
    }

    private void MoveTowards(
        Vector3 targetPosition,
        float speed)
    {
        Vector3 direction =
            targetPosition - transform.position;

        if (direction.sqrMagnitude < 0.01f)
            return;

        transform.position =
            Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );

        Quaternion targetRotation =
            Quaternion.LookRotation(direction);

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
    }

    private void DetectPlayer()
    {
        Vector3 origin =
            transform.position;

        Vector3 targetPosition =
            player.position + Vector3.up;

        Vector3 direction =
            targetPosition - origin;

        float distance =
            direction.magnitude;

        // Player is outside detection range
        if (distance > detectionRange)
        {
            SetPlayerDetected(false);
            return;
        }

        direction.Normalize();

        float angle =
            Vector3.Angle(
                transform.forward,
                direction
            );

        // Player is outside field of view
        if (angle > fieldOfView * 0.5f)
        {
            SetPlayerDetected(false);
            return;
        }

        // Check line of sight
        if (Physics.Raycast(
            origin,
            direction,
            out RaycastHit hit,
            detectionRange,
            detectionLayers,
            QueryTriggerInteraction.Ignore))
        {
            if (hit.transform.CompareTag("Player"))
            {
                SetPlayerDetected(true);
                return;
            }
        }

        SetPlayerDetected(false);
    }

    private void SetPlayerDetected(bool detected)
    {
        if (playerDetected == detected)
            return;

        playerDetected = detected;

        if (playerDetected)
        {
            Debug.Log(
                "DRONE DETECTED PLAYER!"
            );
        }
        else
        {
            Debug.Log(
                "DRONE LOST PLAYER."
            );
        }
    }
}