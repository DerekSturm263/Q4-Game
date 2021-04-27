using UnityEngine;

public class BouncePlatforn : MonoBehaviour
{
    private Animator anim;

    public float bounceForce;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D rb2D = collision.gameObject.GetComponent<Rigidbody2D>();

        if (rb2D.velocity.y > 0f)
            return;

        rb2D.velocity = new Vector2(rb2D.velocity.x, bounceForce);

        anim.SetTrigger("Bounce");
    }
}
