using System.Collections;
using System.Collections.Generic;

public enum DailyQuestType
{
	Levels,
	Collect
}

public class DailyQuestConfigData
{
//	public List<ChestData> chests {get; set;}
	public List<RewardItem> possibleRewards {get; set;}
}
