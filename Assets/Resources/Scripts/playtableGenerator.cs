using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class playtableGenerator : MonoBehaviour
{
    float horiPadding = 1;
    float vertPadding = 1;
    int edgePadding = 3;
    public GameObject cardHolder;
    [HideInInspector]
    public GameObject playTable;

    public cardPlaying cardManager; 
  
    

    void Start()
    {
        createTable();
    }

    void createTable()
    {
        int horiCard = 6;
        int vertCard = 3;
        int playerCount = 1;
        generateTable(playerCount,horiCard,vertCard);


        Vector3 currentPosition = playTable.transform.position;
        float xDivide =(int)Math.Ceiling(playerCount/2.0);
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
        Vector3 extents = playTable.transform.GetComponent<Renderer>().bounds.extents;
        for(int i = 0; i < playerCount; ++i)
        {
            if(i % 2 == 0)
            {
                populateTabe(currentPosition + zAdjustment + xAdjustment,horiCard,vertCard,extents.x/xDivide,extents.z/zDivide);
            }
            else
            {
                populateTabe(currentPosition - zAdjustment + xAdjustment,horiCard,vertCard,extents.x/xDivide,extents.z/zDivide);
                currentPosition -= xAdjustment*2;
            }
        }
    }
    void generateTable(int playerCount, int horiCardCount, int vertCardCount)
    {
        Renderer cardRender = cardHolder.transform.GetComponent<Renderer>();
        float horiTableSize = cardRender.bounds.extents.x * 2 * horiCardCount + horiPadding * (horiCardCount-1) + edgePadding*2;
        float vertTableSize = cardRender.bounds.extents.y * 2 * vertCardCount + vertPadding * (vertCardCount-1) + edgePadding*2;
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
        Debug.Log(scaleVector);
        cube.transform.localScale = scaleVector;
        playTable = cube;
        playTable.transform.GetComponent<Renderer>().material.SetColor("_Color", Color.black);
        cardManager.playTable = playTable;
    }

    void populateTabe(Vector3 middlePos, int horiCard, int vertCard, float sectionedXExtents, float sectionedYExtents)
    {
        Renderer cardRender = cardHolder.transform.GetComponent<Renderer>();
        float yPos = playTable.transform.position.y + playTable.GetComponent<Renderer>().bounds.extents.y + cardRender.bounds.extents.z;
        Vector3 topLeft = new Vector3(middlePos.x + sectionedXExtents - cardRender.bounds.extents.x - edgePadding,yPos,middlePos.z - sectionedYExtents +cardRender.bounds.extents.y + edgePadding );
        for(int i = 0; i < vertCard; ++i)
        {
            GameObject raycasterObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            raycasterObject.transform.position = new Vector3(middlePos.x,middlePos.y,topLeft.z + i*cardHolder.transform.localScale.y + vertPadding*i);
            raycasterObject.transform.localScale = new Vector3(horiCard*(cardRender.bounds.extents.x*2 + horiPadding),transform.localScale.y,cardRender.bounds.extents.y*2 + vertPadding);
            raycasterObject.name = $"{i}";
            raycasterObject.GetComponent<Renderer>().enabled = false;
            raycasterObject.transform.parent = playTable.transform;
            raycasterObject.layer = LayerMask.NameToLayer("Table");
            for(int a = 0; a < horiCard; ++a)
            {
                GameObject temp = Instantiate(cardHolder,new Vector3(topLeft.x - a*cardHolder.transform.localScale.x - horiPadding*a,topLeft.y,topLeft.z + i*cardHolder.transform.localScale.y + vertPadding*i), Quaternion.Euler(90,0,0));
                temp.GetComponent<Renderer>().material.SetColor("_Color", Color.yellow);
                temp.transform.parent = raycasterObject.transform;
                //temp.GetComponent<Renderer>().enabled = false;
            }
        }
        
        cardManager.horiFloor = yPos;
    }


    public Vector3 findClosestCard(Vector3 position, Transform searchedBar)
    {
        float distance = -1.0f;
        Vector3 retPos = Vector3.zero;
        foreach(Transform child in searchedBar.transform)
        {
            if(distance == -1.0f)
            {
                distance = Vector3.Distance(child.transform.position,position);
                retPos = child.transform.position;
            }
            else
            {
                float distanceFromPosition = Vector3.Distance(child.transform.position,position);
                if(distanceFromPosition < distance)
                {
                    distance = distanceFromPosition;
                    retPos = child.transform.position;
                }
                
            }
        }
        if(distance == -1)
        {
            Debug.LogError("There were no card spots present");
        }
        retPos.y += cardHolder.GetComponent<Renderer>().bounds.extents.z*2;
        return retPos;
    }
}
