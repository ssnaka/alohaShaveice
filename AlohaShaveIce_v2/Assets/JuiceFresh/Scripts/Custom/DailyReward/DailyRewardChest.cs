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
//	[SerializeField]
//	GameObject openChestBoxPrefab;

	RewardItem rewardItem;

	DateTime dailyRewardAwardedTime;
	DateTime nextDailyRewardTime;
	bool isNewRewardForToday = false;
	bool didWatchRewardAds = false;

//	int openCountToday = 0;
	Vector2 originalBoxSize = Vector2.zero;

	[SerializeField]
	List<LocalizedText> titleTextAssets;
	[SerializeField]
	LocalizedText openTextAsset;

	void OnEnable ()
	{
		didWatchRewardAds = System.Convert.ToBoolean(ZPlayerPrefs.GetInt("didWatchRewardAds", 0));
//		openCountToday = PlayerPrefs.GetInt("dailyRewardOpenCountToday", 0);
		string lastDailyRewardAwardedTime = ZPlayerPrefs.GetString("dailyRewardAwardedTime", DateTime.Now.AddDays(-1).ToString());
		dailyRewardAwardedTime = System.Convert.ToDateTime(lastDailyRewardAwardedTime);
		nextDailyRewardTime = dailyRewardAwardedTime.AddHours(24);
		if (data.type.Equals(ChestType.premium))
		{
			timerText.gameObject.SetActive(false);
			chestImageAnimation.clip = chestImageAnimation.GetClip("chest_button_premium_image_idle");
			chestImageAnimation.Stop();
			chestImageAnimation.Play();
		}
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
				//			openCountToday = 0;
				timerText.gameObject.SetActive(false);
				CheckDailyRewardOnceFromUpdate();
			}
		}
	}

	public void SetupData (ChestData _chestData)
	{
		data = _chestData;
//		SetupView();
		CheckDailyReward();
	}

	void SetupView (int _day, bool _isNewReward)
	{
		if (originalBoxSize.Equals(Vector2.zero))
		{
			originalBoxSize = boxImage.rectTransform.sizeDelta;
		}

		adButton.gameObject.SetActive(false);
//		boxTitle.text = data.type.ToString().ToUpper();
		titleTextBehaviour.LocalizedAsset = titleTextAssets.Find(item => item.name.Equals(data.textAssetName));
		chestSprite = Resources.Load<Sprite>("Custom/Sprite/" + data.chestImage);
		boxImage.overrideSprite = chestSprite;
//		chest3DPrefab = Resources.Load<GameObject>("Custom/Chest/" + data.chestPrefab);
//		if (chest3D == null)
//		{
//			chest3D = Instantiate<GameObject>(chest3DPrefab, transform);
//			chest3D.transform.localPosition = new Vector3(0.0f, -12.0f, -120.0f);
//			chest3D.transform.eulerAngles = new Vector3(0.0f, 162.0f, 0.0f);
//			chest3D.transform.localScale = new Vector3(50.0f, 50.0f, 50.0f);
//		}

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
						adButton.gameObject.SetActive(true);
					}
					priceString = data.price.ToString();
				}
				else
				{
					currencyTextBehaviour.LocalizedAsset = openTextAsset;
					priceString = currencyText.text;
					shouldShowCurrencyImage = false;
				}

				break;
			case ChestType.premium:
				boxImage.rectTransform.sizeDelta = originalBoxSize * 1.15f;
				priceString = data.price.ToString();
				break;
			default:
				break;
		}
			
//		currencyImage.overrideSprite = ;

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
//		adButton.gameObject.SetActive(false);
		InitScript.Instance.currentReward = RewardedAdsType.ChestBox;
		InitScript.Instance.ShowRewardedAds();

		GameTutorialManager.Instance.CloseTutorial();
	}

	public void OnOpenButtonPressed ()
	{
		// Open box
//		box3DPrefab
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
						GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
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
					GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
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
//		openCountToday += 1;
//		PlayerPrefs.SetInt("dailyRewardOpenCountToday", openCountToday);
//		PlayerPrefs.Save();
//		DailyRewardManager.Instance.ShowOpenChest(rewardItem.possibleRewards, chest3DPrefab);
		DailyRewardManager.Instance.ShowOpenChest(rewardItem.possibleRewards, chestSprite, data.type, _withGems, _fromAds);
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
