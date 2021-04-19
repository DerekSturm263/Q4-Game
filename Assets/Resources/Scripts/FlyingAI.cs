using UnityEngine;

public class FlyingAI : EntityAI
{
    private Vector2 currentGoal; // Enemy's current goal while wandering. Will determine where the enemy will walk while there isn't a likeable object present.
    [SerializeField] private Vector2[] positions = new Vector2[];
    private int posIterator = 0;

    private override void Chase(Transform target)
    {
        // TODO: Add an arc motion.

        // Applies proper velocity.
        currentSpeed = chaseSpeed;
        moveVel = new Vector2(transform.position.x - target.position.x, transform.position.y - target.position.y).normalized;
    }

    private override void Wander()
    {
        // Checks to see if the current movement goal is close enough to find a new one.
        if (Vector2.Distance(transform.position, currentGoal) < 0.05f)
        {
            currentGoal = NewPosition();
            currentSpeed = minWanderSpeed;
        }

        // Applies proper velocity.
        moveVel = new Vector2(transform.position.x - currentGoal.x, transform.position.y - currentGoal.y).normalized;
    }

    private Vector2 NewPosition()
    {
        if (++posIterator >= positions.Length)
        {
            posIterator = 0;
        }

        return positions[posIterator];
    }

    private void OnCollisionEnter2D(Collision collision)
    {
        if (collision.CompareTag("Player"))
        {
            Kill(collision.GetComponent<PlayerMovement>());
        }
    }
}
