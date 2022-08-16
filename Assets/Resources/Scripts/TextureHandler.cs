using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    
    public class Player
    {
        public string username {get;set;}
        public string uuid {get;set;}
        public bool isReady{get;set;}
        public List<Card> deckList {get;set;}

        public List<string> missedCards {get;set;}
        public List<string> missedTextures {get;set;}
        
    }

     public class Card
     {
        public Card(CardInfo cardInfo)
        {
            this.m_cardinfo = cardInfo;
        }
        public Texture2D m_cardTexture {get;set;}
        public CardInfo m_cardinfo {get;set;}
     }
     public class AllPart
    {
        public string @object { get; set; }
        public string id { get; set; }
        public string component { get; set; }
        public string name { get; set; }
        public string type_line { get; set; }
        public string uri { get; set; }
    }

    public class ImageUris
    {
        public string small { get; set; }
        public string normal { get; set; }
        public string large { get; set; }
        public string png { get; set; }
        public string art_crop { get; set; }
        public string border_crop { get; set; }
    }

    public class Legalities
    {
        public string standard { get; set; }
        public string future { get; set; }
        public string historic { get; set; }
        public string gladiator { get; set; }
        public string pioneer { get; set; }
        public string explorer { get; set; }
        public string modern { get; set; }
        public string legacy { get; set; }
        public string pauper { get; set; }
        public string vintage { get; set; }
        public string penny { get; set; }
        public string commander { get; set; }
        public string brawl { get; set; }
        public string historicbrawl { get; set; }
        public string alchemy { get; set; }
        public string paupercommander { get; set; }
        public string duel { get; set; }
        public string oldschool { get; set; }
        public string premodern { get; set; }
    }

    public class PurchaseUris
    {
        public string tcgplayer { get; set; }
        public string cardmarket { get; set; }
        public string cardhoarder { get; set; }
    }

    public class RelatedUris
    {
        public string gatherer { get; set; }
        public string tcgplayer_infinite_articles { get; set; }
        public string tcgplayer_infinite_decks { get; set; }
        public string edhrec { get; set; }
    }

    public class CardInfo
    {
        public string @object { get; set; }
        public string id { get; set; }
        public string oracle_id { get; set; }
        public List<string> multiverse_ids { get; set; }
        public string mtgo_id { get; set; }
        public string mtgo_foil_id { get; set; }
        public string tcgplayer_id { get; set; }
        public string cardmarket_id { get; set; }
        public string name { get; set; }
        public string lang { get; set; }
        public string released_at { get; set; }
        public string uri { get; set; }
        public string scryfall_uri { get; set; }
        public string layout { get; set; }
        public bool highres_image { get; set; }
        public string image_status { get; set; }
        public ImageUris image_uris { get; set; }
        public string mana_cost { get; set; }
        public string cmc { get; set; }
        public string type_line { get; set; }
        public string oracle_text { get; set; }
        public string power { get; set; }
        public string toughness { get; set; }
        public List<string> colors { get; set; }
        public List<string> color_identity { get; set; }
        public List<object> keywords { get; set; }
        public List<AllPart> all_parts { get; set; }
        public Legalities legalities { get; set; }
        public List<string> games { get; set; }
        public bool reserved { get; set; }
        public bool foil { get; set; }
        public bool nonfoil { get; set; }
        public List<string> finishes { get; set; }
        public bool oversized { get; set; }
        public bool promo { get; set; }
        public bool reprint { get; set; }
        public bool variation { get; set; }
        // public string set_id { get; set; }
        // public string set { get; set; }
        // public string set_name { get; set; }
        // public string set_type { get; set; }
        // public string set_uri { get; set; }
        // public string set_search_uri { get; set; }
        // public string scryfall_set_uri { get; set; }
        public string rulings_uri { get; set; }
        public string prints_search_uri { get; set; }
        public string collector_number { get; set; }
        public bool digital { get; set; }
        public string rarity { get; set; }
        public string watermark { get; set; }
        public string flavor_text { get; set; }
        public string card_back_id { get; set; }
        public string artist { get; set; }
        public List<string> artist_ids { get; set; }
        public string illustration_id { get; set; }
        public string border_color { get; set; }
        public string frame { get; set; }
        public string security_stamp { get; set; }
        public bool full_art { get; set; }
        public bool textless { get; set; }
        public bool booster { get; set; }
        public bool story_spotlight { get; set; }
        public string edhrec_rank { get; set; }
        public string penny_rank { get; set; }
        public RelatedUris related_uris { get; set; }
        public PurchaseUris purchase_uris { get; set; }
    }    
       
// public class TextureHandler 
// {
//     public TextureHandler()
//     {
//         textureDict = new Dictionary<string, Texture2D>();
//         incorrectlyLoaded = new List<string>();
//     }


    
    
//     public Texture2D getTexture(string name)
//     {
//         return textureDict[name];
//     }

//     IEnumerator GetTexture(string link, string name)
//     {
//         UnityWebRequest www = UnityWebRequestTexture.GetTexture(link);
//         yield return www.SendWebRequest();

//         if (www.result != UnityWebRequest.Result.Success)
//         {
//             errorList.Add(link);
//         }
//         else
//         {
//             Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
//             Texture2D atlas = new Texture2D(1344, 1873);
//             atlas.PackTextures(new Texture2D[] { myTexture, cardback }, 0, 1873);
//             textureDict.Add(name, atlas);
            
//         }
//     }
    

//     Dictionary<string, Texture2D> textureDict;
//     List<string> incorrectlyLoaded;


// }
