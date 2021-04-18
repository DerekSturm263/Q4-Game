using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(SpriteRenderer))]
public abstract class EntityAI : MonoBehaviour
{
    private Rigidbody2D rb2D;
    protected Animator anim;
    protected SpriteRenderer sprtRndr;

    [SerializeField] private bool isHostile = true;
    [SerializeField] private bool isActive = false;

    [SerializeField] private float viewDist; // How far the enemy can see (in units). This affects the enemy's ability to see both the player, other enemies, and items.
    protected Transform target; // Current transform that they enemy is targeting.
    
    [SerializeField] private Transform[] likeableObjects = new Transform[]; // Transforms that the enemy will be drawn towards. Sort in order of priority.

    [SerializeField] protected float minWanderSpeed; // Minimum speed to wander at.
    [SerializeField] protected float maxWanderSpeed; // Maximum speed to wander at.
    [SerializeField] protected float chaseSpeed; // Speed to chase at.

    protected float currentSpeed;
    protected Vector2 moveVel;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprtRndr = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        if (1isActive)
            return;

        Vector2 transformDir = sprtRndr.flipX ? Vector2.Left : Vector2.Right;

        if (target == null)
        {
            for (int i = 0; i < likeableObjects.Count; ++i)
            {
                if (Vector2.Distance(transform.position, likeableObjects[i].position) <= viewDist // Target is within view distance.
                    && Physics2D.Linecast(transform.position, likeableObjects[i].position // There are no obstacles blocking the vision between the enemy and the target.
                    && Vector2.Dot(transformDir, likeableObjects[i].position) > 0f) // The enemy is facing towards the target.
                {
                    target = likeableObjects[i];
                    isFound = true;
                    Chase(target);

                    break;
                }
            }

            Wander();
        }
        else
        {
            if (Vector2.Distance(transform.position, target.position) <= viewDist // Target is within view distance.
                && Physics2D.Linecast(transform.position, target.position // There are no obstacles blocking the vision between the enemy and the target.
                && Vector2.Dot(transformDir, target.position) > 0f) // The enemy is facing towards the target.
            {
                Chase(target);
            }
            else
            {
                target = null;
                Wander();
            }
        }
    }

    private void FixedUpdate()
    {
        rb2D.velocity = new Vector2(moveVel.x * currentSpeed, 0f);
    }

    protected void Kill(PlayerMovement player)
    {
        if (!isHostile)
            return;

        player.Die();
    }
    
    protected virtual void Chase(Transform chaseTarget);
    protected virtual void Wander();
}
