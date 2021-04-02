using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(CapsuleCollider2D))]
public class PlayerMovement : MonoBehaviour
{
    public enum MovementState
    {
        Land, Swimming, Climbing
    }

    private Controls controls;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;

    private Vector2 currentInputVal;
    private float currentSpeed;

    private MovementState moveState;

    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed; // Speed the player can walk at.
    [SerializeField] private float runSpeed; // Speed the player can run at.
    [SerializeField] private float swimSpeed; // Speed the player can swim at.
    [SerializeField] private float crawlSpeed; // Speed the player can crawl at.
    [SerializeField] private float climbSpeed; // Speed the player can climb at.

    [SerializeField] private float jumpForce; // Force added when the player jumps.
    [SerializeField] private float swimDashForce; // Force added when the player uses a swim dash (jump while swimming).

    [Header("Boxcast Settings")]
    [SerializeField] private LayerMask ground; // Layermask for the ground that will trigger a grounded collision.

    [SerializeField] private Vector2 boxOffset; // Offset for the grounded boxcast collision.
    [SerializeField] private Vector2 boxSize; // Size for the grounded boxcast collision.
    [SerializeField] private float boxDist; // Distance for the grounded boxcast collision.

    [Header("Miscellaneous")]
    [SerializeField] private bool showDebugs;

    private void Awake()
    {
        controls = new Controls();

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        rb2D.AddForce(new Vector2(currentInputVal.x * currentSpeed, 0f));
    }

    public void Move(InputAction.CallbackContext ctx)
    {
        // Moving while on the ground only uses the left and right buttons.
        // Moving while swimming can move in all directions.

        currentInputVal = ctx.ReadValue<Vector2>();

        if (showDebugs)
        {
            Debug.Log("Player Movement" + currentInputVal);
        }
    }

    public void Run(InputAction.CallbackContext ctx)
    {
        // Pressing run while grounded will allow you to run.
        // Pressing run while swimming will do nothing.

        currentSpeed = ctx.ReadValue<float>() == 1f ? runSpeed : walkSpeed;

        if (showDebugs)
        {
            Debug.Log("Player Speed: " + currentSpeed);
        }
    }

    public void Jump()
    {
        // Pressing jump while grounded will perform a jump.
        // Pressing jump while swimming will cause you to swim forwards faster.

        if (!IsGrounded())
            return;

        rb2D.AddForce(new Vector2(0f, jumpForce), ForceMode2D.Impulse);

        if (showDebugs)
        {
            Debug.Log("Player Jumped");
        }
    }

    public void Pounce()
    {
        // Pouncing is only allowed in mid-air.

        if (IsGrounded() || currentInputVal.x == 0f)
            return;

        rb2D.AddForce(new Vector2(currentInputVal.x / Mathf.Abs(currentInputVal.x), -jumpForce), ForceMode2D.Impulse);

        if (showDebugs)
        {
            Debug.Log("Player Pounced");
        }
    }

    public void BeginClimb()
    {
        // Beginning a climb can only happen when grounded and on land.

        if (showDebugs)
        {
            Debug.Log("Player Began Climbing");
        }
    }

    public void Crouch(InputAction.CallbackContext ctx)
    {
        // Crouching will allow the player to duck and move slowly.

        currentSpeed = ctx.ReadValue<float>() == 1f ? crawlSpeed : walkSpeed;

        if (showDebugs)
        {
            Debug.Log("Player Began Crouching");
        }
    }

    public void LookUp(InputAction.CallbackContext ctx)
    {
        // Player can only look up when grounded.

        if (showDebugs)
        {
            Debug.Log("Player Began Looking Up");
        }
    }

    private bool IsGrounded()
    {
        return Physics2D.BoxCast((Vector2) transform.position - boxOffset * boxDist, boxSize, 0f, Vector2.down, 0f, ground);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2) transform.position - boxOffset * boxDist, boxSize);
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
