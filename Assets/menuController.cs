using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class menuController : MonoBehaviour
{

    public GameObject lastSelectedCard;
    public GameObject box;
    // Start is called before the first frame update
    public void makeMenuAppear()
    {
        Debug.Log("reciving");
        transform.gameObject.SetActive(true);
        transform.position = new Vector3(Input.mousePosition.x,Input.mousePosition.y,0);
    }

    public void flipCard()
    {
        if(lastSelectedCard == null)
        {
            Debug.LogError("There was no gameobject to flip!");
            return;
        }
        cardOnFieldBehavior behavior = lastSelectedCard.GetComponent<cardOnFieldBehavior>();
        if(behavior.ownerType == 'N')
        {
            behavior.flipped = !behavior.flipped;
            behavior.setRotation();
        }
        
        transform.gameObject.SetActive(false);


    }


    void Update()
    {
        checkForLeftClick();
    }

    void checkForLeftClick()
    {
        if(Input.GetMouseButtonDown(0) && (EventSystem.current.currentSelectedGameObject == null || (!EventSystem.current.currentSelectedGameObject.transform.IsChildOf(transform))))
        {
            transform.gameObject.SetActive(false);
        }
    }

    public void revealPile()
    {
        if(lastSelectedCard == null)
        {
            Debug.LogError("There was no gameobject to flip!");
            return;
        }
        cardOnFieldBehavior behavior = lastSelectedCard.GetComponent<cardOnFieldBehavior>();

        List<GameObject> gameList = behavior.spotBehavior.cardsOnPile;
        Vector3 position = box.transform.position;
        for(int i = 0; i < gameList.Count; ++i)
        {
            GameObject cardCopy = Instantiate(gameList[i].transform.GetChild(0).gameObject,position, Quaternion.identity);
            RectTransform rt = cardCopy.GetComponent<RectTransform>();
            if(i == 0)
            {
                position -= new Vector3(-rt.sizeDelta.x * rt.localScale.x,rt.sizeDelta.y * rt.localScale.y,0);
            }
            rt.anchoredPosition = position;
            position += new Vector3(rt.sizeDelta.x * rt.localScale.x,0,0);
            rt.localScale = new Vector3(0.1f,0.1f,0.1f);
            cardCopy.SetActive(true);
            cardCopy.transform.SetParent(box.transform);
        }

        transform.gameObject.SetActive(false);
    }
}
