using UnityEngine;
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


		LevelManager.OnPowerUpUsed += LevelManager_OnPowerUpUsed;
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

		LevelManager.OnAppStart += LevelManager_OnAppStart;
		LevelManager.OnAppEnd += LevelManager_OnAppEnd;
		LevelManager.OnPowerUpUsed += LevelManager_OnPowerUpUsed;
	}

	#region GAME_EVENTS

	void OnMapState ()
	{
	}

	void OnEnterGame ()
	{
		AnalyticsEvent("OnEnterGame", LevelManager.THIS.currentLevel);
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
		AnalyticsEvent("OnPowerUpUsed", LevelManager.THIS.currentLevel, LevelManager.THIS.ActivatedBoost.type.ToString());
	}

	#endregion

	void AnalyticsEvent (string _event, params object[] _obj)
	{
#if UNITY_ANALYTICS
		Dictionary<string, object> dic = new Dictionary<string, object>();
		for (int i = 0 ; i < _obj.Length; i++)
		{
			dic.Add(_event, _obj[i]);
		}
		Analytics.CustomEvent(_event, dic);

#endif
	}
}
