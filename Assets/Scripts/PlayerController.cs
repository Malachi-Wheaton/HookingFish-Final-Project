using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float playerHealth = 100f;
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public Transform playerCamera;
    public float mouseSensitivity = 100f;
    public TextMeshProUGUI healthTextTMP;
    public AudioSource audioSource;
    public AudioClip gethit;

    private Rigidbody rb;
    private float xRotation = 0f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        UpdateHealthUI();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        MoveInput();

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            Jump();
        }
    }
    void LateUpdate()
    {
        LookAround();
    }

    void MoveInput()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = (transform.right * moveX + transform.forward * moveZ) * moveSpeed;
        Vector3 velocity = new Vector3(move.x, rb.velocity.y, move.z);

        rb.velocity = velocity;
    }

    void LookAround()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); 
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void OnCollisionStay(Collision collision)
    {
        isGrounded = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }

    public void TakeDamage(float amount)
    {
        playerHealth -= amount;
        UpdateHealthUI();
        audioSource.PlayOneShot(gethit);

        if (playerHealth <= 0)
        {
            Die();
        }
    }

    void UpdateHealthUI()
    {
        if (healthTextTMP != null)
            healthTextTMP.text = $"Health: {playerHealth}";
    }

    void Die()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
