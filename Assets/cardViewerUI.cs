using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardViewerUI : MonoBehaviour
{
    // Start is called before the first frame update


    float horiPadding = .1f;
    float vertPadding = .1f;
    GameObject copyCard = null;
    RectTransform rt;

    void Start()
    {
        rt = transform.GetComponent<RectTransform>();
    }
    void Update()
    {
        
    }

    public void setImage(GameObject  card)
    {
        if(copyCard != null)
        {
            GameObject.Destroy(copyCard);
        }

        copyCard = Instantiate(card);
        copyCard.SetActive(true);
        RectTransform cardSize = copyCard.GetComponent<RectTransform>();

        float calculatedXPadding = Screen.width*(1-rt.anchorMin.x);
        float calculatedYPadding = Screen.height*(1-rt.anchorMin.y);

        float actualXSpace = calculatedXPadding * (1-horiPadding*2);
        float actualYSpace = calculatedYPadding * (1-vertPadding*2);


        float actualXScale = cardSize.localScale.x/actualXSpace;
        copyCard.transform.localScale = new Vector3(actualXSpace/cardSize.sizeDelta.x,actualYSpace/cardSize.sizeDelta.y,0.2f);

        float xOffset = (cardSize.sizeDelta.x*cardSize.localScale.x)/2;
        float yOffset = (cardSize.sizeDelta.y*cardSize.localScale.y)/2;
 
        
        copyCard.transform.position = new Vector3(Screen.width - xOffset - calculatedXPadding * horiPadding,Screen.height -yOffset - calculatedYPadding*vertPadding,0);
        copyCard.transform.localEulerAngles = new Vector3(0,0,0);
        copyCard.transform.SetParent(transform);


    }
}
