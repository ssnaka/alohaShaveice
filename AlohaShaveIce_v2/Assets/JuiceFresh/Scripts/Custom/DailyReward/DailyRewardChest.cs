using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using GameToolkit.Localization;

public class DailyRewardChest : MonoBehaviour
{

	public ChestData data {get; private set;}

	[SerializeField]
	Text boxTitle;
	[SerializeField]
	LocalizedTextBehaviour titleTextBehaviour;
	[SerializeField]
	Image boxImage;

	[SerializeField]
	Image currencyImage;
	[SerializeField]
	Text currencyText;
	[SerializeField]
	LocalizedTextBehaviour currencyTextBehaviour;

	[SerializeField]
	Text timerText;
	[SerializeField]
	Button openButton;
	[SerializeField]
	Button adButton;

	[SerializeField]
	Animation chestImageAnimation;

	// this will be loaded from resources
	GameObject chest3DPrefab;
	Sprite chestSprite;

	GameObject chest3D;

	RewardItem rewardItem;

	DateTime dailyRewardAwardedTime;
	DateTime nextDailyRewardTime;
	bool isNewRewardForToday = false;
	bool didWatchRewardAds = false;

	Vector2 originalBoxSize = Vector2.zero;

	[SerializeField]
	List<LocalizedText> titleTextAssets;
	[SerializeField]
	LocalizedText openTextAsset;

    [SerializeField]
    Image bgImage;
    [SerializeField]
    Sprite regularBGSprite;
    [SerializeField]
    Sprite preniumBGSprite;

	void OnEnable ()
	{
		didWatchRewardAds = System.Convert.ToBoolean(ZPlayerPrefs.GetInt("didWatchRewardAds", 0));
		string lastDailyRewardAwardedTime = ZPlayerPrefs.GetString("dailyRewardAwardedTime", DateTime.Now.AddDays(-1).ToString());
		dailyRewardAwardedTime = System.Convert.ToDateTime(lastDailyRewardAwardedTime);
		nextDailyRewardTime = dailyRewardAwardedTime.AddHours(24);
//        bgImage.overrideSprite = regularBGSprite;
//        if (data != null && data.type.Equals(ChestType.premium))
//		{
//            Debug.LogError("+++++++++");
//			timerText.gameObject.SetActive(false);
//			chestImageAnimation.clip = chestImageAnimation.GetClip("chest_button_premium_image_idle");
//			chestImageAnimation.Stop();
//			chestImageAnimation.Play();
//            bgImage.overrideSprite = preniumBGSprite;
//		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (data.type.Equals(ChestType.daily))
		{
			if (DateTime.Now.CompareTo(nextDailyRewardTime) < 0)
			{
				timerText.gameObject.SetActive(true);
				didCheckDailyRewardOnceFromUpdate = false;
				TimeSpan timeLeftSpan = nextDailyRewardTime.Subtract(DateTime.Now);
				timerText.text = string.Format("{0:00}:{1:00}:{2:00}", timeLeftSpan.Hours, timeLeftSpan.Minutes, timeLeftSpan.Seconds);
			}
			else
			{
				didWatchRewardAds = false;
				ZPlayerPrefs.SetInt("didWatchRewardAds", System.Convert.ToInt32(didWatchRewardAds));
				ZPlayerPrefs.Save();
				timerText.gameObject.SetActive(false);
				CheckDailyRewardOnceFromUpdate();
			}
		}
	}

	public void SetupData (ChestData _chestData)
	{
		data = _chestData;
		CheckDailyReward();
	}

	void SetupView (int _day, bool _isNewReward)
	{
		if (originalBoxSize.Equals(Vector2.zero))
		{
			originalBoxSize = boxImage.rectTransform.sizeDelta;
		}

        adButton.interactable = false;//.gameObject.SetActive(false);
		titleTextBehaviour.LocalizedAsset = titleTextAssets.Find(item => item.name.Equals(data.textAssetName));
		chestSprite = Resources.Load<Sprite>("Custom/Sprite/" + data.chestImage);
		boxImage.overrideSprite = chestSprite;

		currencyTextBehaviour.LocalizedAsset = null;
		string priceString = currencyText.text;
		bool shouldShowCurrencyImage = true;
		switch (data.type)
		{
			case ChestType.daily:
				if (!_isNewReward)
				{
					if (!didWatchRewardAds)
					{
                        adButton.interactable = true;//.gameObject.SetActive(true);
					}
					priceString = data.price.ToString();
				}
				else
				{
					currencyTextBehaviour.LocalizedAsset = openTextAsset;
					priceString = currencyText.text;
					shouldShowCurrencyImage = false;
				}
                bgImage.overrideSprite = regularBGSprite;

				break;
			case ChestType.premium:
                bgImage.overrideSprite = preniumBGSprite;
				boxImage.rectTransform.sizeDelta = originalBoxSize * 1.15f;
				priceString = data.price.ToString();
                
                adButton.gameObject.SetActive(false);
                timerText.gameObject.SetActive(false);
                chestImageAnimation.clip = chestImageAnimation.GetClip("chest_button_premium_image_idle");
                chestImageAnimation.Stop();
                chestImageAnimation.Play();
				break;
			default:
				break;
		}
			
		currencyImage.gameObject.SetActive(shouldShowCurrencyImage);
		currencyText.alignment = TextAnchor.MiddleCenter;
		currencyText.rectTransform.sizeDelta = new Vector2(100.0f, currencyText.rectTransform.sizeDelta.y);
		if (shouldShowCurrencyImage)
		{
			currencyText.rectTransform.sizeDelta = new Vector2(30.0f, currencyText.rectTransform.sizeDelta.y);
			currencyText.alignment = TextAnchor.MiddleLeft;
		}

		currencyText.text = priceString;
	}

	bool didCheckDailyRewardOnceFromUpdate = false;
	void CheckDailyRewardOnceFromUpdate ()
	{
		if (didCheckDailyRewardOnceFromUpdate)
		{
			return;
		}

		didCheckDailyRewardOnceFromUpdate = true;
		CheckDailyReward();
	}

	public bool CheckDailyReward ()
	{
		int dailyRewardDayCount = -1;
		string lastDailyRewardAwardedTime = string.Empty;
		isNewRewardForToday = false;

		switch (data.type)
		{
			case ChestType.daily:
				dailyRewardDayCount = ZPlayerPrefs.GetInt("dailyRewardDayCount", 0);
                if (dailyRewardDayCount >= data.data.Count)
                {
                    dailyRewardDayCount = 0;
                }
                
				lastDailyRewardAwardedTime = ZPlayerPrefs.GetString("dailyRewardAwardedTime", DateTime.Now.AddDays(-1).ToString());
				didWatchRewardAds = System.Convert.ToBoolean(ZPlayerPrefs.GetInt("didWatchRewardAds", 0));
				
				dailyRewardAwardedTime = System.Convert.ToDateTime(lastDailyRewardAwardedTime);
				nextDailyRewardTime = dailyRewardAwardedTime.AddHours(24);

				if (DateTime.Now.CompareTo(nextDailyRewardTime) >= 0)
				{
					isNewRewardForToday = true;
					dailyRewardDayCount += 1;
					
					didWatchRewardAds = false;
					ZPlayerPrefs.SetInt("didWatchRewardAds", System.Convert.ToInt32(didWatchRewardAds));
					ZPlayerPrefs.Save();
				}
                if (dailyRewardDayCount == 0)
                {
                    dailyRewardDayCount += 1;
                }
                
                ZPlayerPrefs.SetInt("dailyRewardDayCount", dailyRewardDayCount);
                ZPlayerPrefs.Save();
				break;
			case ChestType.premium:

				break;
			default:
				break;
		}


        RewardData rewardData = data.data.Find(item => item.day.Equals(dailyRewardDayCount));

		rewardItem = GetRewardForToday(rewardData);

		SetupView(dailyRewardDayCount, isNewRewardForToday);
		return isNewRewardForToday;
	}

	RewardItem GetRewardForToday (RewardData _rewardData)
	{
		int totalWeight = 0;
		for (int i = 0 ; i < _rewardData.items.Count; i++)
		{
			RewardItem rewardItem = _rewardData.items[i];
			totalWeight += rewardItem.weight;
		}

		int randomWeight = UnityEngine.Random.Range(0, totalWeight);

		int weight = 0;
		for (int i = 0 ; i <_rewardData.items.Count; i++)
		{
			RewardItem rewardItem = _rewardData.items[i];
			weight += rewardItem.weight;
			if (randomWeight < weight)
			{
				return rewardItem;
			}
		}

		return null;
	}

	public void OnAdsButtonPressed ()
	{
		InitScript.Instance.currentReward = RewardedAdsType.ChestBox;
		InitScript.Instance.ShowRewardedAds();

		GameTutorialManager.Instance.CloseTutorial();
	}

	public void OnOpenButtonPressed ()
	{
		// Open box
		GameTutorialManager.Instance.CloseTutorial();
		bool shouldSpendGems = false;
		switch (data.type)
		{
			case ChestType.daily:
				if (!isNewRewardForToday)
				{
					if (InitScript.Gems < data.price)
					{
						SoundBase.Instance.PlaySound(SoundBase.Instance.click);
                        InitScript.Instance.menuController.menuPanelScript.OnMenuButtonPressed((int)MenuItemType.bank);
						return;
					}
					else
					{
						shouldSpendGems = true;
					}
				}

				break;
			case ChestType.premium:
				if (InitScript.Gems < data.price)
				{
					SoundBase.Instance.PlaySound(SoundBase.Instance.click);
                    InitScript.Instance.menuController.menuPanelScript.OnMenuButtonPressed((int)MenuItemType.bank);
					return;
				}
				else
				{
					shouldSpendGems = true;
				}
				break;
			default:
			break;
		}

		if (shouldSpendGems)
		{
			InitScript.Instance.SpendGems(data.price);
		}
		else
		{
			ZPlayerPrefs.SetString("dailyRewardAwardedTime", DateTime.Now.ToString());
			int dailyRewardDayCount = ZPlayerPrefs.GetInt("dailyRewardDayCount", 0);
			dailyRewardDayCount += 1;
			if (dailyRewardDayCount > data.data.Count)
			{
				dailyRewardDayCount = 0;
			}
			ZPlayerPrefs.SetInt("dailyRewardDayCount", dailyRewardDayCount);

			string lastDailyRewardAwardedTime = ZPlayerPrefs.GetString("dailyRewardAwardedTime", DateTime.Now.AddDays(-1).ToString());
			dailyRewardAwardedTime = System.Convert.ToDateTime(lastDailyRewardAwardedTime);
			nextDailyRewardTime = dailyRewardAwardedTime.AddHours(24);

			ZPlayerPrefs.Save();
		}

		ShowOpenChest(shouldSpendGems);
	}

	public void ShowOpenChest (bool _withGems, bool _fromAds = false)
	{
		if (_fromAds)
		{
			didWatchRewardAds = true;
			ZPlayerPrefs.SetInt("didWatchRewardAds", System.Convert.ToInt32(didWatchRewardAds));
			ZPlayerPrefs.Save();
		}

        DailyRewardManager dailyRewardManager = InitScript.Instance.menuController.menuPanelScript.GetBodyPanelScript(MenuItemType.chest) as DailyRewardManager;
        dailyRewardManager.ShowOpenChest(rewardItem.possibleRewards, chestSprite, data.type, _withGems, _fromAds, false);
        InitScript.Instance.menuController.menuPanelScript.OnOpenCloseButtonPressed();
		CheckDailyReward();
	}

	public void CheckTutorial ()
	{
		if (data.type.Equals(ChestType.daily))
		{
			GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.Open_ChestBox, openButton.image.rectTransform);
			GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.Open_ChestBox_WithAd, adButton.image.rectTransform);
		}
	}

}
