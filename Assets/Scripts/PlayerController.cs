using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI countText;
    public TextMeshProUGUI stopwatchText;
    public TextMeshProUGUI buffText;
    public GameObject winTextObject;
    public GameObject restartButtonObject;

    [Header("Movement")]
    public float speed = 20f;
    public Transform pickupParent;

    [Header("Danger Zone")]
    public float dangerHeightY = 2f;
    public float dangerTimeLimit = 5f;

    private Rigidbody rb;
    private int count;
    private float movementX;
    private float movementY;
    private float dangerTimer;

    private bool isInDangerZone;
    private bool isGameOver;

    private float elapsedTime;

    private float baseSpeed;
    private readonly List<float> speedBoostEndTimes = new();
    private float slowEnemyBuffEndTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        baseSpeed = speed;
        dangerTimer = dangerTimeLimit;
        elapsedTime = 0f;

        SetCountText();
        winTextObject.SetActive(false);
        if (restartButtonObject != null)
            restartButtonObject.SetActive(false);
        UpdateStopwatchText();

        if (buffText != null)
            UpdateBuffStatusText();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            elapsedTime += Time.deltaTime;
            UpdateStopwatchText();
        }

        for (int i = speedBoostEndTimes.Count - 1; i >= 0; i--)
        {
            if (Time.time >= speedBoostEndTimes[i])
                speedBoostEndTimes.RemoveAt(i);
        }

        UpdateSpeedFromBoostStacks();
        UpdateBuffStatusText();
    }

    private void UpdateSpeedFromBoostStacks()
    {
        speed = baseSpeed * Mathf.Pow(2f, speedBoostEndTimes.Count);
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(movementX, 0.0f, movementY);
        rb.AddForce(movement * speed);
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isGameOver = true;
            Destroy(gameObject);
            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
            if (restartButtonObject != null)
                restartButtonObject.SetActive(true);
        }
    }

    private void SetCountText()
    {
        countText.text = "Count: " + count;
        if (count >= 18)
        {
            isGameOver = true;
            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Win!";
            if (restartButtonObject != null)
                restartButtonObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));

            foreach (Transform child in pickupParent)
            {
                Destroy(child.gameObject);
            }
        }
    }

    private void UpdateStopwatchText()
    {
        if (stopwatchText == null)
            return;

        int totalSeconds = Mathf.FloorToInt(elapsedTime);
        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;
        stopwatchText.text = $"Time: {minutes:00}:{seconds:00}";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            other.gameObject.SetActive(false);
            count += 1;
            SetCountText();
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ApplySpeedBoost(float duration)
    {
        // Add a new stack and refresh ALL stacks to the new end time
        float newEndTime = Time.time + duration;
        speedBoostEndTimes.Add(newEndTime);

        // Refresh all existing stacks to the same end time
        for (int i = 0; i < speedBoostEndTimes.Count; i++)
            speedBoostEndTimes[i] = newEndTime;

        UpdateSpeedFromBoostStacks();
        UpdateBuffStatusText();
        Debug.Log($"BUFF: Speed Boost (stacks: {speedBoostEndTimes.Count})");
    }

    public void ApplySlowEnemyBuff(float duration, float multiplier)
    {
        slowEnemyBuffEndTime = Time.time + duration;

        EnemyMovement[] enemies = FindObjectsByType<EnemyMovement>(FindObjectsSortMode.None);
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
                enemies[i].ApplySlow(multiplier, duration);
        }

        UpdateBuffStatusText();
        Debug.Log("BUFF: Slow Enemy (active)");
    }

    private void UpdateBuffStatusText()
    {
        if (buffText == null)
            return;

        int speedStacks = speedBoostEndTimes.Count;
        float speedRemaining = 0f;
        float slowRemaining = Mathf.Max(0f, slowEnemyBuffEndTime - Time.time);

        // All stacks share the same end time now, so just check the first one
        if (speedStacks > 0)
            speedRemaining = Mathf.Max(0f, speedBoostEndTimes[0] - Time.time);

        // Build display lines for each active buff
        string result = "";

        if (speedStacks > 0 && speedRemaining > 0f)
            result += $"Speed Boost x{speedStacks}: {speedRemaining:0.0}s";

        if (slowRemaining > 0f)
        {
            if (result.Length > 0)
                result += "\n";
            result += $"Slow Enemy: {slowRemaining:0.0}s";
        }

        buffText.text = result;
    }
}
