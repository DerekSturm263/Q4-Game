using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public abstract class EntityAI : MonoBehaviour
{
    private Camera cam;
    private PlayerMovement playerMov;

    [SerializeField] protected LayerMask ground = 1 << 9;
    [SerializeField] protected LayerMask player = 1 << 12;
    [SerializeField] protected LayerMask notEnemy = ~(1 << 13);

    protected Rigidbody2D rb2D;
    protected Animator anim;
    protected SpriteRenderer sprtRndr;

    [SerializeField] private bool isHostile = true;
    [SerializeField] private bool isActive = false;

    [SerializeField] private float viewDist; // How far the enemy can see (in units). This affects the enemy's ability to see both the player, other enemies, and items.
    protected Transform target; // Current transform that they enemy is targeting.
    
    [SerializeField] private List<Transform> likeableObjects = new List<Transform>(); // Transforms that the enemy will be drawn towards. Sort in order of priority.

    [SerializeField] protected float minWanderSpeed = 1.5f; // Minimum speed to wander at.
    [SerializeField] protected float maxWanderSpeed = 2.5f; // Maximum speed to wander at.
    [SerializeField] protected float chaseSpeed = 3f; // Speed to chase at.

    protected Vector2 ogPos;

    protected float currentSpeed;
    protected Vector2 moveVel;

    [SerializeField] protected bool useDebugs;

    private void Awake()
    {
        playerMov = FindObjectOfType<PlayerMovement>();
        ogPos = transform.position;

        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprtRndr = GetComponent<SpriteRenderer>();

        PlayerMovement.lastPosBeforeCaughtByEnemy = playerMov.transform.position;

        cam = Camera.main;
    }
    
    private void Update()
    {
        Vector2 camView = cam.WorldToViewportPoint(transform.position);
        isActive = camView.x > -0.5f && camView.x < 1.5f && camView.y > 0f && camView.y < 1f;

        if (useDebugs)
        {
            Debug.Log(name + " is " + (isActive ? "active" : "not active"));
        }

        if (!isActive)
        {
            PlayerMovement.lastPosBeforeCaughtByEnemy = playerMov.transform.position;

            moveVel = Vector2.zero;
            return;
        }

        Vector2 transformDir = sprtRndr.flipX ? Vector2.left : Vector2.right;

        if (target == null)
        {
            for (int i = 0; i < likeableObjects.Count; ++i)
            {
                if (Vector2.Distance(transform.position, likeableObjects[i].transform.position) <= viewDist
                    && Physics2D.Raycast(transform.position, (likeableObjects[i].position - transform.position).normalized, notEnemy) // There are no obstacles blocking the vision between the enemy and the target and target is within view.
                    && Vector2.Dot(transformDir, likeableObjects[i].position) > 0f) // The enemy is facing towards the target.
                {
                    target = likeableObjects[i];
                    break;
                }
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, target.position) > viewDist
                || !Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, notEnemy))
            {
                target = null;
            }
        }

        if (useDebugs && target)
        {
            Debug.Log(target.name);
        }

        if (target != null)
        {
            Chase(target);
        }
        else
        {
            Wander();
        }

        if (isHostile && CheckForPlayer(out Transform playerObj))
        {
            Kill(playerObj.GetComponent<PlayerMovement>());
        }
    }

    private void FixedUpdate()
    {
        if (moveVel.x != 0f)
        {
            sprtRndr.flipX = moveVel.x < 0f;
        }

        rb2D.velocity = new Vector2(moveVel.x * currentSpeed, rb2D.velocity.y);
    }

    private void OnDrawGizmos()
    {
        if (target)
        {
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }

    private bool CheckForPlayer(out Transform playerObj)
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, new Vector2(transform.localScale.x, transform.localScale.y), 0f, Vector2.down, 0f, player);
        playerObj = hit.transform;

        return hit;
    }

    private void Kill(PlayerMovement player)
    {
        player.Die(1);
    }
    
    protected abstract void Chase(Transform chaseTarget);
    protected abstract void Wander();
}
