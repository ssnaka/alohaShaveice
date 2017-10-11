using UnityEngine;
using System.Collections;

public enum AdType {
    AdmobInterstitial,
    ChartboostInterstitial,
    UnityAdsVideo,
	AppODeal
}

[System.Serializable]
public class AdEvents {
    public GameState gameEvent;
    public AdType adType;
    public int everyLevel;
    public int calls;

}

[System.Serializable]
public class BoostAdEvents {
	public BoostType boostType;
	public AdType adType;
	public int countToReward;
	public int calls;
}