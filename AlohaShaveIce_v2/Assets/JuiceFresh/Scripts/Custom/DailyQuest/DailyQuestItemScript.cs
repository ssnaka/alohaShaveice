using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolkit.Localization;

public class DailyQuestItemScript : MonoBehaviour {

	[SerializeField]
	Text numberText;
	LocalizedTextBehaviour textBehaviour;
	LocalizedText completeText;

	[SerializeField]
	Image stairImage;
	[SerializeField]
	RectTransform iconContainer;
	[SerializeField]
	Image foregroundImage;
	[SerializeField]
	Image starImage;

	DailyQuestType questType;
	public int index {get; private set;}
	public DailyQuestInfo questInfo {get; private set;}

	[SerializeField]
	Animation m_animation;

	void OnDestroy ()
	{
//		DailyQuestManager.Instance.OnQuestUpdate -= DailyQuestManager_Instance_OnQuestUpdate;
	}

	// Use this for initialization
	void Start () 
	{
		
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}

	public void SetupItem (DailyQuestType _questType, DailyQuestInfo _questInfo, int _index)
	{
		if (textBehaviour == null)
		{
			textBehaviour = numberText.GetComponent<LocalizedTextBehaviour>();
			if (textBehaviour != null)
			{
				completeText = textBehaviour.LocalizedAsset;	
			}
		}

		questType = _questType;
		DailyQuestManager.Instance.OnQuestUpdate -= DailyQuestManager_Instance_OnQuestUpdate;
		DailyQuestManager.Instance.OnQuestUpdate += DailyQuestManager_Instance_OnQuestUpdate;
		index = _index;
		questInfo = _questInfo;

		if (questType.Equals(DailyQuestType.Collect))
		{
			textBehaviour.LocalizedAsset = null;
			numberText.text = "×" + questInfo.collecitonCount.ToString();

			string spriteName = "Texture/";
			switch(questInfo.collectionType)
			{
			case DailyQuestCollectionType.item1:
				spriteName += "Items/item_01_02";
				break;
			case DailyQuestCollectionType.item2:
				spriteName += "Items/item_02_02";
				break;
			case DailyQuestCollectionType.item3:
				spriteName += "Items/item_03_02";
				break;
			case DailyQuestCollectionType.item4:
				spriteName += "Items/item_04_02";
				break;
			case DailyQuestCollectionType.item5:
				spriteName += "Items/item_05_02";
				break;
			case DailyQuestCollectionType.item6:
				spriteName += "Items/item_06_02";
				break;
			case DailyQuestCollectionType.ingredient1:
				spriteName += "Items/ingredient_01";
				break;
			case DailyQuestCollectionType.ingredient2:
				spriteName += "Items/ingredient_02";
				break;
			case DailyQuestCollectionType.ingredient3:
				spriteName += "Items/ingredient_03";
				break;
			case DailyQuestCollectionType.ingredient4:
				spriteName += "Items/ingredient_04";
				break;
			case DailyQuestCollectionType.item_bomb:
				spriteName += "Items/" + questInfo.collectionType.ToString();
				break;
			case DailyQuestCollectionType.item_light:
				spriteName += "Items/" + questInfo.collectionType.ToString();
				break;
			case DailyQuestCollectionType.block:
				spriteName += "Blocks/block";
				break;
			case DailyQuestCollectionType.thriving:
				spriteName += "Blocks/thriving_block";
				break;
			case DailyQuestCollectionType.solid:
				spriteName += "Blocks/solidBlock";
				break;
			}

			Sprite itemSprite = Resources.Load<Sprite>(spriteName);
			foregroundImage.overrideSprite = itemSprite;
		}
		else
		{
			numberText.text = questInfo.level.ToString();
		}


		if (iconContainer != null && !questType.Equals(DailyQuestType.Collect))
		{
//			Debug.LogError(iconContainer.localPosition.x);
			iconContainer.anchoredPosition = new Vector2(iconContainer.anchoredPosition.x, _index * 40.0f);
		}
		if (stairImage != null)
		{
			float newHeight = stairImage.rectTransform.sizeDelta.y;
			newHeight *= (_index + 1);
			stairImage.rectTransform.sizeDelta = new Vector2(stairImage.rectTransform.sizeDelta.x, newHeight);
		}

		HandleButtons();
	}

	void HandleButtons ()
	{
		if (foregroundImage != null && !questType.Equals(DailyQuestType.Collect))
		{
			foregroundImage.gameObject.SetActive(true);
			if (!questInfo.completed && index > 0)
			{
				foregroundImage.gameObject.SetActive(false);
			}
		}
			
		if (questInfo.completed)
		{
			starImage.gameObject.SetActive(true);
		}
	}


	void DailyQuestManager_Instance_OnQuestUpdate (int _index, DailyQuestInfo _questInfo)
	{
		if (_index.Equals(index))
		{
			questInfo = _questInfo;
			HandleButtons();
			if (questType.Equals(DailyQuestType.Collect))
			{
				if (questInfo.collecitonCount > 0)
				{
					textBehaviour.LocalizedAsset = null;
					numberText.text = "×" + questInfo.collecitonCount.ToString();
				}
				else
				{
					textBehaviour.LocalizedAsset = completeText;
				}

			}
		}
		else if (_index.Equals(index - 1) && _questInfo.completed)
		{
			if (foregroundImage != null && !questType.Equals(DailyQuestType.Collect))
			{
				foregroundImage.gameObject.SetActive(true);
			}
		}
	}

	public void OnButtonPressed ()
	{
		GameTutorialManager.Instance.CloseTutorial();
		if (!GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf)
		{
			LevelManager.THIS.questInfo = questInfo;
			LevelManager.THIS.questLevel = questInfo.actualLevel;
			GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
		}
	}

	public void StopAnimation ()
	{
		m_animation.Stop();
	}

	public void PlayAnimation ()
	{
		m_animation.Play();
	}

	public void CheckTutorial ()
	{
		if (questType.Equals(DailyQuestType.Collect))
		{
			GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.DailyQuest_CollectItems, iconContainer);
		}
		else
		{
			GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.DailyQuest_OpenLevel, foregroundImage.rectTransform);
		}
	}
}
