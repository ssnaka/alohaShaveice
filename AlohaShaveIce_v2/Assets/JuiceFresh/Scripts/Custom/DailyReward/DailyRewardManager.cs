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
	GameObject openChestBoxCanvas;

	public void Init()
	{
		// do nothing
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
		DailyRewardChest dailyRewardChest = chestList.Find(item => item.data.type.Equals(ChestType.daily));
		if (dailyRewardChest.CheckDailyReward())
		{
			EnableReward(true);
		}
	}

	public void OnCloseButtonPressed ()
	{
		EnableReward(false);
	}

	public void ShowOpenChest (List<PossibleReward> _possibleRewards, GameObject _chestPrefab)
	{
		if (openChestBoxCanvas == null)
		{
			openChestBoxCanvas = Instantiate<GameObject>(openChestBoxPrefab);
		}
		DailyRewardOpenChestCanvasScript script = openChestBoxCanvas.GetComponent<DailyRewardOpenChestCanvasScript>();
		script.SetupOpenChest(_possibleRewards, _chestPrefab);
	}

	public void ShowOpenChest (List<PossibleReward> _possibleRewards, Sprite _chestSprite, ChestType _chestType)
	{
		if (openChestBoxCanvas == null)
		{
			openChestBoxCanvas = Instantiate<GameObject>(openChestBoxPrefab);
		}
		DailyRewardOpenChestCanvasScript script = openChestBoxCanvas.GetComponent<DailyRewardOpenChestCanvasScript>();
		script.SetupOpenChest(_possibleRewards, _chestSprite, _chestType);
	}

	public void ShowOpenChestAsVideoAds ()
	{
		DailyRewardChest dailyRewardChest = chestList.Find(item => item.data.type.Equals(ChestType.daily));
		dailyRewardChest.ShowOpenChest(true);
	}

	#region Animation callback
	public void PlaySoundButton()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
	}

	public void OnFinished()
	{
		
	}
	#endregion
}
