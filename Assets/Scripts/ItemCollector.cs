using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCollector : MonoBehaviour
{
	private static int collectibles = 0;

	[SerializeField] private Text collectiblesText;

	private void Start()
    {
        UpdateCollectiblesText();
    }

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("Collectible"))
		{
			Destroy(collision.gameObject);
			AudioManager.instance.Play("CollectingItem");
			collectibles++; 
			UpdateCollectiblesText();
		}
	}

	public bool TrySpendCollectibles()
    {
        if (collectibles >= 20)
        {
            collectibles -= 20;
            UpdateCollectiblesText();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void UpdateCollectiblesText()
    {
        collectiblesText.text = "" + collectibles;
    }

    public int GetCollectibles()
    {
        return collectibles;
    }

    public static void SetCollectibles(int amount)
    {
        collectibles = amount;
    }

}