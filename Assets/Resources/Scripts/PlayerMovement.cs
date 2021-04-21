using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    private CameraFollow cam;

    public enum MoveState
    {
        Ground, Water, Wall
    }
    private MoveState moveState;

    private Controls controls;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;
    private PolygonCollider2D col2D;

    // Volume Settings.
    private UnityEngine.Rendering.VolumeProfile vol;
    private UnityEngine.Rendering.Universal.Vignette vignette;
    private UnityEngine.Rendering.Universal.ColorAdjustments colAdj;
    private UnityEngine.Rendering.Universal.DepthOfField dof;

    private float defaultVignette;
    private Color defaultColAdj;
    private float defaultDOF;

    private float targetVignette;
    private float targetExposure;
    private float targetDOF;

    [Header("Movement Settings")]
    [SerializeField] private LayerMask ground = 1 << 9; // Layermask for the ground that will trigger a grounded collision.
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
    [SerializeField] private float coyoteTime; // Extra time after leaving the ground for jumping.
    [SerializeField] private Vector2 pounceForce; // Force added when the player pounces.
    [SerializeField] private TrailRenderer pounceTrail; // Trail renderer for the pounce move.
    private float timeSinceGround;

    [Header("Climbing Settings")]
    [SerializeField] private LayerMask wall = 1 << 10; // Layermask for the ground that the player can climb on.
    [SerializeField] private float climbSpeed = 6f; // Speed the player can climb at.
    [SerializeField] private float climbRunSpeed = 12f; // Speed the player can climb at.
    [SerializeField] private float climbTurnAroundSpeed = 10f; // Speed the player turns around while climbing.

    [Header("Swim Settings")]
    [SerializeField] private LayerMask water = 1 << 4; // Layermask for the water that the player can swim in.
    [SerializeField] private float swimSpeed = 6f; // Speed the player can swim at.
    [SerializeField] private float swimRunSpeed = 9f; // Speed the player can swim at while running underwater.
    [SerializeField] private float swimForce = 5f; // Force added when the player jumps while underwater.
    [SerializeField] private float swimTurnAroundSpeed = 2f; // Speed the player turns around while swimming.
    [SerializeField] private float underwaterGravity = 0.5f; // Gravity for underwater.
    [SerializeField] private float maxUnderwaterBreath = 10f; // How long (in seconds) you can breath underwater for.
    [SerializeField] private float newMaxUnderwaterBreath = 20f; // How long (in seconds) you can breath underwater for after upgrading.
    [SerializeField] private float swimMaxVelocity = 15f; // Maximum velocity you can achieve whilst going upwards underwater.
    [SerializeField] private UnityEngine.UI.Image breathMeter;

    [Header("Boxcast Settings")]
    [SerializeField] private Vector2 groundedBoxOffset = new Vector2(0f, -1.35f); // Offset for the grounded boxcast collision.
    [SerializeField] private Vector2 groundedBoxSize = new Vector2(0.75f, 0.05f); // Size for the grounded boxcast collision.
    [SerializeField] private Vector2 wallBoxSize = new Vector2(1f, 1f); // Size for the wall boxcast collision.

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem walkRun; // Particles for when the player walks.
    private ParticleSystem.EmissionModule walkRunParticles;
    [SerializeField] private ParticleSystem jumpLandParticles; // Particles for when the player jumps.
    [SerializeField] private ParticleSystem breathing; // Particles for when the player is breathing and not underwater.
    private ParticleSystem.EmissionModule breathingParticles;
    [SerializeField] private ParticleSystem underwater; // Particles for when the player is breathing underwater.
    private ParticleSystem.EmissionModule underwaterParticles;
    [SerializeField] private ParticleSystem waterSplashParticles; // Particles for when the player jumps.

    [Header("Miscellaneous")]
    [SerializeField] private bool showDebugs = false;
    [SerializeField] private byte abilities = 0b_0000_0000; // Byte value representing unlocked abilities that the player has.
    private readonly byte wallClimb = 0b_0000_0001; // Byte value representing unlocked abilities that the player has. 1.
    private readonly byte nightVision = 0b_0000_0010; // Byte value representing unlocked abilities that the player has. 2.
    private readonly byte longerUnderwater = 0b_0000_0100; // Byte value representing unlocked abilities that the player has. 4.
    public UnityEngine.UI.Image fade; // Dark overlay that appears when respawning.
    private Animator fadeAnim;

    [SerializeField] private Vector2 throwVector = new Vector2(10f, 10f); // Vector applied when throwing an item.

    [HideInInspector] public GameObject overlappingItem;
    [HideInInspector] public Pickup heldItem;

    private float aboveGroundGravity;

    private float currentSpeed;
    private float currentTurnAroundSpeed;

    private Vector2 moveVel;
    private Vector2 jumpVel;
    private Vector2 pounceVel;

    // Miscellaneous.
    private float jumpLeft;
    private bool nextToWall;
    private bool isPouncing;
    private bool playerIsLookingUp;
    private float breathLeftUnderwater;
    [SerializeField] private LayerMask itemMask = 1 << 11;

    [SerializeField] private ParticleSystem[] snowfall = new ParticleSystem[3];
    private ParticleSystem.EmissionModule[] snowfallParticles = new ParticleSystem.EmissionModule[3];

    // Respawning.
    public static Vector2 lastPosBeforeSwimOrPit; // The player's last grounded position before going in water or falling in a bottomless pit.
    public static Vector2 lastPosBeforeCaughtByEnemy; // The player's last position before an enemy spotted them and killed them.
    public static Vector2 lastPosBeforeFoodRunsOut; // The player's last position during the beginning of the day before their food ran out later that day.
    public static int deathCause; // 0 represents swimming or falling in a bottomless pit, 1 represents being caughtByAnEnemy, 2 represents running out of food.

    public bool lockMovement = false;

    private Vector2[] colPoints = new Vector2[8];
    private Vector2[] colCrouchingPoints = new Vector2[8];

    private void Awake()
    {
        cam = Camera.main.GetComponent<CameraFollow>();

        PlayerRespawn.player = this;

        controls = new Controls();
        currentSpeed = walkSpeed;
        currentTurnAroundSpeed = walkTurnAroundSpeed;

        anim = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        sprtRndr = GetComponent<SpriteRenderer>();
        col2D = GetComponent<PolygonCollider2D>();

        vol = Camera.main.GetComponent<UnityEngine.Rendering.Volume>().profile;
        vol.TryGet(out vignette);
        vol.TryGet(out colAdj);
        vol.TryGet(out dof);

        defaultVignette = vignette.intensity.value;
        defaultColAdj = colAdj.colorFilter.value;
        defaultDOF = dof.focusDistance.value;

        targetVignette = defaultVignette;
        targetDOF = defaultDOF;

        walkRunParticles = walkRun.emission;
        breathingParticles = breathing.emission;
        underwaterParticles = underwater.emission;
        
        for (int i = 0; i < snowfallParticles.Length; ++i)
        {
            snowfallParticles[i] = snowfall[i].emission;
        }

        controls.Player.BeginClimb.performed += _ => BeginClimb();
        controls.Player.Pounce.performed += _ => Pounce();
        controls.Player.Use.performed += _ => Use();
        controls.Player.Swim.performed += _ => Swim();

        fadeAnim = fade.GetComponent<Animator>();

        colPoints = col2D.points;
        
        for (int i = 0; i < colCrouchingPoints.Length; ++i)
        {
            colCrouchingPoints[i] = colPoints[i] / new Vector2(0.5f, 2f) - new Vector2(0.25f, 0.6f);
        }

        aboveGroundGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        // Slows the pounce down once the player hits the ground.
        if (isPouncing && IsGrounded() || rb2D.velocity == Vector2.zero)
        {
            SlowPounce();
        }
        pounceTrail.emitting = isPouncing;

        // Applies coyote time.
        if (IsGrounded() && jumpVel == Vector2.zero)
        {
            timeSinceGround = 0f;
        }
        else
        {
            timeSinceGround += Time.deltaTime;
        }

        #region Underwater Behaviour

        float breath = (abilities & longerUnderwater) == 0 ? maxUnderwaterBreath : newMaxUnderwaterBreath;
        if (moveState == MoveState.Water)
        {
            underwaterParticles.rateOverTime = 7.5f;
            breathingParticles.rateOverTime = 0f;

            vignette.intensity.value = 0.55f - (breathLeftUnderwater / breath) * 0.35f;

            if ((breathLeftUnderwater -= Time.deltaTime) <= 0f)
            {
                dof.focusDistance.value -= Time.deltaTime * 2f;

                if (vignette.intensity.value >= 0.625f)
                {
                    Die(0);
                }
            }
        }
        else
        {
            breathingParticles.rateOverTime = 100f;
            underwaterParticles.rateOverTime = 0f;

            if (IsGrounded())
            {
                lastPosBeforeSwimOrPit = transform.position;
            }

            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, Time.deltaTime);
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, targetDOF, Time.deltaTime);
        }
        breathMeter.fillAmount = breathLeftUnderwater / breath;

        #endregion

        #region Night Vision

        if (UIController.timeTitle == "dusk" || UIController.timeTitle == "night")
        {
            ActivateNightVision();
        }
        else
        {
            DeactivateNightVision();
        }
        colAdj.postExposure.value = Mathf.Lerp(colAdj.postExposure.value, targetExposure, Time.deltaTime * 2.5f);

        #endregion

        anim.SetFloat("Velocity Y", rb2D.velocity.y);
    }

    // Physics calculations and movement setting.
    private void FixedUpdate()
    {
        Move(controls.Player.Move.ReadValue<Vector2>());
        Run(controls.Player.Run.ReadValue<float>() == 1f);
        Jump(controls.Player.Jump.ReadValue<float>() == 1f);
        Crouch(controls.Player.Crouch.ReadValue<float>() == 1f);
        LookUp(controls.Player.LookUp.ReadValue<float>() == 1f);

        Vector2 targetVel;
        if (moveState != MoveState.Wall)
        {
            targetVel = new Vector2(0f, rb2D.velocity.y); // Sets targetVel to be determinant on the current Y velocity.
        }
        else
        {
            targetVel = Vector2.zero; // Sets targetVel to be nothing.
        }

        // Adds proper force to player.
        targetVel += moveVel * currentSpeed;
        targetVel += jumpVel;
        targetVel += pounceVel;

        if (targetVel.x != 0f)
        {
            sprtRndr.flipX = targetVel.x < 0f;
        }

        rb2D.velocity = targetVel;

        // Caps swimming speed.
        if (moveState == MoveState.Water)
        {
            rb2D.velocity = rb2D.velocity.y <= swimMaxVelocity ? rb2D.velocity : new Vector2(0f, swimMaxVelocity);
        }
    }

    private void Move(Vector2 moveVal)
    {
        // Locks movement while pouncing.
        if (Mathf.Abs(pounceVel.x) > 1f || lockMovement)
            return;

        isPouncing = false;
        pounceVel = Vector2.zero;

        switch (moveState)
        {
            // Climbing a wall.
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
            // Walking/Running/Swimming.
            default:
                moveVel = Vector2.Lerp(moveVel, new Vector2(moveVal.x, 0f), Time.deltaTime * currentTurnAroundSpeed);
                break;
        }

        // Zeroes out movement.
        if (moveVel.magnitude < 0.01f)
        {
            moveVel = Vector2.zero;
        }

        // Displays walking/running particles.
        if (moveVal.x != 0f && IsGrounded() && moveState == MoveState.Ground)
        {
            walkRunParticles.rateOverTime = 25f;
        }
        else
        {
            walkRunParticles.rateOverTime = 0f;
        }

        anim.SetFloat("Move X", moveVel.x);
        anim.SetFloat("Move Y", moveVel.y);

        if (showDebugs)
        {
            Debug.Log("Player Movement" + moveVel);
        }
    }

    private void Run(bool isRunning)
    {
        // Doesn't let the player run under these circumstances.
        if ((!IsGrounded() && moveState == MoveState.Ground) || playerIsLookingUp || lockMovement)
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

        if (heldItem != null)
        {
            currentSpeed /= heldItem.weight;
        }

        anim.SetBool("Is Running", isRunning);

        if (showDebugs)
        {
            Debug.Log("Player Speed: " + currentSpeed);
        }
    }

    private void Jump(bool isJumping)
    {
        // Doesn't allow the player to jump underwater.
        if (moveState == MoveState.Water || playerIsLookingUp || lockMovement)
            return;

        // Jump button is held.
        if (isJumping)
        {
            // Lets player jump off wall.
            if (moveState == MoveState.Wall)
            {
                EndClimb();

                jumpVel = new Vector2(0f, jumpForce);
                moveVel = new Vector2(rb2D.velocity.x / 5f, moveVel.y);
                jumpLeft = extraJumpForce;
            }
            // Jump while grounded.
            else if (timeSinceGround < coyoteTime)
            {
                jumpLandParticles.Play();
                pounceVel = new Vector2(pounceVel.x, 0f);

                jumpVel = new Vector2(0f, jumpForce);
                jumpLeft = extraJumpForce;
            }
            // Apply extra momentum while Jump button is held.
            else if (jumpLeft > 0f)
            {
                jumpVel = new Vector2(0f, jumpLeft);

                if ((jumpLeft -= Time.deltaTime * jumpDecreaseTime) < 0f)
                {
                    jumpLeft = 0f;
                }
            }

            timeSinceGround = coyoteTime;
            if (showDebugs)
            {
                Debug.Log("Player Is Jumping");
            }
        }
        // Jump button is not held.
        else
        {
            jumpLeft = 0f;

            if (showDebugs)
            {
                Debug.Log("Player Is Not Jumping");
            }
        }

        if (heldItem != null)
        {
            jumpVel /= heldItem.weight;
        }

        // Apply proper velocity.
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

    private void Crouch(bool isCrouching)
    {
        // Only allows the player to crouch when walking or still.
        if (moveState != MoveState.Ground || !IsGrounded() || currentSpeed > walkSpeed || lockMovement)
            return;

        anim.SetBool("Is Crouching", isCrouching);

        // Applies proper speed.
        if (isCrouching)
        {
            currentSpeed = crawlSpeed;
            currentTurnAroundSpeed = crawlTurnAroundSpeed;

            col2D.points = colCrouchingPoints;
            groundedBoxSize = new Vector2(1.75f, 0.05f);

            if (showDebugs)
            {
                Debug.Log("Player Is Crouching");
            }
        }
        else
        {
            col2D.points = colPoints;
            groundedBoxSize = new Vector2(0.75f, 0.05f);
        }
    }

    private void LookUp(bool isLookingUp)
    {
        // Only allows player to look up while standing still and grounded.
        if (moveState != MoveState.Ground || !IsGrounded() || Mathf.Abs(moveVel.x) > 0.1f || lockMovement)
            return;

        anim.SetBool("Is Looking Up", isLookingUp);
        playerIsLookingUp = isLookingUp;

        // Applies proper speed and camera offset.
        if (isLookingUp)
        {
            currentSpeed = 0f;
            currentTurnAroundSpeed = 20f;
            cam.offset = new Vector3(cam.offset.x, 2.5f, cam.offset.z);

            if (showDebugs)
            {
                Debug.Log("Player Is Looking Up");
            }
        }
        else
        {
            cam.offset = new Vector3(cam.offset.x, 0f, cam.offset.z);
        }
    }

    private void Swim()
    {
        // Only allows you to swim underwater.
        if (moveState != MoveState.Water || lockMovement)
            return;

        rb2D.AddForce(new Vector2(0f, swimForce), ForceMode2D.Impulse);

        if (showDebugs)
        {
            Debug.Log("Player Is Swimming");
        }
    }

    private void Pounce()
    {
        // Doesn't let the player pounce unless they're in the air.
        if (IsGrounded() || moveState != MoveState.Ground || isPouncing || jumpLeft != 0f || heldItem != null || lockMovement)
            return;

        pounceVel = Vector2.Scale(new Vector2(sprtRndr.flipX ? -1f : 1f, 1f), pounceForce);
        isPouncing = true;

        anim.SetTrigger("Pounce");

        if (showDebugs)
        {
            Debug.Log("Player Pounced");
        }
    }

    private void SlowPounce()
    {
        // Stops slowing pounce once a certain speed is hit.
        if (Mathf.Abs(pounceVel.x) <= 0.01f)
        {
            pounceVel = Vector2.zero;
            isPouncing = false;

            return;
        }

        // Displays particles.
        if (IsGrounded())
        {
            walkRunParticles.rateOverTime = 25f;
        }

        pounceVel = Vector2.Lerp(pounceVel, Vector2.zero, Time.deltaTime * 5f);
        rb2D.gravityScale = moveState == MoveState.Ground ? aboveGroundGravity : underwaterGravity;

        if (showDebugs)
        {
            Debug.Log("Player Slowing Pounce");
        }
    }

    private void BeginClimb()
    {
        // Only allows player to climb wall when ability is unlocked.
        if (!nextToWall || moveState != MoveState.Ground || (abilities & wallClimb) == 0 || lockMovement) // Make it so you need the ability for it to work.
            return;

        moveState = MoveState.Wall;
        rb2D.gravityScale = 0f;

        // Cancels other velocity.
        jumpVel = Vector2.zero;
        pounceVel = Vector2.zero;

        anim.SetBool("Is Climbing", true);

        if (showDebugs)
        {
            Debug.Log("Player Began Climbing");
        }
    }

    private void EndClimb()
    {
        moveState = MoveState.Ground;
        rb2D.gravityScale = aboveGroundGravity;

        // Cancels other velocity.
        moveVel = Vector2.zero;
        jumpVel = Vector2.zero;
        pounceVel = Vector2.zero;

        anim.SetBool("Is Climbing", false);

        if (showDebugs)
        {
            Debug.Log("Player Ended Climbing");
        }
    }

    private void Use()
    {
        if (lockMovement)
            return;

        // Pick up or interacts with an item.
        if (heldItem == null)
        {
            GameObject item = null;

            if (IsItemOverlap(ref item))
            {
                overlappingItem = item;

                // Pickup Item.
                if (overlappingItem.CompareTag("Pickup"))
                {
                    Pickup itemScript = overlappingItem.GetComponent<Pickup>();

                    itemScript.Grab(gameObject);
                    heldItem = itemScript;
                }
                // Interact with Item.
                else if (overlappingItem.CompareTag("Interactable"))
                {
                    overlappingItem.GetComponent<Interactable>().effect.Invoke();
                }
            }
        }
        // Throws the currently held item.
        else
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

        for (int i = 0; i < snowfallParticles.Length; ++i)
        {
            snowfallParticles[i].rateOverTime = 0f;
        }

        currentSpeed = swimSpeed;
        currentTurnAroundSpeed = swimTurnAroundSpeed;

        // Sets volume.
        targetVignette = defaultVignette;
        colAdj.colorFilter.value = new Color(0.8f, 0.8f, 1f);
        targetDOF = defaultDOF;

        breathMeter.gameObject.SetActive(false);
        breathMeter.gameObject.SetActive(true);

        pounceVel = Vector2.zero;
        rb2D.velocity = new Vector2(rb2D.velocity.x, rb2D.velocity.y / 10f);

        // Sets gravity.
        rb2D.gravityScale = underwaterGravity;
        breathLeftUnderwater = (abilities & longerUnderwater) == 0 ? maxUnderwaterBreath: newMaxUnderwaterBreath;

        anim.SetBool("Is Underwater", true);
    }

    private void ExitWater(bool useJump = true)
    {
        moveState = MoveState.Ground;

        for (int i = 0; i < snowfallParticles.Length; ++i)
        {
            snowfallParticles[i].rateOverTime = 5f;
        }

        currentTurnAroundSpeed = aerialTurnAroundSpeed;

        // Sets volume.
        targetVignette = defaultVignette;
        colAdj.colorFilter.value = defaultColAdj;
        targetDOF = defaultDOF;

        if (useJump)
        {
            rb2D.velocity = new Vector2(rb2D.velocity.x, 0f);
            rb2D.AddForce(new Vector2(moveVel.x * currentSpeed, 15f), ForceMode2D.Impulse);
        }

        // Sets gravity.
        rb2D.gravityScale = aboveGroundGravity;
        breathLeftUnderwater = (abilities & longerUnderwater) == 0 ? maxUnderwaterBreath : newMaxUnderwaterBreath;

        breathMeter.GetComponent<Animator>().SetTrigger("Exit");
        anim.SetBool("Is Underwater", false);
    }

    private void ActivateNightVision()
    {
        // Checks to make sure ability can be used.
        if ((abilities & nightVision) == 0)
            return;

        targetExposure = 5f;
    }

    private void DeactivateNightVision()
    {
        // Checks to make sure ability can be used.
        if ((abilities & nightVision) == 0)
            return;

        targetExposure = 0f;
    }

    public void UnlockAbility(byte ability)
    {
        abilities |= ability;
    }

    // Checks if the player is grounded or not. Separate from Jump Coyote Time.
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + groundedBoxOffset, groundedBoxSize, 0f, Vector2.down, 0f, ground);
        bool isGrounded = hit;

        if (showDebugs)
        {
            Debug.Log("Grounded: " + isGrounded);
        }

        anim.SetBool("Is Grounded", isGrounded);

        if (!isGrounded && moveState == MoveState.Ground)
        {
            currentTurnAroundSpeed = aerialTurnAroundSpeed;
        }
        return isGrounded;
    }

    // Checks if there is a climbable wall in any direction of the player. Will prevent the player from climbing in that direction if there isn't.
    private bool IsBackgroundWall(Vector2 dir)
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + dir, wallBoxSize, 0f, Vector2.zero, 0f, wall);
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

    // Checks if the player is mostly submerged in water.
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

    public void Die(int death)
    {
        lockMovement = true;
        anim.SetTrigger("Death");
        deathCause = death;
        moveVel = Vector2.zero;
        jumpVel = Vector2.zero;
        pounceVel = Vector2.zero;

        fade.gameObject.SetActive(true);
        fadeAnim.SetTrigger("Death");

        if (showDebugs)
        {
            Debug.Log("Player Died");
        }
    }

    public void Respawn()
    {
        lockMovement = false;
        fadeAnim.ResetTrigger("Death");

        switch (deathCause)
        {
            case 0:
                transform.position = lastPosBeforeSwimOrPit;
                break;
            case 1:
                transform.position = lastPosBeforeCaughtByEnemy;
                break;
            default:
                transform.position = lastPosBeforeFoodRunsOut;
                break;
        }

        cam.gameObject.transform.position = transform.position + cam.offset;

        ExitWater(false);
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
        else if (collision.CompareTag("Water"))
        {
            waterSplashParticles.Play();
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
        if (collision.CompareTag("Water"))
        {
            // Enter water when submerged
            if (moveState != MoveState.Water && IsWater())
            {
                EnterWater();
            }
            // Exit water when unsubmerged.
            else if (moveState == MoveState.Water && !IsWater())
            {
                ExitWater();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            nextToWall = false;
        }
        else if (collision.CompareTag("Water"))
        {
            waterSplashParticles.Play();
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
        // Grounded boxcast.
        Gizmos.DrawCube((Vector2) transform.position + groundedBoxOffset, groundedBoxSize);
    }

    // For Input System.
    private void OnEnable()
    {
        controls.Enable();
    }

    // For Input System.
    private void OnDisable()
    {
        controls.Disable();
    }
}
