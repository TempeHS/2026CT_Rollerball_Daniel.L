using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerController : MonoBehaviour

{
    public float speed = 0; 
    public TextMeshProUGUI countText;
    public GameObject winTextObject;
    public GameObject restartButtonObject;

    private Rigidbody rb; 
    private int count;
    private float movementX;
    private float movementY;

    void Start()
    {
        rb = GetComponent <Rigidbody>(); 
        count = 0;

        SetCountText();
        winTextObject.SetActive(false);
    }

    private void FixedUpdate() 
    {
        Vector3 movement = new Vector3 (movementX, 0.0f, movementY);

        rb.AddForce(movement * speed); 
    }

    private void OnMove (InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>(); 
        movementX = movementVector.x; 
        movementY = movementVector.y; 
    }

    private void OnCollisionEnter(Collision collision)
    {
   if (collision.gameObject.CompareTag("Enemy")) {
        Destroy(gameObject); 
        winTextObject.gameObject.SetActive(true);
        winTextObject.GetComponent<TextMeshProUGUI>().text = "You Lose!";
        restartButtonObject.SetActive(true);
        }
    }

    private void SetCountText() 
    {
        countText.text =  "Count: " + count.ToString();       
        if(count >= 18  ) {
            winTextObject.SetActive(true);  
            restartButtonObject.SetActive(true);
            Destroy(GameObject.FindGameObjectWithTag("Enemy"));      
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.CompareTag("PickUp")) {
        other.gameObject.SetActive(false);
        count = count + 1;

        SetCountText();
        }

        
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}