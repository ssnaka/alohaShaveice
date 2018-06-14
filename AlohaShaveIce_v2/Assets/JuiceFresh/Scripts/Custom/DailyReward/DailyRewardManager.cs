using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Prefab("Custom/DailyReward/DailyRewardManager")]
public class DailyRewardManager : Singleton<DailyRewardManager> 
{
	[Header("RewardChestItem")]
	[SerializeField]
	GameObject rewardChestItemPrefab;
	[SerializeField]
	GameObject rewardChestContainer;

	DailyRewardData data;

	List<DailyRewardChest> chestList = new List<DailyRewardChest>();

	[SerializeField]
	GameObject openChestBoxPrefab;
	public GameObject openChestBoxCanvas { get; private set; }

	public void Init()
	{
		EnableReward(false);
		ReadData();
		SetupChestBoxes();
	}

	void ReadData ()
	{
		string jsonString = ReadJsonFile();

		if (string.IsNullOrEmpty(jsonString))
		{
			return;
		}

		data = Newtonsoft.Json.JsonConvert.DeserializeObject<DailyRewardData>(jsonString);
	}

	string ReadJsonFile ()
	{
		TextAsset textAsset = Resources.Load<TextAsset>("Custom/Json/DailyReward");
		return textAsset != null ? textAsset.text : null;
	}

	void SetupChestBoxes ()
	{
		if (chestList.Count > 0)
		{
			return;
		}

		for (int i = 0 ; i < data.chests.Count ; i++)
		{
			ChestData chestData = data.chests[i];
			DailyRewardChest chestScript = Instantiate<GameObject>(rewardChestItemPrefab, Vector3.one, Quaternion.identity, rewardChestContainer.transform).GetComponent<DailyRewardChest>();
			chestScript.transform.localPosition = Vector3.zero;
			chestScript.SetupData(chestData);
			chestList.Add(chestScript);
		}
	}

	public void EnableReward (bool _enable)
	{
		gameObject.SetActive(_enable);
	}

	public void CheckRewardToShow ()
	{
		bool result = false;
		DailyRewardChest dailyRewardChest = chestList.Find(item => item.data.type.Equals(ChestType.daily));
		if (dailyRewardChest.CheckDailyReward() && GameTutorialManager.Instance.GetLocalTutorialStatus(TutorialType.First_Tutorial))
		{
			result = true;
			EnableReward(true);
		}

		if (!result)
		{
			if (GameTutorialManager.Instance.GetLocalTutorialStatus(TutorialType.First_Tutorial))
			{
				GameTutorialManager.Instance.CheckBoostShopTutorial();
			}
		}
	}

	public void OnCloseButtonPressed ()
	{
		EnableReward(false);
		GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.Try_ChestBox, GameObject.Find("DailyReward").GetComponent<RectTransform>());
		GameTutorialManager.Instance.CheckBoostShopTutorial();
	}

//	public void ShowOpenChest (List<PossibleReward> _possibleRewards, GameObject _chestPrefab)
//	{
//		if (openChestBoxCanvas == null)
//		{
//			openChestBoxCanvas = Instantiate<GameObject>(openChestBoxPrefab);
//		}
//		DailyRewardOpenChestCanvasScript script = openChestBoxCanvas.GetComponent<DailyRewardOpenChestCanvasScript>();
//		script.SetupOpenChest(_possibleRewards, _chestPrefab);
//	}

	public void ShowOpenChest (List<PossibleReward> _possibleRewards, Sprite _chestSprite, ChestType _chestType, bool _withGems, bool _withAds, bool _isQuest)
	{
		if (openChestBoxCanvas == null)
		{
			openChestBoxCanvas = Instantiate<GameObject>(openChestBoxPrefab);
		}
		DailyRewardOpenChestCanvasScript script = openChestBoxCanvas.GetComponent<DailyRewardOpenChestCanvasScript>();
		script.SetupOpenChest(_possibleRewards, _chestSprite, _chestType, _withGems, _withAds, _isQuest);
	}

	public void ShowOpenChestAsVideoAds ()
	{
		DailyRewardChest dailyRewardChest = chestList.Find(item => item.data.type.Equals(ChestType.daily));
		dailyRewardChest.ShowOpenChest(false, true);
	}

	#region Animation callback
	public void PlaySoundButton()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
	}

	public void OnFinished()
	{
		StartCoroutine(TutorialCheckRoutine());
	}
	#endregion

	IEnumerator TutorialCheckRoutine ()
	{
		yield return new WaitForEndOfFrame();
		DailyRewardChest dailyRewardChest = chestList.Find(item => item.data.type.Equals(ChestType.daily));
		dailyRewardChest.CheckTutorial();
	}
}
