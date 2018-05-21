using System.Collections;
using System.Collections.Generic;

public enum TutorialType
{
	First_Tutorial,			//0
	Level1,					//1

	Buy_Boosts,				//2
	Buy_Boosts_WithAd,		//3

	Open_ChestBox,			//4
	Open_ChestBox_WithAd,	//5
	Try_ChestBox,			//6

	Use_Stripe,				//7
	Use_Bomb,				//8
	Use_ColorBomb,			//9
	Use_Shovel,				//10
	Use_Energy,				//11
	Use_ExtraTime,			//12
	Use_ExtraMove,			//13

	Use_Bomb_InLevel,		//14

	Level_Target_Block,		//15
	Level_Target_Bomb,		//16
	Level_Target_Bubble,	//17
	Level_Target_Ingredient,//18
	Level_Target_Item,		//19

	Use_CrossBomb_InLevel,	//20
	UndestroyableBlock,		//21

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
