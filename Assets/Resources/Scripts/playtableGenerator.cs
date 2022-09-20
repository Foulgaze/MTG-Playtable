using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class playtableGenerator : MonoBehaviour
{
    float horiPadding = 3;
    float vertPadding = 1;

    int vertCard = 3;
    int edgePadding = 3;
    public GameObject cardHolder;
    [HideInInspector]
    public GameObject playTable;

    public Texture2D cardHolderTexture;
    public Material tableTexture;

    public cardPlaying cardManager; 
    public GameObject playingCardObject;
    public GameObject cardBarHolder;
    public int actualPlayer;
    public Dictionary<int,cardSpotBehavior> cardHolderDictionary = new Dictionary<int, cardSpotBehavior>();

    public int cardID = 0;

    GameObject cardPlaceHolder;

    public int lastHoriValue = 0;

    List<boardInformation> boardExpansionList = new List<boardInformation>();

    int topExpansion = 0;
    int bottomExpansion = 0;

    public int playerCount = 4;

    class boardInformation
    {
        public boardInformation(int vertCard)
        {
            boardSize = 0;
            spotParent = null;
            expansionSlots = new List<GameObject>();
            potentialExpansion = new List<GameObject>();
            this.vertCard = vertCard;
        }
        int vertCard;
        public int boardSize {get;set;}
        public Vector3 topLeft {get;set;}
        public GameObject spotParent {get;set;}
        public GameObject barParent {get;set;}

        public List<GameObject> expansionSlots {get;set;}
        public List<GameObject> potentialExpansion {get;set;}



        public bool checkForExpansion()
        {
            List<GameObject> checkForExpansion = expansionSlots.Count == 0 ? potentialExpansion : expansionSlots;
            for(int i = 0; i < vertCard; ++i)
            {
                if(checkForExpansion[checkForExpansion.Count - 1 - i].GetComponent<cardSpotBehavior>().cardsOnPile.Count > 0)
                {
                    return true;
                }
            }
            return false;
        }
    }
    void Start()
    {
        
    }
    
    public void startManaging()
    {
        cardPlaceHolder = new GameObject();
        cardPlaceHolder.name = "cardPlaceHolder";
        cardBarHolder = new GameObject();
        cardBarHolder.name = "cardBarHolder";
        createTable(playerCount,7);
    }
    public void checkForPlayerExpansion(int player)
    {
        if(player >= boardExpansionList.Count)
        {
            Debug.LogError("Outside player range");
        }
        else
        {
            if(boardExpansionList[player].checkForExpansion())
            {
                
                movePlayer(player,1,1);
                populateTable(Vector3.zero,1,0,0,false,player,false);

            }
        }
    }

    void createTable(int playerCount, int horiCard)
    {

        generateTable(playerCount,horiCard);


        Vector3 currentPosition = playTable.transform.position;
        float xDivide =(int)Math.Ceiling(playerCount/2.0);
        Debug.Log(xDivide);
        float zDivide = 2;
        Vector3 zAdjustment = new Vector3(0,0,playTable.transform.localScale.z/4);
        if(playerCount == 1)
        {
            zAdjustment *= 0;
            zDivide= 1;
        }
        Vector3 xAdjustment = new Vector3(playTable.transform.localScale.x/(xDivide*2),0,0);
        if(playerCount < 3)
        {
            xAdjustment *= 0;
            xDivide = 1;
        }
        if(playerCount > 4)
        {
            currentPosition += xAdjustment * (xDivide - 2);
        }
        Debug.Log(xAdjustment);
        Vector3 extents = playTable.transform.GetComponent<Renderer>().bounds.extents;
        for(int i = 0; i < playerCount; ++i)
        {
            boardExpansionList.Add(new boardInformation(vertCard));
        }
        for(int i = 0; i < playerCount; ++i)
        {
            if(i % 2 == 0)
            {
                populateTable(currentPosition + zAdjustment + xAdjustment,horiCard,extents.x/xDivide,extents.z/zDivide, false,i,true);
            }
            else
            {
                populateTable(currentPosition - zAdjustment + xAdjustment,horiCard,extents.x/xDivide,extents.z/zDivide,true,i,true);
                currentPosition -= xAdjustment*2;
            }
        }
    }
    void generateTable(int playerCount, int horiCardCount)
    {
        Renderer cardRender = cardHolder.transform.GetComponent<Renderer>();
        float horiTableSize = cardRender.bounds.extents.x * 2 * horiCardCount + horiPadding * (horiCardCount-1) + edgePadding*2;
        float vertTableSize = cardRender.bounds.extents.y * 2 * vertCard + vertPadding * (vertCard-1) + edgePadding*2;
        if(playerCount <= 0)
        {
            Debug.LogError("Player Size Cannot Be This Small");
            return;
        }
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Vector3 scaleVector = new Vector3();

        if(playerCount > 1)
        {
            scaleVector += new Vector3(0,1,vertTableSize*2);
        }
        else
        {
            scaleVector += new Vector3(0,1,vertTableSize);
        }

        for(int i = 0; i < playerCount; ++i)
        {
            if(i % 2 == 0)
            {
                scaleVector += new Vector3(horiTableSize,0,0);
            }
           
        }
        cube.transform.localScale = scaleVector;
        playTable = cube;
       // playTable.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        playTable.transform.GetComponent<Renderer>().material = tableTexture;
        cardManager.playTable = playTable;
    }

    void populateTable(Vector3 middlePos, int horiCard, float sectionedXExtents, float sectionedYExtents, bool isOtherSide, int player, bool firstExpansion)
    {
        if(player == actualPlayer && firstExpansion)
        {
            Camera.main.transform.position = new Vector3(middlePos.x,15,middlePos.z);
            if(actualPlayer % 2 != 0)
            {
                Camera.main.transform.Rotate(new Vector3(0,180,0), Space.World);
            }
        }

        
        GameObject newCardHolderParent = boardExpansionList[player].spotParent;
        GameObject cardBarParent = boardExpansionList[player].barParent;
        if(newCardHolderParent == null)
        {
            newCardHolderParent = new GameObject();
            newCardHolderParent.name = $"{player}";
            newCardHolderParent.transform.SetParent(cardPlaceHolder.transform);
            boardExpansionList[player].spotParent = newCardHolderParent;

            cardBarParent = new GameObject();
            cardBarParent.name = $"{player}";
            cardBarParent.transform.SetParent(cardBarHolder.transform);
            boardExpansionList[player].barParent = cardBarParent;
        }
        
        Renderer cardRender = cardHolder.transform.GetComponent<Renderer>();
        float yPos = playTable.transform.position.y + playTable.GetComponent<Renderer>().bounds.extents.y + cardRender.bounds.extents.z;
        if(firstExpansion)
        {
            GameObject handPlacement = new GameObject();

            if(player % 2 == 0)
            {
                handPlacement.transform.position = middlePos + new Vector3(0,20,cardRender.bounds.extents.y * (vertCard + 1));
            }
            else
            {
                handPlacement.transform.position = middlePos - new Vector3(0,20,cardRender.bounds.extents.y * (vertCard + 1));
            }
            cardManager.displayHandPositions.Add(handPlacement);
        }

        Vector3 topLeft;
        if(boardExpansionList[player].boardSize == 0)
        {
            topLeft = new Vector3(middlePos.x + sectionedXExtents - cardRender.bounds.extents.x - edgePadding,yPos,middlePos.z - sectionedYExtents +cardRender.bounds.extents.y + edgePadding );
            boardExpansionList[player].topLeft = topLeft;
        }
        else
        {
            topLeft = boardExpansionList[player].topLeft;
        }


        for(int i = 0, secondval = 0; i < vertCard; ++i)
        {
            if(firstExpansion)
            {
                
                GameObject raycasterObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                raycasterObject.transform.position = new Vector3(middlePos.x,middlePos.y,topLeft.z + i*cardHolder.transform.localScale.y + vertPadding*i);
                raycasterObject.transform.localScale = new Vector3(horiCard*(cardRender.bounds.extents.x*2 + horiPadding),transform.localScale.y,cardRender.bounds.extents.y*2 + vertPadding);
                raycasterObject.name = $"{i}";
                raycasterObject.GetComponent<Renderer>().enabled = false;
                //raycasterObject.transform.parent = playTable.transform;
                raycasterObject.layer = LayerMask.NameToLayer("Table");
                raycasterObject.AddComponent<cardList>();
                raycasterObject.transform.SetParent(cardBarParent.transform);

            }
            
            if( i == 0 && isOtherSide && secondval == 0 && firstExpansion)
            {
                secondval = 3;
            }
            else if(!isOtherSide && vertCard-i <= 3 && secondval == 0 && firstExpansion)
            {
                ++secondval;
            }
            for(int a = 0; a < horiCard; ++a)
            {
                float yRotation = player % 2 == 0? 0 :180;
                GameObject newlyInstantiatedCard = Instantiate(cardHolder,new Vector3(topLeft.x - (a + boardExpansionList[player].boardSize) *cardHolder.transform.localScale.x - horiPadding*(a + boardExpansionList[player].boardSize),topLeft.y,topLeft.z + i*cardHolder.transform.localScale.y + vertPadding*i), Quaternion.Euler(90,yRotation,0));
                cardSpotBehavior spotBehavior = newlyInstantiatedCard.AddComponent<cardSpotBehavior>();
                spotBehavior.card = playingCardObject;
                if(a == 0 && secondval < 4 && firstExpansion)
                {
                    spotBehavior.horipadding = 0;
                    spotBehavior.vertpadding = 0;

                    if(secondval == 1) // Exile
                    {
                        newlyInstantiatedCard.transform.name = $"Exile Holder Position {i},{a}";
                        //newlyInstantiatedCard.GetComponent<Renderer>().material.SetColor("_Color", Color.green);
                        secondval += isOtherSide ? -1 : 1;
                        spotBehavior.deckType = 'E';
                    }
                    else if(secondval == 2) // Graveyard
                    {
                        newlyInstantiatedCard.transform.name = $"Graveyard Holder Position {i},{a}";
                        //newlyInstantiatedCard.GetComponent<Renderer>().material.SetColor("_Color", Color.blue);
                        secondval += isOtherSide ? -1 : 1;
                        spotBehavior.deckType = 'G';

                    }
                    else if(secondval == 3) //Deck
                    {
                        newlyInstantiatedCard.transform.name = $"Deck Holder Position {i},{a}";
                        //newlyInstantiatedCard.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                        secondval += isOtherSide ? -1 : 1;
                        
                            cardManager.deckScripts.Add(spotBehavior);
                        
                        spotBehavior.deckType = 'D';

                    }
                    spotBehavior.pileLimit = 200;
                }
                else
                {
                    newlyInstantiatedCard.transform.name = $"Card Holder Position {i},{boardExpansionList[player].boardSize + a}";
                    //newlyInstantiatedCard.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                    spotBehavior.deckType = 'N';


                }
                newlyInstantiatedCard.GetComponent<Renderer>().material.mainTexture = cardHolderTexture;

                //newlyInstantiatedCard.transform.parent = raycasterObject.transform;
                cardBarParent.transform.GetChild(i).GetComponent<cardList>().addToList(newlyInstantiatedCard);
                newlyInstantiatedCard.transform.SetParent(newCardHolderParent.transform);
                cardHolderDictionary.Add(cardID,spotBehavior);
                spotBehavior.playerOwner = player;
                spotBehavior.hashCode = cardID++;
                if(!firstExpansion)
                {
                    boardExpansionList[player].expansionSlots.Add(newlyInstantiatedCard);
                }
                //newlyInstantiatedCard.GetComponent<Renderer>().enabled = false;
                if(a == horiCard - 1 && firstExpansion)
                {
                    boardExpansionList[player].potentialExpansion.Add(newlyInstantiatedCard);
                }
            }
        }
        boardExpansionList[player].boardSize += horiCard;
        cardManager.horiFloor = yPos;
    }


    

    public bool potentiallyDeleteSlots(int player)
    {
        bool deleted = false;
        while(deleteExtraSlots(player))
        {
            movePlayer(player,1,-1);
            deleted = true;
        }
        return deleted;
    }

    bool deleteExtraSlots(int player)
    {
        if(player >= boardExpansionList.Count)
        {
            return false;
        }

        for(int i = 0; i < vertCard; ++i)
        {
            
            if(boardExpansionList[player].expansionSlots.Count != 0)
            {
                //Debug.Log(boardExpansionList[player].expansionSlots[i].name);
                if(boardExpansionList[player].expansionSlots[boardExpansionList[player].expansionSlots.Count - 1-i].GetComponent<cardSpotBehavior>().cardsOnPile.Count != 0)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            
        }

        for(int i = 0; i < vertCard; ++i)
        {
            if(boardExpansionList[player].expansionSlots.Count != 0)
            {
                GameObject toBeDestroyed = boardExpansionList[player].expansionSlots[boardExpansionList[player].expansionSlots.Count - 1];
                boardExpansionList[player].expansionSlots.Remove(toBeDestroyed);
                GameObject.Destroy(toBeDestroyed);
            }
            else
            {
                return false;
            }
        }
        boardExpansionList[player].boardSize -= 1;
        
        return true;
    }

    public void allSlotCheck()
    {
        for(int i =0 ;i < playerCount; ++i)
        {
            potentiallyDeleteSlots(i);
            checkForPlayerExpansion(i);
        }
    }


    public void movePlayer(int avoidMove, int moveAmount, int reverse)
    {
        bool changedValue = false;
        foreach(Transform child in cardPlaceHolder.transform)
        {
            int playerNumber = Int32.Parse(child.transform.name);
            if(playerNumber == avoidMove || playerNumber < avoidMove)
            {
                if(playerNumber == avoidMove)
                {
                    foreach(Transform bar in boardExpansionList[avoidMove].barParent.transform)
                    {
                        bar.transform.localScale += new Vector3(moveAmount * cardHolder.transform.localScale.x + horiPadding*moveAmount,0,0) * reverse;
                        bar.transform.position -= new Vector3(moveAmount * cardHolder.transform.localScale.x + horiPadding*moveAmount,0,0)/2 * reverse;
                        if(reverse == -1)
                        {
                            int cardCode = bar.GetComponent<cardList>().removeFromList();
                            GameObject.Destroy(cardHolderDictionary[cardCode]);
                            cardHolderDictionary.Remove(cardCode);
                        }
                    }

                }
                continue;
            }

            if((avoidMove % 2 != 0 && playerNumber % 2 != 0) || (avoidMove % 2 == 0 && playerNumber % 2 == 0))
            {
                child.transform.position -= new Vector3(moveAmount * cardHolder.transform.localScale.x + horiPadding*moveAmount,0,0) * reverse;
                boardExpansionList[playerNumber].barParent.transform.position -= new Vector3(moveAmount * cardHolder.transform.localScale.x + horiPadding*moveAmount,0,0) * reverse;

                changedValue = true;
            }
        }

        if(changedValue)
        {
            if(avoidMove % 2 == 0 && (topExpansion > bottomExpansion || (reverse == -1 && bottomExpansion <= topExpansion)))
            {
                bottomExpansion += 1 * reverse;
            }
            else if(avoidMove % 2 != 0 && (bottomExpansion > topExpansion || (reverse == -1 && bottomExpansion >= topExpansion)))
            {
                topExpansion += 1 * reverse;
            }
            else
            {
                playTable.transform.localScale += new Vector3(moveAmount * cardHolder.transform.localScale.x + horiPadding*moveAmount,0,0) * reverse;
                playTable.transform.position -= new Vector3(moveAmount * cardHolder.transform.localScale.x + horiPadding*moveAmount,0,0)/2  * reverse;
                if(avoidMove % 2 == 0)
                {
                    bottomExpansion += 1 * reverse;
                }
                else
                {
                    topExpansion += 1 * reverse;
                }
            }
            //int multValue = avoidMove % 2 != 0? -1: 1;
            
        }
        
    }
    public Vector3 findClosestCard(Vector3 position, Transform searchedBar, cardPlaying cp, GameObject heldCard, GameObject cardOnField)
    {
        float distance = -1.0f;
        Vector3 retPos = Vector3.zero;
        GameObject closestObject = null;
        foreach(GameObject GO in searchedBar.transform.GetComponent<cardList>().cardHolderList)
        {
            Transform child = GO.transform;
            // if(GameObject.Equals(child.gameObject, heldCard))
            // {
            //     continue;
            // }
            cardSpotBehavior transformScript = child.GetComponent<cardSpotBehavior>();
            if(!transformScript.isFull() || GameObject.Equals(child.gameObject, heldCard))
            {
                Vector3 newPos = transformScript.getNewPilePosition(-1,cardOnField);
                if(distance == -1.0f)
                {
                    distance = Vector3.Distance(newPos,position);
                    retPos = newPos;
                    closestObject = child.gameObject;
                }
                else
                {
                    
                    float distanceFromPosition = Vector3.Distance(newPos,position);
                    if(distanceFromPosition < distance)
                    {
                        distance = distanceFromPosition;
                        retPos = newPos;
                        closestObject = child.gameObject;

                    }
                    
                }
            }
        }
        if(distance == -1)
        {
            Debug.LogError("There were no card spots present");
        }
        cp.setLastPositionObject(closestObject);
        return retPos;
    }


}
