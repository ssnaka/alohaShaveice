using System.Collections;
using System.Collections.Generic;

public enum DailyQuestType
{
	None,
	RandomLevel,
	PreviousLevel,
	NextLevel,
	Collect
}

public enum DailyQuestCollectionType
{
	None,
	item1,
	item2,
	item3,
	item4,
	item5,
	item6,
	ingredient1,
	ingredient2,
	ingredient3,
	ingredient4,
	item_bomb,
	item_light,
	solid,
	thriving,
	block
}

public class DailyQuestConfigData
{
//	public List<ChestData> chests {get; set;}
	public List<QuestConfigData> dailyQuests {get; set;}
	public List<DailyQuestCollectionType> collection {get; set;}
	public List<int> collectionCount {get; set;}
	public List<PossibleReward> rewards {get; set;}
}

public class QuestConfigData
{
	public DailyQuestType type {get; set;}
	public int count {get; set;}
}