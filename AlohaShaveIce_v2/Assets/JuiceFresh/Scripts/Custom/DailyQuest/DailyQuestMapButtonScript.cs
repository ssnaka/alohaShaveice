using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DailyQuestMapButtonScript : MonoBehaviour 
{

	[SerializeField]
	Text timerText;
	DateTime nextDay;

	void OnEnable ()
	{
		nextDay = DateTime.Now.Date.AddDays(1);
	}

	// Use this for initialization
	void Start () 
	{
		DailyQuestManager.Instance.SetupDailyQuest();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (DateTime.Now.Date >= nextDay)
		{
			nextDay = DateTime.Now.Date.AddDays(1);
			DailyQuestManager.Instance.ResetQuest();
		}

		TimeSpan span = nextDay.Subtract(DateTime.Now);
		timerText.text = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
	}

	public void OnButtonPressed ()
	{
//		DailyQuestManager.Instance.ResetQuest();
		GameTutorialManager.Instance.CloseTutorial();
		DailyQuestManager.Instance.SetupDailyQuest(true);
	}

}
