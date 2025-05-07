using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;

    [Header("Movement Settings")]
    public float moveSpeed = 6f;
    public float airMultiplier = 0.5f;
    public float groundDrag = 6f;
    public float airDrag = 0f;

    [Header("Jump Settings")]
    public float jumpForce = 8f;
    public float jumpCooldown = 0.25f;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight = 2f;
    public LayerMask groundMask;
    public bool grounded;

    [HideInInspector] public Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;
    private bool readyToJump = true;
    public bool freeze = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Ground check with ray and debug visualization
        grounded = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.4f, groundMask);
        Debug.DrawRay(transform.position, Vector3.down * (playerHeight * 0.5f + 0.4f), grounded ? Color.green : Color.red);

        if (!freeze)
        {
            HandleInput();
            ApplyDrag();

            if (Input.GetKeyDown(jumpKey) && grounded && readyToJump)
            {
                readyToJump = false;
                Jump();
                Invoke(nameof(ResetJump), jumpCooldown);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!freeze)
            MovePlayer();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        moveDirection.Normalize();

        if (grounded)
        {
            Vector3 move = moveDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + move);
        }
        else
        {
            rb.AddForce(moveDirection * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    private void ApplyDrag()
    {
        rb.drag = grounded ? groundDrag : airDrag;
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z); // reset vertical velocity
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    public void JumpToPosition(Vector3 target, float height)
    {
        Vector3 velocity = CalculateJumpVelocity(transform.position, target, height);
        rb.velocity = velocity;
    }

    private Vector3 CalculateJumpVelocity(Vector3 start, Vector3 end, float height)
    {
        float gravity = Physics.gravity.y;
        float displacementY = end.y - start.y;
        Vector3 displacementXZ = new Vector3(end.x - start.x, 0, end.z - start.z);

        float time = Mathf.Sqrt(-2 * height / gravity) + Mathf.Sqrt(2 * (displacementY - height) / gravity);
        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / time;

        return velocityXZ + velocityY;
    }
}




