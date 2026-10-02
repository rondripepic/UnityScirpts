using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour
{
    // Variables \\
    // floats \\
    public float moveSpeed;
    public float oldmovespeed;
    public float jumpForce = 7f;
    public float moveInput;

    // bools \\
    public bool isGrounded = false;
    public bool isTouchingWall = false;
    public bool retainspeed; // false == not retaining speed, true == retaining speed \\

    // walls \\
    [Header("Wall Jump")]
    public float wallJumpHorizontalForce = 12f;
    public float wallJumpVerticalForce = 10f;

    private float wallJumpTimer = 0f; 
    public float wallJumpDuration = 0.2f;

    public float wallJumpControlLock = 0.12f;
    public float wallJumpCooldown = 0.08f;
    public float wallStickTime = 0.15f;

    private float wallJumpControlTimer = 0f;
    private float wallJumpCooldownTimer = 0f;
    private float wallStickTimer = 0f;
    private int wallDirection = 0;

    public bool isWallJumping; // false == not wall jumping, true == wall jumping \\
    public bool way_of_facing; // false == left, true == right \\
    public bool firstwalljump = true; // false == not first wall jump, true == first wall jump
    public float wallJumpMultiplier = 2f; // Start with a big wall jump
    public float wallJumpMultiplierDecay = 0.7f; // How much the multiplier decreases each wall jump
    public float wallJumpMultiplierMin = 1f; // Minimum multiplier value


    // dealing with ground check \\
    private Rigidbody2D rb;
    public Transform groundCheckPoint;
    public Transform wallcheckright;
    public Transform wallcheckleft;
    public float groundCheckRadius = 0.1f; //if not working increase by .1 \\
    public LayerMask groundLayer;


    // animation stuff \\
    public Animator animator;
    public float YAcceleration;
    public GameObject playerimage;
    public standingscripotgoon stand;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        oldmovespeed = moveSpeed;
        wallJumpVerticalForce = jumpForce;
    }

    void Update()
    {
        if (way_of_facing)
        {
            playerimage.transform.rotation = Quaternion.Euler(0f,0f, stand.degreeofstanding);
        } else
        {
            playerimage.transform.rotation = Quaternion.Euler(0f, 180f, -1*stand.degreeofstanding);
        }
        moveInput = Input.GetAxis("Horizontal");
        animator.SetFloat("Input", Mathf.Abs(moveInput));
        if (rb.linearVelocity.y > 0.25f)
        {
            YAcceleration += 0.01f;
        }
        else if (rb.linearVelocity.y < -0.25f)
        {
            YAcceleration -= 0.01f;
        }
        else
        {
            YAcceleration = 0f;
        }

        animator.SetFloat("y accerlation", YAcceleration);
        print("fake velcoity: " + animator.GetFloat("y accerlation"));
        print("real y velcoity: " + rb.linearVelocity.y);
        // Check if grounded every frame \\
        isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer);

        if (isGrounded)
        {
            wallJumpMultiplier = 2f;
        } else

        // wall jump \\
        // Timers
        if (wallJumpControlTimer > 0)
            wallJumpControlTimer -= Time.deltaTime;

        if (wallJumpCooldownTimer > 0)
            wallJumpCooldownTimer -= Time.deltaTime;

        if (wallStickTimer > 0)
            wallStickTimer -= Time.deltaTime;


        // WALL JUMP
        if (isTouchingWall &&
            Input.GetKeyDown(KeyCode.Space) &&
            wallJumpCooldownTimer <= 0f &&
            !isGrounded)
        {
            DoWallJump();
        }

        if (isGrounded)
        {
            if (wallJumpMultiplier != 2f)
            wallJumpMultiplier = 2f; // Reset to big jump when grounded \\
        }

        // Wall jump timer logic \\
        if (isWallJumping)
        {
            wallJumpTimer -= Time.deltaTime;
            if (wallJumpTimer <= 0f)
            {
                isWallJumping = false;
            }
        }

        if (isGrounded && !firstwalljump)
        {
            firstwalljump = true;
            moveSpeed = oldmovespeed;
        }

        if (isGrounded && retainspeed)
        {
            moveSpeed = oldmovespeed;
            retainspeed = false;
        }

        // Jump \\
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // Fast fall \\
        if (Input.GetKeyDown(KeyCode.S) && !isGrounded)
        {
            rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Min(rb.linearVelocity.y, -20f));
        }

        // Slide (yo kanye ref) \\
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded)
        {
            moveSpeed = oldmovespeed * 2;
        }
        if (Input.GetKey(KeyCode.LeftControl) && isGrounded)
        {
            if (moveSpeed > 0)
            {
                moveSpeed = moveSpeed - 00.1f;
            }
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            if (moveSpeed > oldmovespeed)
            {
                retainspeed = true;
            }
            else
            {
                moveSpeed = oldmovespeed;
            }
        }
    }

    void FixedUpdate()
    {
        // Update facing
        if (Input.GetKey(KeyCode.A))
            way_of_facing = false;

        if (Input.GetKey(KeyCode.D))
            way_of_facing = true;


        // Don't interfere with the initial wall jump
        if (wallJumpControlTimer > 0f)
            return;


        // Normal movement
        float targetSpeed = moveInput * moveSpeed;

        // Smoothly control horizontal movement after wall jump
        float newX = Mathf.MoveTowards(
            rb.linearVelocity.x,
            targetSpeed,
            50f * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector2(
            newX,
            rb.linearVelocity.y
        );
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Walls"))
            return;

        isTouchingWall = true;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            // Wall is to the right of player
            if (contact.normal.x < -0.5f)
            {
                wallDirection = 1;
            }

            // Wall is to the left of player
            else if (contact.normal.x > 0.5f)
            {
                wallDirection = -1;
            }
        }
    }
    /* BUGGY BUGGY CODE DONT USE UNLESS FUN
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("platform"))
        {
            if (rb.linearVelocity.x <= 0)
            {
                rb.linearVelocity = new Vector2(-3 * (moveInput * moveSpeed), rb.linearVelocity.y);
                Debug.Log("Hit platform, velocity set to: " + rb.linearVelocity);
            }
        }
    }
    */
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Walls"))
        {
            isTouchingWall = false;
        }
    }


    void DoWallJump()
    {
        // Determine which direction to launch
        float jumpDirection;

        if (wallDirection == 1)
        {
            // Wall is on the right -> launch left
            jumpDirection = -1f;
        }
        else
        {
            // Wall is on the left -> launch right
            jumpDirection = 1f;
        }

        // Calculate forces
        float horizontalForce = wallJumpHorizontalForce * wallJumpMultiplier;
        float verticalForce = wallJumpVerticalForce * wallJumpMultiplier;

        // Give the player the actual wall jump
        rb.linearVelocity = new Vector2(
            jumpDirection * horizontalForce,
            verticalForce
        );

        // Start wall jump state
        isWallJumping = true;

        // Temporarily prevent normal movement from overriding the jump
        wallJumpControlTimer = wallJumpControlLock;

        // Prevent immediately wall jumping again
        wallJumpCooldownTimer = wallJumpCooldown;

        // Temporarily prevent wall sticking
        wallStickTimer = wallStickTime;

        // Decay multiplier
        wallJumpMultiplier = Mathf.Max(
            wallJumpMultiplier * wallJumpMultiplierDecay,
            wallJumpMultiplierMin
        );

        

        Debug.Log(
            "WALL JUMP | Direction: " + jumpDirection +
            " | Horizontal: " + horizontalForce +
            " | Vertical: " + verticalForce
        );
    }
}
