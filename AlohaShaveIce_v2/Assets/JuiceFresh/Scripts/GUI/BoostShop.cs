using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GameToolkit.Localization;

public enum BoostType
{
    ExtraMoves,
    Stripes,
    ExtraTime,
    Bomb,
    Colorful_bomb,
    Shovel,
    Energy,
    None
}

public class BoostShop : MonoBehaviour
{
    public Sprite[] icons;
    public string[] descriptions;
    public int[] prices;
    public Image icon;
    public Text description;
	[SerializeField]
	LocalizedTextBehaviour descriptionTextBehaviour;
    [SerializeField]
    Text gemsText;
	public BoostType boostType { get; private set; }

    public List<BoostProduct> boostProducts = new List<BoostProduct>();

	[SerializeField]
	GameObject videoButtonObject;
	[SerializeField]
	Text videoCounter;
    [SerializeField]
    Animation counterAnimation;

    void OnEnable ()
    {
//        SetVideoCounter();
        InitScript.Instance.OnVideoAdFinished += InitScript_Instance_OnVideoAdFinished;
        InitScript.Instance.OnGemUpdate -= InitScript_Instance_OnGemUpdate;
        InitScript.Instance.OnGemUpdate += InitScript_Instance_OnGemUpdate;
        InitScript_Instance_OnGemUpdate(InitScript.Gems);
    }

    void OnDisable ()
    {
        InitScript.Instance.OnVideoAdFinished -= InitScript_Instance_OnVideoAdFinished;
        InitScript.Instance.OnGemUpdate -= InitScript_Instance_OnGemUpdate;
    }

    void InitScript_Instance_OnGemUpdate (int _gemCount)
    {
        gemsText.text = _gemCount.ToString();;
    }

    void InitScript_Instance_OnVideoAdFinished (BoostType _boostType)
    {
        if (boostType.Equals(_boostType))
        {
            SetVideoCounter();
            counterAnimation.Play();
        }
    }

    // Update is called once per frame
    public void SetBoost(BoostType _boostType)
    {
        boostType = _boostType;
        gameObject.SetActive(true);
        icon.sprite = boostProducts[(int)_boostType].icon;
		descriptionTextBehaviour.LocalizedAsset = boostProducts[(int)_boostType].descriptionLocalizedText;
//        description.text = boostProducts[(int)_boostType].description;
        for (int i = 0; i < 3; i++)
        {
            transform.Find("Image/BuyBoost" + (i + 1) + "/Count").GetComponent<Text>().text = "x" + boostProducts[(int)_boostType].count[i];
            transform.Find("Image/BuyBoost" + (i + 1) + "/Price").GetComponent<Text>().text = "" + boostProducts[(int)_boostType].GemPrices[i];
        }

        SetVideoCounter();
		GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.Buy_Boosts_WithAd, videoButtonObject.GetComponent<RectTransform>());
    }

    public void SetVideoCounter ()
    {
        BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(boostType);
        if (boostAdEvent != null)
        {
            int playedCount = PlayerPrefs.GetInt(boostType.ToString() + "_watch", 0);
            videoCounter.text = (boostAdEvent.countToReward - playedCount).ToString();
            if (playedCount >= boostAdEvent.countToReward)
            {
                videoButtonObject.SetActive(false);
            }
        }
    }

    public void BuyBoost(GameObject button)
    {
        int count = int.Parse(button.transform.Find("Count").GetComponent<Text>().text.Replace("x", ""));
        int price = int.Parse(button.transform.Find("Price").GetComponent<Text>().text);
        GetComponent<AnimationManager>().BuyBoost(boostType, price, count);
    }
}

[System.Serializable]
public class BoostProduct
{
    public Sprite icon;
    public string description;
	public LocalizedText descriptionLocalizedText;
    public int[] count;
    public int[] GemPrices;
}