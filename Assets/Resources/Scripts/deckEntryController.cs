using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using System;
public class deckEntryController : MonoBehaviour
{
    // Start is called before the first frame update
    public serverConnectUI serverConnectUI;
    public TMP_InputField decklistInputField;
    public TextMeshProUGUI currentPlayerTextElement;
    public Button readyUpBtn;
    public TextMeshProUGUI readyUpBtnTxt;
    public GameObject errorMsg;
    void Start()
    {
        errorMsg.SetActive(false);
    }
    public void updatePlayerTextElement(List<Player> playerList)
    {
        string playerListDisplay = $"Connected Players ({playerList.Count}): \n";
        for(int i = 0; i < playerList.Count; ++i)
        {
            playerListDisplay += playerList[i].username + "\n";
        }
        currentPlayerTextElement.text = playerListDisplay;
    }

    public void openFileSelector()
    {
        // if(!decklistInputField.readOnly)
        // {
        //     string path = EditorUtility.OpenFilePanel("MTG Deck", "", "txt");
        //     string fileData = "";
        //     if(path != "")
        //     {
        //         try
        //         {
        //             fileData = System.IO.File.ReadAllText(path);
        //         }
        //         catch(Exception e)
        //         {
        //             errorMsg.GetComponent<TextMeshProUGUI>().text = "There was an error retriving the file!";
        //             serverConnectUI.showError(errorMsg, 2);
        //             Debug.LogError(e);
        //         }
        //     }

        //     if(fileData != "")
        //     {
        //         decklistInputField.text = fileData;
        //     }
        // }
        
    }
    public string getDeckList()
    {
        return decklistInputField.text;
    }
    public void readyUp()
    {
        
        if(decklistInputField.readOnly)
        {
            decklistInputField.readOnly = false;
            readyUpBtnTxt.text = "Ready Up!";
            serverConnectUI.sendTCPToServer($"{serverManager.username} Is No Longer Ready!","01","04");

        }
        else
        {
            decklistInputField.readOnly = true;
            readyUpBtnTxt.text = "UnReady Up!";
            serverConnectUI.sendTCPToServer($"{serverManager.username} Is Ready!","01","03");
            // Tell Servers
        }
        // serverConnectUI.readyUp(decklistInputField.text);
    }

}
