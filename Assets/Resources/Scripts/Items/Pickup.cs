﻿using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private PolygonCollider2D col;
    private SpriteRenderer sprtRndr;

    private ParticleSystem particles;
    private ParticleSystem.EmissionModule emission;

    private SpriteRenderer carrierSprtRndr;

    [SerializeField] private float underwaterGravity = 0.5f;
    private float aboveWaterGravity;

    [HideInInspector] public GameObject carrier;
    public float weight = 1f;

    [SerializeField] private bool showDebugs;
    [HideInInspector] public float timeSinceGrabbed;

    [HideInInspector] public Vector2 outsideVel;

    public float maxYVelocity = 15f;
    [HideInInspector] public float defaultMaxYVelocity;

    [HideInInspector] public Vector2 offset;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        col = GetComponent<PolygonCollider2D>();
        sprtRndr = GetComponent<SpriteRenderer>();

        particles = GetComponentInChildren<ParticleSystem>();
        emission = particles.emission;

        aboveWaterGravity = rb2D.gravityScale;
    }

    private void Update()
    {
        if (carrier == null)
        {
            if (outsideVel.magnitude > 0.2f)
            {
                rb2D.velocity = Vector2.Lerp(rb2D.velocity, outsideVel / weight * 1.475f, Time.deltaTime * 5f);
            }
            return;
        }

        sprtRndr.flipX = carrierSprtRndr.flipX;

        timeSinceGrabbed += Time.deltaTime;
        transform.rotation = Quaternion.identity;
        transform.position = Vector2.Lerp(transform.position, (Vector2) carrier.transform.position + offset * (carrierSprtRndr.flipX ? -1f : 1f), timeSinceGrabbed);
    }

    public void Grab(GameObject carrier)
    {
        this.carrier = carrier;
        emission.rateOverTime = 0f;

        if (carrier.GetComponent<PlayerMovement>() != null)
        {
            offset = carrier.GetComponent<PlayerMovement>().carrySpot;
        }
        else
        {
            offset = carrier.GetComponent<EntityAI>().carrySpot;
        }
        carrierSprtRndr = carrier.GetComponent<SpriteRenderer>();
        timeSinceGrabbed = 0f;
        col.enabled = false;
        rb2D.gravityScale = 0f;
        rb2D.velocity = Vector2.zero;
    }

    public void Throw(Vector2 throwVec)
    {
        carrier = null;
        emission.rateOverTime = 10f;

        carrierSprtRndr = null;
        timeSinceGrabbed = 0f;
        col.enabled = true;
        rb2D.gravityScale = aboveWaterGravity;
        rb2D.velocity = Vector2.zero;

        rb2D.AddForce(throwVec / weight, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            if (showDebugs)
            {
                Debug.Log(name + " Entered Water");
            }

            EnterWater();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Water"))
        {
            if (showDebugs)
            {
                Debug.Log(name + " Exited Water");
            }

            ExitWater();
        }
    }

    private void EnterWater()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x / 2f, rb2D.velocity.y / 10f);
        rb2D.gravityScale = underwaterGravity;
    }

    private void ExitWater()
    {
        rb2D.gravityScale = aboveWaterGravity;
    }
}