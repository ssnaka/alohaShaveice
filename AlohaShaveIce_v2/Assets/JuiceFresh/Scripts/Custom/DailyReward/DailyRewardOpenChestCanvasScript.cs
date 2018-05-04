using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyRewardOpenChestCanvasScript : MonoBehaviour 
{
	[SerializeField]
	Image chestImage;
	[SerializeField]
	Image chestImageOpen;
	[SerializeField]
	GameObject chest3DContainer;
	[SerializeField]
	GameObject rewardItemsView;
	[SerializeField]
	GameObject rewardItemContainer;
	[SerializeField]
	GameObject rewardItemPrefab;

	GameObject chest3DObject;
	List<RewardResultContent> rewardResultContentList = new List<RewardResultContent>();

	List<PossibleReward> possibleRewards;

	[SerializeField]
	Button openButton;
	[SerializeField]
	Animation chestAnimation;

	// sprite
	public void SetupOpenChest (List<PossibleReward> _possibleRewards, Sprite _chestSprite, ChestType _chestType)
	{
		DailyRewardManager.Instance.EnableReward(false);
		gameObject.SetActive(true);
		rewardItemsView.SetActive(false);

		possibleRewards = _possibleRewards;
		for (int i = 0 ; i < rewardResultContentList.Count ; i++)
		{
			RewardResultContent rewardResultContent = rewardResultContentList[i];
			Destroy(rewardResultContent.gameObject);
		}
		rewardResultContentList.Clear();

		for (int i = 0 ; i < possibleRewards.Count ; i++)
		{
			PossibleReward possibleReward = possibleRewards[i];

			GameObject rewardItem = Instantiate(rewardItemPrefab, rewardItemContainer.transform);
			rewardItem.transform.localScale = Vector3.one;

			RewardResultContent rewardResultContent = rewardItem.GetComponent<RewardResultContent>();
			rewardResultContent.SetItemImage(possibleReward);
			rewardResultContentList.Add(rewardResultContent);
		}

		chestImage.overrideSprite = _chestSprite;
//		chestImage.gameObject.SetActive(false);
		chestImageOpen.overrideSprite = Resources.Load<Sprite>("Custom/Sprite/" + _chestSprite.name + "_open");
		openButton.interactable = true;
		chestImage.gameObject.SetActive(true);
		chestImageOpen.gameObject.SetActive(false);

		string idleAnimationName = "chest_regular_image_idle";
		switch (_chestType)
		{
		case ChestType.daily:
			idleAnimationName = "chest_regular_image_idle";
			break;
		case ChestType.premium:
			idleAnimationName = "chest_premium_image_idle";
			break;
			default:
			break;
		}

		chestAnimation.Stop();
		chestAnimation.clip = chestAnimation.GetClip(idleAnimationName);
		chestAnimation.Play();
//		StartCoroutine(RunOpenChestRoutine());
	}

	// 3d
	public void SetupOpenChest (List<PossibleReward> _possibleRewards, GameObject _chestPrefab)
	{
		DailyRewardManager.Instance.EnableReward(false);
		gameObject.SetActive(true);
		rewardItemsView.SetActive(false);

		possibleRewards = _possibleRewards;
		for (int i = 0 ; i < rewardResultContentList.Count ; i++)
		{
			RewardResultContent rewardResultContent = rewardResultContentList[i];
			Destroy(rewardResultContent.gameObject);
		}
		rewardResultContentList.Clear();

		for (int i = 0 ; i < possibleRewards.Count ; i++)
		{
			PossibleReward possibleReward = possibleRewards[i];

			GameObject rewardItem = Instantiate(rewardItemPrefab, rewardItemContainer.transform);
			rewardItem.transform.localScale = Vector3.one;

			RewardResultContent rewardResultContent = rewardItem.GetComponent<RewardResultContent>();
			rewardResultContent.SetItemImage(possibleReward);
			rewardResultContentList.Add(rewardResultContent);
		}


		if (chest3DObject != null && !chest3DObject.name.Contains(_chestPrefab.name))
		{
			Destroy(chest3DObject);
			chest3DObject = null;
		}

		if (chest3DObject == null)
		{
			CreateNewChest(_chestPrefab);
		}

		StartCoroutine(RunOpenChestRoutine());
	}

	void CreateNewChest (GameObject _chestPrefab)
	{
		chest3DObject = Instantiate<GameObject>(_chestPrefab, chest3DContainer.transform);
		chest3DObject.transform.localScale = new Vector3 (100.0f, 100.0f, 100.0f); //Vector3.one;
		chest3DObject.transform.localPosition = Vector3.zero;
		chest3DObject.transform.eulerAngles = new Vector3(0.0f, 162.0f, 0.0f);
		chest3DContainer.SetActive(false);
	}

	public void OnOpenButtonPressed ()
	{
		openButton.interactable = false;
		StartCoroutine(RunOpenChestRoutine());
	}

	IEnumerator RunOpenChestRoutine ()
	{
		//chest animation
//		chest3DContainer.SetActive(true);
//		chestImage.gameObject.SetActive(true);
		chestAnimation.Stop();
		chestAnimation.clip = chestAnimation.GetClip("chestOpen_anim");
		chestAnimation.Play();
		yield return new WaitForSeconds(1.5f);

		rewardItemsView.SetActive(true);
		chestImage.gameObject.SetActive(false);
		chestImageOpen.gameObject.SetActive(false);
//		chest3DContainer.SetActive(false);
	}

	public void OnCloseButtonPressed ()
	{
		for (int i = 0 ; i < possibleRewards.Count ; i++)
		{
			PossibleReward possibleReward = possibleRewards[i];
			InitScript.Instance.GiveDailyReward(possibleReward);
		}

		gameObject.SetActive(false);
		DailyRewardManager.Instance.EnableReward(true);
	}
}
