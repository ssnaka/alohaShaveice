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
    			int min =_possibleReward.count / 60;
    			newText = min.ToString() + ":00";
    			break;
			default:
    			break;
		}
		countText.text = newText;
	}

    void OnEnable ()
    {
        StartCoroutine(ResetImageSize());
    }

    IEnumerator ResetImageSize ()
    {
        yield return new WaitForEndOfFrame();
        itemImage.rectTransform.sizeDelta = new Vector2(GetComponent<RectTransform>().sizeDelta.x, itemImage.rectTransform.sizeDelta.y);   
    }
}
