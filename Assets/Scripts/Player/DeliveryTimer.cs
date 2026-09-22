using UnityEngine;
using TMPro;

public class DeliveryTimer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private float deliveryTime = 90f;
    [SerializeField] private float extraOrderTime = 60f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Delivery")]
    [SerializeField] private DeliveryManager deliveryManager;

    private float currentTime;
    private bool timerRunning;

    public bool IsLate { get; private set; }

    private void Start()
    {
        ResetTimer();
    }

    private void Update()
    {
        if (!timerRunning)
            return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            timerRunning = false;
            IsLate = true;

            UpdateTimerUI();

            Debug.Log("DELIVERY IS LATE!");
            return;
        }

        UpdateTimerUI();
    }

    // =========================================
    // FIRST ORDER
    // =========================================

    public void StartTimer()
    {
        currentTime = deliveryTime;

        timerRunning = true;
        IsLate = false;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        UpdateTimerUI();

        Debug.Log(
            "Delivery Timer Started: " +
            deliveryTime +
            " seconds"
        );
    }

    // =========================================
    // EXTRA ORDER
    // =========================================

    public void AddExtraTime()
    {
        currentTime += extraOrderTime;

        // If timer was already late, extra order
        // gives the player a fresh active timer.
        if (currentTime > 0f)
        {
            timerRunning = true;
            IsLate = false;
        }

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        UpdateTimerUI();

        Debug.Log(
            "Extra Order Added! +" +
            extraOrderTime +
            " seconds"
        );
    }

    // =========================================
    // STOP
    // =========================================

    public void StopTimer()
    {
        timerRunning = false;

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    // =========================================
    // RESET
    // =========================================

    public void ResetTimer()
    {
        currentTime = deliveryTime;
        timerRunning = false;
        IsLate = false;

        if (timerText != null)
            timerText.gameObject.SetActive(false);
    }

    // =========================================
    // UI
    // =========================================

    private void UpdateTimerUI()
    {
        if (timerText == null)
            return;

        int minutes =
            Mathf.FloorToInt(currentTime / 60f);

        int seconds =
            Mathf.FloorToInt(currentTime % 60f);

        timerText.text =
            "TIME: " +
            minutes.ToString("00") +
            ":" +
            seconds.ToString("00");
    }
}