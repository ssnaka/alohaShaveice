using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using GameToolkit.Localization;

public class LIFESAddCounter : MonoBehaviour
{
    Text text;
    static float TimeLeft;
    float TotalTimeForRestLife = 15f * 60;  //8 minutes for restore life
    bool startTimer;
    DateTime templateTime;

	static int infiniteLifeRnadomStart = 300;
	static int infiniteLifeRnadomEnd = 3600;
	static int infiniteLifeCut = 30;
	public bool isInfiniteLife = false;

	bool readyToUpdate;

	[SerializeField]
	LocalizedText fullTextAsset;
	[SerializeField]
	LocalizedTextBehaviour fullTextBehaviour;

    // Use this for initialization
    void Start()
    {
        text = GetComponent<Text>();
        
        //if (TotalTimeForRestLife != InitScript.RestLifeTimer) { InitScript.RestLifeTimer = TotalTimeForRestLife;  }
//		SetupInfiniteLife();
    }

    bool CheckPassedTime()
    {
        //print(InitScript.DateOfExit );
        if (InitScript.DateOfExit == "" || InitScript.DateOfExit == default(DateTime).ToString())
            InitScript.DateOfExit = DateTime.Now.ToString();

        DateTime dateOfExit = DateTime.Parse(InitScript.DateOfExit);

		float timeForReset = TotalTimeForRestLife * (InitScript.Instance.CapOfLife - InitScript.lifes);
		if (isInfiniteLife)
		{
			timeForReset = InitScript.RestLifeTimer;
		}

		if (DateTime.Now.Subtract(dateOfExit).TotalSeconds > timeForReset)
        {
			InitScript.RestLifeTimer = 0;
			isInfiniteLife = false;
            InitScript.Instance.RestoreLifes();
            return false;    ///we dont need lifes
		}
        else
        {
        	TimeCount((float)DateTime.Now.Subtract(dateOfExit).TotalSeconds);
            return true;     ///we need lifes
		}
    }

    void TimeCount(float tick)
    {
        if (InitScript.RestLifeTimer <= 0)
            ResetTimer();
		
        InitScript.RestLifeTimer -= tick;

		if (isInfiniteLife)
		{
			if (InitScript.RestLifeTimer <= 1)
			{
				InitScript.RestLifeTimer = 0;
				isInfiniteLife = false;
				InitScript.Instance.RestoreLifes();
				ResetTimer();
			}
		}
		else
		{
	        if (InitScript.RestLifeTimer <= 1 && InitScript.lifes < InitScript.Instance.CapOfLife)
	        {
	            InitScript.Instance.AddLife(1);
	            ResetTimer();
	        }
		}
    }

    void ResetTimer()
    {
        InitScript.RestLifeTimer = TotalTimeForRestLife;
    }

    // Update is called once per frame
    void Update()
    {
		if (!readyToUpdate)
		{
			return;
		}

        if (!startTimer && DateTime.Now.Subtract(DateTime.Now).Days == 0)
        {
            InitScript.DateOfRestLife = DateTime.Now;

            if (InitScript.lifes < InitScript.Instance.CapOfLife)
            {
                if (CheckPassedTime())
                    startTimer = true;
                //	StartCoroutine(TimeCount());
            }
        }

        if (startTimer)
            TimeCount(Time.deltaTime);

        if (gameObject.activeSelf)
        {
			if (fullTextBehaviour.LocalizedAsset != null)
			{
				fullTextBehaviour.LocalizedAsset = null;
			}
            if (InitScript.lifes < InitScript.Instance.CapOfLife)
            {
                if (InitScript.Instance.TotalTimeForRestLifeHours > 0)
                {
                    int hours = Mathf.FloorToInt(InitScript.RestLifeTimer / 3600);
                    int minutes = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) / 60);
                    int seconds = Mathf.FloorToInt((InitScript.RestLifeTimer - hours * 3600) - minutes * 60);

                    text.enabled = true;
                    text.text = "" + string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
                }
                else
                {
                    int minutes = Mathf.FloorToInt(InitScript.RestLifeTimer / 60F);
                    int seconds = Mathf.FloorToInt(InitScript.RestLifeTimer - minutes * 60);

                    text.enabled = true;
                    text.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);

                }
                InitScript.timeForReps = text.text;
                //				//	text.text = "+1 in \n " + Mathf.FloorToInt( MainMenu.RestLifeTimer/60f) + ":" + Mathf.RoundToInt( (MainMenu.RestLifeTimer/60f - Mathf.FloorToInt( MainMenu.RestLifeTimer/60f))*60f);
            }
            else
            {
				if (fullTextBehaviour.LocalizedAsset == null)
				{
					fullTextBehaviour.LocalizedAsset = fullTextAsset;
				}
//				text.text = "  Full";
            }
        }
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            //	StopCoroutine("TimeCount");
            InitScript.DateOfExit = DateTime.Now.ToString();
            //			PlayerPrefs.SetString("DateOfExit",DateTime.Now.ToString());
            //			PlayerPrefs.Save();
        }
        else
        {
            startTimer = false;
            //MainMenu.today = DateTime.Now; 
            //		MainMenu.DateOfExit = PlayerPrefs.GetString("DateOfExit");
        }
    }

    void OnEnable()
    {
        startTimer = false;
    }

    void OnApplicationQuit()  //1.4  
    {
        InitScript.DateOfExit = DateTime.Now.ToString();
        //print(InitScript.DateOfExit);
    }

	public void SetupInfiniteLife (bool _forceUpdate = true)
	{
		isInfiniteLife = InitScript.lifes < 0 ? true : false;
		if (!isInfiniteLife)
		{
			TotalTimeForRestLife = InitScript.Instance.TotalTimeForRestLifeHours * 60 * 60 + InitScript.Instance.TotalTimeForRestLifeMin * 60 + InitScript.Instance.TotalTimeForRestLifeSec;
		}

		readyToUpdate = true;
	}

	public void SetupInfiniteLifeWithTime (int _duration)
	{
		if (InitScript.lifes >= 0 && InitScript.lifes <= InitScript.Instance.CapOfLife)
		{
			InitScript.RestLifeTimer = 0.0f;
		}

		InitScript.lifes = -1;
		isInfiniteLife = InitScript.lifes < 0 ? true : false;

		InitScript.RestLifeTimer += _duration;
		InitScript.DateOfExit = DateTime.Now.ToString();

		ZPlayerPrefs.SetFloat("RestLifeTimer", InitScript.RestLifeTimer);
		TotalTimeForRestLife = _duration;

		readyToUpdate = true;
	}
}
