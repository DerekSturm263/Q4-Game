using System.Collections;
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

    [SerializeField] private Vector2 windVector = new Vector2(-5f, 0f);

    private List<Rigidbody2D> objectsInWind = new List<Rigidbody2D>();

    private void Awake()
    {
        player = FindObjectOfType<PlayerMovement>();
        col = GetComponent<BoxCollider2D>();
        ps = gameObject.AddComponent<ParticleSystem>();

        emission = ps.emission;
        shape = ps.shape;
        velocity = ps.velocityOverLifetime;

        emission.rateOverTime = Mathf.Abs(windVector.magnitude) * 37.5f;

        shape.position = col.offset;
        shape.scale = col.size;

        velocity.x = windVector.x * 2.5f;
        velocity.y = windVector.y * 2.5f;
        velocity.z = -5f;
    }

    private void Update()
    {
        foreach (Rigidbody2D rb2D in objectsInWind)
        {
            rb2D.velocity += windVector;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.outsideVel = windVector;
            return;
        }

        objectsInWind.Add(collision.GetComponent<Rigidbody2D>());
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.outsideVel = Vector2.zero;
            return;
        }

        objectsInWind.Remove(collision.GetComponent<Rigidbody2D>());
    }
}
