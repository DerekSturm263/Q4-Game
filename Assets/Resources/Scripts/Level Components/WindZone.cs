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

    [SerializeField] private List<Vector2> windVectors = new List<Vector2>();
    [SerializeField] private float windTime = 0f;

    private int vectorIterator;
    private Vector2 targetDirection;

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        col = GetComponent<BoxCollider2D>();
        ps = GetComponent<ParticleSystem>();

        emission = ps.emission;
        shape = ps.shape;
        velocity = ps.velocityOverLifetime;

        if (Settings.useParticles)
            emission.rateOverTime = Mathf.Abs(windVectors[0].magnitude) * 37.5f;
        else
            emission.rateOverTime = 0f;

        shape.position = col.offset;
        shape.scale = col.size;

        velocity.x = windVectors[0].x * 2.5f;
        velocity.y = windVectors[0].y * 2.5f;
        velocity.z = -5f;

        targetDirection = windVectors[0];
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
        if (++vectorIterator >= windVectors.Count)
        {
            vectorIterator = 0;
        }

        targetDirection = windVectors[vectorIterator];
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
