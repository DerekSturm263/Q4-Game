using UnityEngine;
using System.Collections.Generic;

public class FlyingAI : EntityAI
{
    [SerializeField] private float wanderSpeed = 1.5f; // Minimum speed to wander at.

    private Vector2 currentGoal; // Entity's current goal while wandering. Will determine where the entity will walk while there isn't a likeable object present.
    [SerializeField] private List<Vector2> positions = new List<Vector2>();
    private int posIterator = -1;

    private float direction = -1f;
    private bool isDiving;

    protected override void Awake()
    {
        base.Awake();
        currentGoal = NewPosition();
    }

    protected override void Update()
    {
        base.Update();
        rb2D.velocity = moveVel * currentSpeed;
    }

    protected override void Chase(Transform target)
    {
        // Applies proper velocity.
        currentSpeed = chaseSpeed;

        if (!isDiving)
        {
            isDiving = true;
            direction = -1f;
        }

        if (transform.position.y < player.position.y)
        {
            direction = 1f;
        }

        anim.SetBool("Is Diving", true);
        if (sprtRndr.flipX)
        {
            transform.right = -moveVel.normalized;
        }
        else
        {
            transform.right = moveVel.normalized;
        }
        moveVel = Vector2.Lerp(moveVel, new Vector2((transform.position.x - target.position.x) * -0.8f, direction).normalized, Time.deltaTime * chaseTurnAroundSpeed);
    }

    protected override void Wander()
    {
        // Checks to see if the current movement goal is close enough to find a new one.
        if (Vector2.Distance(transform.position, currentGoal) <= 0.25f)
        {
            currentGoal = NewPosition();
            Debug.Log("Hello world");
        }

        isDiving = false;

        // Applies proper velocity.
        anim.SetBool("Is Diving", false);
        transform.right = Vector2.right;
        moveVel = Vector2.Lerp(moveVel, -((Vector2) transform.position - currentGoal).normalized, Time.deltaTime * wanderTurnAroundSpeed);
    }

    protected override Vector2 NewPosition()
    {
        if (useDebugs)
        {
            Debug.Log(name + ": New Position");
        }

        currentSpeed = wanderSpeed;

        if (++posIterator >= positions.Count)
        {
            posIterator = 0;
        }

        return positions[posIterator];
    }
}
