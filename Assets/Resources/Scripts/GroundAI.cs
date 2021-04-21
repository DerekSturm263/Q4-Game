using UnityEngine;

public class GroundAI : EntityAI
{
    private Vector2 currentGoal; // Entity's current goal while wandering. Will determine where the entity will walk while there isn't a likeable object present.
    [SerializeField] private float minWaitTime = 0f;
    [SerializeField] private float maxWaitTime = 2f;
    private float waitTime; // Amount of time that the entity waits to choose a new spot.

    [SerializeField] private float wanderDist;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem walkRun; // Particles for when the player walks.
    private ParticleSystem.EmissionModule walkRunParticles;

    protected override void Awake()
    {
        base.Awake();
        walkRunParticles = walkRun.emission;
    }

    protected override void Chase(Transform target)
    {
        if (Mathf.Abs(transform.position.x - target.position.x) > 0.5f)
        {
            // Applies proper velocity.
            currentSpeed = chaseSpeed;
            moveVel = Vector2.Lerp(moveVel, new Vector2((transform.position.x - target.position.x) * -1f, 0f).normalized, Time.deltaTime * 2.5f);

            walkRunParticles.rateOverTime = 25f;
        }
        else
        {
            currentSpeed = 0f;
            moveVel = Vector2.Lerp(moveVel, Vector2.zero, Time.deltaTime * 2.5f);

            walkRunParticles.rateOverTime = 0f;
        }

    }

    protected override void Wander()
    {
        if (currentGoal != Vector2.zero)
        {
            // Checks to see if the current movement goal is close enough to find a new one.
            if (Mathf.Abs(transform.position.x - currentGoal.x) <= 0.1f)
            {
                if ((waitTime -= Time.deltaTime) <= 0f)
                {
                    walkRunParticles.rateOverTime = 25f;
                    currentGoal = NewPosition();
                }
                else
                {
                    walkRunParticles.rateOverTime = 0f;
                    currentSpeed = 0f;
                }
            }
        }
        else
        {
            currentGoal = NewPosition();
        }

        // Applies proper velocity.
        moveVel = Vector2.Lerp(moveVel, new Vector2((transform.position.x - currentGoal.x) * -1f, 0f).normalized, Time.deltaTime * 2.5f);
    }
    
    // Finds a new spot using raycast for the enemy to wander to.
    private Vector2 NewPosition()
    {
        // Chooses a random direction to move in by a random amount.
        float randomDir = Random.Range(-wanderDist, wanderDist);

        // Creates vectors for the linecast.
        Vector2 startPos = ogPos + new Vector2(randomDir, 0f);
        Vector2 lineDist = Vector2.down * 2f;
        
        RaycastHit2D hit = Physics2D.Linecast(startPos, startPos + lineDist, ground);

        // Sets the amount of time the entity should wait for before finding a new spot.
        waitTime = Random.Range(minWaitTime, maxWaitTime);
        currentSpeed = Random.Range(minWanderSpeed, maxWanderSpeed);

        // Return the new position to wander in.
        if (hit)
        {
            return new Vector2(hit.point.x, transform.position.y);
        }
        
        // Try again.
        return NewPosition();
    }
}
