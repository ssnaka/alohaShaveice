using System.Collections;
using System.Collections.Generic;

public enum ChestType
{
	daily,
	premium
}

public class DailyRewardData
{
	public List<ChestData> chests {get; set;}
}

public class ChestData
{
	public ChestType type {get; set;}
	public int price {get; set;}
	public string chestImage {get; set;}
	public string chestPrefab {get; set;}
	public List<RewardData> data {get; set;}
}

public class RewardData
{
	public int day {get; set;}
	public List<RewardItem> items {get; set;}
}

public class RewardItem
{
	public List<PossibleReward> possibleRewards {get; set;}
	public int weight {get; set;}
}

public class PossibleReward
{
	public RewardedAdsType type {get; set;}
	public int count {get; set;}
}
