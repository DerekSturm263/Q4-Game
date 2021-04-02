using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        Ground, Water, Wall
    }
    private MovementState moveState;

    private Controls controls;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    [Header("Movement Settings")]
    [SerializeField] private LayerMask ground; // Layermask for the ground that will trigger a grounded collision.
    [SerializeField] private float walkSpeed; // Speed the player can walk at.
    [SerializeField] private float runSpeed; // Speed the player can run at.
    [SerializeField] private float crawlSpeed; // Speed the player can crawl at.
    [SerializeField] private float walkTurnAroundSpeed; // Speed the player turns around while walking.
    [SerializeField] private float runTurnAroundSpeed; // Speed the player turns around while running.
    [SerializeField] private float crawlTurnAroundSpeed; // Speed the player turns around while crawling.

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce; // Force immediately added when the player begins to jump.
    [SerializeField] private float extraJumpForce; // Force added when the player holds down jump.
    [SerializeField] private float jumpDecreaseTime; // The speed at which the extraJumpForce goes away.
    [SerializeField] private float aerialTurnAroundSpeed; // The speed the player turns around in the air.

    [Header("Swim Settings")]
    [SerializeField] private LayerMask water; // Layermask for the water that the player can swim in.
    [SerializeField] private float swimSpeed; // Speed the player can swim at.
    [SerializeField] private float swimDashForce; // Force added when the player uses a swim dash (jump while swimming).
    [SerializeField] private float swimTurnAroundSpeed; // Speed the player turns around while swimming.

    [Header("Climbing Settings")]
    [SerializeField] private LayerMask wall; // Layermask for the ground that the player can climb on.
    [SerializeField] private float climbSpeed; // Speed the player can climb at.
    [SerializeField] private float climbingTurnAroundSpeed; // Speed the player turns around while climbing.

    [Header("Boxcast Settings")]
    [SerializeField] private Vector2 boxOffset; // Offset for the grounded boxcast collision.
    [SerializeField] private Vector2 boxSize; // Size for the grounded boxcast collision.

    [Header("Miscellaneous")]
    [SerializeField] private bool showDebugs;

    private float currentSpeed;

    private Vector2 moveVel;
    private Vector2 jumpVel;
    private Vector2 pounceVel;

    private float jumpLeft;
    private bool nextToWall;
    private bool isPouncing;

    private void Awake()
    {
        controls = new Controls();
        currentSpeed = walkSpeed;

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        #region Movement Lerping

        float turnAroundSpeed;
        if (IsGrounded())
        {
            switch (moveState)
            {
                case MovementState.Wall:
                    turnAroundSpeed = climbingTurnAroundSpeed;
                    break;
                case MovementState.Water:
                    turnAroundSpeed = swimTurnAroundSpeed;
                    break;
                default: // Walking/Running/Water/Air.
                    turnAroundSpeed = currentSpeed == walkSpeed ? walkTurnAroundSpeed : currentSpeed == runSpeed ? runTurnAroundSpeed : crawlTurnAroundSpeed;
                    break;
            }
            
        }
        else
        {
            turnAroundSpeed = aerialTurnAroundSpeed;
        }

        #endregion

        #region Jumping

        if (controls.Player.Jump.ReadValue<float>() == 1f && jumpLeft > 0f)
        {
            jumpVel += new Vector2(0f, jumpLeft);
            jumpLeft -= Time.deltaTime * jumpDecreaseTime;
        }
        else if (moveState == MovementState.Ground)
        {
            jumpVel = new Vector2(0f, rb2D.velocity.y);
            jumpLeft = 0f;
        }

        #endregion

        if (isPouncing && IsGrounded())
        {
            CancelPounce();
        }

        Vector2 targetVel = Vector2.zero;

        targetVel += moveVel * currentSpeed; // Adds proper movement force.
        targetVel += jumpVel; // Adds the proper jump force and pounce force.
        targetVel += pounceVel; // Adds the proper jump force and pounce force.

        rb2D.velocity = targetVel;
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        // Moving while on the ground only uses the left and right buttons.
        // Moving while swimming can move in all directions.

        if (isPouncing)
            return;

        switch (moveState)
        {
            case MovementState.Wall:
                moveVel = ctx.ReadValue<Vector2>();
                break;
            case MovementState.Water:
                
                break;
            default: // Walking/Running/Water/Air.
                moveVel = new Vector2(ctx.ReadValue<Vector2>().x, 0f);
                break;
        }

        if (showDebugs)
        {
            Debug.Log("Player Movement" + moveVel);
        }
    }

    public void Run(InputAction.CallbackContext ctx)
    {
        // Pressing run while grounded will allow you to run.

        currentSpeed = ctx.ReadValue<float>() == 1f ? runSpeed : walkSpeed;

        if (showDebugs)
        {
            Debug.Log("Player Speed: " + currentSpeed);
        }
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        // Pressing jump while grounded will perform a jump.
        // Pressing jump while swimming will cause you to swim forwards faster.

        if (moveState == MovementState.Wall)
        {
            EndClimb();
        }
        else
        {
            if (!IsGrounded())
                return;
        }

        if (ctx.ReadValue<float>() == 1f)
        {
            jumpLeft = extraJumpForce;
            jumpVel = new Vector2(0f, jumpForce);

            if (showDebugs)
            {
                Debug.Log("Player Started Jumping");
            }
        }
        else
        {
            jumpLeft = 0f;
            jumpVel = Vector2.zero;

            if (showDebugs)
            {
                Debug.Log("Player Stopped Jumping");
            }
        }
    }

    public void Pounce()
    {
        // Pouncing is only allowed in mid-air.

        if (IsGrounded() || moveVel.x == 0f || moveState != MovementState.Ground)
            return;

        jumpVel = Vector2.zero;
        pounceVel = new Vector2(moveVel.x / Mathf.Abs(moveVel.x) * jumpForce * 5f, -jumpForce);
        isPouncing = true;

        if (showDebugs)
        {
            Debug.Log("Player Pounced");
        }
    }

    private void CancelPounce()
    {
        pounceVel = Vector2.zero;
        moveVel = Vector2.zero;
        isPouncing = false;

        if (showDebugs)
        {
            Debug.Log("Player Hit Ground");
        }
    }

    public void BeginClimb()
    {
        // Beginning a climb can only happen when grounded and on land.

        if (!nextToWall || moveState != MovementState.Ground)
            return;

        moveState = MovementState.Wall;
        rb2D.gravityScale = 0f;

        jumpVel = Vector2.zero;
        pounceVel = Vector2.zero;

        if (showDebugs)
        {
            Debug.Log("Player Began Climbing");
        }
    }

    private void EndClimb()
    {
        // Ending a climb can happen while jumping and climbing a wall or by lowering yourself to the ground.

        moveState = MovementState.Ground;
        rb2D.gravityScale = 2f;

        if (showDebugs)
        {
            Debug.Log("Player Ended Climbing");
        }
    }

    public void Crouch(InputAction.CallbackContext ctx)
    {
        // Crouching will allow the player to duck and move slowly.

        if (moveState != MovementState.Ground || !IsGrounded())
            return;

        currentSpeed = ctx.ReadValue<float>() == 1f ? crawlSpeed : walkSpeed;

        if (showDebugs)
        {
            Debug.Log("Player Began Crouching");
        }
    }

    public void LookUp(InputAction.CallbackContext ctx)
    {
        // Player can only look up when grounded.

        if (moveState != MovementState.Ground || !IsGrounded())
            return;

        if (showDebugs)
        {
            Debug.Log("Player Began Looking Up");
        }
    }

    private bool IsGrounded()
    {
        bool isGrounded = Physics2D.BoxCast((Vector2)transform.position - boxOffset, boxSize, 0f, Vector2.down, 0f, ground);

        if (showDebugs)
        {
            Debug.Log("Grounded: " + isGrounded);
        }

        return isGrounded;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            moveState = MovementState.Water;
        }
        else if (collision.CompareTag("Wall"))
        {
            nextToWall = true;
        }

        if (showDebugs)
        {
            Debug.Log("Player Entered: " + collision.name);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            moveState = MovementState.Ground;
        }
        else if (collision.CompareTag("Wall"))
        {
            nextToWall = false;
            EndClimb();
        }

        if (showDebugs)
        {
            Debug.Log("Player Exited: " + collision.name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2) transform.position - boxOffset, boxSize);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
