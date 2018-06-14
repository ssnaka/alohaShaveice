using System.Collections;
using System.Collections.Generic;
using System;

public class DailyQuestSaveData
{
	public DateTime questDate { get; set; }
	public DailyQuestType type { get; set; }
	public List<DailyQuestInfo> dailyQuestInfos { get; set; }
	public bool completed { get; set; }
	public List<PossibleReward> rewards {get; set;}
}

public class DailyQuestInfo
{
	public int level { get; set; }
	public int actualLevel { get; set; }
	public DailyQuestCollectionType collectionType { get; set; }
	public int collecitonCount { get; set; }
	public bool completed { get; set; }
}
