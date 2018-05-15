using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUtility
{
	public static string GetRewardItemSpriteByType (RewardedAdsType _type)
	{
		switch (_type)
		{
		case RewardedAdsType.Bomb:
			return "Texture/Boosts/boost_bomb";
		case RewardedAdsType.Colorful_bomb:
			return "Texture/Boosts/play_menu_mix_bomb_booster";
		case RewardedAdsType.Energy:
			return "Texture/Boosts/boost_energy";
		case RewardedAdsType.ExtraMoves:
			return "Texture/Boosts/boost_5_mooves";
		case RewardedAdsType.ExtraTime:
			return "Texture/Boosts/boost_5_time";
		case RewardedAdsType.Shovel:
			return "Texture/Boosts/boost_shovel";
		case RewardedAdsType.Stripes:
			return "Texture/Boosts/play_menu_2super-fruits_booster";
		case RewardedAdsType.GetGems:
			return "Texture/Boosts/gems_refilled";
		case RewardedAdsType.GetLifes:
			return "Texture/Boosts/heart_refilled";
		case RewardedAdsType.Unlimited_Life:
			return "Texture/Boosts/heart_refilled";
		case RewardedAdsType.Counter:
			return string.Empty;
		case RewardedAdsType.GetGoOn:
			return string.Empty;
		default:
			return string.Empty;
		}
	}

	public static float DeviceDiagonalSizeInInches ()
	{
		float screenWidth = Screen.width / Screen.dpi;
		float screenHeight = Screen.height / Screen.dpi;
		float diagonalInches = Mathf.Sqrt (Mathf.Pow (screenWidth, 2) + Mathf.Pow (screenHeight, 2));

//		Debug.Log ("Getting device inches: " + diagonalInches);

		return diagonalInches;
	}
}
