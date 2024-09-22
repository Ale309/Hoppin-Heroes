using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private static bool Checkpointed1 = false;
    private static bool Checkpointed2 = false;
    private static bool Checkpointed3 = false;
    private static bool Checkpointed4 = false;
    private static bool Checkpointed5 = false;

    [SerializeField] GameObject door1;
    [SerializeField] GameObject door2;
    [SerializeField] GameObject door3;
    [SerializeField] GameObject door4;
    [SerializeField] GameObject door5;

    [SerializeField] GameObject Player;

    private void Start()
    {
        // Imposta lo stato delle porte in base ai checkpoint attivati
        door1.SetActive(Checkpointed1);
        door2.SetActive(Checkpointed2);
        door3.SetActive(Checkpointed3);
        door4.SetActive(Checkpointed4);
        door5.SetActive(Checkpointed5);
    }

    public void Update()
    {
        
                if(Checkpointed5 && Player.transform.position.x < 6f)
                {
                    door5.SetActive(true);
                }
                if(Checkpointed4 && Player.transform.position.x > -6f)
                {
                    door4.SetActive(true);
                }
                if(Checkpointed3 && Player.transform.position.x < 6f)
                {
                    door3.SetActive(true);
                }
                if(Checkpointed2 && Player.transform.position.x > -6f)
                {
                    door2.SetActive(true);
                }
                if(Checkpointed1 && Player.transform.position.x < 6f)
                {
                    door1.SetActive(true);
                }
    }

	private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            ItemCollector itemCollector = collision.GetComponent<ItemCollector>();

            if (itemCollector != null)
            {
                // Controlla se il giocatore ha abbastanza collectible
                bool hasEnoughCollectibles = itemCollector.TrySpendCollectibles();

                if(this.name == "Checkpoint 5" && !Checkpointed5)
                {
                    if (hasEnoughCollectibles)
                    {
                        PlayerMovement.lastCheckpointPos = new Vector2(0, 81);
                        PlayerData.SaveCollectibles(itemCollector.GetCollectibles());
                        Debug.Log("First checkpoint!");
                        Checkpointed5 = true;
                        AudioManager.instance.Play("Checkpoint");
                    }
                    else
                    {
                        Debug.Log("Not enough collectibles!");
                        AudioManager.instance.Play("NotEnough");
                    }
                }
                else if(this.name == "Checkpoint 4" && !Checkpointed4)
                {
                    if (hasEnoughCollectibles)
                    {
                        PlayerMovement.lastCheckpointPos = new Vector2(0, 176);
                        PlayerData.SaveCollectibles(itemCollector.GetCollectibles());
                        Debug.Log("Second checkpoint!");
                        Checkpointed4 = true;
                        AudioManager.instance.Play("Checkpoint");
                    }
                    else
                    {
                        Debug.Log("Not enough collectibles!");
                    }
                }
                else if(this.name == "Checkpoint 3" && !Checkpointed3)
                {
                    if (hasEnoughCollectibles)
                    {
                        PlayerMovement.lastCheckpointPos = new Vector2(0, 273);
                        PlayerData.SaveCollectibles(itemCollector.GetCollectibles());
                        Debug.Log("Third checkpoint!");
                        Checkpointed3 = true;
                        AudioManager.instance.Play("Checkpoint");
                    }
                    else
                    {
                        Debug.Log("Not enough collectibles!");
                    }
                }
                else if(this.name == "Checkpoint 2" && !Checkpointed2)
                {
                    if (hasEnoughCollectibles)
                    {
                        PlayerMovement.lastCheckpointPos = new Vector2(0, 369);
                        PlayerData.SaveCollectibles(itemCollector.GetCollectibles());
                        Debug.Log("Fourth checkpoint!");
                        Checkpointed2 = true;
                        AudioManager.instance.Play("Checkpoint");
                    }
                    else
                    {
                        Debug.Log("Not enough collectibles!");
                    }
                }
                else if(this.name == "Checkpoint 1" && !Checkpointed1)
                {
                    if (hasEnoughCollectibles)
                    {
                        PlayerMovement.lastCheckpointPos = new Vector2(0, 465);
                        PlayerData.SaveCollectibles(itemCollector.GetCollectibles());
                        Debug.Log("Fifth checkpoint!");
                        Checkpointed1 = true;
                        AudioManager.instance.Play("Checkpoint");
                    }
                    else
                    {
                        Debug.Log("Not enough collectibles!");
                    }
                }
            }
        }
    }
}