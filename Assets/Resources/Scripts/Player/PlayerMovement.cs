using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    private CameraFollow cam;

    [SerializeField] private bool useDebugs = false;
    [SerializeField] private bool useCheats = false;

    public enum MoveState
    {
        Ground, Water, Wall
    }
    [HideInInspector] public MoveState moveState;

    public static Controls controls;

    private Animator anim;
    private Rigidbody2D rb2D;
    private SpriteRenderer sprtRndr;
    private PolygonCollider2D col2D;
    private LineRenderer lineRndr;
    private AudioSource audioSrc;

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
    [SerializeField] private UnityEngine.UI.Image breathMeter2;

    [Header("Throwing Settings")]
    [SerializeField] private float minThrowForce = 1f; // Minimum force applied when throwing an item.
    [SerializeField] private float maxThrowForce = 10f; // Maximum force applied when throwing an item.
    [SerializeField] private Vector2 throwVector = new Vector2(1f, 1.25f); // Maximum force applied when throwing an item.
    private float throwForce;

    [Header("Boxcast Settings")]
    [SerializeField] private Vector2 groundedBoxOffset = new Vector2(0f, -1.35f); // Offset for the grounded boxcast collision.
    [SerializeField] private Vector2 groundedBoxSize = new Vector2(0.75f, 0.05f); // Size for the grounded boxcast collision.
    [SerializeField] private Vector2 wallBoxSize = new Vector2(1f, 1f); // Size for the wall boxcast collision.
    [SerializeField] private Vector2 waterBoxSize = new Vector2(0.1f, 0.1f); // Size for the water boxcast collision.

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem walkRun; // Particles for when the player walks.
    private ParticleSystem.EmissionModule walkRunParticles;
    [SerializeField] private ParticleSystem jumpLandParticles; // Particles for when the player jumps.
    [SerializeField] private ParticleSystem breathing; // Particles for when the player is breathing and not underwater.
    private ParticleSystem.EmissionModule breathingParticles;
    [SerializeField] private ParticleSystem underwater; // Particles for when the player is breathing underwater.
    private ParticleSystem.EmissionModule underwaterParticles;
    [SerializeField] private ParticleSystem waterSplashParticles; // Particles for when the player jumps.

    private UnityEngine.Rendering.VolumeProfile vol;
    private UnityEngine.Rendering.Universal.Vignette vignette;
    private UnityEngine.Rendering.Universal.ColorAdjustments colAdj;
    private UnityEngine.Rendering.Universal.DepthOfField dof;

    [Header("Volume Settings")]
    [SerializeField] private float defaultVignette = 0.2f;
    [SerializeField] private Color defaultColAdj = new Color(1f, 1f, 1f);
    [SerializeField] private Color underwaterColorAdj = new Color(0.8f, 0.8f, 1f);
    [SerializeField] private Color nightVisionColorAdj = new Color(0f, 1f, 1f);
    [SerializeField] private float nightVisionExposure = 7f;
    [SerializeField] private float defaultDOF = 4f;

    private float targetVignette;
    private Color targetColAdj;
    private float targetExposure;
    private float targetDOF;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip[] footstep = new AudioClip[3];
    [SerializeField] public AudioClip[] landOnGround = new AudioClip[3];
    [SerializeField] private AudioClip[] waterSplash = new AudioClip[3];
    [SerializeField] private AudioClip[] swim = new AudioClip[3];

    [Header("Miscellaneous")]
    public Vector2 carrySpot = new Vector2(0.25f, 0f);

    public static byte abilities = 0b_0000_0000; // Byte value representing unlocked abilities that the player has.
    public static readonly byte wallClimb = 0b_0000_0001; // Byte value representing unlocked abilities that the player has. 1.
    public static readonly byte nightVision = 0b_0000_0010; // Byte value representing unlocked abilities that the player has. 2.
    public static readonly byte longerUnderwater = 0b_0000_0100; // Byte value representing unlocked abilities that the player has. 4.

    public float maxYVelocity = 15f;
    [HideInInspector] public float defaultMaxYVelocity;

    public UnityEngine.UI.Image fade; // Dark overlay that appears when respawning.
    private Animator fadeAnim;
    [SerializeField] private int throwVecResolution = 5;

    [HideInInspector] public GameObject overlappingItem;
    [HideInInspector] public Pickup heldItem;

    private float aboveGroundGravity;

    private float currentSpeed;
    private float currentTurnAroundSpeed;

    private Vector2 moveVel;
    private float jumpVel;
    private Vector2 pounceVel;
    [HideInInspector] public Vector2 outsideVel;

    private Vector2 targetVel;

    [HideInInspector] public float iceSlipperiness = 1f;
    [SerializeField] private float iceSlip = 0.2f;
    private float jumpLeft;
    private float throwLeft;
    private bool nextToWall;
    private bool isPouncing;
    private bool isSliding;
    private bool playerIsCrouching;
    private bool playerIsLookingUp;
    [HideInInspector] public float breathLeftUnderwater;
    [SerializeField] private LayerMask itemMask = 1 << 11;

    public static bool isInDarkZone = false;

    [SerializeField] private ParticleSystem[] snowfall = new ParticleSystem[3];
    private ParticleSystem.EmissionModule[] snowfallParticles = new ParticleSystem.EmissionModule[3];

    // Respawning.
    public static Vector2 lastPosBeforeSwimOrPit; // The player's last grounded position before going in water or falling in a bottomless pit.
    public static Vector2 lastPosBeforeCaughtByEnemy; // The player's last position before an enemy spotted them and killed them.
    public static Vector2 lastPosBeforeFoodRunsOut; // The player's last position during the beginning of the day before their food ran out later that day.
    public static Vector2 lastPosBeforeThorns; // The player's last position before touching thorns.

    public static int deathCause; // 0 represents swimming or falling in a bottomless pit, 1 represents being caughtByAnEnemy, 2 represents running out of food.

    public static bool lockMovement = false;

    private GameObject[] thorns;

    private Vector2[] colPoints = new Vector2[8];
    private Vector2[] colCrouchingPoints = new Vector2[8];

    public static uint deathCount = 0;

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
        lineRndr = GetComponent<LineRenderer>();
        audioSrc = GetComponent<AudioSource>();

        vol = Camera.main.GetComponent<UnityEngine.Rendering.Volume>().profile;
        vol.TryGet(out vignette);
        vol.TryGet(out colAdj);
        vol.TryGet(out dof);

        defaultVignette = vignette.intensity.value;
        defaultColAdj = colAdj.colorFilter.value;
        defaultDOF = dof.focusDistance.value;

        targetVignette = defaultVignette;
        targetColAdj = defaultColAdj;
        targetDOF = defaultDOF;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            dof.active = false;
        }

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

        controls.UI.Select.performed += _ => LoadTutorial.Disable();
        controls.UI.Pause.performed += _ => UIController.TryPause();

        fadeAnim = fade.GetComponent<Animator>();

        colPoints = col2D.points;
        
        for (int i = 0; i < colCrouchingPoints.Length; ++i)
        {
            colCrouchingPoints[i] = colPoints[i] / new Vector2(0.5f, 2f) - new Vector2(0.25f, 0.6f);
        }

        thorns = GameObject.FindGameObjectsWithTag("Thorns");
        
        defaultMaxYVelocity = maxYVelocity;
        aboveGroundGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        bool isGrounded = IsGrounded();

        Crouch(controls.Player.Crouch.ReadValue<float>() == 1f, isGrounded);
        LookUp(controls.Player.LookUp.ReadValue<float>() == 1f, isGrounded);
        Throw(controls.Player.Throw.ReadValue<float>() == 1f);
        Move(controls.Player.Move.ReadValue<Vector2>(), isGrounded);
        Run(controls.Player.Run.ReadValue<float>() == 1f, isGrounded);

        // Slows the pounce down once the player hits the ground.
        if (isPouncing && isGrounded || rb2D.velocity == Vector2.zero || pounceVel.y > 0f)
        {
            SlowPounce(isGrounded);
        }
        pounceTrail.emitting = isPouncing;

        // Applies coyote time.
        if (isGrounded && jumpVel == 0f)
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
            MusicPlayer.SetVolume(0, Mathf.Lerp(MusicPlayer.volume[0], 0f, Time.deltaTime * 2f) * GameController.musicScalar);
            MusicPlayer.SetVolume(1, Mathf.Lerp(MusicPlayer.volume[1], GameController.musicVolume2, Time.deltaTime * 2f) * GameController.musicScalar);

            if (Settings.useParticles)
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
            MusicPlayer.SetVolume(0, Mathf.Lerp(MusicPlayer.volume[0], GameController.musicVolume, Time.deltaTime * 2f) * GameController.musicScalar);
            MusicPlayer.SetVolume(1, Mathf.Lerp(MusicPlayer.volume[1], 0f, Time.deltaTime * 2f) * GameController.musicScalar);

            if (Settings.useParticles)
                breathingParticles.rateOverTime = 100f;

            underwaterParticles.rateOverTime = 0f;

            if (isGrounded)
            {
                lastPosBeforeSwimOrPit = transform.position;
            }

            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, targetVignette, Time.deltaTime);
            dof.focusDistance.value = Mathf.Lerp(dof.focusDistance.value, targetDOF, Time.deltaTime);
        }
        breathMeter.fillAmount = breathLeftUnderwater / breath;
        colAdj.colorFilter.value = Color.Lerp(colAdj.colorFilter.value, targetColAdj, Time.deltaTime * 5f);

        #endregion

        #region Night Vision

        if (UIController.timeTitle == "night" || isInDarkZone)
        {
            ActivateNightVision();
        }
        else
        {
            DeactivateNightVision();
        }
        colAdj.postExposure.value = Mathf.Lerp(colAdj.postExposure.value, targetExposure, Time.deltaTime * 2.5f);

        #endregion

        // Check to see if there are any thorns in the player's camera.
        if (isGrounded && !AreThornsPresent())
        {
            lastPosBeforeThorns = transform.position;
        }
        
        // TODO: Add respawn position for when food runs out.

        // Caps swimming speed.
        if (moveState == MoveState.Water)
        {
            rb2D.velocity = rb2D.velocity.y <= swimMaxVelocity ? rb2D.velocity : new Vector2(0f, swimMaxVelocity);
        }

        if (outsideVel.y > 0.5f)
        {
            maxYVelocity = Mathf.Lerp(maxYVelocity, outsideVel.y * 1.5f, Time.deltaTime * 5f);
        }
        else
        {
            maxYVelocity = Mathf.Lerp(maxYVelocity, defaultMaxYVelocity, Time.deltaTime);
        }

        anim.SetFloat("Y Vel", rb2D.velocity.y);

        #region Debug

        if (useCheats)
        {
            abilities = 0b_0000_0111;

            breathLeftUnderwater = maxUnderwaterBreath;
        }

        #endregion
    }

    // Physics calculations and movement setting.
    private void FixedUpdate()
    {
        Jump(controls.Player.Jump.ReadValue<float>() == 1f);

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
        anim.SetFloat("Move Speed", Mathf.Abs(targetVel.x) / 13f);
        targetVel.y += jumpVel;
        targetVel += pounceVel;

        if (targetVel.x != 0f && moveState != MoveState.Wall)
        {
            sprtRndr.flipX = targetVel.x < 0f;
        }

        targetVel += outsideVel;

        if (isSliding)
        {
            targetVel = new Vector2(0f, targetVel.y - 5f);
        }

        if (targetVel.y > maxYVelocity)
        {
            targetVel.y = maxYVelocity;
        }

        rb2D.velocity = targetVel;
    }

    private void Move(Vector2 moveVal, bool isGrounded)
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
                    anim.SetFloat("Climbing Vel", moveVel.magnitude);
                }
                else
                {
                    moveVel = Vector2.zero;
                    anim.SetFloat("Climbing Vel", 0f);
                }
                break;
            // Walking/Running/Swimming.
            default:
                moveVel = Vector2.Lerp(moveVel, new Vector2(moveVal.x, 0f), Time.deltaTime * currentTurnAroundSpeed * iceSlipperiness);
                break;
        }

        // Zeroes out movement.
        if (moveVel.magnitude < 0.01f)
        {
            moveVel = Vector2.zero;
        }

        // Displays walking/running particles.
        if (moveVal.x != 0f && isGrounded && moveState == MoveState.Ground)
        {
            if (Settings.useParticles)
                walkRunParticles.rateOverTime = 25f;
        }
        else
        {
            walkRunParticles.rateOverTime = 0f;
        }

        anim.SetFloat("Move Vel", moveVel.x);

        if (useDebugs)
        {
            Debug.Log("Player Movement" + moveVel);
        }
    }

    private void Run(bool isRunning, bool isGrounded)
    {
        // Doesn't let the player run under these circumstances.
        if ((!isGrounded && moveState == MoveState.Ground) || playerIsLookingUp || lockMovement || playerIsCrouching)
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

                if (Mathf.Abs(moveVel.x) > 0.1f)
                {
                    PlaySound(footstep, false, Mathf.Clamp(anim.GetCurrentAnimatorStateInfo(0).speedMultiplier, 0.5f, 1f));
                }
                break;
        }

        if (heldItem != null)
        {
            currentSpeed /= heldItem.weight;
        }

        anim.SetBool("Is Running", isRunning);

        if (useDebugs)
        {
            Debug.Log("Player Speed: " + currentSpeed);
        }
    }

    private void Jump(bool isJumping)
    {
        // Doesn't allow the player to jump underwater.
        if (moveState == MoveState.Water || lockMovement || playerIsCrouching || LoadTutorial.IsActive() || UIController.IsActive())
            return;

        // Jump button is held.
        if (isJumping)
        {
            // Lets player jump off wall.
            if (moveState == MoveState.Wall)
            {
                EndClimb();

                jumpVel = jumpForce;
                moveVel = new Vector2(rb2D.velocity.x / 5f, moveVel.y);
                jumpLeft = extraJumpForce;
            }
            // Jump while grounded.
            else if (timeSinceGround < coyoteTime && rb2D.gravityScale != 0f && !playerIsLookingUp)
            {
                if (Settings.useParticles)
                    jumpLandParticles.Play();

                PlaySound(landOnGround, true, 1f, 0.5f);
                anim.SetTrigger("Jump");

                if (isPouncing)
                {
                    pounceVel = Vector2.zero;
                    isPouncing = false;

                    jumpVel = jumpForce;
                    moveVel = new Vector2(sprtRndr.flipX ? -1.5f : 1.5f, 0f);
                }
                else
                {
                    jumpVel = jumpForce;
                }

                jumpLeft = extraJumpForce;
            }
            // Apply extra momentum while Jump button is held.
            else if (jumpLeft > 0f)
            {
                jumpVel = jumpLeft;

                if ((jumpLeft -= Time.deltaTime * jumpDecreaseTime) < 0f)
                {
                    jumpLeft = 0f;
                }
            }

            timeSinceGround = coyoteTime;
            if (useDebugs)
            {
                Debug.Log("Player Is Jumping");
            }
        }
        // Jump button is not held.
        else
        {
            jumpLeft = 0f;

            if (useDebugs)
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
            jumpVel = 0f;
        }
    }

    private void Crouch(bool isCrouching, bool isGrounded)
    {
        // Only allows the player to crouch when walking or still.
        if (moveState != MoveState.Ground || !isGrounded || currentSpeed > walkSpeed || lockMovement || heldItem != null)
            return;

        anim.SetBool("Is Crouching", isCrouching);

        playerIsCrouching = isCrouching;

        // Applies proper speed.
        if (isCrouching)
        {
            currentSpeed = crawlSpeed;
            currentTurnAroundSpeed = crawlTurnAroundSpeed;

            col2D.points = colCrouchingPoints;
            groundedBoxSize = new Vector2(1.75f, 0.05f);

            if (useDebugs)
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

    private void LookUp(bool isLookingUp, bool isGrounded)
    {
        // Only allows player to look up while standing still and grounded.
        if (moveState != MoveState.Ground || !isGrounded || Mathf.Abs(moveVel.x) > 0.1f || lockMovement || UIController.isPaused)
            return;

        anim.SetBool("Is Looking Up", isLookingUp);
        playerIsLookingUp = isLookingUp;

        // Applies proper speed and camera offset.
        if (isLookingUp)
        {
            currentSpeed = 0f;
            currentTurnAroundSpeed = 20f;
            cam.SetOffset(CameraFollow.lookUpOffset);

            if (useDebugs)
            {
                Debug.Log("Player Is Looking Up");
            }
        }
        else
        {
            cam.SetOffset(CameraFollow.defaultOffset);
        }
    }

    private void Swim()
    {
        // Only allows you to swim underwater.
        if (moveState != MoveState.Water || lockMovement)
            return;

        anim.SetTrigger("Jump");
        PlaySound(swim, true, 1f, 0.5f);
        rb2D.AddForce(new Vector2(0f, swimForce), ForceMode2D.Impulse);

        if (useDebugs)
        {
            Debug.Log("Player Is Swimming");
        }
    }

    private void Pounce()
    {
        // Doesn't let the player pounce unless they're in the air.
        if (IsGrounded() || moveState != MoveState.Ground || isPouncing || jumpLeft != 0f || heldItem != null || lockMovement)
            return;

        moveVel = Vector2.zero;
        pounceVel = Vector2.Scale(new Vector2(sprtRndr.flipX ? -1f : 1f, 1f), pounceForce);
        isPouncing = true;

        anim.SetBool("Is Pouncing", true);
        anim.SetTrigger("Pounce");

        if (useDebugs)
        {
            Debug.Log("Player Pounced");
        }
    }

    private void SlowPounce(bool isGrounded)
    {
        // Stops slowing pounce once a certain speed is hit.
        if (Mathf.Abs(pounceVel.x) <= 0.01f)
        {
            pounceVel = Vector2.zero;
            isPouncing = false;
            anim.SetBool("Is Pouncing", false);

            return;
        }

        // Displays particles.
        if (isGrounded)
        {
            if (Settings.useParticles)
                walkRunParticles.rateOverTime = 25f;
        }

        pounceVel = Vector2.Lerp(pounceVel, Vector2.zero, Time.deltaTime * 5f);
        rb2D.gravityScale = moveState == MoveState.Ground ? aboveGroundGravity : underwaterGravity;

        if (useDebugs)
        {
            Debug.Log("Player Slowing Pounce");
        }
    }

    public void BeginClimb(bool skipCheck = false)
    {
        // Only allows player to climb wall when ability is unlocked.
        if ((!nextToWall || moveState != MoveState.Ground || (abilities & wallClimb) == 0 || lockMovement || isPouncing || heldItem != null) && !skipCheck) // Make it so you need the ability for it to work.
            return;

        moveState = MoveState.Wall;
        rb2D.gravityScale = 0f;

        // Cancels other velocity.
        jumpVel = 0f;
        pounceVel = Vector2.zero;
        moveVel = Vector2.zero;

        anim.SetBool("Is Climbing", true);

        if (useDebugs)
        {
            Debug.Log("Player Began Climbing");
        }
    }

    private void EndClimb()
    {
        moveState = MoveState.Ground;
        rb2D.gravityScale = aboveGroundGravity;

        // Cancels other velocity.
        jumpVel = 0f;
        moveVel = Vector2.zero;
        pounceVel = Vector2.zero;

        anim.SetBool("Is Climbing", false);

        if (useDebugs)
        {
            Debug.Log("Player Ended Climbing");
        }
    }

    private void Use()
    {
        if (lockMovement || heldItem != null || !IsGrounded())
            return;

        GameObject item = null;
        if (IsItemOverlap(ref item))
        {
            overlappingItem = item;

            // Pickup Item.
            if (overlappingItem.CompareTag("Pickup"))
            {
                if (useDebugs)
                {
                    Debug.Log("Player Picked Up An Item");
                }

                Pickup itemScript = overlappingItem.GetComponent<Pickup>();

                itemScript.Grab(gameObject);
                heldItem = itemScript;
                throwLeft = 0f;
            }
            // Interact with Item.
            else if (overlappingItem.CompareTag("Interactable"))
            {
                overlappingItem.GetComponent<Interactable>().Effect();
            }
        }
    }

    private void Throw(bool isThrowing)
    {
        if (heldItem == null || heldItem.timeSinceGrabbed < 0.5f)
            return;

        if (isThrowing)
        {
            throwLeft += Time.deltaTime;
            throwForce = Mathf.Lerp(minThrowForce, maxThrowForce, throwLeft);

            if (moveState != MoveState.Water)
            {
                RenderThrowingArc(throwVector, throwForce / 1.925f / heldItem.weight, throwVecResolution);
            }
            else
            {
                RenderThrowingArc(throwVector, throwForce / (1.925f * 2f) / heldItem.weight, throwVecResolution);
            }
        }
        else
        {
            if (throwLeft > 0f)
            {
                // Throw the item.
                heldItem.GetComponent<Pickup>().Throw(Vector2.Scale(new Vector2(sprtRndr.flipX ? -1f : 1f, 1f), throwVector * throwForce));
                heldItem = null;
                overlappingItem = null;

                throwLeft = 0f;
                RenderThrowingArc(Vector2.zero, 0f, 0);
            }
        }

        if (useDebugs)
        {
            Debug.Log("Player Threw An Item");
        }
    }

    private void RenderThrowingArc(Vector2 throwVec, float velocity, int resolution)
    {
        lineRndr.positionCount = resolution;

        float angle = Mathf.Atan2(throwVec.y, throwVec.x);
        float maxDistance =  (velocity * velocity * Mathf.Sin(2 * angle)) / aboveGroundGravity;

        lineRndr.SetPositions(CalculateThrowingPositions(resolution, maxDistance, angle, velocity));
    }

    private Vector3[] CalculateThrowingPositions(int resolution, float maxDistance, float angle, float velocity)
    {
        Vector3[] positions = new Vector3[resolution];

        for (int i = 0; i < resolution; ++i)
        {
            float t = (float) i / (float) resolution;
            positions[i] = CalculateArcPoint(t, maxDistance, angle, velocity);
        }

        return positions;
    }

    private Vector2 CalculateArcPoint(float t, float maxDistance, float angle, float velocity)
    {
        float x = t * maxDistance;
        float y = x * Mathf.Tan(angle) - (aboveGroundGravity * x * x / (2 * velocity * velocity * Mathf.Cos(angle) * Mathf.Cos(angle)));

        return new Vector2(x * (sprtRndr.flipX ? -1f : 1f), y) + heldItem.offset * (sprtRndr.flipX ? -1f : 1f);
    }

    public void EnterWater()
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
        targetColAdj = underwaterColorAdj;
        targetDOF = defaultDOF;

        breathMeter.gameObject.SetActive(false);
        breathMeter.gameObject.SetActive(true);
        breathMeter2.gameObject.SetActive(false);
        breathMeter2.gameObject.SetActive(true);

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
            if (Settings.useParticles)
                snowfallParticles[i].rateOverTime = 5f;
        }

        currentTurnAroundSpeed = aerialTurnAroundSpeed;

        // Sets volume.
        targetVignette = defaultVignette;
        targetColAdj = defaultColAdj;
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
        breathMeter2.GetComponent<Animator>().SetTrigger("Exit");
        anim.SetBool("Is Underwater", false);
    }

    private void ActivateNightVision()
    {
        // Checks to make sure ability can be used.
        if ((abilities & nightVision) == 0)
            return;

        targetExposure = nightVisionExposure;
        targetColAdj = nightVisionColorAdj;
    }

    private void DeactivateNightVision()
    {
        // Checks to make sure ability can be used.
        if ((abilities & nightVision) == 0)
            return;

        targetExposure = 0f;
        targetColAdj = moveState == MoveState.Water ? underwaterColorAdj : defaultColAdj;
    }

    public static void UnlockAbility(byte ability)
    {
        abilities |= ability;
    }

    public void PlaySound(AudioClip[] sound, bool interuptSound = false, float pitch = 1f, float volume = 1f)
    {
        if (audioSrc.isPlaying && !interuptSound)
            return;

        audioSrc.clip = sound[Random.Range(0, sound.Length)];
        audioSrc.pitch = pitch;
        audioSrc.volume = volume;
        audioSrc.Play();
    }

    // Checks if the player is grounded or not. Separate from Jump Coyote Time.
    private bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + groundedBoxOffset, groundedBoxSize, 0f, Vector2.down, 0f, ground);
        bool isGrounded = hit;

        if (useDebugs)
        {
            Debug.Log("Grounded: " + isGrounded);
        }

        anim.SetBool("Grounded", isGrounded);

        if (isGrounded)
        {
            anim.ResetTrigger("Jump");
        }

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

        if (useDebugs)
        {
            Debug.Log("Wall: " + isWall);
        }

        return isWall;
    }

    // Checks if there is an item overlapping with the player.
    private bool IsItemOverlap(ref GameObject item)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, transform.localScale, 0f, Vector2.down, 0f, itemMask);
        bool isItem = hit;

        if (useDebugs)
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
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, waterBoxSize, 0f, Vector2.down, 0f, water);
        bool isWater = hit;

        if (useDebugs)
        {
            Debug.Log("Water Detected");
        }

        return isWater;
    }

    private bool AreThornsPresent()
    {
        for (int i = 0; i < thorns.Length; ++i)
        {
            Vector2 camView = Camera.main.WorldToViewportPoint(thorns[i].transform.position);

            if (camView.x > -0.15f && camView.x < 1.15f && camView.y > -0.15f && camView.y < 1.15f)
                return true;
        }

        return false;
    }

    public void Die(int death)
    {
        if (fade.gameObject.activeSelf)
            return;

        deathCount++;
        lockMovement = true;
        anim.SetTrigger("Death");
        deathCause = death;
        moveVel = Vector2.zero;
        jumpVel = 0f;
        pounceVel = Vector2.zero;

        fade.gameObject.SetActive(true);
        fadeAnim.SetTrigger("Death");

        if (useDebugs)
        {
            Debug.Log("Player Died");
        }
    }

    public void Respawn()
    {
        fadeAnim.ResetTrigger("Death");

        switch (deathCause)
        {
            case 0:
                transform.position = lastPosBeforeSwimOrPit;
                break;
            case 1:
                transform.position = lastPosBeforeCaughtByEnemy;
                break;
            case 2:
                transform.position = lastPosBeforeFoodRunsOut;
                break;
            default:
                transform.position = lastPosBeforeThorns;
                break;
        }

        cam.gameObject.transform.position = transform.position + cam.GetOffset();

        anim.enabled = false;
        anim.enabled = true;

        ExitWater(false);
    }

    // Makes you restart the day if the tribe runs out of food.
    public void GameOver()
    {
        anim.SetTrigger("Death");
    }

    public static void Freeze()
    {
        lockMovement = true;

        GameController.player.moveVel = Vector2.zero;
        GameController.player.jumpVel = 0f;
        GameController.player.pounceVel = Vector2.zero;
    }

    public static void UnFreeze()
    {
        lockMovement = false;

        GameController.player.moveVel = Vector2.zero;
        GameController.player.jumpVel = 0f;
        GameController.player.pounceVel = Vector2.zero;
    }

    public void StartCutscene(GameObject target)
    {
        targetVignette = 0.3f;
        cam.followObj = target.transform;
        cam.SetOffset(new Vector3(0f, 0f, -9.5f));
    }

    public void EndCutscene()
    {
        targetVignette = defaultVignette;
        cam.followObj = transform;
        cam.SetOffset(new Vector3(0f, 0f, -10f));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            nextToWall = true;
        }
        else if (collision.CompareTag("Water"))
        {
            if (Settings.useParticles)
                waterSplashParticles.Play();

            PlaySound(waterSplash, true);
        }
        else if (collision.CompareTag("Thorns"))
        {
            Die(3);
        }
        else if (collision.CompareTag("Pickup"))
        {
            overlappingItem = collision.gameObject;
        }

        if (useDebugs)
        {
            Debug.Log("Player Entered: " + collision.name);
        }
    }

    public void RestoreBreath()
    {
        breathLeftUnderwater = (abilities & longerUnderwater) == 0 ? maxUnderwaterBreath : newMaxUnderwaterBreath;
        dof.focusDistance.value = defaultDOF;
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
            if (Settings.useParticles)
                waterSplashParticles.Play();

            PlaySound(waterSplash, true);
        }
        else if (collision.CompareTag("Pickup"))
        {
            overlappingItem = null;
        }

        if (useDebugs)
        {
            Debug.Log("Player Exited: " + collision.name);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            iceSlipperiness = iceSlip;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        isSliding = targetVel.x != 0f && Mathf.Abs(rb2D.velocity.magnitude) < 0.1f && !IsGrounded();

        if (useDebugs && isSliding)
        {
            Debug.Log("Player is Sliding");
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        isSliding = false;
        iceSlipperiness = 1f;
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
