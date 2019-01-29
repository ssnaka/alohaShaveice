using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBoostShop : MonoBehaviour {

    [SerializeField]
    Text gemsText;

    [SerializeField]
    BoostType boostType;

    [SerializeField]
    NewBoostProduct boostProducts;

	// Use this for initialization
	void Start () 
    {
        SetBoost();
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}

        // Update is called once per frame
    public void SetBoost ()
    {
        if (gemsText == null)
        {
            return;
        }
        gemsText.text = boostProducts.gemPrices.ToString();
    }

    public void BuyBoost ()
    {
        int count = 1;
        int price = boostProducts.gemPrices;
        BuyBoost(boostType, price, count);
    }

    void BuyBoost (BoostType boostType, int price, int count)
    {
        SoundBase.Instance.PlaySound(SoundBase.Instance.click);
        if (InitScript.Gems >= price)
        {
            SoundBase.Instance.PlaySound(SoundBase.Instance.cash);
            InitScript.Instance.SpendGems(price);
            InitScript.Instance.BuyBoost(boostType, price, count);
        } 
        else 
        {
//            BuyGems();
            if (!InitScript.Instance.menuController.menuPanelScript.isOpened)
            {
                InitScript.Instance.menuController.EnableMainMenu(true, true);
                InitScript.Instance.menuController.menuPanelScript.OnOpenCloseButtonPressed();
            }

            InitScript.Instance.menuController.menuPanelScript.OnMenuButtonPressed((int)MenuItemType.bank);
        }
    }
}

[System.Serializable]
public class NewBoostProduct
{
    public int gemPrices;
}