using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MrBlowMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;

    private float dirX = 1f;
    private float moveSpeed = 1f;
    private bool isJumping = false;
    private bool isDead = false;

    public enum MovementState { idle, walking, blowing, dizzy }
    private MovementState state;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        state = MovementState.walking;

        StartCoroutine(StateRoutine());
    }

    void Update()
    {
        if (!isDead)
        {
            if (state == MovementState.walking)
            {
                rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
            
            UpdateAnimationState();
        }
    }

    private void UpdateAnimationState()
    {
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
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            if (collisionNormal.y < -0.5f && state != MovementState.blowing)
            {
                Die();
            }
        }
    }

    private void Die()
    {
        anim.SetTrigger("DieMrBlow");
        isDead = true;
        rb.velocity = Vector2.zero;
        Destroy(GetComponent<Collider2D>(), .5f);
        Destroy(gameObject, 5f); // Distrugge il nemico dopo 5 secondi
    }

    private IEnumerator StateRoutine()
    {
        while (!isDead)
        {
            // Stato walking
            state = MovementState.walking;
            yield return new WaitForSeconds(Random.Range(2f, 6f));

            // Stato blowing
            state = MovementState.blowing;
            yield return new WaitForSeconds(3f);

            // Stato dizzy
            state = MovementState.dizzy;
            yield return new WaitForSeconds(3f);

            // Stato idle
            state = MovementState.idle;
            yield return new WaitForSeconds(2f);
        }
    }

    public MovementState GetCurrentState()
    {
        return state;
    }
}