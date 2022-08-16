using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class customScroll : MonoBehaviour
{
    
    public GameObject scrollHandle;
    [HideInInspector]
    public bool scrolling = false;
    RectTransform positionOfHandle;
    float mouseInitialXValue = 0;
    public GameObject canvasObject;
    public RectTransform objectToBeScrolled;
    public cardPlaying handController;
    Canvas scaleCanvas;

    float referenceWidth;

    void Start()
    {
        positionOfHandle = scrollHandle.transform.GetComponent<RectTransform>();
        scaleCanvas = canvasObject.GetComponent<Canvas>();
        referenceWidth = canvasObject.GetComponent<CanvasScaler>().referenceResolution.x/2;
        Debug.Log(referenceWidth);
    }

    public void startScrolling()
    {
        scrolling = true;
        mouseInitialXValue = Input.mousePosition.x/scaleCanvas.scaleFactor - positionOfHandle.anchoredPosition.x;
    }

    void Update()
    {
        updateScroll();
    }

    void updateScroll()
    {
        if(Input.GetMouseButton(0) && scrolling)
        {
            Vector2 newPosition = new Vector2(Input.mousePosition.x/scaleCanvas.scaleFactor - mouseInitialXValue,positionOfHandle.anchoredPosition.y);
            if(newPosition.x > referenceWidth*-1 && newPosition.x < referenceWidth)
            {
                Vector2 cardHolderPosition = (newPosition - positionOfHandle.anchoredPosition);
                cardHolderPosition.x +=  Mathf.Max(1,((handController.testCard.width*0.2f)*handController.hand.Count-(referenceWidth*2)/(handController.testCard.width*0.2f)));
                objectToBeScrolled.anchoredPosition -= cardHolderPosition;
                positionOfHandle.anchoredPosition = newPosition;
            }
        }
        else
        {
            scrolling = false;
        }
    }

    public void resetScroll()
    {
        scrolling = false;
        positionOfHandle.anchoredPosition = new Vector2(0,0);
        objectToBeScrolled.anchoredPosition = new Vector2(0,0);

    }
}
