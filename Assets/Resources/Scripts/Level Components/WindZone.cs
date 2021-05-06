using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    private PlayerMovement player;

    private BoxCollider2D col;

    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.ShapeModule shape;
    private ParticleSystem.VelocityOverLifetimeModule velocity;

    [SerializeField] private Vector2 windVector = new Vector2(-2.5f, 0f);
    [SerializeField] private float windTime = 0f;
    [SerializeField] private Vector2 windVector2 = new Vector2(2.5f, 0f);

    private Vector2 targetDirection;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        col = GetComponent<BoxCollider2D>();
        ps = GetComponent<ParticleSystem>();

        emission = ps.emission;
        shape = ps.shape;
        velocity = ps.velocityOverLifetime;

        emission.rateOverTime = Mathf.Abs(windVector.magnitude) * 37.5f;

        shape.position = col.offset;
        shape.scale = col.size;

        velocity.x = windVector.x * 2.5f;
        velocity.y = windVector.y * 2.5f;
        velocity.z = -5f;

        targetDirection = windVector;
        if (windTime != 0f)
        {
            InvokeRepeating("ChangeDirections", windTime, windTime);
        }
    }

    private void Update()
    {
        if (windTime == 0f)
            return;

        velocity.x = Mathf.Lerp(velocity.x.constant, targetDirection.x * 2.5f, Time.deltaTime * 2.5f);
        velocity.y = Mathf.Lerp(velocity.y.constant, targetDirection.y * 2.5f, Time.deltaTime * 2.5f);
    }

    private void ChangeDirections()
    {
        if (targetDirection == windVector)
        {
            targetDirection = windVector2;
        }
        else
        {
            targetDirection = windVector;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.outsideVel = new Vector2(velocity.x.constant / 2f, velocity.y.constant / 2f);
        }
        else if (collision.CompareTag("Pickup"))
        {
            collision.GetComponent<Pickup>().outsideVel = new Vector2(velocity.x.constant, velocity.y.constant);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.outsideVel = Vector2.zero;
        }
        else if (collision.CompareTag("Pickup"))
        {
            collision.GetComponent<Pickup>().outsideVel = Vector2.zero;
        }
    }
}
