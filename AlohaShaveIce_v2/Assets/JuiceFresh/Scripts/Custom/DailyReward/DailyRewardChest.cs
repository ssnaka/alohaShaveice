using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DailyRewardChest : MonoBehaviour
{

	public ChestData data {get; private set;}

	[SerializeField]
	Text boxTitle;
	[SerializeField]
	Image boxImage;


	[SerializeField]
	Image currencyImage;
	[SerializeField]
	Text currencyText;

	[SerializeField]
	Text timerText;
	[SerializeField]
	Button adButton;

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
	int openCountToday = 0;

	void OnEnable ()
	{
		openCountToday = PlayerPrefs.GetInt("dailyRewardOpenCountToday", 0);
		string lastDailyRewardAwardedTime = PlayerPrefs.GetString("dailyRewardAwardedTime", DateTime.Now.AddDays(-1).ToString());
		dailyRewardAwardedTime = System.Convert.ToDateTime(lastDailyRewardAwardedTime);
		nextDailyRewardTime = dailyRewardAwardedTime.AddHours(24);
		if (data.type.Equals(ChestType.premium))
		{
			timerText.gameObject.SetActive(false);
		}
	}

	// Update is called once per frame
	void Update ()
	{
		if (data.type.Equals(ChestType.daily) && DateTime.Now.CompareTo(nextDailyRewardTime) < 0)
		{
			timerText.gameObject.SetActive(true);
			didCheckDailyRewardOnceFromUpdate = false;
			TimeSpan timeLeftSpan = nextDailyRewardTime.Subtract(DateTime.Now);
			timerText.text = string.Format("{0:00}:{1:00}:{2:00}", timeLeftSpan.Hours, timeLeftSpan.Minutes, timeLeftSpan.Seconds);
		}
		else
		{
			openCountToday = 0;
			timerText.gameObject.SetActive(false);
			CheckDailyRewardOnceFromUpdate();
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
		adButton.gameObject.SetActive(false);
		boxTitle.text = data.type.ToString().ToUpper();
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

		string priceString = "Open";
		bool shouldShowCurrencyImage = true;
		switch (data.type)
		{
			case ChestType.daily:
				if (!_isNewReward)
				{
					if (openCountToday == 1)
					{
						adButton.gameObject.SetActive(true);
					}
					priceString = data.price.ToString();
				}
				else
				{
					shouldShowCurrencyImage = false;
				}

				break;
			case ChestType.premium:
				boxImage.rectTransform.sizeDelta = boxImage.rectTransform.sizeDelta * 1.15f;
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
			openCountToday = PlayerPrefs.GetInt("dailyRewardOpenCountToday", 0);
			dailyRewardDayCount = PlayerPrefs.GetInt("dailyRewardDayCount", 0);
			lastDailyRewardAwardedTime = PlayerPrefs.GetString("dailyRewardAwardedTime", DateTime.Now.AddDays(-1).ToString());

				dailyRewardAwardedTime = System.Convert.ToDateTime(lastDailyRewardAwardedTime);
				nextDailyRewardTime = dailyRewardAwardedTime.AddHours(24);

				if (dailyRewardDayCount <= 0 || DateTime.Now.CompareTo(nextDailyRewardTime) >= 0)
				{
					isNewRewardForToday = true;
					dailyRewardDayCount += 1;
					openCountToday = 0;
					PlayerPrefs.SetInt("dailyRewardOpenCountToday", openCountToday);
					PlayerPrefs.Save();
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
	}

	public void OnOpenButtonPressed ()
	{
		// Open box
//		box3DPrefab
		bool shouldSpendGems = false;
		switch (data.type)
		{
			case ChestType.daily:
				if (!isNewRewardForToday)
				{
					if (openCountToday > 1)
					{
						if (InitScript.Gems <= data.price)
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
				}

				break;
			case ChestType.premium:
				shouldSpendGems = true;
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
			PlayerPrefs.SetString("dailyRewardAwardedTime", DateTime.Now.ToString());
			int dailyRewardDayCount = PlayerPrefs.GetInt("dailyRewardDayCount", 0);
			dailyRewardDayCount += 1;
			if (dailyRewardDayCount > data.data.Count)
			{
				dailyRewardDayCount = 0;
			}
			PlayerPrefs.SetInt("dailyRewardDayCount", dailyRewardDayCount);

			string lastDailyRewardAwardedTime = PlayerPrefs.GetString("dailyRewardAwardedTime", DateTime.Now.AddDays(-1).ToString());
			dailyRewardAwardedTime = System.Convert.ToDateTime(lastDailyRewardAwardedTime);
			nextDailyRewardTime = dailyRewardAwardedTime.AddHours(24);

			PlayerPrefs.Save();
		}

		ShowOpenChest();
	}

	public void ShowOpenChest ()
	{
		openCountToday += 1;
		PlayerPrefs.SetInt("dailyRewardOpenCountToday", openCountToday);
		PlayerPrefs.Save();
//		DailyRewardManager.Instance.ShowOpenChest(rewardItem.possibleRewards, chest3DPrefab);
		DailyRewardManager.Instance.ShowOpenChest(rewardItem.possibleRewards, chestSprite);
		CheckDailyReward();
	}


}
