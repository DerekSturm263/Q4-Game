using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindZone : MonoBehaviour
{
    private BoxCollider2D col;

    private ParticleSystem ps;
    private ParticleSystem.EmissionModule emission;
    private ParticleSystem.ShapeModule shape;
    private ParticleSystem.VelocityOverLifetimeModule velocity;

    [SerializeField] private Vector2 windVector = new Vector2(-5f, 0f);

    private List<Rigidbody2D> objectsInWind = new List<Rigidbody2D>();

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        ps = GetComponent<ParticleSystem>();

        emission = ps.emission;
        shape = ps.shape;
        velocity = ps.velocityOverLifetime;

        emission.rateOverTime = Mathf.Abs(windVector.magnitude) * 20f;

        shape.position = col.offset;
        shape.scale = col.size;

        velocity.x = windVector.x;
        velocity.y = windVector.y;
        velocity.z = -0.5f;
    }

    private void Update()
    {
        foreach (Rigidbody2D rb2D in objectsInWind)
        {
            rb2D.velocity = windVector;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        objectsInWind.Add(collision.GetComponent<Rigidbody2D>());
    }
}
