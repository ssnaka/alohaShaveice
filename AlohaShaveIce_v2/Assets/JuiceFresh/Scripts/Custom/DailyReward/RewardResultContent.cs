using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardResultContent : MonoBehaviour {

	[SerializeField]
	Image itemImage;

	[SerializeField]
	Text countText;
	public void SetItemImage (PossibleReward _possibleReward)
	{
		string spriteName = GameUtility.GetRewardItemSpriteByType(_possibleReward.type);
		Sprite rewardItemSprite = Resources.Load<Sprite>(spriteName);
		itemImage.overrideSprite = rewardItemSprite;

		string newText = "x " + _possibleReward.count.ToString();
		switch(_possibleReward.type)
		{
		case RewardedAdsType.Unlimited_Life:
			newText = _possibleReward.count.ToString() + "sec";
			break;
			default:
			break;
		}
		countText.text = newText;
	}
}
