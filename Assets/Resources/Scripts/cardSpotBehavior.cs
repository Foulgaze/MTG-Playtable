using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.CodeAnalysis;

public class cardSpotBehavior : MonoBehaviour
{
    public List<GameObject> cardsOnPile = new List<GameObject>();
    public GameObject card;
    
    Vector3 cardDimensions;

    public int pileLimit = 3;
    public float horipadding = 0.3f;
    public float vertpadding = 0.05f;

    public char deckType;

    public int playerOwner;

    public int actualPlayer;
    public int hashCode;


    void Start()
    {
        
    }
    public Vector3 getNewPilePosition(int countNumber = -1, GameObject checkForPresent = null)
    {
        if(cardDimensions == Vector3.zero)
        {
            cardDimensions = card.GetComponent<Renderer>().bounds.extents;
        }
        Vector3 returnPosition = transform.position + new Vector3(0,cardDimensions.z*2,0);
        countNumber = countNumber == -1 ? cardsOnPile.Count : countNumber;
        for(int i = 0; i < countNumber; ++i)
        {
            if(checkForPresent != null && GameObject.Equals(cardsOnPile[i],checkForPresent))
            {
                return returnPosition;
            }
            returnPosition += new Vector3(-horipadding*cardDimensions.x*2,cardDimensions.z*2,vertpadding*cardDimensions.y*2);
        }
        return returnPosition;
    }

    public void shuffleHelper(int seed)
    {
        Debug.Log("shuffling");
        Shuffle<GameObject>(cardsOnPile,seed);
        updatePilePositions();
    }


    public static void Shuffle<T>(List<T> list, int seed)
    {
        var rng = new System.Random(seed);
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }

    }

    public bool addToHand(GameObject newCard)
    {
        if(isFull() || cardsOnPile.Contains(newCard))
        {
            return false;
        }
        cardOnFieldBehavior cardBehavior = newCard.transform.GetComponent<cardOnFieldBehavior>();
        cardBehavior.playerOwner = playerOwner;
        cardBehavior.ownerType = deckType;
        cardBehavior.spotBehavior = this;
        switch(deckType)
        {
            default:
            case 'N':
                break;
            case 'D':
                cardBehavior.flipped = true;
                cardBehavior.sideways = false;
                break;
            
        }
        cardBehavior.setRotation();

        cardsOnPile.Add(newCard);
        removeCardFromhand(null);
        return true;
    }

public void removeCardFromhand(GameObject card)
    {
        cardsOnPile.Remove(card);
        updatePilePositions();
    }

    public void updatePilePositions()
    {
        for(int i = 0; i < cardsOnPile.Count; ++i)
        {
            cardsOnPile[i].transform.position = getNewPilePosition(i);
        }
    }
    
    public bool isFull()
    {
        return cardsOnPile.Count >= pileLimit;
    }
}
