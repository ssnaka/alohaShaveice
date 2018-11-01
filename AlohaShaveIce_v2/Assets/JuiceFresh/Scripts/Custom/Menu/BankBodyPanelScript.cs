using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BankBodyPanelScript : MenuBodyPanelScript
{
    [SerializeField]
    List<Text> countTexts;
    [SerializeField]
    List<Text> priceTexts;

    // Use this for initialization
    void Start ()
    {
        for (int i = 0; i < LevelManager.THIS.gemsProducts.Count; i++)
        {
            countTexts[i].text =  "x " + LevelManager.THIS.gemsProducts[i].count;
            priceTexts[i].text =  "$" + LevelManager.THIS.gemsProducts[i].price;
        }
    }

    public void Buy (GameObject pack)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        if (pack.name == "Pack1")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
            #if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded ();
            CloseMenu ();
            return;
            #endif
            #if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[0]);
            #elif AMAZON
            AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [0]);

            #endif

        }

        if (pack.name == "Pack2")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
            #if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded ();
            CloseMenu ();
            return;
            #endif
            #if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[1]);
            #elif AMAZON
            AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [1]);

            #endif


        }
        if (pack.name == "Pack3")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
            #if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded ();
            CloseMenu ();
            return;
            #endif
            #if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[2]);
            #elif AMAZON
            AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [2]);

            #endif


        }
        if (pack.name == "Pack4")
        {
            InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
            #if UNITY_WEBPLAYER || UNITY_WEBGL
            InitScript.Instance.PurchaseSucceded ();
            CloseMenu ();
            return;
            #endif
            #if UNITY_INAPPS
            UnityInAppsIntegration.THIS.BuyProductID(LevelManager.THIS.InAppIDs[3]);
            #elif AMAZON
            AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [3]);

            #endif


        }
    }
}
