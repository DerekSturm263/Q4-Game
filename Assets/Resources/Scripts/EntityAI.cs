using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public abstract class EntityAI : MonoBehaviour
{
    public static List<GameObject> entities = new List<GameObject>();

    [SerializeField] protected bool showDebugs;

    private Camera cam;
    private PlayerMovement playerMov;
    protected Transform player;

    [Header("Layer Mask Settings")]
    [SerializeField] protected LayerMask ground = 1 << 9;
    [SerializeField] protected LayerMask playerMask = 1 << 12;
    [SerializeField] protected LayerMask notEnemy = ~(1 << 13 & 1 << 8 & 1 << 10);

    protected Rigidbody2D rb2D;
    protected Animator anim;
    protected SpriteRenderer sprtRndr;

    [SerializeField] private float underwaterGravity = 0.5f;
    private float aboveWaterGravity;

    [Header("Target Settings")]
    [SerializeField] private bool isHostile = true;
    [HideInInspector] public bool isActive = false;

    [SerializeField] protected float viewDist = 10f; // How far the enemy can see (in units). This affects the enemy's ability to see both the player, other enemies, and items.
    protected Transform target; // Current transform that they enemy is targeting.
    
    [SerializeField] private List<Transform> likeableObjects = new List<Transform>(); // Transforms that the enemy will be drawn towards. Sort in order of priority.

    [Header("Speed Settings")]
    [SerializeField] protected float chaseSpeed = 3f; // Speed to chase at.
    [SerializeField] protected float wanderTurnAroundSpeed = 5f; // Speed to turn around at while wandering.
    [SerializeField] protected float chaseTurnAroundSpeed = 2.5f; // Speed to turn around at while chasing.

    [SerializeField] protected float chaseWaitTime = 1f; // Amount of time after chasing to make the enemy relaxed.
    protected float chaseTime;

    private Vector2 lastPos;
    protected Vector2 ogPos;

    protected float currentSpeed;
    protected Vector2 moveVel;

    [HideInInspector] public bool isSatisfied = false; // Sets to true if the enemy grabs an object that they like.

    private GameObject warningSignal;
    private GameObject interestedSignal;

    protected virtual void Awake()
    {
        entities.Add(gameObject);
        playerMov = FindObjectOfType<PlayerMovement>();
        player = playerMov.transform;
        ogPos = transform.position;

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprtRndr = GetComponent<SpriteRenderer>();

        lastPos = transform.position;
        PlayerMovement.lastPosBeforeCaughtByEnemy = playerMov.transform.position;

        aboveWaterGravity = rb2D.gravityScale;

        if (isHostile)
        {
            try
            {
                foreach (Transform t in GetComponentInChildren<Transform>())
                {
                    if (t.CompareTag("Warning"))
                    {
                        warningSignal = t.gameObject;
                    }
                    else if (t.CompareTag("Interested"))
                    {
                        interestedSignal = t.gameObject;
                    }
                }
                warningSignal.SetActive(false);
                interestedSignal.SetActive(false);
            }
            catch { }
        }
        cam = Camera.main;
    }

    protected virtual void Update()
    {
        // Make the enemy active if they are within the camera view.
        Vector2 camView = cam.WorldToViewportPoint(transform.position);
        isActive = camView.x > -0.25f && camView.x < 1.25f && camView.y > -0.25f && camView.y < 1.25f;

        if (showDebugs)
        {
            Debug.Log(name + " is " + (isActive ? "active" : "not active"));
        }

        if (!isActive)
        {
            moveVel = Vector2.zero;
            return;
        }

        // Change vision direction based on sprite flip settings.
        Vector2 transformDir = sprtRndr.flipX ? Vector2.left : Vector2.right;

        if (Vector2.Distance(transform.position, playerMov.transform.position) > viewDist)
        {
            SetVectors();
        }

        // Look for a likeableObject to chase.
        if (target == null && !isSatisfied)
        {
            for (int i = 0; i < likeableObjects.Count; ++i)
            {
                if (Vector2.Distance(transform.position, likeableObjects[i].transform.position) <= viewDist
                    && Physics2D.Linecast(transform.position, likeableObjects[i].position, notEnemy).transform == likeableObjects[i] // There are no obstacles blocking the vision between the enemy and the target and target is within view.
                    && (Vector2.Dot(transformDir, likeableObjects[i].position) > 0f
                    || Vector2.Distance(transform.position, likeableObjects[i].transform.position) <= viewDist / 3f)) // The enemy is facing towards the target.
                {
                    target = likeableObjects[i];

                    chaseTime = chaseWaitTime;
                    break;
                }
            }
        }
        else if (target != null)
        {
            chaseTime -= Time.deltaTime;

            if (Vector2.Distance(transform.position, target.position) > viewDist
                || Physics2D.Linecast(transform.position, target.position, notEnemy).transform != target
                && chaseTime <= 0f)
            {
                target = null;
            }
        }

        if (showDebugs && target)
        {
            Debug.Log(target.name);
        }

        // Decide to chase target or wander around.
        if (target != null && !isSatisfied)
        {
            Chase(target);

            if (target == player)
            {
                warningSignal.SetActive(true);
            }
            else
            {
                interestedSignal.SetActive(true);
            }
        }
        else
        {
            Wander();

            warningSignal.GetComponent<Animator>().SetTrigger("Exit");
            interestedSignal.GetComponent<Animator>().SetTrigger("Exit");
        }

        // Adjust sprite flip settings.
        if (moveVel.x != 0f)
        {
            sprtRndr.flipX = moveVel.x < 0f;
        }

        // Checks if it's touching the player or an object. If it's the player, it kills the player, if it's an object then it grabs the object.
        if (!isSatisfied && isHostile && CheckForObject(out Transform obj))
        {
            if (showDebugs)
            {
                Debug.Log(obj);
            }

            PlayerMovement player = obj.GetComponent<PlayerMovement>();
            Pickup item = obj.GetComponent<Pickup>();

            if (player)
            {
                Kill(player);
            }
            else if (item)
            {
                item.Grab(gameObject);
                isSatisfied = true;
                chaseTime = 0f;
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }

    private void SetVectors()
    {
        PlayerMovement.lastPosBeforeCaughtByEnemy = playerMov.transform.position;
        lastPos = transform.position;
    }

    private bool CheckForObject(out Transform obj)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 0f, Vector2.down, 0f, notEnemy);
        obj = hit.transform;

        return hit && likeableObjects.Contains(obj);
    }

    private void Kill(PlayerMovement player)
    {
        player.Die(1);
    }
    
    public void Respawn()
    {
        transform.position = lastPos;

        if (showDebugs)
        {
            Debug.Log(name + " Respawned");
        }

        try
        {
            warningSignal.SetActive(false);
            interestedSignal.SetActive(false);
        } catch { }

        NewPosition();
    }

    private void EnterWater()
    {
        rb2D.gravityScale = underwaterGravity;
    }

    private void ExitWater()
    {
        rb2D.gravityScale = aboveWaterGravity;
    }

    protected abstract void Chase(Transform chaseTarget);
    protected abstract void Wander();
    protected abstract Vector2 NewPosition();
}
