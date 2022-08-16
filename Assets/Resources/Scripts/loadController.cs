using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
public class testFile
{
    public string name;
}
public class loadController : MonoBehaviour
{
    public Image bar;
    Dictionary<string,string> str = new Dictionary<string,string>();
    public void setBar(float f)
    {
        bar.transform.localScale = new Vector3(f,bar.transform.localScale.y,bar.transform.localScale.z);
    }

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.J))
        {
            Debug.Log("started");
            List<string> g = new List<string>();
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
                        str[cardname] = result;
                        //JsonConvert.PopulateObjSSect(result,newCardInfo);
                        //cardInfoDict[newCardInfo.name] = newCardInfo;
                    }

                }
                
                
            }
            Debug.Log("done");
        }
    }


}
