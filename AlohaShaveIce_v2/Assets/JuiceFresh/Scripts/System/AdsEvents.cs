﻿using UnityEngine;
using System.Collections;

public enum AdType {
    AdmobInterstitial,
    ChartboostInterstitial,
    UnityAdsVideo,
	Appodeal
}

[System.Serializable]
public class AdEvents {
    public GameState gameEvent;
    public AdType adType;
    public int everyLevel;
    public int calls;

}