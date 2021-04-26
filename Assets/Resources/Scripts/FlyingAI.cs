using UnityEngine;
using System.Collections.Generic;

public class FlyingAI : EntityAI
{
    [SerializeField] private float wanderSpeed = 1.5f; // Minimum speed to wander at.

    private Vector2 currentGoal; // Entity's current goal while wandering. Will determine where the entity will walk while there isn't a likeable object present.
    [SerializeField] private List<Vector2> positions = new List<Vector2>();
    private int posIterator = -1;

    private bool isSwooping;

    [SerializeField] private float perchWaitTime = 5f;
    private float timeSincePerch = 0f;

    private float swoopStartX;
    private float swoopEndX;
    private float swoopOffset;
    private Vector2 endSwoopPos;

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

        if (Vector2.Distance(transform.position, target.position) > viewDist)
        {
            isSwooping = false;
            return;
        }

        if (!isSwooping)
        {
            swoopStartX = transform.position.x;
            swoopEndX = (transform.position.x - target.position.x) * -2f;
            swoopOffset = Mathf.Abs(swoopStartX) + Mathf.Abs(swoopEndX) - target.transform.position.y;
            endSwoopPos = new Vector2(swoopEndX, transform.position.y);
        }

        float xPos = swoopStartX + (transform.position.x - target.position.x < 0f ? -Time.deltaTime : Time.deltaTime);
        float yPos = SwoopYPos(transform.position.x, swoopStartX, swoopEndX, 7f, swoopOffset);

        moveVel = -new Vector2(xPos, yPos).normalized;

        if (isSwooping)
        {
            return;
        }

        isSwooping = true;
    }

    protected override void Wander()
    {
        // Checks to see if the current movement goal is close enough to find a new one.
        if (Vector2.Distance(transform.position, currentGoal) <= 0.25f)
        {
            currentGoal = NewPosition();
        }

        // Applies proper velocity.
        moveVel = Vector2.Lerp(moveVel, -((Vector2) transform.position - currentGoal).normalized, Time.deltaTime * wanderTurnAroundSpeed);
    }

    protected override Vector2 NewPosition()
    {
        if (showDebugs)
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

    private float SwoopYPos(in float xPos, float xInt1, float xInt2, float steepness, float yOffset)
    {
        return ((xPos - xInt1) * (xPos - xInt2) / steepness) + yOffset;
    }
}
