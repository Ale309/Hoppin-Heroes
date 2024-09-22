using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullSoldierMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer sprite;
    private Animator anim;
    private BoxCollider2D boxCollider;

    private float dirX = 1f;
    private float moveSpeed = 2f;

    private bool isWalking = true;
    private int life = 2;

    private enum MovementState{ walking, abouttodie }

    private MovementState state;

    private Vector2 idleSize = new Vector2(1.48f, 2.125f);
    private Vector2 idleOffset = new Vector2(-0.124f, -0.1235f);

    private Vector2 secondSize = new Vector2(1.438f, 1.183f);
    private Vector2 secondOffset = new Vector2(-0.029f, -0.09f);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>(); 
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.size = idleSize;
        boxCollider.offset = idleOffset;
        state = MovementState.walking;
    }

    // Update is called once per frame
    void Update()
    {
        if(life == 2)
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

            if(this.transform.position.y > 210 && this.transform.position.y < 250)
            {
                if(this.transform.position.x < -3)
                {
                    dirX = 1f;
                    sprite.flipX = false;
                }
                else if(this.transform.position.x > 3)
                {
                    dirX = -1f;
                    sprite.flipX = true;
                }
            }
        }
        else if(life == 1)                    
        {
            state = MovementState.abouttodie;
            anim.SetInteger("state", (int)state);
            boxCollider.size = secondSize;
            boxCollider.offset = secondOffset;
            rb.velocity = new Vector2(dirX * moveSpeed * 5, rb.velocity.y);
        }
        if(life == 0)
        {
            Die();
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
                life = life - 1;
                
            }
            else{
                dirX = dirX * (-1.0f);
                sprite.flipX = !sprite.flipX;
            }
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            dirX = dirX * (-1.0f);
            sprite.flipX = !sprite.flipX;
        }
    }

    private void Die()
    {
        Destroy(GetComponent<Collider2D>(), .5f);
    }
}
