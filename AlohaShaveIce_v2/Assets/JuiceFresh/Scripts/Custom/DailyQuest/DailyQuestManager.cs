using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using GameToolkit.Localization;

[Prefab("Custom/DailyQuest/DailyQuestManager")]
public class DailyQuestManager : Singleton<DailyQuestManager>
{
	[SerializeField]
	LocalizedTextBehaviour descriptionText;
	[SerializeField]
	LocalizedText levelDescription;
	[SerializeField]
	LocalizedText collectionDescription;
	[SerializeField]
	RectTransform background;
	[SerializeField]
	public RectTransform panel;
	[SerializeField]
	RectTransform closeButton;
	[SerializeField]
	HorizontalLayoutGroup questContainer;
	[SerializeField]
	GameObject questLevelPrefab;
	[SerializeField]
	GameObject questCollectPrefab;

	List<DailyQuestItemScript> dailyQuestItemScripts = new List<DailyQuestItemScript>();

	DailyQuestConfigData configData;
	public DailyQuestSaveData saveData {get; private set;}

	[SerializeField]
	Animation mAnimation;

	public event Action<int, DailyQuestInfo> OnQuestUpdate;
	//	public void Init ()
	//	{
	//		panel.gameObject.SetActive(false);
	//	}

	void OnDestroy ()
	{
		InitScript.Instance.OnItemDestroyed -= InitScript_Instance_OnItemDestroyed;
	}

	// Use this for initialization
	void Start ()
	{
		InitScript.Instance.OnItemDestroyed += InitScript_Instance_OnItemDestroyed;
	}

	//	// Update is called once per frame
	//	void Update () {
	//
	//	}

	DailyQuestConfigData ReadConfigData ()
	{
		TextAsset textasset = Resources.Load<TextAsset>("Custom/Json/DailyQuestConfig");
		return Newtonsoft.Json.JsonConvert.DeserializeObject<DailyQuestConfigData>(textasset.text);
	}

	void ReadSaveData ()
	{
		string dailyQuestSaveString = PlayerPrefs.GetString("DailyQuestSave");
		if (!string.IsNullOrEmpty(dailyQuestSaveString))
		{
			saveData = Newtonsoft.Json.JsonConvert.DeserializeObject<DailyQuestSaveData>(dailyQuestSaveString);
			if (saveData.questDate.Date < DateTime.Now.Date)
			{
				saveData = null;

				for (int i = 0; i < dailyQuestItemScripts.Count; i++)
				{
					DailyQuestItemScript script = dailyQuestItemScripts[i];
					GameObject.Destroy(script.gameObject);
				}
				dailyQuestItemScripts.Clear();
			}
		}

		// TODO: partial rewards?!?
	}

	void SaveSaveData (DailyQuestSaveData _data)
	{
		if (_data == null)
		{
			PlayerPrefs.DeleteKey("DailyQuestSave");
		}
		else
		{
			PlayerPrefs.SetString("DailyQuestSave", Newtonsoft.Json.JsonConvert.SerializeObject(_data));
		}
		PlayerPrefs.Save();
	}

	public void SetupDailyQuest (bool _enableView = false)
	{
		ReadSaveData();

		if (saveData == null)
		{
			saveData = CreateNewSaveData();
		}

		if (dailyQuestItemScripts.Count <= 0)
		{
			int completedIndex = -1;
			DailyQuestInfo completedInfo = null;
			GameObject prefab = questLevelPrefab;
			descriptionText.LocalizedAsset = levelDescription;
			if (saveData.type.Equals(DailyQuestType.Collect))
			{
				descriptionText.LocalizedAsset = collectionDescription;
				prefab = questCollectPrefab;
			}
			for (int i = 0; i < saveData.dailyQuestInfos.Count; i++)
			{
				DailyQuestInfo questInfo = saveData.dailyQuestInfos[i];
				if (questInfo.completed)
				{
					completedIndex = i;
					completedInfo = questInfo;
				}
				GameObject go = Instantiate<GameObject>(prefab, questContainer.transform);
				go.transform.localScale = Vector3.one;
				DailyQuestItemScript dailyQuestItemScript = go.GetComponent<DailyQuestItemScript>();
				dailyQuestItemScript.SetupItem(saveData.type, questInfo, i);
				dailyQuestItemScripts.Add(dailyQuestItemScript);
			}

			if (completedInfo != null)
			{
				if (OnQuestUpdate != null)
				{
					OnQuestUpdate(completedIndex, completedInfo);
				}
			}
		}

		EnableDailyQuestManager(_enableView);

		if (_enableView)
		{
			HandleRewards();
		}
	}

	DailyQuestSaveData CreateNewSaveData ()
	{
		DailyQuestSaveData result = new DailyQuestSaveData();

		DailyQuestType questType = DailyQuestType.None;
		List<DailyQuestInfo> questInfoList = null;
		configData = ReadConfigData();
		int randomQuestIndex = UnityEngine.Random.Range(0, configData.dailyQuests.Count);
		QuestConfigData questConfigData = configData.dailyQuests[randomQuestIndex];
//		questConfigData = configData.dailyQuests[3];
		questType = questConfigData.type;

		switch (questType)
		{
		case DailyQuestType.RandomLevel:
			questInfoList = CreateQuestList(questConfigData.count, 50, LevelsMap._instance.GetMapLevels().Count);
			for (int i = 0; i < questInfoList.Count; i++)
			{
				DailyQuestInfo questInfo = questInfoList[i];
				questInfo.level = i + 1;
			}
			break;
		case DailyQuestType.PreviousLevel:
			int lastUnlockedNumber = LevelsMap._instance.GetMapLevels().Where(l => !l.IsLocked).Select(l => l.Number).Max();
			if (lastUnlockedNumber < questConfigData.count * 5)
			{
				return CreateNewSaveData();
			}
			questInfoList = CreateQuestList(questConfigData.count, 1, lastUnlockedNumber + 1);
			break;
		case DailyQuestType.NextLevel:
			lastUnlockedNumber = LevelsMap._instance.GetMapLevels().Where(l => !l.IsLocked).Select(l => l.Number).Max();
			if (lastUnlockedNumber > LevelsMap._instance.GetMapLevels().Count - questConfigData.count)
			{
				return CreateNewSaveData();
			}
			questInfoList = new List<DailyQuestInfo>();
			for (int i = lastUnlockedNumber; i < lastUnlockedNumber + 5; i++)
			{
				DailyQuestInfo questInfo = new DailyQuestInfo(); 
				questInfo.level = questInfo.actualLevel = i;
				questInfoList.Add(questInfo);
			}
			break;
		case DailyQuestType.Collect:
			questInfoList = new List<DailyQuestInfo>();
			int randomCountIndex = UnityEngine.Random.Range(0, configData.collectionCount.Count);
			for (int i = 0; i < randomCountIndex + 1; i++)
			{
				int maxRange = configData.collection.Count;
				lastUnlockedNumber = LevelsMap._instance.GetMapLevels().Where(l => !l.IsLocked).Select(l => l.Number).Max();

				if (lastUnlockedNumber < 6)
				{
					maxRange = 6;
				}
				else if (lastUnlockedNumber < 11)
				{
					maxRange = 7;
				}
				else if (lastUnlockedNumber < 15)
				{
					maxRange = 8;
				}

				int randomCollectionIndex = UnityEngine.Random.Range(0, maxRange);
				DailyQuestCollectionType collectionType = configData.collection[randomCollectionIndex];
				if (questInfoList.Find(item => item.collectionType.Equals(collectionType)) == null)
				{
					DailyQuestInfo questInfo = new DailyQuestInfo();
					questInfo.collectionType = collectionType;
					questInfo.collecitonCount = configData.collectionCount[randomCountIndex];
					questInfoList.Add(questInfo);
				}
				else
				{
					i--;
				}
			}
			break;
		default:
			break;
		}


		List<PossibleReward> rewards = CreateRewards(2);

		result.type = questType;
		result.dailyQuestInfos = questInfoList;
		result.questDate = DateTime.Now.Date;
		result.rewards = rewards;

		SaveSaveData(result);
		return result; 
	}

	List<DailyQuestInfo> CreateQuestList (int _count, int _minIndex, int _maxIndex)
	{
		List<DailyQuestInfo> result = new List<DailyQuestInfo>();
		for (int i = 0; i < _count; i++)
		{
			int randomLevel = UnityEngine.Random.Range(_minIndex, _maxIndex);
			if (result.Find(item => item.actualLevel.Equals(randomLevel)) == null)
			{
				DailyQuestInfo questInfo = new DailyQuestInfo(); 
				questInfo.level = questInfo.actualLevel = randomLevel;
				result.Add(questInfo);
			}
			else
			{
				i--;
			}
		}

		return result;
	}

	List<PossibleReward> CreateRewards (int _count)
	{
		List<PossibleReward> result = new List<PossibleReward>();
		List<int> randomIndexSet = new List<int>();;
		for (int i = 0; i < _count; i++)
		{
			int randomIndex = UnityEngine.Random.Range(0, configData.rewards.Count);
			if (!randomIndexSet.Contains(randomIndex))
			{
				randomIndexSet.Add(randomIndex);
				result.Add(configData.rewards[randomIndex]);
			}
			else
			{
				i--;
			}
		}

		return result;
	}

	public void UpdateQuestInfo (DailyQuestInfo _info)
	{
		if (saveData == null || _info == null)
		{
			return;
		}

		DailyQuestInfo questInfo = null;
		DailyQuestItemScript itemScript = null;
		switch (saveData.type)
		{
		case DailyQuestType.RandomLevel:
		case DailyQuestType.PreviousLevel:
		case DailyQuestType.NextLevel:
			questInfo = saveData.dailyQuestInfos.Find(item => item.actualLevel.Equals(_info.actualLevel));
			if (questInfo == null)
			{
				return;
			}

			questInfo.completed = true;

			itemScript = dailyQuestItemScripts.Find(item => item.questInfo.actualLevel.Equals(questInfo.actualLevel));
//			if (OnQuestUpdate != null)
//			{
//				OnQuestUpdate(itemScript.index, questInfo);
//			}
			break;
		case DailyQuestType.Collect:
//			questInfo = saveData.dailyQuestInfos.Find(item => item.collectionType.Equals(_info.collectionType));
			questInfo = _info;
			if (questInfo.collecitonCount <= 0)
			{
				questInfo.completed = true;
			}
			itemScript = dailyQuestItemScripts.Find(item => item.questInfo.collectionType.Equals(questInfo.collectionType));
			break;
		}

		if (OnQuestUpdate != null)
		{
			OnQuestUpdate(itemScript.index, questInfo);
		}

		SaveSaveData(saveData);
	}

	public bool HandleRewards ()
	{
		if (saveData.completed)
		{
			return false;
		}

		DailyQuestInfo questInfo = saveData.dailyQuestInfos.Find(item => item.completed.Equals(false));
		if (questInfo == null)
		{
			saveData.completed = true;
			// reward here.
			closeButton.gameObject.SetActive(false);
			EnableDailyQuestManager(true);
			StartCoroutine(RewardAnimationRoutine());
		}

		SaveSaveData(saveData);
		return saveData.completed;
	}

	IEnumerator RewardAnimationRoutine ()
	{
		for (int i = 0; i < dailyQuestItemScripts.Count; i++)
		{
			dailyQuestItemScripts[i].StopAnimation();
		}
		for (int i = 0; i < dailyQuestItemScripts.Count; i++)
		{
			dailyQuestItemScripts[i].PlayAnimation();
			yield return new WaitForSeconds(0.5f);
		}

		yield return new WaitForSeconds(0.5f);

		Sprite chestSprite = Resources.Load<Sprite>("Custom/Sprite/regularChestbox");
		DailyRewardManager.Instance.ShowOpenChest(saveData.rewards, chestSprite, ChestType.daily, false, false, true);
		closeButton.gameObject.SetActive(true);
		OnCloseButtonPressed();
	}

	public void ResetQuest ()
	{
		saveData = null;
		SaveSaveData(saveData);
		SetupDailyQuest();
	}

	void InitScript_Instance_OnItemDestroyed (Square _square)
	{
		if (!saveData.type.Equals(DailyQuestType.Collect))
		{
			return;
		}

		DailyQuestCollectionType collectionType = DailyQuestCollectionType.None;

		switch (_square.type)
		{
		case SquareTypes.BLOCK:
			if (saveData.dailyQuestInfos.Find(item => item.collectionType.Equals(DailyQuestCollectionType.block)) != null)
			{
				collectionType = DailyQuestCollectionType.block;
			}
			break;
		case SquareTypes.SOLIDBLOCK:
			if (saveData.dailyQuestInfos.Find(item => item.collectionType.Equals(DailyQuestCollectionType.solid)) != null)
			{
				collectionType = DailyQuestCollectionType.solid;
			}
			break;
		case SquareTypes.THRIVING:
			if (saveData.dailyQuestInfos.Find(item => item.collectionType.Equals(DailyQuestCollectionType.thriving)) != null)
			{
				collectionType = DailyQuestCollectionType.thriving;
			}
			break;
		}

		if (_square.item != null)
		{
			if (collectionType == DailyQuestCollectionType.None)
			{
				switch (_square.item.currentType)
				{
				case ItemsTypes.SQUARE_BOMB:
					collectionType = DailyQuestCollectionType.item_bomb;
					break;
				case ItemsTypes.CROSS_BOMB:
					collectionType = DailyQuestCollectionType.item_light;
					break;
				}
			}

			if (collectionType == DailyQuestCollectionType.None)
			{
				switch(_square.item.color)
				{
				case 0:
					collectionType = DailyQuestCollectionType.item1;
					break;
				case 1:
					collectionType = DailyQuestCollectionType.item2;
					break;
				case 2:
					collectionType = DailyQuestCollectionType.item3;
					break;
				case 3:
					collectionType = DailyQuestCollectionType.item4;
					break;
				case 4:
					collectionType = DailyQuestCollectionType.item5;
					break;
				case 5:
					collectionType = DailyQuestCollectionType.item6;
					break;
				case 1001:
					collectionType = DailyQuestCollectionType.ingredient1;
					break;
				case 1002:
					collectionType = DailyQuestCollectionType.ingredient2;
					break;
				case 1003:
					collectionType = DailyQuestCollectionType.ingredient3;
					break;
				case 1004:
					collectionType = DailyQuestCollectionType.ingredient4;
					break;
				}
			}
		}

//		Debug.LogError(collectionType);
		DailyQuestInfo questInfo = saveData.dailyQuestInfos.Find(item => item.collectionType.Equals(collectionType));
		if (questInfo  == null)
		{
			return;
		}

		if (questInfo.collecitonCount > 0)
		{
			questInfo.collecitonCount -= 1;
		}
		UpdateQuestInfo(questInfo);
	}

	#region Animation callback

	public void PlaySoundButton ()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
	}

	public void OnFinished ()
	{
		StartCoroutine(TutorialCheckRoutine());
	}

	#endregion

	IEnumerator TutorialCheckRoutine ()
	{
		yield return new WaitForEndOfFrame();
		if (dailyQuestItemScripts != null && dailyQuestItemScripts.Count > 0)
		{
			dailyQuestItemScripts[0].CheckTutorial();
		}
//		DailyRewardChest dailyRewardChest = chestList.Find(item => item.data.type.Equals(ChestType.daily));
//		dailyRewardChest.CheckTutorial();
	}
	public void EnableDailyQuestManager (bool _enable)
	{
		background.gameObject.SetActive(_enable);
		panel.gameObject.SetActive(_enable);
		mAnimation.Play();
	}

	public void OnCloseButtonPressed ()
	{
		LevelManager.Instance.questInfo = null;
		EnableDailyQuestManager(false);
//		GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.Try_ChestBox, GameObject.Find("DailyReward").GetComponent<RectTransform>());
//		GameTutorialManager.Instance.CheckBoostShopTutorial();
	}
}


