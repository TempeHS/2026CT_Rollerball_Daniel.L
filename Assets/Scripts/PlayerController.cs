using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI countText;
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

    private float baseSpeed;
    private float speedBoostUntil;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        count = 0;
        baseSpeed = speed;
        dangerTimer = dangerTimeLimit;

        SetCountText();
        winTextObject.SetActive(false);
    }

    private void Update()
    {
        if (Time.time >= speedBoostUntil && speed != baseSpeed)
        {
            speed = baseSpeed;
        }
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
            winTextObject.SetActive(true);
            winTextObject.GetComponent<TextMeshProUGUI>().text = "You Win!";
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));

            foreach (Transform child in pickupParent)
            {
                Destroy(child.gameObject);
            }
        }
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
        speed = baseSpeed * 2f;
        speedBoostUntil = Time.time + duration;
        Debug.Log("BUFF: Speed boost");
    }
}
