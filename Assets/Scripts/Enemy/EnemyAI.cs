using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Detection")]
    [SerializeField] private float detectionRange = 12f;
    [SerializeField] private float loseRange = 18f;

    [Header("Movement")]
    [SerializeField] private float chaseSpeed = 3.5f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Model Rotation")]
    [SerializeField] private float modelYRotationOffset = 0f;

    [Header("Catch")]
    [SerializeField] private float catchDistance = 1.5f;

    private GameOverManager gameOverManager;

    private NavMeshAgent agent;
    private Transform player;

    private bool chasing;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        // Agent should NOT control visual rotation.
        agent.updateRotation = false;
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        gameOverManager =
            FindFirstObjectByType<GameOverManager>();

        agent.speed = chaseSpeed;
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (!chasing)
        {
            if (distance <= detectionRange)
            {
                StartChasing();
            }
        }
        else
        {
            if (distance <= loseRange)
            {
                ChasePlayer();
            }
            else
            {
                StopChasing();
            }
        }
    }

    private void StartChasing()
    {
        chasing = true;

        Debug.Log(
            "Enemy detected player!"
        );

        ChasePlayer();
    }

    private void ChasePlayer()
    {
        if (!agent.isOnNavMesh)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        if (distance <= catchDistance)
        {
            CatchPlayer();
            return;
        }

        agent.isStopped = false;

        agent.SetDestination(
            player.position
        );

        RotateTowardsPlayer();
    }

    private void RotateTowardsPlayer()
    {
        Vector3 direction =
            player.position -
            transform.position;

        // Ignore vertical difference.
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation =
            Quaternion.LookRotation(
                direction
            );

        targetRotation *=
            Quaternion.Euler(
                0f,
                modelYRotationOffset,
                0f
            );

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed *
                Time.deltaTime
            );
    }

    private void StopChasing()
    {
        chasing = false;

        if (!agent.isOnNavMesh)
            return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    private void CatchPlayer()
    {
        if (gameOverManager == null)
            return;

        agent.isStopped = true;

        gameOverManager.PlayerCaught();
    }
}