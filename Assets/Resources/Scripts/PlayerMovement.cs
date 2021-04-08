using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

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

    // Volume Settings.
    private VolumeProfile vol;
    private Vignette vignette;
    private ColorAdjustments colAdj;
    private DepthOfField dof;

    private float defaultVignette;
    private Color defaultColAdj;
    private float defaultDOF;

    private float targetVignette;
    private float targetDOF;

    [Header("Movement Settings")]
    [SerializeField] private LayerMask ground; // Layermask for the ground that will trigger a grounded collision.
    [SerializeField] private float walkSpeed = 7f; // Speed the player can walk at.
    [SerializeField] private float runSpeed = 14f; // Speed the player can run at.
    [SerializeField] private float crawlSpeed = 2f; // Speed the player can crawl at.
    [SerializeField] private float walkTurnAroundSpeed = 10f; // Speed the player turns around while walking.
    [SerializeField] private float runTurnAroundSpeed = 3f; // Speed the player turns around while running.
    [SerializeField] private float crawlTurnAroundSpeed = 5f; // Speed the player turns around while crawling.

    [Header("Jump/Aerial Settings")]
    [SerializeField] private float jumpForce = 6f; // Force immediately added when the player begins to jump.
    [SerializeField] private float extraJumpForce = 2.5f; // Force added when the player holds down jump.
    [SerializeField] private float jumpDecreaseTime = 20f; // The speed at which the extraJumpForce goes away.
    [SerializeField] private float aerialTurnAroundSpeed = 1.5f; // The speed the player turns around in the air.

    [Header("Climbing Settings")]
    [SerializeField] private LayerMask wall; // Layermask for the ground that the player can climb on.
    [SerializeField] private float climbSpeed = 6f; // Speed the player can climb at.
    [SerializeField] private float climbRunSpeed = 12f; // Speed the player can climb at.
    [SerializeField] private float climbTurnAroundSpeed = 10f; // Speed the player turns around while climbing.

    [Header("Swim Settings")]
    [SerializeField] private LayerMask water; // Layermask for the water that the player can swim in.
    [SerializeField] private float swimSpeed = 6f; // Speed the player can swim at.
    [SerializeField] private float swimRunSpeed = 9f; // Speed the player can swim at while running underwater.
    [SerializeField] private float swimJumpForce = 5f; // Force added when the player jumps while underwater.
    [SerializeField] private float swimTurnAroundSpeed = 2f; // Speed the player turns around while swimming.
    [SerializeField] private float underwaterGravity = 0.5f; // Gravity for underwater.
    [SerializeField] private float maxUnderwaterBreath = 10f; // How long (in seconds) you can breath underwater for.
    [SerializeField] private UnityEngine.UI.Image breathMeter;

    [Header("Boxcast Settings")]
    [SerializeField] private Vector2 groundedBoxOffset = new Vector2(0f, -1.35f); // Offset for the grounded boxcast collision.
    [SerializeField] private Vector2 groundedBoxSize = new Vector2(0.625f, 0.05f); // Size for the grounded boxcast collision.
    [SerializeField] private Vector2 wallBoxOffset = new Vector2(1.5f, 0f); // Size for the wall boxcast collision.
    [SerializeField] private Vector2 wallBoxSize = new Vector2(1f, 1f); // Size for the wall boxcast collision.

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem walkRunParticles; // Particles for when the player walks.
    [SerializeField] private ParticleSystem jumpLandParticles; // Particles for when the player jumps.
    [SerializeField] private ParticleSystem breathingParticles; // Particles for when the player is breathing and not underwater.
    [SerializeField] private ParticleSystem underwaterParticles; // Particles for when the player is breathing underwater.

    [Header("Miscellaneous")]
    [SerializeField] private bool showDebugs = false;
    public byte abilities = 0b_0000_0000; // Byte value representing unlocked abilities that the player has.
    private readonly byte wallClimb = 0b_0000_0001; // Byte value representing unlocked abilities that the player has.
    private readonly byte nightVision = 0b_0000_0010; // Byte value representing unlocked abilities that the player has.
    private readonly byte longerUnderwater = 0b_0000_0100; // Byte value representing unlocked abilities that the player has.
    public UnityEngine.UI.Image fade; // Dark overlay that appears when respawning.
    private Animator fadeAnim;

    [SerializeField] private Vector2 throwVector = new Vector2(10f, 10f); // Vector applied when throwing an item.

    public GameObject overlappingItem;
    public GameObject heldItem;

    private float aboveGroundGravity;
    private bool isNightVisionActive;

    private float currentSpeed;
    private float currentTurnAroundSpeed;

    private Vector2 moveVel;
    private Vector2 jumpVel;
    private Vector2 pounceVel;

    // Miscellaneous.
    private float jumpLeft;
    private bool nextToWall;
    private bool isPouncing;
    private float breathLeftUnderwater;
    private LayerMask itemMask = 1 << 11;

    // Respawning.
    private Vector2 lastPosBeforeSwim;

    private void Awake()
    {
        PlayerRespawn.player = this;

        controls = new Controls();
        currentSpeed = walkSpeed;
        currentTurnAroundSpeed = walkTurnAroundSpeed;

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        vol = Camera.main.GetComponent<Volume>().profile;
        vol.TryGet(out vignette);
        vol.TryGet(out colAdj);
        vol.TryGet(out dof);

        defaultVignette = vignette.intensity.value;
        defaultColAdj = colAdj.colorFilter.value;
        defaultDOF = dof.focusDistance.value;

        targetVignette = defaultVignette;
        targetDOF = defaultDOF;

        controls.Player.BeginClimb.performed += ctx => BeginClimb();
        controls.Player.Pounce.performed += ctx => Pounce();
        controls.Player.Use.performed += ctx => Use();

        fadeAnim = fade.GetComponent<Animator>();

        aboveGroundGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        if (isPouncing && IsGrounded() || rb2D.velocity == Vector2.zero)
        {
            SlowPounce();
        }

        if (moveState == MoveState.Water)
        {
            if (!underwaterParticles.gameObject.activeSelf)
            {
                underwaterParticles.gameObject.SetActive(true);
            }
            if (breathingParticles.gameObject.activeSelf)
            {
                breathingParticles.gameObject.SetActive(false);
            }

            vignette.intensity.value = 0.55f - (breathLeftUnderwater / maxUnderwaterBreath) * 0.35f;

            if ((breathLeftUnderwater -= Time.deltaTime) <= 0f)
            {
                dof.focusDistance.value -= Time.deltaTime * 2f;

                if (vignette.intensity.value >= 0.65f)
                {
                    Drown();
                }
            }
        }
        else
        {
            if (!breathingParticles.gameObject.activeSelf)
            {
                breathingParticles.gameObject.SetActive(true);
            }
            if (underwaterParticles.gameObject.activeSelf)
            {
                underwaterParticles.gameObject.SetActive(false);
            }

            if (IsGrounded())
            {
                lastPosBeforeSwim = transform.position;
            }

            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, Time.deltaTime);
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, targetDOF, Time.deltaTime);
        }

        breathMeter.fillAmount = breathLeftUnderwater / maxUnderwaterBreath;
        anim.SetFloat("Velocity Y", rb2D.velocity.y);
    }

    private void FixedUpdate()
    {
        Move(controls.Player.Move.ReadValue<Vector2>()); // Check and apply movement.
        Run(controls.Player.Run.ReadValue<float>() == 1f); // Check and apply whether the player is running or not.
        Jump(controls.Player.Jump.ReadValue<float>() == 1f); // Check and apply whether the player is holding jump or not.
        Crouch(controls.Player.Crouch.ReadValue<float>() == 1f); // Check and apply whether the player is crouching or not.
        LookUp(controls.Player.LookUp.ReadValue<float>() == 1f); // Check and apply whether the player is looking up or not.

        Vector2 targetVel;
        if (moveState != MoveState.Wall)
        {
            targetVel = new Vector2(0f, rb2D.velocity.y); // Sets targetVel to be determinant on the current Y velocity.
        }
        else
        {
            targetVel = Vector2.zero; // Sets targetVel to be nothing.
        }

        targetVel += moveVel * currentSpeed; // Adds movement force.
        targetVel += pounceVel; // Adds pounce force.
        targetVel += jumpVel;

        if (targetVel.x != 0f)
        {
            sprtRndr.flipX = targetVel.x < 0f;
        }

        rb2D.velocity = targetVel;
    }

    // Handles movement.
    public void Move(Vector2 moveVal)
    {
        // Moving while on the ground only uses the left and right buttons.
        // Moving while swimming can move in all directions.

        if (Mathf.Abs(pounceVel.x) > 1f)
            return;

        isPouncing = false;
        pounceVel = Vector2.zero;

        switch (moveState)
        {
            case MoveState.Wall:
                if (IsBackgroundWall(moveVal * 1.5f))
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

        if (moveVel.magnitude < 0.01f)
        {
            moveVel = Vector2.zero;
        }

        if (moveVel.x != 0f && IsGrounded() && moveState == MoveState.Ground)
        {
            walkRunParticles.gameObject.SetActive(true);
        }
        else
        {
            walkRunParticles.gameObject.SetActive(false);
        }

        anim.SetFloat("Move X", moveVel.x);
        anim.SetFloat("Move Y", moveVel.y);

        if (showDebugs)
        {
            Debug.Log("Player Movement" + moveVel);
        }
    }

    // Lets the player run.
    public void Run(bool isRunning)
    {
        // Pressing run while grounded will allow you to run.

        if (!IsGrounded() && moveState != MoveState.Wall && currentSpeed != crawlSpeed)
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

        anim.SetBool("Is Running", isRunning);

        if (showDebugs)
        {
            Debug.Log("Player Speed: " + currentSpeed);
        }
    }

    // Jumps. Holding jump can make you jump higher.
    public void Jump(bool isJumping)
    {
        // Apply upward momentum.
        if (isJumping)
        {
            if (moveState == MoveState.Wall)
            {
                EndClimb();

                jumpVel = new Vector2(0f, jumpForce);
                jumpLeft = extraJumpForce;
                currentTurnAroundSpeed = aerialTurnAroundSpeed;
            }
            else if (moveState == MoveState.Water)
            {
                jumpLeft = extraJumpForce;
                jumpVel = new Vector2(0f, swimJumpForce);
            }
            else if (IsGrounded())
            {
                jumpLandParticles.Play();

                jumpVel = new Vector2(0f, jumpForce);
                jumpLeft = extraJumpForce;
                currentTurnAroundSpeed = aerialTurnAroundSpeed;
            }
            else if (jumpLeft > 0f)
            {
                jumpVel = new Vector2(0f, jumpLeft);

                if ((jumpLeft -= Time.deltaTime * jumpDecreaseTime) < 0f)
                {
                    jumpLeft = 0f;
                }
            }

            if (showDebugs)
            {
                Debug.Log("Player Is Jumping");
            }
        }
        // Do not apply upward momentum.
        else
        {
            jumpLeft = 0f;

            if (showDebugs)
            {
                Debug.Log("Player Is Not Jumping");
            }
        }

        if (jumpLeft == 0f)
        {
            if (jumpVel.y > 0f)
            {
                jumpVel = new Vector2(jumpVel.x, 0f);
            }
            else if (jumpVel.y < 0f)
            {
                jumpVel = Vector2.zero;
            }
        }
    }

    // Pounces in the middle of the air.
    public void Pounce()
    {
        if (IsGrounded() || moveState != MoveState.Ground || isPouncing || jumpLeft != 0f || heldItem != null)
            return;

        pounceVel = new Vector2(sprtRndr.flipX ? -jumpForce * 2.5f: jumpForce * 2.5f, -jumpForce * 0.5f);
        isPouncing = true;

        anim.SetTrigger("Pounce");

        if (showDebugs)
        {
            Debug.Log("Player Pounced");
        }
    }

    // Slows down the pounce.
    private void SlowPounce()
    {
        if (Mathf.Abs(pounceVel.x) <= 0.01f)
        {
            pounceVel = Vector2.zero;
            isPouncing = false;

            return;
        }
        else
        {
            pounceVel = Vector2.Lerp(pounceVel, Vector2.zero, Time.deltaTime * 4f);
            rb2D.gravityScale = moveState == MoveState.Ground ? aboveGroundGravity : underwaterGravity;
        }

        if (showDebugs)
        {
            Debug.Log("Player Slowing Pounce");
        }
    }

    // Begins to climb a wall when there is one nearby.
    public void BeginClimb()
    {
        if (!nextToWall || moveState != MoveState.Ground && (abilities & wallClimb) != 0) // Make it so you need the ability for it to work.
            return;

        moveState = MoveState.Wall;
        rb2D.gravityScale = 0f;

        jumpVel = Vector2.zero;
        pounceVel = Vector2.zero;

        anim.SetBool("Is Climbing", true);

        if (showDebugs)
        {
            Debug.Log("Player Began Climbing");
        }
    }

    // Finishes climbing a wall.
    private void EndClimb()
    {
        moveState = MoveState.Ground;
        rb2D.gravityScale = aboveGroundGravity;

        moveVel = Vector2.zero;
        jumpVel = Vector2.zero;
        pounceVel = Vector2.zero;

        anim.SetBool("Is Climbing", false);

        if (showDebugs)
        {
            Debug.Log("Player Ended Climbing");
        }
    }

    // Crouching to allow the player to duck and move slowly.
    public void Crouch(bool isCrouching)
    {
        if (moveState != MoveState.Ground || !IsGrounded() || currentSpeed > walkSpeed)
            return;

        anim.SetBool("Is Crouching", isCrouching);

        if (isCrouching)
        {
            currentSpeed = crawlSpeed;
            currentTurnAroundSpeed = currentSpeed == crawlSpeed ? crawlTurnAroundSpeed : walkTurnAroundSpeed;

            if (showDebugs)
            {
                Debug.Log("Player Is Crouching");
            }
        }
    }

    // Looking Up moves the camera up.
    public void LookUp(bool isLookingUp)
    {
        if (moveState != MoveState.Ground || !IsGrounded())
            return;

        anim.SetBool("Is Looking Up", isLookingUp);

        if (isLookingUp)
        {
            currentSpeed = 0f;
            currentTurnAroundSpeed = 20f;

            if (showDebugs)
            {
                Debug.Log("Player Is Looking Up");
            }
        }
    }

    // Interacts or picks up nearby items depending on the item.
    public void Use()
    {
        if (heldItem == null) // Pick Up.
        {
            GameObject item = null;

            if (IsItemOverlap(ref item))
            {
                overlappingItem = item;

                if (overlappingItem.CompareTag("Pickup")) // Pickup Item.
                {
                    overlappingItem.GetComponent<Pickup>().Grab(gameObject);
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
            heldItem.GetComponent<Pickup>().Throw(Vector2.Scale(new Vector2(sprtRndr.flipX ? -1f : 1f, 1f), throwVector));
            heldItem = null;

            overlappingItem = null;
        }

        if (showDebugs)
        {
            Debug.Log("Player Tried Using An Item");
        }
    }

    private void EnterWater()
    {
        moveState = MoveState.Water;

        targetVignette = defaultVignette;
        colAdj.colorFilter.value = new Color(0.8f, 0.8f, 1f);
        targetDOF = defaultDOF;
        breathMeter.gameObject.SetActive(true);

        rb2D.velocity = new Vector2(rb2D.velocity.x, -1f);

        rb2D.gravityScale = underwaterGravity;
        breathLeftUnderwater = maxUnderwaterBreath;

        anim.SetBool("Is Underwater", true);
    }

    private void ExitWater()
    {
        moveState = MoveState.Ground;

        targetVignette = defaultVignette;
        colAdj.colorFilter.value = defaultColAdj;
        targetDOF = defaultDOF;

        rb2D.gravityScale = aboveGroundGravity;
        breathLeftUnderwater = maxUnderwaterBreath;

        breathMeter.GetComponent<Animator>().SetTrigger("Exit");
        anim.SetBool("Is Underwater", false);
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
    
    // Checks if the player is grounded or not. Automatically set to true if the player is underwater.
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2) transform.position + groundedBoxOffset, groundedBoxSize, 0f, Vector2.down, 0f, ground);
        bool isGrounded = hit;

        if (showDebugs)
        {
            Debug.Log("Grounded: " + isGrounded);
        }

        anim.SetBool("Is Grounded", isGrounded);

        return isGrounded;
    }

    // Checks if there is a climbable wall in any direction of the player.
    private bool IsBackgroundWall(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2) transform.position + dir, wallBoxSize, 0f, Vector2.zero, 0f, wall);
        bool isWall = hit;
        
        if (showDebugs)
        {
            Debug.Log("Wall: " + isWall);
        }
        
        return isWall;
    }

    // Checks if there is an item overlapping with the player.
    private bool IsItemOverlap(ref GameObject item)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 0f, Vector2.down, 0f, itemMask);
        bool isItem = hit;

        if (showDebugs)
        {
            Debug.Log("Item Detected");
        }

        if (isItem)
        {
            item = hit.collider.gameObject;
        }

        return isItem;
    }

    // Checks if the player is submerged in water.
    private bool IsWater()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(0.1f, 0.1f), 0f, Vector2.down, 0f, water);
        bool isWater = hit;

        if (showDebugs)
        {
            Debug.Log("Water Detected");
        }

        return isWater;
    }

    // Sends you back to the last position you were at on the ground before swimming.
    public void Drown()
    {
        fade.gameObject.SetActive(true);
        fadeAnim.SetBool("Player Drown", true);

        anim.SetTrigger("Death");
    }

    // Sends the player and camera to the last position before dying.
    public void Respawn()
    {
        transform.position = lastPosBeforeSwim;
        Camera.main.transform.position = transform.position - new Vector3(0f, 0f, 10f);

        anim.SetTrigger("Respawn");
    }

    // Makes you restart the day if the tribe runs out of food.
    public void GameOver()
    {
        anim.SetTrigger("Death");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            nextToWall = true;
        }
        else if (collision.CompareTag("Pickup"))
        {
            overlappingItem = collision.gameObject;
        }
        else if (collision.CompareTag("Bubble"))
        {
            breathLeftUnderwater = maxUnderwaterBreath;
            collision.gameObject.SetActive(false);
            dof.focusDistance.value = defaultDOF;
        }

        if (showDebugs)
        {
            Debug.Log("Player Entered: " + collision.name);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Water") && moveState != MoveState.Water)
        {
            if (IsWater())
            {
                EnterWater();
            }
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
        Gizmos.DrawCube((Vector2) transform.position + groundedBoxOffset, groundedBoxSize);
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
