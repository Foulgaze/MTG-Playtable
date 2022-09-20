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

    public serverConnectUI serverConnection;

    public float horiPadding = 0.1f;
    public float vertPadding = 0.1f;
    CardHolderUI currentCardInfo = null;
    public int player = 0;
    

    int cardNumber = 0;

     public List<List<int>> allHands = new List<List<int>>();
     public List<GameObject> displayHandPositions = new List<GameObject>();

    public Dictionary<int,cardOnFieldBehavior> cardDict = new Dictionary<int, cardOnFieldBehavior>();

    public List<GameObject> hand = new List<GameObject>();

    public Texture2D cardTexture;

    public cardViewerUI viewerUI;
    public menuController menuController;
    public List<cardSpotBehavior> deckScripts;


    Camera cam;

    GameObject lastExpandedCard = null;
    public LayerMask filterLayer;
    public float scaleVal = 0.25f;
    public float hoverOverIncrease = 1.25f;

    public int cardHash;

    Vector2 handRect;
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    public class CardHolderUI
    {
        public CardHolderUI(Vector3 cardPos, Image png, GameObject obj, GameObject actualCard, GameObject copyCard, bool fromField)
        {
            this.originalCardPosition = cardPos;
            this.currentCardImage = png;
            this.originalCardObject = obj;
            this.cardOnField = actualCard;
            this.behavior = cardOnField.GetComponent<cardOnFieldBehavior>();
            this.copyCard = copyCard;
            this.fromField = fromField;
            lastHeldPosition = null;
            originalOnFieldPosition = null;
            cardOnFieldCopy = null;
        }
        public Vector3 originalCardPosition {get;}

        public cardOnFieldBehavior behavior {get;set;}
        public Image currentCardImage {get;}
        public GameObject originalCardObject {get;}
        public GameObject cardOnField{get;}
        public GameObject copyCard{get;}
        public bool fromField {get;}
        public GameObject lastHeldPosition {set;get;}
        public GameObject originalOnFieldPosition {set;get;}
        public GameObject cardOnFieldCopy {set;get;}

    }

    void Start()
    {
        cam = Camera.main;
        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GetComponent<EventSystem>();
        handRect = handHolder.transform.GetComponent<RectTransform>().anchorMax;

        

    }    

    public void initPlayerList(int playerCount)
    {
        for(int i = 0; i < playerCount; ++i)
        {
            allHands.Add(new List<int>());
        }
    }
    void Update()
    {                   
        checkForNewCardHold(); // First check for new cards to be held
        moveHeldCard(); // Then move any cards currently being held
        releaseCard();  // Check for any cards that are being released 
        raycastTest(); // Hover over card functionality
        checkForKeybindings();

    
    }

    void checkForKeybindings()
    {
        if(!isHolding() && Input.GetKeyDown(KeyCode.L) && (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
        {
            System.Random rnd = new System.Random();
            int randSeed = rnd.Next(2147483647);
            deckScripts[player].shuffleHelper(randSeed);
        }
    }



    void raycastTest()
    {
        //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            int min = -1;
            GameObject botObj = null;
            for(int i = 0; i < results.Count; ++i)
            {
                if(results[i].gameObject.tag == "cardInHand")
                {
                    int tempMin = Int32.Parse(results[i].gameObject.name);
                    if(min == -1 || tempMin > min)
                    {
                        if(!(isHolding() && GameObject.Equals(currentCardInfo.originalCardObject,results[i].gameObject)))
                        {
                            min = tempMin;
                            botObj = results[i].gameObject;
                        }
                        
                    }
                }
            }
            if(min != -1)
            {
                if(lastExpandedCard != null)
                {
                    lastExpandedCard.transform.localScale = Vector3.one * scaleVal;
                }

                botObj.transform.SetAsLastSibling();
                botObj.transform.localScale =  Vector3.one * scaleVal*hoverOverIncrease;
                lastExpandedCard = botObj;

                if(isHolding())
                {
                    currentCardInfo.originalCardObject.transform.SetAsLastSibling();
                }

            }
    }


    public void addCard(Texture2D newCard, int deckToAdd, Texture2D onFieldCard)
    {
        GameObject card = new GameObject();
        card.transform.parent = transform;
        card.tag = "cardInHand";
        card.AddComponent<Button>();
        card.AddComponent<storageScript>().text = onFieldCard;
        card.AddComponent<makeLast>().handController = transform.gameObject;
        
        Image cardPic = card.AddComponent<Image>();
        cardPic.sprite = Sprite.Create(newCard, new Rect(0,0,newCard.width, newCard.height),new Vector2(0,0));
        RectTransform rt = card.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(newCard.height*.72f,newCard.height);
        rt.pivot = new Vector2(0.5f,0.5f);
        rt.anchorMin = new Vector2(0.5f,0);
        rt.anchorMax = new Vector2(0.5f,0);
        card.transform.localScale = new Vector3(scaleVal,scaleVal,scaleVal);
        // card.transform.SetParent(handHolder.transform);
        
        GameObject onField = createCardForDeck(card,deckToAdd,onFieldCard);
        card.AddComponent<actualCardReference>().siblingCard = onField;
        deckScripts[deckToAdd].addToHand(onField);

        cardOnFieldBehavior onfieldScript = onField.GetComponent<cardOnFieldBehavior>();
        cardDict.Add(cardHash,onfieldScript);
        onfieldScript.hashCode = cardHash++;

        

        //cardPic.color = UnityEngine.Random.ColorHSV();

        // hand.Add(card);
        // updateHand();
    }
    public void updateHand()
    {
        Rect handArea = RectTransformUtility.PixelAdjustRect(handHolder.transform.GetComponent<RectTransform>(),transform.GetComponent<Canvas>());
        float startPos = handArea.width/-2 + handArea.width * horiPadding;
        if(hand.Count > 0)
        {
            RectTransform cardSize = hand[0].transform.GetComponent<RectTransform>();
            cardSize.localScale = Vector3.one * scaleVal;
            if(cardSize.sizeDelta.x > handArea.width)
            {
                Debug.LogError("Window Too Small!");
            }
            
                float addAmount = Mathf.Min(cardSize.sizeDelta.x * cardSize.localScale.x,(handArea.width*(1-horiPadding*2)/(Math.Max(1,hand.Count-1))));
                float horiPosition = (handArea.height - cardSize.sizeDelta.y*cardSize.localScale.y)/2 + (cardSize.sizeDelta.y*cardSize.localScale.y)/2;
                for(int i =0 ; i < hand.Count; ++i)
                {
                    RectTransform rt = hand[i].GetComponent<RectTransform>();
                    rt.anchoredPosition = new Vector2(startPos,horiPosition);
                    hand[i].transform.localScale = new Vector3(scaleVal,scaleVal,scaleVal);
                    hand[i].name = $"{i}";
                    hand[i].transform.SetSiblingIndex(i);
                    startPos += addAmount;
                }
        }
        Renderer cardRender = card.transform.GetComponent<Renderer>();

        for(int i = 0; i < displayHandPositions.Count; ++i)
        {
            if(i != player)
            {
                
                while(displayHandPositions[i].transform.childCount != allHands[i].Count)
                {
                    if(displayHandPositions[i].transform.childCount > allHands[i].Count)
                    {
                        GameObject.Destroy(displayHandPositions[i].transform.GetChild(0));
                    }
                    else
                    {
                        GameObject newCard = Instantiate(card);
                        newCard.transform.SetParent(displayHandPositions[i].transform);
                    }
                }
                Vector3 cardPositionMiddle = displayHandPositions[i].transform.position;
                int childCount = displayHandPositions[i].transform.childCount;
                int horiPadding = 2;
                cardPositionMiddle -= new Vector3(childCount/2 * (cardRender.bounds.extents.x + horiPadding),0,0);
                for(int a = 0; a < childCount; ++a)
                {
                    GameObject currChild  = displayHandPositions[i].transform.GetChild(a).gameObject;
                    currChild.transform.position = cardPositionMiddle;
                    cardPositionMiddle += new Vector3(cardRender.bounds.extents.x + horiPadding,0,0);
                }

            }
        }
        
    }
    void checkForNewCardHold()
    {
        if(EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag == "cardInHand" && !isHolding() && Input.GetMouseButtonDown(0))
        {
                    Debug.Log("here");
                    GameObject heldCard = EventSystem.current.currentSelectedGameObject;

                    heldCard.GetComponent<RectTransform>().localScale = Vector3.one * scaleVal;
                    GameObject copyCard = Instantiate(heldCard,heldCard.transform.position,heldCard.transform.rotation);
                    copyCard.transform.SetParent(heldCard.transform.parent);
                    copyCard.name = heldCard.name;
                    copyCard.GetComponent<RectTransform>().localScale = heldCard.GetComponent<RectTransform>().localScale;
                    copyCard.GetComponent<RectTransform>().position = heldCard.GetComponent<RectTransform>().position;
                    copyCard.GetComponent<RectTransform>().sizeDelta = heldCard.GetComponent<RectTransform>().sizeDelta;

                    Image cardPNG = heldCard.transform.GetComponent<Image>();
                    int reverse = player %2 == 0 ? -1: 1;
                    //GameObject actualCard = Instantiate(card,Vector3.zero,Quaternion.Euler(90 * reverse,0,0));
                    //actualCard.GetComponent<Renderer>().material.SetColor("_Color",heldCard.GetComponent<Image>().color);
                    // actualCard.GetComponent<Renderer>().material.mainTexture = heldCard.GetComponent<storageScript>().text;
                    // StartCoroutine(gameController.applyTexture(actualCard));
                    // actualCard.SetActive(false);
                    currentCardInfo = new CardHolderUI(cardPNG.rectTransform.position,cardPNG,heldCard,heldCard.GetComponent<actualCardReference>().siblingCard.gameObject,copyCard,false);    
                    // actualCard.GetComponent<Renderer>().material.SetFloat("_Glossiness",1.0f);

        }
        
    }
    void moveHeldCard()
    {
        
        if(isHolding())

        {
            bool inHand = inHandBox();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit,Mathf.Infinity,filterLayer) && !inHand) 
            {
                    // currentCardInfo.originalCardObject.SetActive(false);
                    // currentCardInfo.cardOnField.SetActive(true);
                    currentCardInfo.behavior.CardActivate();
                    Vector3 worldPoint = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Vector3.Distance(cam.gameObject.transform.position,hit.transform.position)));
                    worldPoint = playtableScript.findClosestCard(worldPoint,hit.transform, this,currentCardInfo.originalOnFieldPosition, currentCardInfo.cardOnField);
                    //worldPoint.y = horiFloor;
                    currentCardInfo.cardOnField.transform.position = worldPoint;
                    
            }
            else 
            {   
                
                currentCardInfo.behavior.UIActivate();
                currentCardInfo.currentCardImage.rectTransform.position = new Vector3(Input.mousePosition.x,Input.mousePosition.y,currentCardInfo.originalCardPosition.z);
                
                
            }
            if(!inHand && lastExpandedCard != null)
            {
                lastExpandedCard.transform.localScale = Vector3.one * scaleVal;
                lastExpandedCard = null;
            }
            
                
        }
    }

    GameObject createCardForDeck(GameObject passedCard, int deckNumber,Texture2D text)
    {
        int reverse = player %2 == 0 ? -1: 1;
        GameObject actualCard = Instantiate(card,Vector3.zero,Quaternion.Euler(90 * reverse,0,0));
        //actualCard.GetComponent<Renderer>().material.SetColor("_Color",heldCard.GetComponent<Image>().color);
        actualCard.GetComponent<Renderer>().material.mainTexture = text;
        StartCoroutine(gameController.applyTexture(actualCard));
        actualCard.GetComponent<Renderer>().material.SetFloat("_Glossiness",1.0f);
        
        cardOnFieldBehavior onField = actualCard.AddComponent<cardOnFieldBehavior>();
        onField.handManagerScript = this;
        onField.handRepresentation = card;
        onField.viewerUI = viewerUI;
        // passedCard.transform.SetParent(actualCard.transform);
        onField.menuController = menuController;
        onField.spotBehavior = deckScripts[deckNumber];
        onField.ownerType = 'D';
        onField.cardHoldPosition = deckScripts[deckNumber].gameObject;
        onField.siblingCard = passedCard;


        // passedCard.SetActive(false);
        // actualCard.SetActive(true);
        actualCard.name = cardNumber.ToString();
        cardNumber++;
        return actualCard;


    }
    public void releaseCard()
    {
        if(!Input.GetMouseButton(0) && isHolding())
        {
            if(currentCardInfo.fromField)
            {
                GameObject.Destroy(currentCardInfo.cardOnFieldCopy);
            }

            if(!inHandBox() && !currentCardInfo.originalCardObject.activeSelf) // Card is being placed on field somewhere
            {
                // // Do Something
                // hand.Remove(currentCardInfo.originalCardObject);

                // cardOnFieldBehavior onField;
                // if(!currentCardInfo.fromField) // Card Was Not On Field
                // {
                //     GameObject.Destroy(currentCardInfo.copyCard);
                //     onField = currentCardInfo.cardOnField.AddComponent<cardOnFieldBehavior>();
                //     onField.handManagerScript = this;
                //     onField.handRepresentation = currentCardInfo.originalCardObject;
                //     onField.viewerUI = viewerUI;
                //     currentCardInfo.originalCardObject.transform.SetParent(currentCardInfo.cardOnField.transform);
                //     onField.menuController = menuController;


                // }
                // else    // Card Was Already On Field
                // {
                //     onField = currentCardInfo.cardOnField.GetComponent<cardOnFieldBehavior>();
                //     currentCardInfo.originalCardObject.transform.SetParent(currentCardInfo.cardOnField.transform);

                //     // Card going from on field to somewhere else on field
                // }

                // if(!GameObject.Equals(currentCardInfo.lastHeldPosition,onField.cardHoldPosition) && currentCardInfo.fromField)
                // {
                //     cardSpotBehavior originalCardSpot = currentCardInfo.originalOnFieldPosition.GetComponent<cardSpotBehavior>();
                //     originalCardSpot.removeCardFromhand(currentCardInfo.cardOnField);

                    
                // }

                // onField.cardHoldPosition = currentCardInfo.lastHeldPosition;
                // currentCardInfo.lastHeldPosition.GetComponent<cardSpotBehavior>().addToHand(currentCardInfo.cardOnField);
                // onField.isHeld = false; 
                
                // currentCardInfo.cardOnField.transform.SetParent(null);
                // currentCardInfo.originalCardObject.transform.SetParent(currentCardInfo.cardOnField.transform);
                
                // int boolFromField = 0;
                // if(currentCardInfo.fromField)
                // {
                //     boolFromField = 1;
                // }
                int ogSpot = -1;
                if(currentCardInfo.originalOnFieldPosition != null)
                {
                    ogSpot =currentCardInfo.originalOnFieldPosition.GetComponent<cardSpotBehavior>().hashCode;
                }
                
                    GameObject.Destroy(currentCardInfo.copyCard);
                // if(ogSpot == -1)
                // {
                //     currentCardInfo.cardOnField = currentCardInfo.originalCardObject.transform.GetChild(0);
                // }
                Debug.Log($"OnField:{currentCardInfo.originalOnFieldPosition},CardOnField:{currentCardInfo.cardOnField},LastHeld:{currentCardInfo.lastHeldPosition}");
                serverConnection.sendTCPToServer($"{ogSpot},{currentCardInfo.behavior.hashCode},{currentCardInfo.lastHeldPosition.GetComponent<cardSpotBehavior>().hashCode}","01","08");
            }
            else // Card is being placed in hand OR Returning to previous place on field
            {
                int insertPosition = -1;
                if(lastExpandedCard != null && (currentCardInfo.fromField || !GameObject.Equals(currentCardInfo.copyCard,lastExpandedCard)))
                {
                    hand.Remove(currentCardInfo.originalCardObject);
                    if(Input.mousePosition.x > lastExpandedCard.transform.position.x)
                    {
                        if(currentCardInfo.fromField)
                        {
                            insertPosition = hand.IndexOf(lastExpandedCard)+1;
                        }
                        else
                        {
                            hand.Insert(hand.IndexOf(lastExpandedCard)+1,currentCardInfo.originalCardObject);
                        }
                    }
                    else
                    {
                        if(!currentCardInfo.fromField)
                        {
                           hand.Insert(hand.IndexOf(lastExpandedCard),currentCardInfo.originalCardObject);
                        } 
                        else
                        {
                            insertPosition = hand.IndexOf(lastExpandedCard);
                        }
                    }
                    
                }
                
                if(!currentCardInfo.fromField) // Hand
                {
                    currentCardInfo.behavior.UIActivate();
                    GameObject.Destroy(currentCardInfo.copyCard);
                    currentCardInfo.originalCardObject.GetComponent<RectTransform>().position = currentCardInfo.originalCardPosition;
                }
                else
                {
                    if(handHolder.activeSelf && inHandBox()) // Added to hand
                    {
                        // if(lastExpandedCard == null)
                        // {
                        //     hand.Add(currentCardInfo.originalCardObject);
                        // }
                        // currentCardInfo.originalCardObject.SetActive(true);
                        // currentCardInfo.cardOnField.SetActive(false);
                        // currentCardInfo.originalOnFieldPosition.GetComponent<cardSpotBehavior>().removeCardFromhand(currentCardInfo.cardOnField);
                        // GameObject.Destroy(currentCardInfo.cardOnField);
                        currentCardInfo.cardOnField.transform.position = currentCardInfo.originalCardPosition;
                        // currentCardInfo.originalCardObject.transform.SetParent(currentCardInfo.cardOnField.transform);
                        // currentCardInfo.originalCardObject.SetActive(false);
                        // currentCardInfo.cardOnField.SetActive(true);

                        currentCardInfo.behavior.CardActivate();
                        
                        serverConnection.sendTCPToServer($"{currentCardInfo.originalOnFieldPosition.GetComponent<cardSpotBehavior>().hashCode},{currentCardInfo.cardOnField.GetComponent<cardOnFieldBehavior>().hashCode},{player},{insertPosition}","01","09");
                    }
                    else // Being replaced on field in original position
                    {
                        // currentCardInfo.originalCardObject.SetActive(false);
                        // currentCardInfo.cardOnField.SetActive(true);

                        currentCardInfo.behavior.CardActivate();
                        currentCardInfo.cardOnField.transform.position = currentCardInfo.originalCardPosition;
                        // currentCardInfo.originalCardObject.transform.SetParent(currentCardInfo.cardOnField.transform);
                        GameObject.Destroy(currentCardInfo.cardOnFieldCopy);
                        currentCardInfo.cardOnField.GetComponent<cardOnFieldBehavior>().isHeld = false;
                    }

                    
                    
                }
                
                

            }
            currentCardInfo = null;
        }
    }
    public bool isHolding()
    {
        return currentCardInfo != null;
    }

    public bool inHandBox()
    {
        Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        return mousePos.y < handRect.y && mousePos.x < handRect.x;

        // return Input.mousePosition.y < handRect.height/2 && Input.mousePosition.x < handRect.width/2;
    }

    private void OnRectTransformDimensionsChange()
    {        
        
        
    }

    public void minimizeHand()
    {
        handHolder.SetActive(!handHolder.activeSelf);
    }
    public void moveOnFieldCard(GameObject onFieldCard, GameObject cardInHand, GameObject originalPositionObject)
    {
        GameObject onFieldClone = Instantiate(onFieldCard,onFieldCard.transform.position,onFieldCard.transform.rotation);
        cardInHand.name = $"{hand.Count}";
        cardInHand.transform.SetParent(handHolder.transform);
        currentCardInfo = new CardHolderUI(onFieldCard.transform.position,cardInHand.GetComponent<Image>(),cardInHand,onFieldCard,null,true);
        currentCardInfo.lastHeldPosition = onFieldCard.GetComponent<cardOnFieldBehavior>().cardHoldPosition;
        currentCardInfo.originalOnFieldPosition = originalPositionObject;
        currentCardInfo.cardOnFieldCopy = onFieldClone;
        cardInHand.transform.rotation = Quaternion.Euler(0,0,0);
        

    }

    public void setLastPositionObject(GameObject lastPosition)
    {
        if(!isHolding())
        {
            Debug.LogError("Tried to place when not holding an object");
        }
        currentCardInfo.lastHeldPosition = lastPosition;
    }

    public void releaseCardOnField(int originalPositionHash, int cardHash, int endPositionHash, int playerNum)
    {
        bool isMe = playerNum == player;
        bool fromHand = originalPositionHash == -1;
        if( (fromHand || playtableScript.cardHolderDictionary.ContainsKey(originalPositionHash)) && playtableScript.cardHolderDictionary.ContainsKey(endPositionHash) && cardDict.ContainsKey(cardHash))
        {
            cardSpotBehavior oldSpot = fromHand? null : playtableScript.cardHolderDictionary[originalPositionHash];

            
            cardSpotBehavior newSpot = playtableScript.cardHolderDictionary[endPositionHash];
            cardOnFieldBehavior actualCard = cardDict[cardHash];

            if(fromHand || oldSpot.cardsOnPile.Contains(actualCard.gameObject))
            {
                if(newSpot.cardsOnPile.Count < newSpot.pileLimit && (fromHand || oldSpot != newSpot))
                {
                    actualCard.siblingCard.transform.SetParent(null);
                    if(originalPositionHash != -1)
                    {
                        oldSpot.removeCardFromhand(actualCard.gameObject);
                        if(oldSpot.deckType == 'D')
                        {
                            actualCard.flipped = false;
                        }
                    }
                    else
                    {
                        if(isMe)
                        {
                            for(int i = 0; i < hand.Count; ++i)
                            {
                                if(GameObject.Equals(actualCard.siblingCard, hand[i]))
                                {
                                    hand[i].SetActive(false);
                                    hand.RemoveAt(i);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            allHands[playerNum].Remove(actualCard.hashCode);
                        }
                        updateHand();

                    }
                    actualCard.CardActivate();
                    newSpot.addToHand(actualCard.gameObject);
                    actualCard.cardHoldPosition = newSpot.gameObject;
                    actualCard.isHeld = false;
                }
                else
                {
                    actualCard.isHeld = false;
                }
                playtableScript.allSlotCheck();
            }
            else
            {
                Debug.Log("oof 2");
            }
        }
        // Add to hand of player

        if(isMe)
        {
            
        }
    }

    public void addCardToHand(int originalPositionHash, int cardHash, int endPlayer, int insertPosition,bool isMe)
    {
        if(playtableScript.cardHolderDictionary.ContainsKey(cardHash) && cardDict.ContainsKey(cardHash))
        {
            cardSpotBehavior oldSpot = playtableScript.cardHolderDictionary[originalPositionHash];
            cardOnFieldBehavior actualCard = cardDict[cardHash];

            if(oldSpot.cardsOnPile.Contains(actualCard.gameObject))
            {
                actualCard.flipped = false;
                actualCard.sideways = false;

                oldSpot.removeCardFromhand(actualCard.gameObject);
                actualCard.gameObject.SetActive(false);
                if(isMe)
                {   
                    GameObject uiCard = actualCard.siblingCard.gameObject;
                    uiCard.transform.SetParent(handHolder.transform);
                    uiCard.SetActive(true);

                    if(insertPosition == -1 || insertPosition > hand.Count )
                    {
                        insertPosition = 0;
                    }
                    if(hand.Count == 0)
                    {
                        hand.Add(uiCard);
                    }
                    else
                    {
                        hand.Insert(insertPosition,uiCard);
                    }
                }   
                allHands[endPlayer].Add(actualCard.hashCode);
                actualCard.setRotation();
                updateHand();
                return;
            }
            else
            {
                Debug.Log("NOT IN PILE");
            }
            
        }
        else
        {
            Debug.Log("NOT IN DICT");
        }
    }

}
