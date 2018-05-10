using System.Collections;
using System.Collections.Generic;

public enum TutorialType
{
	First_Tutorial,
	Level1,

	Buy_Boosts,
	Buy_Boosts_WithAd,

	Open_ChestBox,
	Open_ChestBox_WithAd,
	Try_ChestBox,

	Use_Stripe,
	Use_Bomb,
	Use_ColorBomb,
	Use_Shovel,
	Use_Energy,
	Use_ExtraTime,
	Use_ExtraMove,

	Use_Bomb_InLevel,

	None,
}

public class TutorialSaveData 
{
	public List<SavedTutorial> tutorials {get; set;}
}

public class SavedTutorial
{
	public TutorialType type {get; set;}
	public bool status {get; set;}
}
