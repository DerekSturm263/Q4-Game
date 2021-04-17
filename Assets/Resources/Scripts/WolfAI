using UnityEngine

public class WolfAI : EnemyAI
{
    public float chaseSpeed; // Speed at which the enemy runs at while chasing something.
    
    private Vector2 moveVel; // Controls the enemy's movement direction.
    private float moveSpeed; // The speed at which the enemy moves.
    private Vector2 currentGoal; // Enemy's current goal while wandering. Will determine where the enemy will walk while there isn't a likeable object present.
    private float waitTime; // Amount of time that the enmy waits to choose a new spot.

    override void Chase(Transform target)
    {
        // Applies proper velocity.
        moveVel = (transform.position - target.position).normalized * chaseSpeed;
        rb2D.velocity = moveVel;
    }
    
    override void Wander(Transform target)
    {
        // Checks to see if the current movement goal is close enough to find a new one.
        if (Vector2.Distance(transform.position, currentGoal) < 0.1f)
        {
            if (waitTime -= Time.deltaTime <= 0f)
            {
                currentGoal = NewPosition();
                float dist = Vector2.Distance(transform.position, currentGoal);
                moveSpeed = Random.Range(dist * 0.25f, dist * 0.75f);
            }
        }
        else
        {
            waitTime = Random.Range(0.5f, 2.5f); // Might change later if this becomes inefficent.
        }
        
        // Applies proper velocity.
        moveVel = (transform.position - currentGoal).normalized * moveSpeed;
        rb2D.velocity = moveVel;
    }
    
    // Finds a new spot using raycast for the enemy to wander to.
    private Vector2 NewPosition()
    {
        // Chooses a random direction to move in by a random amount.
        float randomDir = Random.Range(-10f, 10f);
        
        // Creates vectors for the linecast.
        Vector2 startPos = transform.position + new Vector2(randomDir, 0f);
        Vector2 lineDist = 0.5f;
        
        RaycastHit2D hit = Physics2D.Linecast(startPos, startPos + lineDist);
        
        // Return the new position to wander in.
        if (hit)
        {
            return hit.point;
        }
        
        // Try again.
        return NewPosition();
    }
}
