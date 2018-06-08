﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class EventsListener : MonoBehaviour
{

	void OnEnable ()
	{
		LevelManager.OnMapState += OnMapState;
		LevelManager.OnEnterGame += OnEnterGame;
		LevelManager.OnLevelLoaded += OnLevelLoaded;
		LevelManager.OnMenuPlay += OnMenuPlay;
		LevelManager.OnMenuComplete += OnMenuComplete;
		LevelManager.OnStartPlay += OnStartPlay;
		LevelManager.OnWin += OnWin;
		LevelManager.OnLose += OnLose;
		LevelManager.OnLevelComplete += LevelManager_OnLevelComplete;

		LevelManager.OnPowerUpUsed += LevelManager_OnPowerUpUsed;

		InitScript.OnAppStart += InitScript_OnAppStart;
		InitScript.OnAppEnd += InitScript_OnAppEnd;
		InitScript.OnVideoAdShown += InitScript_OnVideoAdShown;
		InitScript.OnFreeChestOpen += InitScript_OnFreeChestOpen;
		InitScript.OnFreeChestOpenWithAds += InitScript_OnFreeChestOpenWithAds;;
		InitScript.OnDailyChestOpen += InitScript_OnDailyChestOpen;
		InitScript.OnPremiumChestOpen += InitScript_OnPremiumChestOpen;

		GameTutorialManager.onTutorialEvent += GameTutorialManager_onTutorialEvent;
	}

	void OnDisable ()
	{
		LevelManager.OnMapState -= OnMapState;
		LevelManager.OnEnterGame -= OnEnterGame;
		LevelManager.OnLevelLoaded -= OnLevelLoaded;
		LevelManager.OnMenuPlay -= OnMenuPlay;
		LevelManager.OnMenuComplete -= OnMenuComplete;
		LevelManager.OnStartPlay -= OnStartPlay;
		LevelManager.OnWin -= OnWin;
		LevelManager.OnLose -= OnLose;
		LevelManager.OnLevelComplete -= LevelManager_OnLevelComplete;

		LevelManager.OnAppEnd -= LevelManager_OnAppEnd;
		LevelManager.OnPowerUpUsed -= LevelManager_OnPowerUpUsed;
		LevelManager.OnPowerUpUsed -= LevelManager_OnPowerUpUsed;

		InitScript.OnAppStart -= InitScript_OnAppStart;
		InitScript.OnAppEnd -= InitScript_OnAppEnd;
		InitScript.OnVideoAdShown -= InitScript_OnVideoAdShown;
		InitScript.OnFreeChestOpen -= InitScript_OnFreeChestOpen;
		InitScript.OnFreeChestOpenWithAds -= InitScript_OnFreeChestOpenWithAds;
		InitScript.OnDailyChestOpen -= InitScript_OnDailyChestOpen;
		InitScript.OnPremiumChestOpen -= InitScript_OnPremiumChestOpen;
	}

	#region GAME_EVENTS

	void OnMapState ()
	{
	}

	void OnEnterGame ()
	{
		AnalyticsEvent("OnEnterLevel", LevelManager.THIS.currentLevel);
	}

	void LevelManager_OnLevelComplete (bool _isWin)
	{
		Dictionary<string, object> dic = new Dictionary<string, object>();
		dic.Add("level", LevelManager.THIS.currentLevel);
		dic.Add("didwin", _isWin);
		AnalyticsEvent("OnEnterLevel", dic);
	}

	void OnLevelLoaded ()
	{
	}

	void OnMenuPlay ()
	{
	}

	void OnMenuComplete ()
	{
	}

	void OnStartPlay ()
	{
	}

	void OnWin ()
	{
		AnalyticsEvent("OnWin", LevelManager.THIS.currentLevel);
	}

	void OnLose ()
	{
		AnalyticsEvent("OnLose", LevelManager.THIS.currentLevel);
	}

	void LevelManager_OnAppStart ()
	{
		AnalyticsEvent("OnAppStart", System.DateTime.Now);
	}

	void LevelManager_OnAppEnd ()
	{
		AnalyticsEvent("OnAppEnd", System.DateTime.Now);
	}

	void LevelManager_OnPowerUpUsed ()
	{
		Dictionary<string, object> dic = new Dictionary<string, object>();
		dic.Add("level", LevelManager.THIS.currentLevel);
		dic.Add("boost", LevelManager.THIS.ActivatedBoost.type.ToString());
		AnalyticsEvent("OnPowerUpUsed", dic);
	}

	void InitScript_OnAppStart ()
	{
		AnalyticsEvent("OnAppStart", System.DateTime.Now);
	}

	void InitScript_OnAppEnd ()
	{
		AnalyticsEvent("OnAppEnd", System.DateTime.Now);
	}

	void InitScript_OnPremiumChestOpen ()
	{
		AnalyticsEvent("OnPremiumChestOpen", "");
	}

	void InitScript_OnDailyChestOpen ()
	{
		AnalyticsEvent("OnDailyChestOpen", "");
	}

	void InitScript_OnFreeChestOpenWithAds ()
	{
		AnalyticsEvent("OnFreeChestOpenWithAds", "");
	}

	void InitScript_OnFreeChestOpen ()
	{
		AnalyticsEvent("OnFreeChestOpen", "");
	}

	void InitScript_OnVideoAdShown ()
	{
		AnalyticsEvent("OnVideoAdShown", "");
	}

	void GameTutorialManager_onTutorialEvent (TutorialType _tutorialType)
	{
		AnalyticsEvent("OnTutorialEvent", _tutorialType.ToString());
	}
	#endregion


	void AnalyticsEvent (string _event, params object[] _obj)
	{
#if UNITY_ANALYTICS
		Dictionary<string, object> dic = new Dictionary<string, object>();
		for (int i = 0 ; i < _obj.Length; i++)
		{
			dic[_event] = _obj[i];
		}
		Analytics.CustomEvent(_event, dic);
#endif
	}
}
