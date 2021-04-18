using UnityEngine;

public class GroundAI : EtityAI
{
    private Vector2 currentGoal; // Enemy's current goal while wandering. Will determine where the enemy will walk while there isn't a likeable object present.
    private float waitTime; // Amount of time that the enmy waits to choose a new spot.

    private override void Chase(Transform target)
    {
        // Applies proper velocity.
        currentSpeed = chaseSpeed;
        moveVel = new Vector2(transform.position.x - target.position.x, 0f).normalized;
    }
    
    private override void Wander()
    {
        // Checks to see if the current movement goal is close enough to find a new one.
        if (Vector2.Distance(transform.position, currentGoal) < 0.05f)
        {
            if (waitTime -= Time.deltaTime <= 0f)
            {
                currentGoal = NewPosition();
                currentSpeed = Random.Range(minWanderSpeed, maxWanderSpeed);
            }
            else
            {
                currentSpeed = 0f;
            }
        }
        else
        {
            waitTime = Random.Range(0f, 2f); // Might change later if this becomes inefficent.
        }

        // Applies proper velocity.
        moveVel = new Vector2(transform.position.x - currentGoal.x, 0f).normalized;
    }
    
    // Finds a new spot using raycast for the enemy to wander to.
    private Vector2 NewPosition()
    {
        // Chooses a random direction to move in by a random amount.
        float randomDir = Random.Range(-10f, 10f);
        
        // Creates vectors for the linecast.
        Vector2 startPos = transform.position + new Vector2(randomDir, 0f);
        Vector2 lineDist = new Vector2(0f, -0.5f);
        
        RaycastHit2D hit = Physics2D.Linecast(startPos, startPos + lineDist);
        
        // Return the new position to wander in.
        if (hit)
        {
            return hit.point;
        }
        
        // Try again.
        return NewPosition();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Kill(collision.GetComponent<PlayerMovement>());
        }
    }
}
