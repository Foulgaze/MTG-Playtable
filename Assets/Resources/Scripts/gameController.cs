using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

public class gameController : MonoBehaviour
{
    // Start is called before the first frame update
    List<Player> playerList = new List<Player>();
    Dictionary<string,string> cardRawJSON = new Dictionary<string, string>();
    Dictionary<string, Card> cardDict = new Dictionary<string, Card>();
    Dictionary<string, CardInfo> cardInfoDict = new Dictionary<string, CardInfo>();
    Dictionary<string, Texture2D> cardPictureDict = new Dictionary<string, Texture2D>();
    Dictionary<string, Texture2D> playableCardDict = new Dictionary<string, Texture2D>();

    public int readyUpCount = 0;
    public int readyUpTotal = 2; // Arbitrary Number
    GameObject plane;
    public Texture2D cardback;
    public serverConnectUI serverConnectUI;
    public GameObject card;

    public playtableGenerator playtableGenerator;
    public cardPlaying cardPlaying;

    bool isJSONFileLoaded = false;

    HashSet<string> uuidList = new HashSet<string>();
    HashSet<string> nameList = new HashSet<string>();

    public string uuid;

    int individualDeckLoad =0 ;
    int cardsLoaded = 0;

    int playerNumber;
    async void Start()
    {
        //readFile();
        //readFileSync();
        //StartCoroutine(loadCards());
    }

    public void addToPlayerList(bool currentPlayer,string name,string uuid)
    {
        Player insertPlayer = new Player();
        insertPlayer.username = name;
        insertPlayer.uuid = uuid;
        playerList.Add(insertPlayer);
    }


    public void readFileSync()
    {
        string result;
        using (StreamReader reader = File.OpenText(Application.dataPath + "/StreamingAssets/cardJson/default-cards-20220811090446.json"))
        {
            while((result = reader.ReadLine()) != null)
            {
                if(result.Length > 1)
                {
                    //CardInfo newCardInfo = new CardInfo();
                    int namePos = result.IndexOf("name\":") + 7;
                    int endPos = result.IndexOf("lang\"") - 3;
                    string cardname = result.Substring(namePos,endPos-namePos);
                    cardRawJSON[cardname] = result;
                    //JsonConvert.PopulateObjSSect(result,newCardInfo);
                    //cardInfoDict[newCardInfo.name] = newCardInfo;
                }

            }
            
            
        }
        Debug.Log("done");
        isJSONFileLoaded = true;

    }
    public void parseDeck(string deckList, string uuid)
    {
        serverConnectUI.switchToLoad();
        Player currentPlayer = getPlayer(uuid);
        loadCards(deckList,currentPlayer,uuid);
    }



    public Player getPlayer(string uuid)
    {
        for(int i = 0; i < playerList.Count; ++i)
        {
            if(playerList[i].uuid == uuid)
            {
                return playerList[i];
            }
        }

        return null;
    }
    public void readyUpPlayer(string uuid)
    {
        readyUpCount += 1;
        if(readyUpCount != readyUpTotal)
        {
            serverConnectUI.addMessage($"[{readyUpCount}/{readyUpTotal}] Players Are Now Ready");
        }
        else
        {
           startGame();
        }
    }
    public void startGame()
    {
            serverConnectUI.addMessage($"[{readyUpCount}/{readyUpTotal}] Players Ready. Starting Game.");
            //serverConnectUI.switchToPlay();
            serverConnectUI.sendTCPToServer(serverConnectUI.getDeckList(),"01","07");

    }
    public void unreadyUpPlayer(string uuid)
    {
        readyUpCount -= 1;
        serverConnectUI.addMessage($"[{readyUpCount}/{readyUpTotal}] Players Are Now Ready");
    }
    public void setMax(string uuid, int newReadyUpTotal)
    {
        serverConnectUI.addMessage($"{getPlayer(uuid).username} Has Changed the Ready Total to {newReadyUpTotal} from {readyUpTotal}");
        readyUpTotal = newReadyUpTotal;
        if(readyUpCount >= readyUpTotal)
        {
            startGame();

        }
    }
    public void removeFromPlayerList(string name)
    {
        int deleteNum = -1;
        for(int i = 0; i < playerList.Count; ++i)
        {
            if(playerList[i].username == name)
            {
                deleteNum = i;
            }
        }
        if(deleteNum != -1)
        {
            playerList.RemoveAt(deleteNum);
        }
    }
    public List<Player> getPlayerList()
    {
        return playerList;
    }
    public void loadCards(string cardList, Player player, string uuid)
    {
        readFileSync();
        string[] cards = cardList.Split("\n");
        List<string> actualDeckList = new List<string>();
        foreach(string card in cards)
        {
            if(card.Length > 2 && card[0] != '/')
            {
                ++individualDeckLoad;
                actualDeckList.Add(card);
            }
            
        }
        uuidList.Add(uuid);
        foreach(string card in cards)
        {
            if(card.Length > 2 && card[0] != '/' )
            {
                string cardName = card.Substring(card.IndexOf(" ")+1);
                
                if(!nameList.Contains(cardName))
                {
                    nameList.Add(cardName);
                    StartCoroutine(GetTexture($"https://api.scryfall.com/cards/named?exact={cardName}&format=image&version=png", cardName,player));
                }
                else
                {
                    ++cardsLoaded;
                }
            }
            
        }
        player.cardList = actualDeckList;
    }
    IEnumerator GetTexture(string url, string name, Player player)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success )
        {
            Debug.Log($"{name} Error");
            player.missedTextures.Add(name);

        }
        else
        {
            Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Texture2D atlas = new Texture2D(myTexture.width *3, myTexture.height);
            atlas.PackTextures(new Texture2D[] { myTexture, cardback },0);
            cardPictureDict.Add(name,myTexture);
            playableCardDict.Add(name,atlas);
        }
        //Debug.Log($"Finished Loading {name}");
        ++cardsLoaded;
        callbackFunction();
    }

    void generateGame()
    {
        playerList.Sort(delegate(Player x, Player y){return x.uuid.CompareTo(y.uuid);});
        for(int i = 0; i < playerList.Count; ++i)
        {
            if(playerList[i].uuid == uuid)
            {
                playerNumber = i;
            }
        }
        if(playerNumber % 2 != 0)
        {
            Camera.main.GetComponent<cameraMovement>().isInverted = false;
        }

        playtableGenerator.playerCount = playerList.Count;
        playtableGenerator.actualPlayer = playerNumber;
        cardPlaying.player = playerNumber;
        cardPlaying.initPlayerList(playerList.Count);
        playtableGenerator.startManaging();
        string temp = "";
        foreach(string card in cardPictureDict.Keys)
        {
            temp += card + ",";
        }
        Debug.Log(temp);
        for(int i = 0; i < playerList.Count; ++i)
        {
            for(int a = 0; a < playerList[i].cardList.Count; ++a)
            {
                string currCard = playerList[i].cardList[a];
                int result;
                if(Int32.TryParse(currCard.Substring(0,currCard.IndexOf(" ")),out result))
                {
                    currCard = currCard.Substring(currCard.IndexOf(" ")+1);
                    for(int c = 0; c < result; ++c)
                    {
                        if(cardPictureDict.ContainsKey(currCard))
                        {
                            cardPlaying.addCard(cardPictureDict[currCard],i, playableCardDict[currCard]);
                        }
                        else
                        {
                            Debug.LogError($"Was not able to find card! {currCard}");
                        }
                    }
                    
                }
                

            }
        }
    }
    public void callbackFunction()
    {
        if(individualDeckLoad == cardsLoaded && uuidList.Count == readyUpTotal)
        {
            serverConnectUI.switchToPlay();
            generateGame();
        }
        else
        {
            Debug.Log($"{cardsLoaded}/{individualDeckLoad}");
            serverConnectUI.setBar(((float) cardsLoaded)/individualDeckLoad);
        }
    
    }

    public int getPlayerNumFromUUID(string uuid)
    {
        for(int i = 0; i < playerList.Count; ++i)
        {
            if(playerList[i].uuid == uuid)
            {
                return i;
            }
        }
        return -1;
    }
    public static IEnumerator applyTexture( GameObject go) 
    {
     Mesh mesh = go.GetComponent<MeshFilter>().mesh;
     Vector2[] UVs = new Vector2[mesh.vertices.Length];
     // Front
     UVs[0] = new Vector2(0.0f, 0.0f);
     UVs[1] = new Vector2(0.3638f, 0.0f);
     UVs[2] = new Vector2(0.0f, 0.5078f);
     UVs[3] = new Vector2(0.3638f, 0.5078f);
     // Top
     UVs[4] = new Vector2(0.8f, 0.8f);
     UVs[5] = new Vector2(0.8f, 0.8f);
     UVs[8] = new Vector2(0.8f, 0.8f);
     UVs[9] = new Vector2(0.8f, 0.8f);
     // Back
     UVs[7] = new Vector2(0.3638f, 0.0f);
     UVs[6] = new Vector2(0.7276f, 0.0f);
     UVs[11] = new Vector2(0.3638f, 0.5078f);
     UVs[10] = new Vector2(0.7276f, 0.5078f);
     // Bottom
     UVs[12] = new Vector2(0.8f, 0.8f);
     UVs[13] = new Vector2(0.8f, 0.8f);
     UVs[14] = new Vector2(0.8f, 0.8f);
     UVs[15] = new Vector2(0.8f, 0.8f);
     // Left
     UVs[16] = new Vector2(0.8f, 0.8f);
     UVs[17] = new Vector2(0.8f, 0.8f);
     UVs[18] = new Vector2(0.8f, 0.8f);
     UVs[19] = new Vector2(0.8f, 0.8f);
     // Right        
     UVs[20] = new Vector2(0.8f, 0.8f);
     UVs[21] = new Vector2(0.8f, 0.8f);
     UVs[22] = new Vector2(0.8f, 0.8f);
     UVs[23] = new Vector2(0.8f, 0.8f);
     mesh.uv = UVs;
     yield break;
 }

    public void releaseCardOnField(int originalPositionHash, int cardHash, int endPositionHash,string uuid)
    {
        
        cardPlaying.releaseCardOnField(originalPositionHash,cardHash,endPositionHash,getPlayerNumFromUUID(uuid));
    }

    public void addCardToHand(int originalPositionHash, int cardHash, int endPlayer,int insertPosition, bool isMe)
    {
        cardPlaying.addCardToHand(originalPositionHash,cardHash,endPlayer,insertPosition,isMe);
    }

}
