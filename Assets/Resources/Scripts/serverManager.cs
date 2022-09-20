using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net.Sockets;

public class serverManager : MonoBehaviour
{
    TcpClient client;
    NetworkStream rwStream;
    [HideInInspector] public String uuid;
    
    int bufferSize = 4096;
    public gameController gameController;
    public serverConnectUI serverConnectUI;
    [HideInInspector] public static string username;

    string messageBuffer = "";

    // Start is called before the first frame update
    void Start()
    {
        client = null;
        uuid = System.Guid.NewGuid().ToString();
        gameController.uuid = uuid;
        //Connect("localhost","connection",54000);
    }

    public int Connect(string server, string message, int port)
    {
        try
        {
            // Create a TcpClient.
            // Note, for this client to work you need to have a TcpServer
            // connected to the same address as specified by the server, port
            // combination.
            client = new TcpClient(server, port);

            // Translate the passed message into ASCII and store it as a Byte array.
            message = addMessageSize(message);
            byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            // Get a client stream for reading and writing.
        //  Stream stream = client.GetStream();

            NetworkStream stream = client.GetStream();

            // Send the message to the connected TcpServer.
            stream.Write(data, 0, data.Length);

            Debug.Log($"Sent: {message}");
            rwStream = client.GetStream();

            // Receive the TcpServer.response.
            return 1;
        }
        catch (ArgumentNullException e)
        {
            Debug.Log($"ArgumentNullException: {e}");
            return -1;
        }
        catch (SocketException e)
        {
            Debug.Log($"SocketException: {e}");
            return -2;
        }
    }

    // Update is called once per frame
    void Update()
    {
        readSocketData();
        parseSocketData();
         
    }


    void readSocketData()
    {
        if(rwStream != null  && rwStream.DataAvailable)
        {
            string completeMessage = string.Empty;
            
            // Buffer to store the response bytes.
            byte[] data = new byte[bufferSize];

            do
            {

                int dataSize = rwStream.Read(data,0,data.Length);
                completeMessage += System.Text.Encoding.UTF8.GetString(data, 0, dataSize);
            }while(rwStream.DataAvailable); // Read entire stream in case buffer is too small

            // Data format {4 Byte Command Length 32 Byte UUID|2 Byte OpCode| Up to 4060 Byte Instruction}
            // Example [00442bc7f400-c637-462b-b28c-83ce20e74692|00|Foulgaze] A connection message from a new user named Foulgaze, with length 44 bytes
            messageBuffer += completeMessage;
            Debug.Log($"Received: {completeMessage}");
            

        }
    }
    
    void parseSocketData()
    {
        while(true)
        {
            string messageLength = "";
            int messageLengthRemaining = 0;
            int i = 0;
            while(messageLength.Length != 4)
            {
                if(i >= messageBuffer.Length)
                {
                    return;
                }

                messageLength = messageLength + messageBuffer[i++];
            }
            
            messageLengthRemaining = Int32.Parse(messageLength);
            
            
            string currentCommand = "";
            if(messageLengthRemaining > messageBuffer.Length - 4)
            {
                return;
            }
            
            currentCommand = messageBuffer.Substring(i,messageLengthRemaining);
            parseCommand(currentCommand);
            messageBuffer = messageBuffer.Substring(i+messageLengthRemaining);
            
        }
        
    }

    public void parseCommand(string completeMessage)
    {
        // Parsing recieved message into UUID, opCode, and Content

        int breakPos = completeMessage.IndexOf("|");
            string msgUUID = completeMessage.Substring(0,breakPos);
            int opCode = Int32.Parse(completeMessage.Substring(breakPos+1,2));
            string instruction = completeMessage.Substring(breakPos+4); 

            // Doing action based on opCode
            switch(opCode)
            {
                default:
                {
                    Debug.LogError($"The program has recieved an unknown opCode of [{opCode}]");
                    break;
                }
                case 0: // This is when a new player connects to the server. It sets username if necessary, and sends message in chatbox
                {
                    gameController.addToPlayerList(uuid == msgUUID, instruction,msgUUID);
                    if(uuid == msgUUID)
                    {
                        username = instruction;
                    }
                    else
                    {
                        sendMessage($"{gameController.readyUpCount},{gameController.readyUpTotal},{username},{msgUUID}","01","05");
                    }
                    serverConnectUI.addMessage($"{instruction} Has Connected!");
                    serverConnectUI.updateCurrentPlayerList(gameController.getPlayerList());
                    break;                    
                } 
                case 1: // This is when a player sends a chat message. It adds the chat message to the chatbox.
                {
                    serverConnectUI.addMessage(instruction);
                    break;
                }
                case 2: // This is when a player disconnects. It removes them from the gameController and sends a message in the chat
                {
                    serverConnectUI.addMessage($"{instruction}  Has Disconnected!");
                    gameController.removeFromPlayerList(instruction);
                    serverConnectUI.updateCurrentPlayerList(gameController.getPlayerList());
                    break;
                }
                case 3: // When a player readies up
                {
                    serverConnectUI.addMessage(instruction);
                    gameController.readyUpPlayer(msgUUID);
                    break;
                }
                case 4: // When a player unreadies up
                {
                    serverConnectUI.addMessage(instruction);
                    gameController.unreadyUpPlayer(msgUUID);
                    break;
                }
                case 5: 
                {
                    string[] playerAddMessage = instruction.Split(',');

                    if(playerAddMessage[playerAddMessage.Length-1] == uuid) //UUID 
                    {
                        int localReadyTotal = -1;
                        int localReadyUpCount = -1;
                        if(!Int32.TryParse(playerAddMessage[0],out localReadyUpCount) || !Int32.TryParse(playerAddMessage[1],out localReadyTotal))
                        {
                            Debug.LogError($"Probleming parsing readyUpCount: {playerAddMessage[0]}, readyUpTotal: {playerAddMessage[1]}");
                        }
                        gameController.readyUpTotal = localReadyTotal;
                        gameController.readyUpCount = localReadyUpCount;
                        gameController.addToPlayerList(false, playerAddMessage[2],msgUUID);
                        serverConnectUI.updateCurrentPlayerList(gameController.getPlayerList());
                    }
                    break;
                }
                case 6:
                {
                    serverConnectUI.parseTextCommand(instruction + $" {msgUUID}");
                    break;
                }
                case 7:
                {
                    gameController.parseDeck(instruction, msgUUID);
                    break;
                }
                case 8:
                {
                    string[] playerMessage = instruction.Split(',');
                    gameController.releaseCardOnField(Int32.Parse(playerMessage[0]),Int32.Parse(playerMessage[1]),Int32.Parse(playerMessage[2]),uuid);
                    break;
                }
                case 9:
                {
                    string[] playerMessage = instruction.Split(',');
                    gameController.addCardToHand(Int32.Parse(playerMessage[0]),Int32.Parse(playerMessage[1]),Int32.Parse(playerMessage[2]),Int32.Parse(playerMessage[3]),msgUUID.Equals(uuid));
                    break;
                }
            }
    }


    public void sendMessage(string text, string serverOpCode, string clientOpCode) // Sends a message to the server. The send format is {uuid opcode message} The spaces are not present 
    {
        string message = $"{uuid}|{serverOpCode}|{clientOpCode}|{text}";
        byte[] data = System.Text.Encoding.ASCII.GetBytes(addMessageSize(message));
        rwStream.Write(data, 0, data.Length);
    }

    public string addMessageSize(string message)
    {
        string msgByteSize = message.Length.ToString();
        if(msgByteSize.Length > 4)
        {
            Debug.LogError("The message was too large");
        }
        while(msgByteSize.Length != 4)
        {
            msgByteSize = "0" + msgByteSize;
        }
        return msgByteSize + message;
    }
}
