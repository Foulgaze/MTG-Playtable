using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cardOnFieldBehavior : MonoBehaviour
{
    // Start is called before the first frame update
    public cardPlaying handManagerScript;
    public GameObject cardHoldPosition;
    public GameObject handRepresentation;
    public cardSelectionMenu selectionMenu;

    public cardSpotBehavior spotBehavior;

    public cardViewerUI viewerUI;
    public menuController menuController;
    public bool isHeld = false;
    float holdTime = 0;
    int clickCount = 0;

    int tapClickCount = 0;
    public bool sideways = false;

    public float holdCheck = 0.1f;
    public bool flipped = false;

    public int playerOwner;

    float clickRemoveTime = 0;
    public char ownerType = 'N';
    public int hashCode;

    public GameObject siblingCard;

    void Start()
    {
    }



    void OnMouseOver()
    {
        setImageViewer();
        CheckForHold();
        CheckForRightClick();
    }

    void OnMouseExit()
    {
        clickCount = 0;
        holdTime = 0;
    }

    void setImageViewer()
    {
        if(!isHeld && ownerType != 'D')
        {
            viewerUI.setImage(siblingCard);
        }
    }
    
    void CheckForHold()
    {
        if(!isHeld && handManagerScript.player == playerOwner)
        {
            
            if(tapClickCount > 0)
            {
                clickRemoveTime += Time.deltaTime;
            }
            else
            {
                clickRemoveTime = 0;
            }

            

            if(Input.GetMouseButtonDown(0))
            {
                ++clickCount;
                if(ownerType != 'D')
                {
                    ++tapClickCount;
                }
            }

            if(Input.GetMouseButton(0) && clickCount != 0)
            {
                holdTime += Time.deltaTime;
            }
            else
            {
                holdTime = 0;
            }

            if(tapClickCount == 2)
            {
                sideways = !sideways;
                setRotation();
                holdTime = 0;
                tapClickCount = 0;
            }

            if(holdTime > holdCheck)
            {
                isHeld = true;
                handManagerScript.moveOnFieldCard(transform.gameObject,siblingCard,cardHoldPosition);
            }

            if(clickRemoveTime > holdCheck*2)
            {
                tapClickCount = 0;
            }

            
        }
        
        
    }


    public void setRotation()
    {
        Vector3 newAngles = Vector3.zero;
        if(flipped)
        {
            newAngles.x = 90;
        }
        else
        {
            newAngles.x = -90;
        }
        if(playerOwner % 2 == 0)
        {
            if(sideways)
            {
                newAngles.z =90;
            }
            if(flipped)
            {
                newAngles.z = 180;
            }
            if(sideways && flipped)
            {
                newAngles.z = 90;
            }
        }
        else
        {
            newAngles.z = 180;
            if(sideways)
            {
                newAngles.z = 270;
            }
            if(ownerType == 'D' || flipped)
            {
                newAngles.z = 0;
                if(sideways)
                {
                    newAngles.z = 270;
                }
            }


        }
        transform.eulerAngles = newAngles;
                
    }

    void CheckForRightClick()
    {
        if(!isHeld && Input.GetMouseButtonDown(1) && !Input.GetMouseButton(0) && handManagerScript.player == playerOwner)
        {
            menuController.lastSelectedCard = transform.gameObject;
            menuController.makeMenuAppear();
        }
    }
    void Update()
    {
    }

    
    void checkForRelease()
    {

    }

    public void UIActivate()
    {
        siblingCard.SetActive(true);
        this.gameObject.SetActive(false);
    }

    public void CardActivate()
    {
        siblingCard.SetActive(false);
        this.gameObject.SetActive(true);
    }

    public GameObject getUICard()
    {
        return siblingCard;
    }
}
