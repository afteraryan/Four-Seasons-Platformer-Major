using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterController : MonoBehaviour
{
    [Header("Movement Parameters")]
    [SerializeField] private float moveSpeed;
    
    [Header("Jump Parameters")]
    //[SerializeField] private float jumpForce;
    [SerializeField] private float lowJumpForce = 1;
    [SerializeField] private float midJumpForce = 2;
    [SerializeField] private float minMidJumpDuration = 2;
    [SerializeField] private float highJumpForce = 3;
    [SerializeField] private float minHighJumpDuration = 2;

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
        Move();
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
        // Only allow horizontal movement if not in air
        if (!isInAir)
        {
            Vector2 velocity = new Vector2(input.Horizontal * moveSpeed, rb.velocity.y);
            rb.velocity = velocity;
        }

        
    }

    private void Jump(float timeDifference)
    {

        if (timeDifference < minMidJumpDuration)
            rb.AddForce(new Vector2(0, lowJumpForce), ForceMode2D.Impulse);
        else if(timeDifference >= minMidJumpDuration && timeDifference< minHighJumpDuration )
            rb.AddForce(new Vector2(0, midJumpForce), ForceMode2D.Impulse);
        else if(timeDifference >= minHighJumpDuration)
            rb.AddForce(new Vector2(0, highJumpForce), ForceMode2D.Impulse);

        input.JumpRequested = false;
        
        Debug.Log("Time Difference: " + timeDifference);
    }

    #endregion
}
