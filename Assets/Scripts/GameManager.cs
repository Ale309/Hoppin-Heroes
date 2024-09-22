using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // Dichiara una variabile per tenere traccia delle monete raccolte
    private bool[] collectiblesCollected;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // L'oggetto persistente non verrà distrutto durante il cambio di scena
        }
        else
        {
            Destroy(gameObject); // Se c'è già un GameManager, distruggi questo duplicato
        }
    }

    // Metodo per inizializzare lo stato delle monete
    public void InitializeCollectibles(int totalCollectibles)
    {
        collectiblesCollected = new bool[totalCollectibles];
    }

    // Metodo per aggiornare lo stato della moneta collezionata
    public void CollectCollectibles(int collectibleIndex)
    {
        collectiblesCollected[collectibleIndex] = true;
    }

    // Metodo per controllare se una moneta è stata già raccolta
    public bool IsCollectibleCollected(int collectibleIndex)
    {
        return collectiblesCollected[collectibleIndex];
    }
}
