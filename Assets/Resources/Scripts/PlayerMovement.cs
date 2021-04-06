using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    public enum MoveState
    {
        Ground, Water, Wall
    }
    private MoveState moveState;

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

    [Header("Climbing Settings")]
    [SerializeField] private LayerMask wall; // Layermask for the ground that the player can climb on.
    [SerializeField] private float climbSpeed; // Speed the player can climb at.
    [SerializeField] private float climbRunSpeed; // Speed the player can climb at.
    [SerializeField] private float climbTurnAroundSpeed; // Speed the player turns around while climbing.

    [Header("Swim Settings")]
    [SerializeField] private LayerMask water; // Layermask for the water that the player can swim in.
    [SerializeField] private float swimSpeed; // Speed the player can swim at.
    [SerializeField] private float swimRunSpeed; // Speed the player can swim at while running underwater.
    [SerializeField] private float swimJumpForce; // Force added when the player jumps while underwater.
    [SerializeField] private float swimTurnAroundSpeed; // Speed the player turns around while swimming.
    [SerializeField] private float underwaterGravity; // Gravity for underwater.
    [SerializeField] private float maxUnderwaterBreath; // How long (in seconds) you can breath underwater for.

    [Header("Boxcast Settings")]
    [SerializeField] private Vector2 boxOffset; // Offset for the grounded boxcast collision.
    [SerializeField] private Vector2 boxSize; // Size for the grounded boxcast collision.
    [SerializeField] private Vector2 wallBoxOffset; // Size for the wall boxcast collision.
    [SerializeField] private Vector2 wallBoxSize; // Size for the wall boxcast collision.

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem jumpParticles; // Particles for when the player jumps.
    [SerializeField] private ParticleSystem landParticles; // Particles for when the player lands.
    [SerializeField] private ParticleSystem walkParticles; // Particles for when the player walks.
    [SerializeField] private ParticleSystem runParticles; // Particles for when the player runs.
    [SerializeField] private ParticleSystem breathingParticles; // Particles for when the player is breathing and not underwater.
    [SerializeField] private ParticleSystem underwaterParticles; // Particles for when the player is breathing underwater.

    [Header("Miscellaneous")]
    [SerializeField] private bool showDebugs;
    public byte abilities = 0b_0000_0000; // Byte value representing unlocked abilities that the player has.
    private readonly byte wallClimb = 0b_0000_0001; // Byte value representing unlocked abilities that the player has.
    private readonly byte nightVision = 0b_0000_0010; // Byte value representing unlocked abilities that the player has.
    private readonly byte longerUnderwater = 0b_0000_0100; // Byte value representing unlocked abilities that the player has.

    [SerializeField] private float throwForce;

    public GameObject overlappingItem;
    public GameObject heldItem;

    private float aboveGroundGravity;
    private bool isNightVisionActive;

    private float currentSpeed;
    private float currentTurnAroundSpeed;

    private Vector2 moveVel;
    private Vector2 jumpVel;
    private Vector2 pounceVel;

    private float jumpLeft;
    private bool nextToWall;
    private bool isPouncing;
    private float breathLeftUnderwater;

    private void Awake()
    {
        controls = new Controls();
        currentSpeed = walkSpeed;
        currentTurnAroundSpeed = walkTurnAroundSpeed;

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        controls.Player.BeginClimb.performed += ctx => BeginClimb();
        controls.Player.Pounce.performed += ctx => Pounce();
        controls.Player.Use.performed += ctx => Use();

        aboveGroundGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        Move(controls.Player.Move.ReadValue<Vector2>());
        Run(controls.Player.Run.ReadValue<float>());
        Jump(controls.Player.Jump.ReadValue<float>());
        Crouch(controls.Player.Crouch.ReadValue<float>());
        LookUp(controls.Player.LookUp.ReadValue<float>());

        if (isPouncing && IsGrounded())
        {
            EndPounce();
        }

        if (Mathf.Abs(rb2D.velocity.x) > 0.05f)
        {
            sprtRndr.flipX = rb2D.velocity.x < 0f;
        }

        if (moveState == MoveState.Water)
        {
            breathLeftUnderwater -= Time.deltaTime;
        }
    }

    private void FixedUpdate()
    {
        #region Jumping

        if (controls.Player.Jump.ReadValue<float>() == 1f && jumpLeft > 0f)
        {
            jumpVel += new Vector2(0f, jumpLeft);
            jumpLeft -= Time.deltaTime * jumpDecreaseTime;
        }
        else if (moveState != MoveState.Wall)
        {
            jumpVel = new Vector2(0f, rb2D.velocity.y);
            jumpLeft = 0f;
        }

        #endregion

        Vector2 targetVel = Vector2.zero;

        targetVel += moveVel * currentSpeed; // Adds movement force.
        targetVel += jumpVel; // Adds jump force.
        targetVel += pounceVel; // Adds pounce force.

        rb2D.velocity = targetVel;
    }

    // For use with new input system.
    public void Move(InputAction.CallbackContext ctx)
    {
        ApplyMove(ctx.ReadValue<Vector2>());
    }

    // For use with old input system.
    public void Move(Vector2 moveVal)
    {
        ApplyMove(moveVal);
    }

    public void ApplyMove(Vector2 moveVal)
    {
        // Moving while on the ground only uses the left and right buttons.
        // Moving while swimming can move in all directions.

        if (isPouncing)
            return;

        switch (moveState)
        {
            case MoveState.Wall:
                if (CheckWall(moveVal * 2f))
                {
                    moveVel = Vector2.Lerp(moveVel, moveVal, Time.deltaTime * currentTurnAroundSpeed);
                }
                else
                {
                    moveVel = Vector2.zero;
                }
                break;
            default: // Walking/Running/Water/Air.
                moveVel = Vector2.Lerp(moveVel, new Vector2(moveVal.x, 0f), Time.deltaTime * currentTurnAroundSpeed);
                break;
        }

        if (showDebugs)
        {
            Debug.Log("Player Movement" + moveVel);
        }
    }

    // For use with new input system.
    public void Run(InputAction.CallbackContext ctx)
    {
        ApplyRun(ctx.ReadValue<float>() == 1f);
    }

    // For use with old input system.
    public void Run(float runVal)
    {
        ApplyRun(runVal == 1f);
    }

    public void ApplyRun(bool isRunning)
    {
        // Pressing run while grounded will allow you to run.

        if (!IsGrounded() && moveState != MoveState.Wall)
            return;

        switch (moveState)
        {
            case MoveState.Wall:
                currentSpeed = isRunning ? climbRunSpeed : climbSpeed;
                currentTurnAroundSpeed = climbTurnAroundSpeed;
                break;
            case MoveState.Water:
                currentSpeed = isRunning ? swimSpeed : swimRunSpeed;
                currentTurnAroundSpeed = swimTurnAroundSpeed;
                break;
            default:
                currentSpeed = isRunning ? runSpeed : walkSpeed;
                currentTurnAroundSpeed = isRunning ? runTurnAroundSpeed : walkTurnAroundSpeed;
                break;
        }

        if (showDebugs)
        {
            Debug.Log("Player Speed: " + currentSpeed);
        }
    }

    // For use with new input system.
    public void Jump(InputAction.CallbackContext ctx)
    {
        ApplyJump(ctx.ReadValue<float>() == 1f);
    }

    // For use with old input system.
    public void Jump(float jumpVal)
    {
        ApplyJump(jumpVal == 1f);
    }

    public void ApplyJump(bool isJumping)
    {
        // Pressing jump while grounded will perform a jump.
        // Pressing jump while swimming will cause you to swim forwards faster.

        if (moveState == MoveState.Wall && isJumping)
        {
            EndClimb();
        }
        else
        {
            if (!IsGrounded() && moveState != MoveState.Water)
                return;
        }
        
        ActivateParticles(jumpParticles);
        if (isJumping)
        {
            if (moveState == MoveState.Ground || moveState == MoveState.Wall)
            {
                jumpLeft = extraJumpForce;
                jumpVel = new Vector2(0f, jumpForce);
                currentTurnAroundSpeed = aerialTurnAroundSpeed;
            }
            else 
            {
                jumpLeft = extraJumpForce;
                jumpVel = new Vector2(0f, swimJumpForce);
            }

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

        if (IsGrounded() || moveState != MoveState.Ground || rb2D.velocity.y > 0f)
            return;

        jumpVel = Vector2.zero;
        pounceVel = new Vector2(sprtRndr.flipX ? -jumpForce * 5f : jumpForce * 5f, -jumpForce);
        isPouncing = true;

        if (showDebugs)
        {
            Debug.Log("Player Pounced");
        }
    }

    private void EndPounce()
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
        // Beginning a climb can only happen when on land and when there's a climbable wall nearby.

        if (!nextToWall || moveState != MoveState.Ground && (abilities & wallClimb) != 0) // Make it so you need the ability for it to work.
            return;

        moveState = MoveState.Wall;
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

        moveState = MoveState.Ground;
        rb2D.gravityScale = 2f;

        if (showDebugs)
        {
            Debug.Log("Player Ended Climbing");
        }
    }

    // For use with new input system.
    public void Crouch(InputAction.CallbackContext ctx)
    {
        ApplyCrouch(ctx.ReadValue<float>() == 1f);
    }

    // For use with old input system.
    public void Crouch(float crouchVal)
    {
        ApplyCrouch(crouchVal == 1f);
    }

    public void ApplyCrouch(bool isCrouching)
    {
        // Crouching will allow the player to duck and move slowly.

        if (moveState != MoveState.Ground || !IsGrounded() || currentSpeed > walkSpeed)
            return;

        currentSpeed = isCrouching ? crawlSpeed : walkSpeed;

        if (isCrouching)
        {
            currentTurnAroundSpeed = currentSpeed == crawlSpeed ? crawlTurnAroundSpeed : walkTurnAroundSpeed;
        }

        if (showDebugs)
        {
            Debug.Log("Player Began Crouching");
        }
    }

    // For use with new input system.
    public void LookUp(InputAction.CallbackContext ctx)
    {
        ApplyLookUp(ctx.ReadValue<float>() == 1f);
    }

    // For use with old input system.
    public void LookUp(float lookUpVal)
    {
        ApplyLookUp(lookUpVal == 1f);
    }

    public void ApplyLookUp(bool isLookingUp)
    {
        // Player can only look up when grounded.

        if (moveState != MoveState.Ground || !IsGrounded())
            return;

        if (showDebugs)
        {
            Debug.Log("Player Began Looking Up");
        }
    }

    public void Use()
    {
        if (heldItem == null) // Pick Up.
        {
            if (overlappingItem != null)
            {
                if (overlappingItem.CompareTag("Pickup")) // Pickup Item.
                {
                    overlappingItem.GetComponent<Pickup>().Grab(this.gameObject);
                    heldItem = overlappingItem;
                }
                else if (overlappingItem.CompareTag("Interactable")) // Interact with Object.
                {
                    overlappingItem.GetComponent<Interactable>().effect.Invoke();
                }
            }
        }
        else // Throw.
        {
            heldItem.GetComponent<Pickup>().Throw(new Vector2(sprtRndr.flipX ? -1f : 1f, 2f) * throwForce);
            heldItem = null;
        }
    }

    private void EnterWater()
    {
        moveState = MoveState.Water;

        rb2D.gravityScale = underwaterGravity;
        breathLeftUnderwater = maxUnderwaterBreath;
    }

    private void ExitWater()
    {
        moveState = MoveState.Ground;

        rb2D.gravityScale = aboveGroundGravity;
        breathLeftUnderwater = maxUnderwaterBreath;
    }
    
    public void ActivateNightVision()
    {
        if ((abilities & nightVision) == 0)
            return;

        isNightVisionActive = true;
    }
    
    public void DeactivateNightVision()
    {
        if ((abilities & nightVision) == 0)
            return;

        isNightVisionActive = false;
    }
    
    public void UnlockNewAbility()
    {
        abilities <<= 1;
        
        if ((abilities & longerUnderwater) == 0)
            maxUnderwaterBreath *= 2f;
    }
    
    public void ActivateParticles(ParticleSystem particles)
    {
        if (particles != null)
        {
            particles.Play();
        }
    }

    private bool CheckBit(byte b, int pos)
    {
        return (b & (1 << pos)) != 0;
    }

    // Checks if the player is grounded or not. Automatically set to true if the player is underwater.
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2) transform.position + boxOffset, boxSize, 0f, Vector2.down, 0f, ground);
        bool isGrounded = hit;

        if (showDebugs)
        {
            Debug.Log("Grounded: " + isGrounded);
        }

        return isGrounded;
    }

    // Checks if there is a climbable wall in any direction of the player.
    private bool CheckWall(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2) transform.position + dir, wallBoxSize, 0f, Vector2.zero, 0f, wall);
        bool isWall = hit;
        
        if (showDebugs)
        {
            Debug.Log("Wall: " + isWall);
        }
        
        return isWall;
    }

    // Sends you back to the tribe if you die.
    public void Die()
    {

    }

    // Makes you restart the day if the tribe runs out of food.
    public void GameOver()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            EnterWater();
        }
        else if (collision.CompareTag("Wall"))
        {
            nextToWall = true;
        }
        else if (collision.CompareTag("Pickup"))
        {
            overlappingItem = collision.gameObject;
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
            ExitWater();
        }
        else if (collision.CompareTag("Wall"))
        {
            nextToWall = false;
            EndClimb();
        }
        else if (collision.CompareTag("Pickup"))
        {
            overlappingItem = null;
        }

        if (showDebugs)
        {
            Debug.Log("Player Exited: " + collision.name);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube((Vector2) transform.position + boxOffset, boxSize);
        Gizmos.DrawCube((Vector2) transform.position + wallBoxOffset, wallBoxSize);
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
