using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public abstract class EntityAI : MonoBehaviour
{
    public static List<GameObject> entities = new List<GameObject>();

    [SerializeField] protected bool showDebugs;

    private Camera cam;
    private PlayerMovement playerMov;

    [Header("Layer Mask Settings")]
    [SerializeField] protected LayerMask ground = 1 << 9;
    [SerializeField] protected LayerMask player = 1 << 12;
    [SerializeField] protected LayerMask notEnemy = ~(1 << 13 & 1 << 8 & 1 << 10);

    protected Rigidbody2D rb2D;
    protected Animator anim;
    protected SpriteRenderer sprtRndr;

    [Header("Target Settings")]
    [SerializeField] private bool isHostile = true;
    [HideInInspector] public bool isActive = false;

    [SerializeField] protected float viewDist = 10f; // How far the enemy can see (in units). This affects the enemy's ability to see both the player, other enemies, and items.
    protected Transform target; // Current transform that they enemy is targeting.
    
    [SerializeField] private List<Transform> likeableObjects = new List<Transform>(); // Transforms that the enemy will be drawn towards. Sort in order of priority.

    [Header("Speed Settings")]
    [SerializeField] protected float chaseSpeed = 3f; // Speed to chase at.
    [SerializeField] protected float wanderTurnAroundSpeed = 5f; // Speed to chase at.
    [SerializeField] protected float chaseTurnAroundSpeed = 2.5f; // Speed to chase at.

    private Vector2 lastPos;
    protected Vector2 ogPos;

    protected float currentSpeed;
    protected Vector2 moveVel;

    private bool isSatisfied = false; // Sets to true if the enemy grabs an object that they like.

    protected virtual void Awake()
    {
        entities.Add(gameObject);
        playerMov = FindObjectOfType<PlayerMovement>();
        ogPos = transform.position;

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprtRndr = GetComponent<SpriteRenderer>();

        lastPos = transform.position;
        PlayerMovement.lastPosBeforeCaughtByEnemy = playerMov.transform.position;

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
        if (target == null)
        {
            for (int i = 0; i < likeableObjects.Count; ++i)
            {
                if (Vector2.Distance(transform.position, likeableObjects[i].transform.position) <= viewDist
                    && Physics2D.Linecast(transform.position, likeableObjects[i].position, notEnemy).transform == likeableObjects[i] // There are no obstacles blocking the vision between the enemy and the target and target is within view.
                    && (Vector2.Dot(transformDir, likeableObjects[i].position) > 0f
                    || Vector2.Distance(transform.position, likeableObjects[i].transform.position) <= viewDist / 3f)) // The enemy is facing towards the target.
                {
                    target = likeableObjects[i];
                    break;
                }
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, target.position) > viewDist
                || Physics2D.Linecast(transform.position, target.position, notEnemy).transform != target)
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
        }
        else
        {
            Wander();
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

            PlayerMovement m = obj.GetComponent<PlayerMovement>();
            Pickup p = obj.GetComponent<Pickup>();

            if (m)
            {
                Kill(m);
            }
            else if (p)
            {
                p.Grab(gameObject);
                isSatisfied = true;
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
        target = null;
    }

    protected abstract void Chase(Transform chaseTarget);
    protected abstract void Wander();
}