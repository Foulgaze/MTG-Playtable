using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class serverConnectUI : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public serverManager serverManager;
    public gameController gameController;
    public deckEntryController deckEntryController;
    public loadController loadController;
    public GameObject mainMenu;
    public GameObject intermediaryMenu;
    public GameObject gameplayMenu;
    public GameObject chatBox;
    public GameObject errorMsg;
    public GameObject loadMenu;

    public chatboxController chatboxController;
    

    void Start()
    {
        disableAll();
        mainMenu.SetActive(true);
    }
    void disableAll()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void goOnline()
    {
        string username = usernameInput.text;
        if(username != "")
        {
            if(serverManager.Connect("localhost",$"{serverManager.uuid}|00|00|{username}", 54000) == 1)
            {
                mainMenu.SetActive(false);
                intermediaryMenu.SetActive(true);
                chatBox.SetActive(true);
            }
            else
            {
                errorMsg.GetComponent<TextMeshProUGUI>().SetText( "Connection Error :(");
                StartCoroutine(showError(errorMsg,3));
            }
        }
        else
        {
                errorMsg.GetComponent<TextMeshProUGUI>().SetText("You must enter a username!");
                StartCoroutine(showError(errorMsg,3));
        }
    }

    public string getUsername()
    {
        return serverManager.username;
    }

    public string getDeckList()
    {
        return deckEntryController.getDeckList();
    }


    public void switchToPlay()
    {
        disableAll();
        gameplayMenu.SetActive(true);
        chatBox.SetActive(true);
    }

    public void setMax(string uuid, int newReadyUpTotal)
    {
        gameController.setMax(uuid,newReadyUpTotal);
    }

    public IEnumerator showError(GameObject timeoutObj, int waitTime)
    {
        timeoutObj.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        timeoutObj.SetActive(false);

    }

    public void addMessage(string msg)
    {
        chatboxController.addMessage(msg);
    }

    public void parseTextCommand(string msg)
    {
        chatboxController.parseTextCommand(msg, false);
    }

    public void updateCurrentPlayerList(List<Player> playerList)
    {
        deckEntryController.updatePlayerTextElement(playerList);
    }

    public void sendTCPToServer(string msg, string serverOpCode, string clientOpCode)
    {
        serverManager.sendMessage(msg,serverOpCode,clientOpCode);
    }

    public void readyUp(string cardList)
    {
        //gameController.loadCards(cardList);
    }

    public void setBar(float f)
    {
        loadController.setBar(f);
    }

    public void switchToLoad()
    {
        disableAll();
        loadMenu.SetActive(true);
    }


}
