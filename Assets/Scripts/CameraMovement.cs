using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player;
    private Vector2 minPosition = new Vector2(0f, 0f); // Imposta i limiti minimi per X e Y
    private Vector2 maxPosition = new Vector2(0f, 570f); // Imposta i limiti massimi per X e Y
    private Vector3 newPos;
    private Vector3 oldPos;
    private bool hasEnteredRight = false;
    private bool hasEnteredLeft = false;
    private bool hasExitedRight = false;
    private bool hasExitedLeft = false;


    [SerializeField] private PlayerMovement playerMovement;

    private float lerpSpeed = 10f;

    void Update()
    {
        if (player != null)
        {
            if(player.position.x > -10f && player.position.x < 10f && transform.position.x <= 0.1f)
            {
                if(playerMovement.CheckIsGrounded())
                {
                    if(player.position.y > 80f && player.position.y < 87f ||
                        player.position.y > 175f && player.position.y < 182f ||
                        player.position.y > 271f && player.position.y < 278f ||
                        player.position.y > 367f && player.position.y < 374f ||
                        player.position.y > 463f && player.position.y < 470f)
                    {
                        // Calcola la posizione desiderata della telecamera
                        oldPos = transform.position;
                    }
                }
                // Calcola la posizione desiderata della telecamera
                Vector3 desiredPosition = new Vector3(player.position.x, player.position.y, transform.position.z);
                // Controlla i limiti
                desiredPosition.x = Mathf.Clamp(desiredPosition.x, minPosition.x, maxPosition.x);
                desiredPosition.y = Mathf.Clamp(desiredPosition.y, minPosition.y, maxPosition.y);

                // Applica il movimento della telecamera
                transform.position = desiredPosition;
                hasEnteredRight = false;
                hasEnteredLeft = false;
                hasExitedRight = false;
                hasExitedLeft = false;
            }
            else if(player.position.x >= 10f && !hasEnteredRight)
            {
                newPos = new Vector3(transform.position.x + 20.2f, transform.position.y, transform.position.z);
                hasEnteredRight = true;
            }
            else if(player.position.x < 10f && hasEnteredRight)
            {
                hasEnteredRight = false;
                hasExitedRight = true;
            }
            else if(player.position.x <= -10f && !hasEnteredLeft)
            {
                newPos = new Vector3(transform.position.x - 20.2f, transform.position.y, transform.position.z);
                hasEnteredLeft = true;
            }
            else if(player.position.x > -10f && hasEnteredLeft)
            {
                hasEnteredLeft = false;
                hasExitedLeft = true;
            }
            
            
            if(hasEnteredRight || hasEnteredLeft)
            {
                transform.position = Vector3.Lerp(transform.position, newPos, lerpSpeed * Time.deltaTime);
            }
            else if(hasExitedRight || hasExitedLeft)
            {
                //transform.position = new Vector3(player.position.x, player.position.y, transform.position.z);
                transform.position = Vector3.Lerp(transform.position, oldPos, lerpSpeed * Time.deltaTime);
            }
        }
    }

    // Metodo per assegnare il giocatore
    public void SetPlayer(Transform newPlayer)
    {
        player = newPlayer;
    }
}
