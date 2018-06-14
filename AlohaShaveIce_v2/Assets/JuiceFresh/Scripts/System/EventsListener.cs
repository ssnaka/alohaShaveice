using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_ANALYTICS
using UnityEngine.Analytics;
#endif

public class EventsListener : MonoBehaviour
{
	System.DateTime gameStartTime;
	void OnEnable ()
	{
		gameStartTime = System.DateTime.Now;
		LevelManager.OnMapState += OnMapState;
		LevelManager.OnEnterLevel += OnEnterLevel;
		LevelManager.OnLevelLoaded += OnLevelLoaded;
		LevelManager.OnMenuPlay += OnMenuPlay;
//		LevelManager.OnMenuComplete += OnMenuComplete;
//		LevelManager.OnStartPlay += OnStartPlay;
//		LevelManager.OnWin += OnWin;
//		LevelManager.OnLose += OnLose;
		LevelManager.OnLevelComplete += LevelManager_OnLevelComplete;
		LevelManager.OnPowerUpUsed += LevelManager_OnPowerUpUsed;

		LevelManager.OnQuestLevelStart += LevelManager_OnQuestLevelStart;
		LevelManager.OnQuestLevelEnd += LevelManager_OnQuestLevelEnd;
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
		LevelManager.OnEnterLevel -= OnEnterLevel;
		LevelManager.OnLevelLoaded -= OnLevelLoaded;
		LevelManager.OnMenuPlay -= OnMenuPlay;
//		LevelManager.OnMenuComplete -= OnMenuComplete;
//		LevelManager.OnStartPlay -= OnStartPlay;
//		LevelManager.OnWin -= OnWin;
//		LevelManager.OnLose -= OnLose;
		LevelManager.OnLevelComplete -= LevelManager_OnLevelComplete;
		LevelManager.OnPowerUpUsed -= LevelManager_OnPowerUpUsed;

		LevelManager.OnQuestLevelStart -= LevelManager_OnQuestLevelStart;
		LevelManager.OnQuestLevelEnd -= LevelManager_OnQuestLevelEnd;

		InitScript.OnAppStart -= InitScript_OnAppStart;
		InitScript.OnAppEnd -= InitScript_OnAppEnd;
		InitScript.OnVideoAdShown -= InitScript_OnVideoAdShown;
		InitScript.OnFreeChestOpen -= InitScript_OnFreeChestOpen;
		InitScript.OnFreeChestOpenWithAds -= InitScript_OnFreeChestOpenWithAds;
		InitScript.OnDailyChestOpen -= InitScript_OnDailyChestOpen;
		InitScript.OnPremiumChestOpen -= InitScript_OnPremiumChestOpen;
	}

//	void OnApplicationQuit ()
//	{
//		
//	}

	#region GAME_EVENTS

	void OnMapState ()
	{
	}

	void OnEnterLevel ()
	{
		AnalyticsEvent("OnEnterLevel", LevelManager.THIS.currentLevel);
	}

	void LevelManager_OnLevelComplete (bool _isWin, int _limitLeftover, bool _extraLifeUsed)
	{
		Dictionary<string, object> dic = new Dictionary<string, object>();
		dic.Add("level", LevelManager.THIS.currentLevel);
		dic.Add("didwin", _isWin.ToString());
		dic.Add("limit_left", _limitLeftover);
		dic.Add("extraLifeUsed", _extraLifeUsed);
		AnalyticsEvent("OnExitLevel", dic);
	}

	void OnLevelLoaded ()
	{
	}

	void OnMenuPlay ()
	{
	}

//	void OnMenuComplete ()
//	{
//	}

//	void OnStartPlay ()
//	{
//	}

//	void OnWin ()
//	{
//		AnalyticsEvent("OnWin", LevelManager.THIS.currentLevel);
//	}
//
//	void OnLose ()
//	{
//		AnalyticsEvent("OnLose", LevelManager.THIS.currentLevel);
//	}

//	void LevelManager_OnAppStart ()
//	{
//		AnalyticsEvent("OnAppStart", System.DateTime.Now);
//	}

//	void LevelManager_OnAppEnd ()
//	{
//		System.TimeSpan playTime = System.DateTime.Now.Subtract(gameStartTime);
//		AnalyticsEvent("OnAppEnd", playTime.TotalSeconds);
//	}

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
		
		System.TimeSpan playTime = System.DateTime.Now.Subtract(gameStartTime);
		AnalyticsEvent("OnAppEnd", playTime.TotalSeconds);
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



	void LevelManager_OnQuestLevelStart (int _level)
	{
		AnalyticsEvent("OnEnterLevel", _level);
	}

	void LevelManager_OnQuestLevelEnd (int _level, bool _isWin, int _limitLeftOver, bool _extraLifeUsed)
	{
		Dictionary<string, object> dic = new Dictionary<string, object>();
		dic.Add("level", _level);
		dic.Add("didwin", _isWin.ToString());
		dic.Add("limit_left", _limitLeftOver);
		dic.Add("extraLifeUsed", _extraLifeUsed);
		AnalyticsEvent("OnExitLevel", dic);
	}
	#endregion


	void AnalyticsEvent (string _event, params object[] _obj)
	{
#if UNITY_ANALYTICS && !UNITY_EDITOR
		Dictionary<string, object> dic = new Dictionary<string, object>();
		for (int i = 0 ; i < _obj.Length; i++)
		{
			if (_obj[i] is Dictionary<string, object>)
			{
				dic = _obj[i] as Dictionary<string, object>;
			}
			else
			{	
				dic[_event] = _obj[i];
			}
		}
		Analytics.CustomEvent(_event, dic);
#endif
	}
}
