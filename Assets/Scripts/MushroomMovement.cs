using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private float dirX = 1f;
    private float moveSpeed = 1f;
    private bool isJumping = false;
    private bool isDead = false;

    private enum MovementState { walking, jumping, falling }
    private MovementState state;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        state = MovementState.walking;

        StartCoroutine(BehaviorRoutine());
    }

    void Update()
    {
        if (!isDead)
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
            UpdateAnimationState();
        }
    }

    private void UpdateAnimationState()
    {
        if (rb.velocity.y < -0.1f)
        {
            state = MovementState.falling;
        }
        else if (isJumping)
        {
            state = MovementState.jumping;
        }
        else
        {
            state = MovementState.walking;
        }

        anim.SetInteger("state", (int)state);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
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
            else if(collisionNormal.y > 0.5f)
            {
                isJumping = false;
            }
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            if (collisionNormal.y < -0.5f)
            {
                Die();
            }
            else
            {
                dirX = dirX * (-1.0f);
                sprite.flipX = !sprite.flipX;
            }
        }
        else if (collision.gameObject.CompareTag("Floor") || collision.gameObject.CompareTag("Ground"))
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            if(collisionNormal.y > 0.5f)
            {
                isJumping = false;
            }
        }
    }

    private void Die()
    {
        anim.SetTrigger("DieMushroom");
        isDead = true;
        rb.velocity = Vector2.zero;
        Destroy(GetComponent<Collider2D>(), .5f);
        Destroy(gameObject, 5f); // Distrugge il nemico dopo 5 secondi
    }

    private IEnumerator BehaviorRoutine()
    {
        while (!isDead)
        {
            yield return new WaitForSeconds(Random.Range(2f, 4f));

            // Salto
            if (!isJumping)
            {
                rb.velocity = new Vector2(rb.velocity.x, 15f); // Regola la forza del salto come necessario
                isJumping = true;
                UpdateAnimationState();
            }
        }
    }
}
