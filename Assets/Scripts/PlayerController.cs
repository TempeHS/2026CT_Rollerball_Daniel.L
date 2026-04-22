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
    public GameObject winTextObject;

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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        baseSpeed = speed;
        dangerTimer = dangerTimeLimit;
        elapsedTime = 0f;

        SetCountText();
        winTextObject.SetActive(false);
        UpdateStopwatchText();
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
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
        speedBoostEndTimes.Add(Time.time + duration);
        UpdateSpeedFromBoostStacks();
        Debug.Log($"BUFF: Speed boost stacks = {speedBoostEndTimes.Count}");
    }
}
