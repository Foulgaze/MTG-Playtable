using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
public class cardPlaying : MonoBehaviour
{
    float distanceBetween = 5;
    [HideInInspector]
    public GameObject playTable;
    public playtableGenerator playtableScript;
    public GameObject card;
    public GameObject handHolder;

    public Texture2D testCard;
    [HideInInspector]
    public float horiFloor;
    CardHolderUI currentCardInfo = null;

    public List<GameObject> hand = new List<GameObject>();

    public customScroll scrollbar;

    Camera cam;
    public LayerMask filterLayer;
    public float scaleVal = 0.5f;

    public class CardHolderUI
    {
        public CardHolderUI(Vector3 cardPos, Vector3 mousePos, Image png, GameObject obj, GameObject actualCard)
        {
            this.originalCardPosition = cardPos;
            this.originalMousePosition = mousePos;
            this.currentCardImage = png;
            this.originalCardObject = obj;
            this.cardOnField = actualCard;
        }
        public Vector3 originalCardPosition {get;}
        public Vector3 originalMousePosition {get;}
        public Image currentCardImage {get;}
        public GameObject originalCardObject {get;}
        public GameObject cardOnField{get;}
    }

    void Start()
    {
        cam = Camera.main;
    }    
    void Update()
    {                   
        checkForNewCardHold();
        moveHeldCard();
        releaseCard();
        if(Input.GetKeyDown(KeyCode.G))
        {
            addCard(testCard);
        }
    
    }

    public void addCard(Texture2D newCard)
    {
        GameObject card = new GameObject();
        card.transform.parent = transform;
        card.tag = "holdable";
        card.AddComponent<Button>();
        Image cardPic = card.AddComponent<Image>();
        cardPic.sprite = Sprite.Create(newCard, new Rect(0,0,newCard.width, newCard.height),new Vector2(0,0));
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(newCard.height*.72f,newCard.height);
        rt.pivot = new Vector2(0.5f,0);
        rt.anchorMin = new Vector2(0.5f,0);
        rt.anchorMax = new Vector2(0.5f,0);
        card.transform.localScale = new Vector3(scaleVal,scaleVal,scaleVal);
        card.transform.SetParent(handHolder.transform);
        hand.Add(card);
        scrollbar.resetScroll();
        updateHand();
    }

    public void updateHand()
    {
        float startPos = 0;
        if(hand.Count > 0)
        {
            RectTransform cardSize = hand[0].transform.GetComponent<RectTransform>();

            startPos -= (cardSize.sizeDelta.x * (hand.Count - 1)*cardSize.localScale.x)/2;
            for(int i =0 ; i < hand.Count; ++i)
            {
                RectTransform rt = hand[i].GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(startPos,0);
                startPos += cardSize.sizeDelta.x*cardSize.localScale.x;
            }

        }
        
    }



    void checkForNewCardHold()
    {
        if(!isHolding() && Input.GetMouseButtonDown(0) && EventSystem.current.currentSelectedGameObject != null)
        {
            if(EventSystem.current.currentSelectedGameObject.tag == "holdable")
            {
                GameObject heldCard = EventSystem.current.currentSelectedGameObject;
                Image cardPNG = heldCard.transform.GetComponent<Image>();
                GameObject actualCard = Instantiate(card,Vector3.zero,Quaternion.Euler(90,0,0));
                actualCard.SetActive(false);
                currentCardInfo = new CardHolderUI(cardPNG.rectTransform.position,Input.mousePosition,cardPNG,heldCard,actualCard);
            }
            
        }
        
    }
    void moveHeldCard()
    {
    if(isHolding())
        {
            float newYPos = currentCardInfo.originalCardPosition.y + (Input.mousePosition.y - currentCardInfo.originalMousePosition.y)*1.2f;
            if(newYPos >currentCardInfo.originalCardPosition.y)
            {

                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit,Mathf.Infinity,filterLayer) && newYPos - currentCardInfo.originalCardPosition.y >= distanceBetween) 
                {
                        currentCardInfo.currentCardImage.enabled = false;
                        currentCardInfo.cardOnField.SetActive(true);
                        Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Vector3.Distance(cam.gameObject.transform.position,hit.transform.position)));
                        worldPoint = playtableScript.findClosestCard(worldPoint,hit.transform);
                        //worldPoint.y = horiFloor;
                        currentCardInfo.cardOnField.transform.position = worldPoint;
                }
                else if(newYPos - currentCardInfo.originalCardPosition.y >= distanceBetween)
                {

                }
                else
                {

                    currentCardInfo.currentCardImage.enabled = true;
                    currentCardInfo.cardOnField.SetActive(false);
                    currentCardInfo.currentCardImage.rectTransform.position = new Vector3(currentCardInfo.originalCardPosition.x,newYPos,currentCardInfo.originalCardPosition.z);
                }
            }
            
        }
    }
    public void releaseCard()
    {
        if(!Input.GetMouseButton(0) && isHolding())
        {
            float newYPos = currentCardInfo.originalCardPosition.y + (Input.mousePosition.y - currentCardInfo.originalMousePosition.y)*1.2f;
            if(newYPos - currentCardInfo.originalCardPosition.y >= distanceBetween && !currentCardInfo.originalCardObject.GetComponent<Image>().enabled)
            {
                // Do Something
                hand.Remove(currentCardInfo.originalCardObject);
                GameObject.Destroy(currentCardInfo.originalCardObject);
                updateHand();
            }
            else
            {
                currentCardInfo.currentCardImage.enabled = true;
                GameObject.Destroy(currentCardInfo.cardOnField);
                currentCardInfo.currentCardImage.rectTransform.position = currentCardInfo.originalCardPosition;
            }
            currentCardInfo = null;
        }
    }
    public bool isHolding()
    {
        return currentCardInfo != null;
    }



}
