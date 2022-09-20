using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cardList : MonoBehaviour
{
    public List<GameObject> cardHolderList = new List<GameObject>();



    public void addToList(GameObject place)
    {
        cardHolderList.Add(place);
    }

    public int removeFromList()
    {
        GameObject removedItem = cardHolderList[cardHolderList.Count-1];
        cardHolderList.RemoveAt(cardHolderList.Count-1);
        return removedItem.GetComponent<cardSpotBehavior>().hashCode;
    }
}
