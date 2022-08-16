using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
public class chatboxController : MonoBehaviour
{
    public TMP_InputField chatInput;
    public TextMeshProUGUI chatbox;
    //public serverManager serverManager;

    public serverConnectUI serverConnectUI;

    bool selected;

    // Update is called once per frame
    void Update()
    {
        if(selected && Input.GetKeyUp(KeyCode.Return))
        {
            if(chatInput.text[0] == '/')
            {
                parseTextCommand(chatInput.text);
            }
            else
            {
                serverConnectUI.sendTCPToServer($"[{serverManager.username}]: {chatInput.text}", "01","01");
            }
            chatInput.text = "";
            chatInput.ActivateInputField();
        }
    }


    public void parseTextCommand(string msg, bool isLocal = true)
    {
        string[] msgCommands = msg.Split(" ");
        string modifiedMsg = msgCommands[0];
        switch(modifiedMsg)
        {
            default:
            {
                addMessage($"[{msg}] is not a valid command. Type /help for a list of commands");
                break;
            }
            case "/help":
            {
                addMessage(@"{/lobbySize [int]} : changes the lobby size to star the game\n
                             {");
                break;
            }
            case "/lobbySize":
            {
                int newSize;
                if(msgCommands.Length > 1 && int.TryParse(msgCommands[1],out newSize) && newSize < 10)
                {
                    if(isLocal)
                    {
                        serverConnectUI.sendTCPToServer(msg,"01","06");
                    }
                    else
                    {
                        serverConnectUI.setMax(msgCommands[msgCommands.Length-1],newSize);
                    }
                }
                else
                {
                    goto default;
                }
                break;
            }
            case "/roll":
            {
                int diceCount;
                int amount;
                if(msgCommands.Length > 1 && int.TryParse(msgCommands[1], out amount))
                {
                    if(msgCommands.Length > 2)
                    {
                        if(int.TryParse(msgCommands[2],out diceCount))
                        {

                        }
                        else
                        {
                            goto default;
                        }
                    } 
                    else if(msgCommands.Length == 2)
                    {
                        diceCount = 1;
                    }  
                    else
                    {
                        goto default;
                    }
                    diceCount = Math.Abs(diceCount);
                    amount = Math.Abs(amount);
                    if(amount > 1000 || diceCount > 100)
                    {
                        goto default;
                    }
                    string diceMessage = $"[{serverConnectUI.getUsername()}]: Has Rolled a d{amount}";
                    if(diceCount > 1)
                    {
                        diceMessage += $" {diceCount} times";
                    }
                    diceMessage += "\n";
                    for(int i = 0; i < diceCount; ++i)
                    {
                        diceMessage += $"[{i+1}/{diceCount}] : {UnityEngine.Random.Range(1,amount+1)}";
                        if(i != diceCount - 1)
                        {
                            diceMessage += "\n";
                        }
                    }
                    serverConnectUI.sendTCPToServer(diceMessage,"01","01");
                    
                }
                else
                {
                    goto default;
                }
                break;
            }
        }
    }

    public void addMessage(string message)
    {
        chatbox.SetText(chatbox.text + "\n" + message);
    }

    public void select()
    {
        selected = true;
    }
    public void deselect()
    {
        selected = false;
    }
}
