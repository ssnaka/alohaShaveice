using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

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
        if (DateTime.Now.Subtract(dateOfExit).TotalSeconds > TotalTimeForRestLife * (InitScript.Instance.CapOfLife - InitScript.lifes))
        {
            //Debug.Log(dateOfExit + " " + InitScript.today);
            InitScript.Instance.RestoreLifes();
            InitScript.RestLifeTimer = 0;
            return false;    ///we dont need lifes
		}
        else
        {
            TimeCount((float)DateTime.Now.Subtract(dateOfExit).TotalSeconds);
            // Debug.Log(InitScript.today.Subtract(dateOfExit).TotalSeconds / 60 / 15 + " " + dateOfExit);
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
				InitScript.Instance.AddLife(InitScript.Instance.CapOfLife * 2);
				ResetTimer();
				isInfiniteLife = false;
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
				text.text = "  Full";
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
		if (_forceUpdate)
		{
			int randomDuration = UnityEngine.Random.Range(infiniteLifeRnadomStart, infiniteLifeRnadomEnd);
			int mod = randomDuration % infiniteLifeCut;
			randomDuration += (infiniteLifeCut - mod);

			SetupInfiniteLifeWithTime(randomDuration);
		}

		isInfiniteLife = InitScript.lifes < 0 ? true : false;

		if (!isInfiniteLife)
		{
			TotalTimeForRestLife = InitScript.Instance.TotalTimeForRestLifeHours * 60 * 60 + InitScript.Instance.TotalTimeForRestLifeMin * 60 + InitScript.Instance.TotalTimeForRestLifeSec;
		}

		readyToUpdate = true;
	}

	public void SetupInfiniteLifeWithTime (int _duration)
	{
		InitScript.lifes = -1;

		InitScript.RestLifeTimer += _duration;
		InitScript.DateOfExit = DateTime.Now.ToString();

		ZPlayerPrefs.SetFloat("RestLifeTimer", InitScript.RestLifeTimer);
		TotalTimeForRestLife = _duration;
	}
}
