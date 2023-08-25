using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed;

    private Vector2 Direction;
    
    [Header("Jump Parameters")]
    //[SerializeField] private float jumpForce;
    [SerializeField] private float lowJumpForce = 1;
    [SerializeField] private float midJumpForce = 2;
    [SerializeField] private float minMidJumpDuration = 2;
    [SerializeField] private float highJumpForce = 3;
    [SerializeField] private float minHighJumpDuration = 2;
    [SerializeField] private float horizontalJumpBoost = 2.0f; // Boost in horizontal velocity during jump
    [SerializeField] private float jumpApexBoostDivideFactor = 6.0f; // Boost in horizontal velocity during jump
    private float jumpCooldown = 0f;
    private const float JumpCooldownDuration = 0.5f; // Adjust this value as needed
    private float verticalForce = 0f;
    
    [Header("Fall Parameters")]
    [SerializeField] private float jumpFallingForce = 1.5f;
    [SerializeField] private float fallingTerminalVelocity = 1f; //Maximum Falling Velocity
    private float fallDelayTimer = 0f;
    private const float FallDelayDuration = 0.00f; // Adjust this value as needed
    private float currentFallingForce = 0f;
    private const float ForceLerpDuration = 1f; // Adjust this value as needed
    private float forceLerpTimer = 0f;


    private bool hasAppliedDownwardForce = false;
    private bool hasAppliedHorizontalJumpBoost = false;

    [Header("Ground Check Parameters")]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private LayerMask whatIsGround;
    [Header("Raycast Ground Check Parameters")]
    [SerializeField] private float raycastLength = 0.3f; // Length of the raycasts
    [SerializeField] private float raycastSideOffset = 0.5f; // Offset from the center for the side raycasts
    
    [Header("Joystick")]
    [SerializeField] private Joystick joystick;

    private Rigidbody2D rb;
    private bool isInAir = false; // Replaced isGrounded with isInAir
    private InputData input;
    private float jumpButtonPressTime; // Time when the jump button was pressed

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
        CheckIfInAir();
        HandleInput();
    }

    private void FixedUpdate()
    {
        if (jumpCooldown > 0f)
        {
            jumpCooldown -= Time.fixedDeltaTime;
        }

        CalculateDirection();
        Move();
        Fall();
    }

    
    private void CheckIfInAir()
    {
        Vector2 centerPoint = groundCheckPoint.position;
        Vector2 leftPoint = centerPoint + Vector2.left * raycastSideOffset;
        Vector2 rightPoint = centerPoint + Vector2.right * raycastSideOffset;

        bool centerHit = Physics2D.Raycast(centerPoint, Vector2.down, raycastLength, whatIsGround);
        bool leftHit = Physics2D.Raycast(leftPoint, Vector2.down, raycastLength, whatIsGround);
        bool rightHit = Physics2D.Raycast(rightPoint, Vector2.down, raycastLength, whatIsGround);

        /* Debug rays
        Color centerColor = centerHit ? Color.green : Color.red;
        Color leftColor = leftHit ? Color.green : Color.red;
        Color rightColor = rightHit ? Color.green : Color.red;

        Debug.DrawRay(centerPoint, Vector2.down * raycastLength, centerColor);
        Debug.DrawRay(leftPoint, Vector2.down * raycastLength, leftColor);
        Debug.DrawRay(rightPoint, Vector2.down * raycastLength, rightColor);
        */
        
        isInAir = !(centerHit || leftHit || rightHit);
    }


    #region Input Handling

    private void HandleInput()
    {
        // Use joystick input instead of Unity's default input system
        //input.Horizontal = joystick.Horizontal;
        input.Horizontal = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            OnJumpButtonPressed();
        }

        if (Input.GetButtonUp("Jump"))
        {
            OnJumpButtonReleased();
        }
    }

    
    // ReSharper disable Unity.PerformanceAnalysis
    public void OnJumpButtonPressed()
    {
        if (!isInAir)
        {
            jumpButtonPressTime = Time.time;
        }
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void OnJumpButtonReleased()
    {
        if (!isInAir)
        {
            input.JumpRequested = true;
            float timeDifferenceJump = Time.time - jumpButtonPressTime;
            Jump(timeDifferenceJump);
        }
    }

    #endregion
    
    #region Movement

    
    // ReSharper disable Unity.PerformanceAnalysis
    private void Move()
    {
        // Only allow horizontal movement if not in air and not in jump cooldown
        if (!isInAir && jumpCooldown <= 0f)
        {
            Vector2 velocity = new Vector2((input.Horizontal * moveSpeed), rb.velocity.y);
            rb.velocity = velocity;
            hasAppliedDownwardForce = false;  // Reset the flag when not in air
        }
    }

    private void Fall()
    {
        if (!isInAir)
        {
            fallDelayTimer = 0f; // Reset the timer when not in air
            forceLerpTimer = 0f; // Reset the force lerp timer
            currentFallingForce = 0f; // Reset the current falling force
            return;
        }
    
        bool isFalling = rb.velocity.y < 0;

        if (isFalling)
        {
            if (fallDelayTimer < FallDelayDuration)
            {
                fallDelayTimer += Time.fixedDeltaTime;
                return; // Exit the function early, delaying the application of the downward force
            }

            if (!hasAppliedDownwardForce)
            {
                if (!hasAppliedHorizontalJumpBoost)
                {
                    rb.AddForce(new Vector2((verticalForce*Direction.x)/jumpApexBoostDivideFactor, 0), ForceMode2D.Impulse);
                    hasAppliedHorizontalJumpBoost = true;
                }
                // Use the logarithmic function
                if (forceLerpTimer < ForceLerpDuration)
                {
                    forceLerpTimer += Time.fixedDeltaTime;
                    float a = jumpFallingForce / Mathf.Log(ForceLerpDuration + 1); // Adjust this value as needed
                    float b = 3f; // Adjust this value as needed
                    currentFallingForce = a * Mathf.Log(b * forceLerpTimer + 1);
                    rb.AddForce(new Vector2(0, -currentFallingForce), ForceMode2D.Force);
                }
                else
                {
                    hasAppliedDownwardForce = true;  // Set the flag to true after applying the force
                    //rb.AddForce(new Vector2(,0), ForceMode2D.Impulse);
                }
            }

            if (rb.velocity.y < -fallingTerminalVelocity)
            {
                rb.velocity = new Vector2(rb.velocity.x, -fallingTerminalVelocity);
            }
        } 
    }



    
    private void Jump(float timeDifference)
    {
        verticalForce = 0f;
        if (timeDifference < minMidJumpDuration)
            verticalForce = lowJumpForce;
        else if(timeDifference >= minMidJumpDuration && timeDifference < minHighJumpDuration)
            verticalForce = midJumpForce;
        else if(timeDifference >= minHighJumpDuration)
            verticalForce = highJumpForce;

        // Calculate the horizontal force based on the direction of movement
        float horizontalForce = input.Horizontal * horizontalJumpBoost;
        
        // Apply the forces
        rb.AddForce(new Vector2(horizontalForce, verticalForce), ForceMode2D.Impulse);
        
        jumpCooldown = JumpCooldownDuration;
        hasAppliedHorizontalJumpBoost = false;
        input.JumpRequested = false;
    }

        
    private void CalculateDirection()
    {
        // Determine X direction based on Rigidbody2D velocity
        if (rb.velocity.x > 0)
        {
            Direction.x = 1; // Moving right
        }
        else if (rb.velocity.x < 0)
        {
            Direction.x = -1; // Moving left
        }
        else
        {
            Direction.x = 0; // No horizontal movement
        }

        // Determine Y direction based on Rigidbody2D velocity
        if (rb.velocity.y > 0)
        {
            Direction.y = 1; // Moving up
        }
        else if (rb.velocity.y < 0)
        {
            Direction.y = -1; // Moving down
        }
        else
        {
            Direction.y = 0; // No vertical movement
        }
    }


    #endregion
}
