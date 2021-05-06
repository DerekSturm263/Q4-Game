using UnityEngine;

public class GroundAI : EntityAI
{
    private Vector2 currentGoal; // Entity's current goal while wandering. Will determine where the entity will walk while there isn't a likeable object present.
    [SerializeField] private float minWaitTime = 0f;
    [SerializeField] private float maxWaitTime = 2f;
    private float waitTime; // Amount of time that the entity waits to choose a new spot.

    [SerializeField] private float minWanderSpeed = 1.5f; // Minimum speed to wander at.
    [SerializeField] private float maxWanderSpeed = 2.5f; // Maximum speed to wander at.

    [SerializeField] private float wanderDist;

    [Header("Particle Settings")]
    [SerializeField] private ParticleSystem walkRun; // Particles for when the entity walks.
    private ParticleSystem.EmissionModule walkRunParticles;

    private float iceSlipperiness = 1f;

    protected override void Awake()
    {
        base.Awake();

        walkRunParticles = walkRun.emission;
        currentGoal = NewPosition();
    }

    protected override void Update()
    {
        base.Update();

        rb2D.velocity = new Vector2(moveVel.x * currentSpeed, rb2D.velocity.y);

        anim.SetFloat("Move Vel", moveVel.x * currentSpeed);
        anim.SetFloat("Move Speed", Mathf.Abs(rb2D.velocity.x) / 5f);
    }

    protected override void Chase(Transform target)
    {
        if (Mathf.Abs(transform.position.x - target.position.x) > 0.5f)
        {
            // Applies proper velocity.
            currentSpeed = chaseSpeed;
            if (target != player)
            {
                currentSpeed /= 2f;
            }
            moveVel = Vector2.Lerp(moveVel, new Vector2((transform.position.x - target.position.x) * -1f, 0f).normalized, Time.deltaTime * chaseTurnAroundSpeed * iceSlipperiness);

            walkRunParticles.rateOverTime = 25f;
        }
        else
        {
            // Cancels velocity.
            currentSpeed = 0f;
            moveVel = Vector2.Lerp(moveVel, Vector2.zero, Time.deltaTime * chaseTurnAroundSpeed * iceSlipperiness);

            walkRunParticles.rateOverTime = 0f;
        }
    }

    protected override void Wander()
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

        // Applies proper velocity.
        moveVel = Vector2.Lerp(moveVel, new Vector2((transform.position.x - currentGoal.x) * -1f, 0f).normalized, Time.deltaTime * wanderTurnAroundSpeed * iceSlipperiness);
    }

    // Finds a new spot using raycast for the enemy to wander to.
    protected override Vector2 NewPosition()
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ice"))
        {
            iceSlipperiness = 0.25f;
        }
        else if (collision.gameObject.CompareTag("Ground"))
        {
            iceSlipperiness = 1f;
        }
    }
}
