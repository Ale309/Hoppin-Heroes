using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenRunnerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private float dirX = 1f;
    private float moveSpeed = 4f;

    private bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isDead)
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            Vector2 collisionNormal = collision.contacts[0].normal;

            if (collisionNormal.x > 0.5f)
            {
                dirX = 1f;
                sprite.flipX = false;
            }
            else if (collisionNormal.x < -0.5f)
            {
                dirX = -1f;
                sprite.flipX = true;
            }
        }
        else if(collision.gameObject.CompareTag("Player"))
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            if (collisionNormal.y < -0.5f)
            {
                Die();
            }
            else{
                dirX = dirX * (-1.0f);
                sprite.flipX = !sprite.flipX;
            }
        }
    }

    private void Die()
    {
        anim.SetTrigger("DieGreenRunner");
        isDead = true;
        Destroy(GetComponent<Collider2D>(), .5f);
        Destroy(gameObject, 5f); // Distrugge il nemico dopo 5 secondi
    }
}
