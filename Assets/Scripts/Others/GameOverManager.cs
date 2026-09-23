using UnityEngine;
using System.Collections;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerStartPoint;

    [Header("Player Movement")]
    [SerializeField] private MonoBehaviour playerMovement;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Delivery UI")]
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Systems")]
    [SerializeField] private DeliveryManager deliveryManager;
    [SerializeField] private RouteEnemySpawner routeEnemySpawner;
    [SerializeField] private RouteManager routeManager;
    [SerializeField] private DeliveryTimer deliveryTimer;
    [SerializeField] private CustomerSpawner customerSpawner;

    private bool gameOver;

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    private void Start()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    public void PlayerCaught()
    {
        if (gameOver)
            return;

        gameOver = true;

        Debug.Log(
            "PLAYER CAUGHT → DELIVERY FAILED!"
        );

        // Stop player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        // Stop timer
        if (deliveryTimer != null)
        {
            deliveryTimer.StopTimer();
        }

        if (objectiveText != null)
        {
            objectiveText.gameObject.SetActive(false);
        }

        // Clear spawned enemies and drone
        if (routeEnemySpawner != null)
        {
            routeEnemySpawner.ClearEnemiesAndDrones();
        }

        // Clear spawned customers
        if (customerSpawner != null)
        {
            customerSpawner.ClearSpawnedCustomers();
        }

        // Reset route
        if (routeManager != null)
        {
            routeManager.ResetRoute();
        }

        // Reset delivery
        if (deliveryManager != null)
        {
            deliveryManager.ResetAfterCaught();
        }

        // Show Game Over UI
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;
    }

    public void RestartRun()
    {
        StartCoroutine(
            RestartRunCoroutine()
        );
    }

    private IEnumerator RestartRunCoroutine()
    {
        gameOver = false;

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Move player to starting position
        if (player != null &&
            playerStartPoint != null)
        {
            CharacterController controller =
                player.GetComponent<CharacterController>();

            if (controller != null)
            {
                controller.enabled = false;
            }

            player.position =
                playerStartPoint.position;

            player.rotation =
                playerStartPoint.rotation;

            if (controller != null)
            {
                controller.enabled = true;
            }
        }

        // Reset timer
        if (deliveryTimer != null)
        {
            deliveryTimer.ResetTimer();
        }

        // Enable movement again
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;

        yield return null;

        Debug.Log(
            "RUN RESTARTED FROM START."
        );
    }
}