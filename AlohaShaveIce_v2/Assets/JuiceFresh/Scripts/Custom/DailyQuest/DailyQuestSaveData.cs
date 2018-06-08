using System.Collections;
using System.Collections.Generic;

public class DailyQuestSaveData
{
	public List<DailyQuestInfo> dailyQuestInfos { get; set; }
	public bool completed { get; set; }
}

public class DailyQuestInfo
{
	public int level { get; set; }
	public bool completed { get; set; }
}
