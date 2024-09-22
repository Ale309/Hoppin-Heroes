using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using PlayFab;
using PlayFab.ClientModels;

public class PlayerMovement : MonoBehaviour
{
    private PlayFabControls playFabControls;  // Variabile per lo script PlayFabControl

    public GameObject mainPanel;
    public GameObject blackPanel;
    public GameObject successPanel;

    public ParticleSystem dust;
    public ParticleSystem confetti;

    private bool stop = true;

    private Rigidbody2D rb;
    private BoxCollider2D coll;
    private Animator anim;
    private SpriteRenderer sprite;

    private float dirX = 1f;
    private float moveSpeed = 10f;
    private float jumpForce = 25f;
    private int jumped = 0;

    private bool isCollidingRight = false;
    private bool isCollidingLeft = false;
    private bool isGrounded = true;
    private bool onWall = false;
    private bool underWall = false;
    private bool onPlatform = false;
    private bool ended = false;
    private bool once = false;

    private enum MovementState{ idle, running, jumping, falling, sliding, death, win }

    private MovementState state;
     
    private PlayerControl control;

    public static Vector2 lastCheckpointPos = new Vector2(0, -11);
    private static Vector2 set = new Vector2(-0.1f, 582.155f);

    [SerializeField] TextMeshProUGUI timerText;
    private float elapsedTime;
    [SerializeField] TextMeshProUGUI score;

    private void Awake()
    {
        control = new PlayerControl();
        control.Enable();
        this.transform.position = lastCheckpointPos;
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        ItemCollector.SetCollectibles(PlayerData.LoadCollectibles());
        playFabControls = GetComponent<PlayFabControls>();
        state = MovementState.running;
        AudioManager.instance.Play("BGMusic");
    }

    private void Update()
    {
        if(!ended)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60);
            int seconds = Mathf.FloorToInt(elapsedTime % 60);
            int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);
        
            timerText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);


        control.Land.Jump.performed += ctx =>
        {
            if(jumped < 2)
            {
                if(!isCollidingRight && !isCollidingLeft)
                {
                    if(rb.velocity.y < -0.5f)
                    {
                        rb.velocity = new Vector2(dirX * moveSpeed, jumpForce);
                    }
                    else
                    {
                        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                    }
                    jumped++;
                }
                else if(isCollidingRight && !isCollidingLeft)
                {
                    rb.velocity = new Vector2(-moveSpeed, jumpForce);
                    dirX = -1f;
                    sprite.flipX = true;
                    jumped++;
                }
                else if(isCollidingLeft && !isCollidingRight)
                {
                    rb.velocity = new Vector2(moveSpeed, jumpForce);
                    dirX = 1f;
                    sprite.flipX = false;
                    jumped++;
                }
                dust.Play();
                AudioManager.instance.Play("Jumping");
            }
        };

        if(rb.velocity.x > 0.5f)
        {
            sprite.flipX = false;
        }
        else if(rb.velocity.x < -0.5f)
        {
            sprite.flipX = true;
        }
        /*
        if(ended && stop)
        {
            rb.velocity = new Vector2(0, 0);
            state = MovementState.win;
            this.transform.position = set;
            confetti.Play();
            stop = false;
        }
        else 
        */
        
        if(onPlatform)
        {
            state = MovementState.idle;
        }
        else if(isGrounded || onWall)
        {
            if(rb.velocity.y <= 0.1f)
            {
                state = MovementState.running;
            }

            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);

            if(isCollidingRight)
            {
                dirX = -1f;
                sprite.flipX = true;
            }
            else if(isCollidingLeft)
            {
                dirX = 1f;
                sprite.flipX = false;
            }
        }
        else if(!isCollidingLeft && !isCollidingRight)
        {
            if(rb.velocity.y > 0.1f)
            {
                state = MovementState.jumping;
            }
            else if(rb.velocity.y < -0.1f)
            {
                state = MovementState.falling;
                if(dirX == 1f)
                {
                    sprite.flipX = false;
                }
                else if(dirX == -1f)
                {
                    sprite.flipX = true;
                }
            }
        }
        else if((isCollidingLeft && !isCollidingRight) || (!isCollidingLeft && isCollidingRight))
        {
            state = MovementState.sliding;
            
            if(rb.velocity.y < -0.1f)
            {
                rb.velocity = new Vector2(rb.velocity.x, -5f);
                dust.Play();
            }
        }

        if(isCollidingLeft && isCollidingRight)
        {
            Die();
        }


        anim.SetInteger("state", (int)state);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 collisionNormal = collision.contacts[0].normal;

        if(!collision.gameObject.CompareTag("Floor") && collisionNormal.y < -0.5f)
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
        }

        if(collision.gameObject.CompareTag("Finish"))
        {
            if(collisionNormal.x > 0.5f)
            {
                isCollidingLeft = true;
                dirX = 1f;
                sprite.flipX = true;

            }
            else if(collisionNormal.x < -0.5f)
            {
                isCollidingRight = true;
                dirX = -1f;
                sprite.flipX = false;
            }
            else if(collisionNormal.y > 0.5f)
            {
                rb.velocity = new Vector2(0, 0);
                state = MovementState.win;
                anim.SetInteger("state", (int)state);
                this.transform.position = set;
                confetti.Play();
                //stop = false;
                ended = true;
                int minutes = Mathf.FloorToInt(elapsedTime / 60);
                int seconds = Mathf.FloorToInt(elapsedTime % 60);
                int milliseconds = Mathf.FloorToInt((elapsedTime * 1000) % 1000);
        
                score.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
                jumped = 0;
                
            }

        }
        else if(collision.gameObject.CompareTag("Wall"))
        {
            if(collisionNormal.x > 0.5f)
            {
                isCollidingLeft = true;
                dirX = 1f;
                sprite.flipX = true;

            }
            else if(collisionNormal.x < -0.5f)
            {
                isCollidingRight = true;
                dirX = -1f;
                sprite.flipX = false;
            }
            else if(collisionNormal.y > 0.5f)
            {
                onWall = true;
            }
            else if(collisionNormal.y < -0.5f)
            {
                underWall = true;
            }

            if(!isGrounded)
            {
                jumped = 0;
            }
        }
        else if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Floor"))
        {
            if(collisionNormal.y > 0.5f)
            {
                isGrounded = true;
                jumped = 0;
                dust.Play();
            }
        }
        else if(collision.gameObject.CompareTag("Platform"))
        {
            if(collisionNormal.y > 0.5f)
            {
                onPlatform = true;
                jumped = 0;
                rb.velocity = new Vector2(0, 0);
            }
        }
        else if(collision.gameObject.CompareTag("Trap"))
        {
            Die();
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            if(collisionNormal.y > 0.5f)
            {
                state = MovementState.jumping;
                rb.velocity = new Vector2(dirX * moveSpeed, jumpForce);
                jumped++;
                AudioManager.instance.Play("EnemyDeath");
            }
            else
            {
                Die();
            }
        }
        else if(collision.gameObject.CompareTag("MrBlow"))
        {
            MrBlowMovement mrBlow = collision.gameObject.GetComponent<MrBlowMovement>();

            if(collisionNormal.y > 0.5f && mrBlow.GetCurrentState() != MrBlowMovement.MovementState.blowing)
            {
                state = MovementState.jumping;
                rb.velocity = new Vector2(dirX * moveSpeed, jumpForce);
                jumped++;
            }
            else
            {
                Die();
            }
        }

        if(isCollidingLeft && isCollidingRight)
        {
            Die();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {/*
        if (!isGrounded)
        {
            rb.velocity = new Vector2(dirX * moveSpeed, rb.velocity.y);
            Debug.Log("Troy");
        }*/

        if(collision.gameObject.CompareTag("Finish"))
        {
            isCollidingLeft = false;
            isCollidingRight = false;
            ended = false;
        }
        else if(collision.gameObject.CompareTag("Wall"))
        {
            isCollidingLeft = false;
            isCollidingRight = false;
            onWall = false;
            underWall = false;
        }
        else if(collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Floor"))
        {
            isGrounded = false;
        }
        else if(collision.gameObject.CompareTag("Platform"))
        {
            if(onPlatform)
            {
                rb.velocity = new Vector2(dirX * moveSpeed, jumpForce);
                state = MovementState.jumping;
            }
            onPlatform = false;
        }
    }

    private void Die()
    {
        rb.bodyType = RigidbodyType2D.Static;
        coll.enabled = false;
        state = MovementState.death;
        AudioManager.instance.Play("Death");
    }

    public bool CheckIsGrounded()
    {
        return isGrounded;
    }

    public void RestartLevel()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        this.transform.position = lastCheckpointPos;
        rb.bodyType = RigidbodyType2D.Dynamic;
        coll.enabled = true;
        state = MovementState.running;
    }

    public void completeRestartLevel()
    {
        lastCheckpointPos = new Vector2(0, -11);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void Success()
    {
        if(!once)
        {
            AudioManager.instance.Stop("BGMusic");
            AudioManager.instance.Play("Success");
            mainPanel.SetActive(false);
            blackPanel.SetActive(true);
            successPanel.SetActive(true);

            // Imposta la scala iniziale del pannello a zero per "minimizzarlo"
            successPanel.transform.localScale = Vector3.zero;

            // Anima il pannello per ingrandirlo
            LeanTween.scale(successPanel, Vector3.one, 0.5f).setEaseOutBack();  // Anima da zero a scala 1
            once = true;
            if (PlayFabClientAPI.IsClientLoggedIn())
            {
                playFabControls.SendLeaderboard(elapsedTime);
                playFabControls.GetLeaderboard();
            }
            
        }
        
    }
}
