using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;

    [Header("Ground Check Parameters")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius = 0.2f;
    [SerializeField] private LayerMask whatIsGround;
    
    [Header("Joystick")]
    [SerializeField] private Joystick joystick;


    private Rigidbody2D rb;
    private bool isGrounded;
    private InputData input;

    private struct InputData
    {
        public float Horizontal;
        public bool JumpRequested;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        CheckGrounded();
        HandleInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void CheckGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, whatIsGround);
    }

    
    
    private void Move()
    {
        Vector2 velocity = new Vector2(input.Horizontal * moveSpeed, rb.velocity.y);
        rb.velocity = velocity;

        if (input.JumpRequested)
        {
            rb.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);
            input.JumpRequested = false;
        }
    }

    #region Input Handling

    private void HandleInput()
    {
        // Use joystick input instead of Unity's default input system
        input.Horizontal = joystick.Horizontal;

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            input.JumpRequested = true;
        }
    }
    public void OnJumpButtonPressed()
    {
        if (isGrounded)
        {
            input.JumpRequested = true;
        }
    }
    #endregion

}
