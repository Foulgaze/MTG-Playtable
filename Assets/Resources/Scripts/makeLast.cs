using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class makeLast : EventTrigger, IPointerExitHandler
{
    public GameObject handController;
    public int handPosition;
    public override void OnPointerEnter(PointerEventData data)
    {
        // Debug.Log($"{transform.name} entered");
        
        // transform.SetAsLastSibling();
        
        
    }
    
    public override void OnPointerExit(PointerEventData data)
    {
        handController.GetComponent<cardPlaying>().updateHand();
        //transform.SetAsFirstSibling();
    }
}
