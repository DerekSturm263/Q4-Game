using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Animator anim;
    private SpriteRenderer sprtRndr;

    public float viewDist; // How far the enemy can see (in units). This affects the enemy's ability to see both the player, other enemies, and items.
    public Transform target; // Current transform that they enemy is targeting.
    
    public Transform[] likeableObjects = new Transform[]; // Transforms that the enemy will be drawn towards. Sort in order of priority.
    
    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprtRndr = GetComponent<SpriteRenderer>();
    }
    
    private void Update()
    {
        Vector2 transformDir = sprtRndr.flipX ? Vector2.Left : Vector2.Right;
        
        bool foundTarget = false;
        for (int i = 0; i < likeableObjects.Count; ++i)
        {
            if (Vector2.Distance(transform.position, likeableObjects[i].position) <= viewDist // Target is within view distance.
                        && Physics2D.Linecast(transform.position, likeableObjects[i].position // There are no obstacles blocking the vision between the enemy and the target.
                        && Vector2.Dot(transform.position, likeableObjects[i].position) > 0f) // The enemy is facing towards the target.
            {
                target = likeableObjects[i];
                Chase(target);
                foundTarget = true;
                
                break;
            }
        }
        
        if (!foundTarget)
        {
            target = null;
            Wander();
        }
    }
    
    protected void Kill(PlayerMovement player)
    {
        player.Die();
    }
    
    protected virtual void Chase(Transform chaseTarget);
    protected virtual void Wander();
}
