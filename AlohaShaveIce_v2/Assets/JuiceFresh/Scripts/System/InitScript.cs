using UnityEngine;
using System.Collections;
using System;

using System.Collections.Generic;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

#if CHARTBOOST_ADS
using ChartboostSDK;
#endif
#if  GOOGLE_MOBILE_ADS
using GoogleMobileAds.Api;
#endif
#if APPODEAL_ADS
using AppodealAds.Unity.Api;
using AppodealAds.Unity.Common;
#endif

using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum Target
{
	SCORE,
	COLLECT,
	ITEMS,
	BLOCKS,
	CAGES,
	BOMBS
}

public enum LIMIT
{
	MOVES,
	TIME
}

public enum Ingredients
{
	None = 0,
	Ingredient1,
	Ingredient2,
	Ingredient3,
	Ingredient4

}

public enum CollectItems
{
	None = 0,
	Item1,
	Item2,
	Item3,
	Item4,
	Item5,
	Item6
}

public enum CollectStars
{
	STAR_1 = 1,
	STARS_2 = 2,
	STARS_3 = 3
}


public enum RewardedAdsType
{
	GetLifes,
	GetGems,
	GetGoOn,
	Counter,
	ExtraMoves,
	Stripes,
	ExtraTime,
	Bomb,
	Colorful_bomb,
	Shovel,
	Energy,
	All
}

public class InitScript : MonoBehaviour, INonSkippableVideoAdListener, IBannerAdListener, IRewardedVideoAdListener
{
	public static InitScript Instance;
	public static int openLevel;


	public static float RestLifeTimer;
	public static string DateOfExit;
	public static DateTime today;
	public static DateTime DateOfRestLife;
	public static string timeForReps;
	private static int Lifes;

	public List<CollectedIngredients> collectedIngredients = new List<CollectedIngredients>();

	public RewardedAdsType currentReward;

	public static int lifes
	{
		get
		{
			return InitScript.Lifes;
		}
		set
		{
			InitScript.Lifes = value;
		}
	}

	public int CapOfLife = 5;
	public float TotalTimeForRestLifeHours = 0;
	public float TotalTimeForRestLifeMin = 15;
	public float TotalTimeForRestLifeSec = 60;
	public int FirstGems = 20;
	public static int Gems;
	public static int waitedPurchaseGems;
	private int BoostExtraMoves;
	private int BoostPackages;
	private int BoostStripes;
	private int BoostExtraTime;
	private int BoostBomb;
	private int BoostColorful_bomb;
	private int BoostHand;
	private int BoostRandom_color;
	public List<AdEvents> adsEvents = new List<AdEvents>();
	public List<BoostAdEvents> boostAdsEvents = new List<BoostAdEvents>();

	public static bool sound = false;
	public static bool music = false;
	private bool adsReady;
	public string unityAdsIDAndroid;
	public string unityAdsIDIOS;
	public bool enableUnityAds;
	public bool enableGoogleMobileAds;
	public bool enableChartboostAds;
	public string rewardedVideoZone;
	public string nonRewardedVideoZone;
	public int ShowChartboostAdsEveryLevel;
	public int ShowAdmobAdsEveryLevel;
	private bool leftControl;
	#if  GOOGLE_MOBILE_ADS
	private InterstitialAd interstitial;
	private AdRequest requestAdmob;
	#endif
	public string admobUIDAndroid;
	public string admobUIDIOS;

	public int ShowRateEvery;
	public string RateURL;
	public string RateURLIOS;
	private GameObject rate;
	public int rewardedGems = 5;
	public bool losingLifeEveryGame;
	public static Sprite profilePic;
	public GameObject facebookButton;
	//1.3.3

	public int maxVideoPerDay;

	// Use this for initialization
	void Awake ()
	{
		ZPlayerPrefs.Initialize("TryYourBestToGuessPass", "saltIsnotGoingToBeEasy");

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		Application.targetFrameRate = 60;
		Instance = this;
		RestLifeTimer = PlayerPrefs.GetFloat("RestLifeTimer");
//		if (Application.isEditor)//TODO comment it
//			PlayerPrefs.DeleteAll ();

		DateOfExit = PlayerPrefs.GetString("DateOfExit", "");
		Gems = ZPlayerPrefs.GetInt("Gems");
		lifes = ZPlayerPrefs.GetInt("Lifes");
		if (PlayerPrefs.GetInt("Lauched") == 0)
		{    //First lauching
			lifes = CapOfLife;
			ZPlayerPrefs.SetInt("Lifes", lifes);
			Gems = FirstGems;
			ZPlayerPrefs.SetInt("Gems", Gems);
			PlayerPrefs.SetInt("Music", 1);
			PlayerPrefs.SetInt("Sound", 1);

			PlayerPrefs.SetInt("Lauched", 1);
			PlayerPrefs.Save();
		}
		rate = GameObject.Find("CanvasGlobal").transform.Find("Rate").gameObject;
		rate.SetActive(false);
		//rate.transform.SetParent(GameObject.Find("CanvasGlobal").transform);
		//rate.transform.localPosition = Vector3.zero;
		//rate.GetComponent<RectTransform>().anchoredPosition = (Resources.Load("Prefabs/Rate") as GameObject).GetComponent<RectTransform>().anchoredPosition;
		//rate.transform.localScale = Vector3.one;

		GameObject.Find("Music").GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Music");
		SoundBase.Instance.GetComponent<AudioSource>().volume = PlayerPrefs.GetInt("Sound");
		#if UNITY_ADS//1.3
		enableUnityAds = true;
		#else
		enableUnityAds = false;
		#endif
		#if CHARTBOOST_ADS//1.4.1
		enableChartboostAds = true;
		#else
		enableChartboostAds = false;
		#endif


#if FACEBOOK
		FacebookManager fbManager = gameObject.AddComponent<FacebookManager>();//1.3.3
		fbManager.facebookButton = facebookButton;//1.3.3
#endif

#if GOOGLE_MOBILE_ADS
		enableGoogleMobileAds = true;//1.3
#if UNITY_ANDROID
		interstitial = new InterstitialAd (admobUIDAndroid);
#elif UNITY_IOS
        interstitial = new InterstitialAd(admobUIDIOS);
#else
		interstitial = new InterstitialAd (admobUIDAndroid);
#endif

		// Create an empty ad request.
		requestAdmob = new AdRequest.Builder ().Build ();
		// Load the interstitial with the request.
		interstitial.LoadAd (requestAdmob);
		interstitial.OnAdLoaded += HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad += HandleInterstitialFailedToLoad;
		#else
		enableGoogleMobileAds = false; //1.3
#endif

#if APPODEAL_ADS
		string appKey = "9f79bcfc0adf30a16bfa525b336a0337893901ac1f5344a2";
		#if UNITY_IOS
		appKey = "7071382527242050da07addd574e66366b29a9da961d7f36";
		#endif

		Appodeal.disableLocationPermissionCheck();
		Appodeal.setTesting(false);
		Appodeal.setBannerBackground(true);

		Appodeal.initialize(appKey, Appodeal.INTERSTITIAL | Appodeal.BANNER | Appodeal.NON_SKIPPABLE_VIDEO);
		Appodeal.setNonSkippableVideoCallbacks(this);

//		Appodeal.setRewardedVideoCallbacks(this);
		Appodeal.setBannerCallbacks(this);
#endif

		Transform canvas = GameObject.Find("CanvasGlobal").transform;
		foreach (Transform item in canvas)
		{
			item.gameObject.SetActive(false);
		}
	}
	#if GOOGLE_MOBILE_ADS
	
	public void HandleInterstitialLoaded (object sender, EventArgs args) {
		print ("HandleInterstitialLoaded event received.");
	}

	public void HandleInterstitialFailedToLoad (object sender, AdFailedToLoadEventArgs args) {
		print ("HandleInterstitialFailedToLoad event received with message: " + args.Message);
	}
	#endif
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.LeftControl))
			leftControl = true;
		if (Input.GetKeyUp(KeyCode.LeftControl))
			leftControl = false;

		if (Input.GetKeyUp(KeyCode.U))
		{
			for (int i = 1; i < GameObject.Find("Levels").transform.childCount; i++)
			{
				SaveLevelStarsCount(i, 1);
			}

		}
	}

	public void SaveLevelStarsCount (int level, int starsCount)
	{
		Debug.Log(string.Format("Stars count {0} of level {1} saved.", starsCount, level));
		PlayerPrefs.SetInt(GetLevelKey(level), starsCount);

	}

	private string GetLevelKey (int number)
	{
		return string.Format("Level.{0:000}.StarsCount", number);
	}

	public bool GetRewardedUnityAdsReady ()
	{
#if UNITY_ADS

		rewardedVideoZone = "rewardedVideo";
		if (Advertisement.IsReady (rewardedVideoZone)) {
			return true;
		} else {
			rewardedVideoZone = "rewardedVideoZone";
			if (Advertisement.IsReady (rewardedVideoZone)) {
				return true;
			}
		}
#endif

		return false;
	}

	public void ShowRewardedAds ()
	{
#if UNITY_ADS
		Debug.Log ("show Unity Rewarded ads video in " + LevelManager.THIS.gameStatus);

		if (GetRewardedUnityAdsReady ()) {
			Advertisement.Show (rewardedVideoZone, new ShowOptions {
				resultCallback = result => {
					if (result == ShowResult.Finished) {
						CheckRewardedAds ();
					}
				}
			});
		}
#endif
		#if APPODEAL_ADS
		if (Appodeal.isLoaded(Appodeal.NON_SKIPPABLE_VIDEO))
		{
			Appodeal.show(Appodeal.NON_SKIPPABLE_VIDEO);
		}
		#endif
	}

	public void CheckAdsEvents (GameState state)
	{

		foreach (AdEvents item in adsEvents)
		{
			if (item.gameEvent == state)
			{
				if ((LevelManager.THIS.gameStatus == GameState.GameOver || LevelManager.THIS.gameStatus == GameState.Pause ||
				    LevelManager.THIS.gameStatus == GameState.Playing || LevelManager.THIS.gameStatus == GameState.PrepareGame || LevelManager.THIS.gameStatus == GameState.PreWinAnimations ||
				    LevelManager.THIS.gameStatus == GameState.RegenLevel || LevelManager.THIS.gameStatus == GameState.Win))
				{
					item.calls++;
					if (item.calls % item.everyLevel == 0)
						ShowAdByType(item.adType);
				}
				else
				{
					ShowAdByType(item.adType);

				}
			}
		}
	}

	public BoostAdEvents GetBoostAdsEvent (BoostType _boostType)
	{

		return boostAdsEvents.Find(item => item.boostType.Equals(_boostType));
	}

	void ShowAdByType (AdType adType)
	{
		if (adType == AdType.AdmobInterstitial)
			ShowAds(false);
		else if (adType == AdType.UnityAdsVideo)
			ShowVideo();
		else if (adType == AdType.ChartboostInterstitial)
			ShowAds(true);
		else if (adType == AdType.AppODeal)
			ShowRewardedAds();
	}

	public void ShowVideo ()
	{
		Debug.Log("show Unity ads video on " + LevelManager.THIS.gameStatus);
#if UNITY_ADS

		if (Advertisement.IsReady ("video")) {
			Advertisement.Show ("video");
		} else {
			if (Advertisement.IsReady ("defaultZone")) {
				Advertisement.Show ("defaultZone");
			}
		}
#endif
	}

	public void ShowAds (bool chartboost = true)
	{
		if (chartboost)
		{
			Debug.Log("show Chartboost Interstitial on " + LevelManager.THIS.gameStatus);
#if CHARTBOOST_ADS
			Chartboost.showInterstitial (CBLocation.Default);
			Chartboost.cacheInterstitial (CBLocation.Default);
#endif
		}
		else
		{
			Debug.Log("show Google mobile ads Interstitial on " + LevelManager.THIS.gameStatus);
#if GOOGLE_MOBILE_ADS
			if (interstitial.IsLoaded ()) {
				interstitial.Show ();
#if UNITY_ANDROID
				interstitial = new InterstitialAd (admobUIDAndroid);
#elif UNITY_IOS
                interstitial = new InterstitialAd(admobUIDIOS);
#else
				interstitial = new InterstitialAd (admobUIDAndroid);
#endif

				// Create an empty ad request.
				requestAdmob = new AdRequest.Builder ().Build ();
				// Load the interstitial with the request.
				interstitial.LoadAd (requestAdmob);
			}
#endif
		}
	}

	#if APPODEAL_ADS
	#region Rewarded Video callback handlers

			public void onRewardedVideoLoaded()
			{
			Debug.LogError("--Video Loaded");
			}

			public void onRewardedVideoFailedToLoad()
			{
			Debug.LogError("--Video failed");
			}

			public void onRewardedVideoShown()
			{
			Debug.LogError("--Video shown");
			}

			public void onRewardedVideoFinished(int amount, string name)
			{
			Debug.LogError("--Video Finished");
			}

			public void onRewardedVideoClosed(bool finished)
			{
			Debug.LogError("--Video closed");
			}

	public void onNonSkippableVideoLoaded ()
	{
		Debug.LogError("Video Loaded");
	}

	public void onNonSkippableVideoFailedToLoad ()
	{
		Debug.LogError("Video failed");
	}

	public void onNonSkippableVideoShown ()
	{
		Debug.LogError("Video shown");
	}

	public void onNonSkippableVideoClosed (bool _closed)
	{
		Debug.LogError("Video closed");
	}

	public void onNonSkippableVideoFinished ()
	{
		Debug.LogError("Video Finished");
		CheckRewardedAds();
	}

	#endregion

	#region Banner callback handlers

	public void onBannerLoaded (bool _loaded)
	{
		Debug.LogError("banner loaded");
	}

	public void onBannerFailedToLoad ()
	{
		Debug.LogError("banner failed");
	}

	public void onBannerShown ()
	{
		Debug.LogError("banner opened");
	}

	public void onBannerClicked ()
	{
		Debug.LogError("banner clicked");
	}

	#endregion

	public void EnableBannerAds (bool _enabled)
	{
		if (_enabled)
		{
//			Debug.LogError(Appodeal.isLoaded(Appodeal.BANNER));
//			if (Appodeal.isLoaded(Appodeal.BANNER))
//			{
				Appodeal.show(Appodeal.BANNER_BOTTOM);
//			}
		}
		else
		{
			Appodeal.hide(Appodeal.BANNER);
		}
	}

	#endif

	public void ShowRate ()
	{
		rate.SetActive(true);
	}

	public bool CanVideoBePlayed ()
	{
		bool canPlay = true;
		int videoPlayedCount = PlayerPrefs.GetInt("VideoPlayedToday", 0);
		string dateString = PlayerPrefs.GetString("NextVideoResetTime", "");
		if (!string.IsNullOrEmpty(dateString))
		{
			DateTime nextVideoResetTime = DateTime.Parse(dateString);

			if (nextVideoResetTime.CompareTo(DateTime.Now) >= 0)
			{
				if (videoPlayedCount >= maxVideoPerDay)
				{
					canPlay = false;
				}
			}
			else
			{
				PlayerPrefs.SetString("NextVideoResetTime", "");
				PlayerPrefs.SetInt("VideoPlayedToday", 0);

				PlayerPrefs.SetInt(RewardedAdsType.Stripes.ToString() + "_watch", 0);
				PlayerPrefs.SetInt(RewardedAdsType.Stripes.ToString() + "_round", 1);

				PlayerPrefs.SetInt(RewardedAdsType.Colorful_bomb.ToString() + "_watch", 0);
				PlayerPrefs.SetInt(RewardedAdsType.Colorful_bomb.ToString() + "_round", 1);

				PlayerPrefs.SetInt(RewardedAdsType.ExtraMoves.ToString() + "_watch", 0);
				PlayerPrefs.SetInt(RewardedAdsType.ExtraMoves.ToString() + "_round", 1);

				PlayerPrefs.SetInt(RewardedAdsType.ExtraTime.ToString() + "_watch", 0);
				PlayerPrefs.SetInt(RewardedAdsType.ExtraTime.ToString() + "_round", 1);

				PlayerPrefs.SetInt(RewardedAdsType.Bomb.ToString() + "_watch", 0);
				PlayerPrefs.SetInt(RewardedAdsType.Bomb.ToString() + "_round", 1);

				PlayerPrefs.SetInt(RewardedAdsType.Energy.ToString() + "_watch", 0);
				PlayerPrefs.SetInt(RewardedAdsType.Energy.ToString() + "_round", 1);

				PlayerPrefs.SetInt(RewardedAdsType.Shovel.ToString() + "_watch", 0);
				PlayerPrefs.SetInt(RewardedAdsType.Shovel.ToString() + "_round", 1);

				PlayerPrefs.Save();
			}
		}

		return canPlay;
	}


	public void CheckRewardedAds ()
	{
		string dateString = PlayerPrefs.GetString("NextVideoResetTime", "");
		if (string.IsNullOrEmpty(dateString))
		{
			PlayerPrefs.SetString("NextVideoResetTime", DateTime.Now.AddMinutes(-5).AddDays(1).ToString());
			PlayerPrefs.Save();
		}

		int videoPlayed = PlayerPrefs.GetInt("VideoPlayedToday", 0);
		PlayerPrefs.SetInt("VideoPlayedToday", ++videoPlayed);

		RewardIcon reward = GameObject.Find("CanvasGlobal").transform.Find("Reward").GetComponent<RewardIcon>();

		if (currentReward == RewardedAdsType.GetGems)
		{
			reward.SetIconSprite(0);
			reward.gameObject.SetActive(true);
			AddGems(rewardedGems);
			GameObject.Find("CanvasGlobal").transform.Find("GemsShop").GetComponent<AnimationManager>().CloseMenu();
		}
		else if (currentReward == RewardedAdsType.GetLifes)
		{
			reward.SetIconSprite(1);
			reward.gameObject.SetActive(true);
			RestoreLifes();
			GameObject.Find("CanvasGlobal").transform.Find("LiveShop").GetComponent<AnimationManager>().CloseMenu();
		}
		else if (currentReward == RewardedAdsType.GetGoOn)
		{
			GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").GetComponent<AnimationManager>().GoOnFailed();
		}
		else if (currentReward == RewardedAdsType.Counter)
		{
			
		}
		else if (currentReward == RewardedAdsType.Stripes)
		{
			BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(BoostType.Stripes);
			if (boostAdEvent != null)
			{
				int count = PlayerPrefs.GetInt(RewardedAdsType.Stripes.ToString() + "_watch", 0) + 1;
				int round = PlayerPrefs.GetInt(RewardedAdsType.Stripes.ToString() + "_round", 1);
				if (count >= boostAdEvent.countToReward)
				{
					count = round * -1;
					PlayerPrefs.SetInt(RewardedAdsType.Stripes.ToString() + "_round", round + 1);
					int boostCount = ZPlayerPrefs.GetInt("" + BoostType.Stripes);
					BuyBoost(BoostType.Stripes, 0, boostCount + 1);
				}

				PlayerPrefs.SetInt(RewardedAdsType.Stripes.ToString() + "_watch", count);
			}
		}
		else if (currentReward == RewardedAdsType.Colorful_bomb)
		{
			BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(BoostType.Colorful_bomb);
			if (boostAdEvent != null)
			{
				int count = PlayerPrefs.GetInt(RewardedAdsType.Colorful_bomb.ToString() + "_watch", 0) + 1;
				int round = PlayerPrefs.GetInt(RewardedAdsType.Colorful_bomb.ToString() + "_round", 1);
				if (count >= boostAdEvent.countToReward)
				{
					count = round * -1;
					PlayerPrefs.SetInt(RewardedAdsType.Colorful_bomb.ToString() + "_round", round + 1);
					int boostCount = ZPlayerPrefs.GetInt("" + BoostType.Colorful_bomb);
					BuyBoost(BoostType.Colorful_bomb, 0, boostCount + 1);
				}

				PlayerPrefs.SetInt(RewardedAdsType.Colorful_bomb.ToString() + "_watch", count);
			}
		}
		else if (currentReward == RewardedAdsType.ExtraMoves)
		{
			BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(BoostType.ExtraMoves);
			if (boostAdEvent != null)
			{
				int count = PlayerPrefs.GetInt(RewardedAdsType.ExtraMoves.ToString() + "_watch", 0) + 1;
				int round = PlayerPrefs.GetInt(RewardedAdsType.ExtraMoves.ToString() + "_round", 1);
				if (count >= boostAdEvent.countToReward)
				{
					count = round * -1;
					PlayerPrefs.SetInt(RewardedAdsType.ExtraMoves.ToString() + "_round", round + 1);
					int boostCount = ZPlayerPrefs.GetInt("" + BoostType.ExtraMoves);
					BuyBoost(BoostType.ExtraMoves, 0, boostCount + 1);
				}

				PlayerPrefs.SetInt(RewardedAdsType.ExtraMoves.ToString() + "_watch", count);
			}
		}
		else if (currentReward == RewardedAdsType.ExtraTime)
		{
			BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(BoostType.ExtraTime);
			if (boostAdEvent != null)
			{
				int count = PlayerPrefs.GetInt(RewardedAdsType.ExtraTime.ToString() + "_watch", 0) + 1;
				int round = PlayerPrefs.GetInt(RewardedAdsType.ExtraTime.ToString() + "_round", 1);
				if (count >= boostAdEvent.countToReward)
				{
					count = round * -1;
					PlayerPrefs.SetInt(RewardedAdsType.ExtraTime.ToString() + "_round", round + 1);
					int boostCount = ZPlayerPrefs.GetInt("" + BoostType.ExtraTime);
					BuyBoost(BoostType.ExtraTime, 0, boostCount + 1);
				}
				
				PlayerPrefs.SetInt(RewardedAdsType.ExtraTime.ToString() + "_watch", count);
			}
		}
		else if (currentReward == RewardedAdsType.Bomb)
		{
			BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(BoostType.Bomb);
			if (boostAdEvent != null)
			{
				int count = PlayerPrefs.GetInt(RewardedAdsType.Bomb.ToString() + "_watch", 0) + 1;
				int round = PlayerPrefs.GetInt(RewardedAdsType.Bomb.ToString() + "_round", 1);
				if (count >= boostAdEvent.countToReward)
				{
					count = round * -1;
					PlayerPrefs.SetInt(RewardedAdsType.Bomb.ToString() + "_round", round + 1);
					int boostCount = ZPlayerPrefs.GetInt("" + BoostType.Bomb);
					BuyBoost(BoostType.Bomb, 0, boostCount + 1);
				}

				PlayerPrefs.SetInt(RewardedAdsType.Bomb.ToString() + "_watch", count);
			}
		}
		else if (currentReward == RewardedAdsType.Energy)
		{
			BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(BoostType.Energy);
			if (boostAdEvent != null)
			{
				int count = PlayerPrefs.GetInt(RewardedAdsType.Energy.ToString() + "_watch", 0) + 1;
				int round = PlayerPrefs.GetInt(RewardedAdsType.Energy.ToString() + "_round", 1);
				if (count >= boostAdEvent.countToReward)
				{
					count = round * -1;
					PlayerPrefs.SetInt(RewardedAdsType.Energy.ToString() + "_round", round + 1);
					int boostCount = ZPlayerPrefs.GetInt("" + BoostType.Energy);
					BuyBoost(BoostType.Energy, 0, boostCount + 1);
				}

				PlayerPrefs.SetInt(RewardedAdsType.Energy.ToString() + "_watch", count);
			}
		}
		else if (currentReward == RewardedAdsType.Shovel)
		{
			BoostAdEvents boostAdEvent = InitScript.Instance.GetBoostAdsEvent(BoostType.Shovel);
			if (boostAdEvent != null)
			{
				int count = PlayerPrefs.GetInt(RewardedAdsType.Shovel.ToString() + "_watch", 0) + 1;
				int round = PlayerPrefs.GetInt(RewardedAdsType.Shovel.ToString() + "_round", 1);
				if (count >= boostAdEvent.countToReward)
				{
					count = round * -1;
					PlayerPrefs.SetInt(RewardedAdsType.Shovel.ToString() + "_round", round + 1);
					int boostCount = ZPlayerPrefs.GetInt("" + BoostType.Shovel);
					BuyBoost(BoostType.Shovel, 0, boostCount + 1);
				}

				PlayerPrefs.SetInt(RewardedAdsType.Shovel.ToString() + "_watch", count);
			}
		}

		PlayerPrefs.Save();
	}

	public void SetGems (int count)
	{//1.3.3
		Gems = count;
		ZPlayerPrefs.SetInt("Gems", Gems);
		ZPlayerPrefs.Save();
	}

	public void AddGems (int count)
	{
		Gems += count;
		ZPlayerPrefs.SetInt("Gems", Gems);
		ZPlayerPrefs.Save();
		#if PLAYFAB || GAMESPARKS
		NetworkManager.currencyManager.IncBalance(count);
		#endif
	}

	public void SpendGems (int count)
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.cash);
		Gems -= count;
		ZPlayerPrefs.SetInt("Gems", Gems);
		ZPlayerPrefs.Save();
		#if PLAYFAB || GAMESPARKS
		NetworkManager.currencyManager.DecBalance(count);
		#endif
	}


	public void RestoreLifes ()
	{
		lifes = CapOfLife;
		ZPlayerPrefs.SetInt("Lifes", lifes);
		ZPlayerPrefs.Save();
	}

	public void AddLife (int count)
	{
		lifes += count;
		if (lifes > CapOfLife)
			lifes = CapOfLife;
		ZPlayerPrefs.SetInt("Lifes", lifes);
		ZPlayerPrefs.Save();
	}

	public int GetLife ()
	{
		if (lifes > CapOfLife)
		{
			lifes = CapOfLife;
			ZPlayerPrefs.SetInt("Lifes", lifes);
			ZPlayerPrefs.Save();
		}
		return lifes;
	}

	public void PurchaseSucceded ()
	{
		AddGems(waitedPurchaseGems);
		waitedPurchaseGems = 0;
	}

	public void SpendLife (int count)
	{
		if (lifes > 0)
		{
			lifes -= count;
			ZPlayerPrefs.SetInt("Lifes", lifes);
			ZPlayerPrefs.Save();
		}
		//else
		//{
		//    GameObject.Find("Canvas").transform.Find("RestoreLifes").gameObject.SetActive(true);
		//}
	}

	public void BuyBoost (BoostType boostType, int price, int count)
	{
		ZPlayerPrefs.SetInt("" + boostType, count);
		ZPlayerPrefs.Save();
		Messenger.Broadcast<BoostType, int>("BoostValueChanged", boostType, count);
		#if PLAYFAB ||GAMESPARKS
		NetworkManager.dataManager.SetBoosterData();
		#endif

		//   ReloadBoosts();
	}

	public void SpendBoost (BoostType boostType)
	{
		int count = ZPlayerPrefs.GetInt("" + boostType) - 1;
		ZPlayerPrefs.SetInt("" + boostType, count);
		ZPlayerPrefs.Save();
		Messenger.Broadcast<BoostType, int>("BoostValueChanged", boostType, count);

		#if PLAYFAB || GAMESPARKS
		NetworkManager.dataManager.SetBoosterData();
		#endif

	}
	//void ReloadBoosts()
	//{
	//    BoostExtraMoves = PlayerPrefs.GetInt("" + BoostType.ExtraMoves);
	//    BoostPackages = PlayerPrefs.GetInt("" + BoostType.Packages);
	//    BoostStripes = PlayerPrefs.GetInt("" + BoostType.Stripes);
	//    BoostExtraTime = PlayerPrefs.GetInt("" + BoostType.ExtraTime);
	//    BoostBomb = PlayerPrefs.GetInt("" + BoostType.Bomb);
	//    BoostColorful_bomb = PlayerPrefs.GetInt("" + BoostType.Colorful_bomb);
	//    BoostHand = PlayerPrefs.GetInt("" + BoostType.Hand);
	//    BoostRandom_color = PlayerPrefs.GetInt("" + BoostType.Random_color);

	//}
	//public void onMarketPurchase(PurchasableVirtualItem pvi, string payload, Dictionary<string, string> extra)
	//{
	//    PurchaseSucceded();
	//}

	void OnApplicationFocus (bool focusStatus)
	{//1.3.3
		if(focusStatus) {
			if (MusicBase.Instance)
			{
				MusicBase.Instance.GetComponent<AudioSource>().Play();
			}
			#if APPODEAL_ADS
			Appodeal.onResume();
			#endif
		}
	}


	void OnApplicationPause (bool pauseStatus)
	{
		if (pauseStatus)
		{
			if (RestLifeTimer > 0)
			{
				PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
			}
			ZPlayerPrefs.SetInt("Lifes", lifes);
			PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
			PlayerPrefs.Save();
		}
	}

	void OnApplicationQuit ()
	{   //1.4  added 
		if (RestLifeTimer > 0)
		{
			PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
		}
		ZPlayerPrefs.SetInt("Lifes", lifes);
		PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
		PlayerPrefs.Save();
	}

	public void OnLevelClicked (object sender, LevelReachedEventArgs args)
	{
		if (EventSystem.current.IsPointerOverGameObject(-1))
			return;
		if (!GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.activeSelf && !GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.activeSelf)
		{
			PlayerPrefs.SetInt("OpenLevel", args.Number);
			PlayerPrefs.Save();
			LevelManager.THIS.MenuPlayEvent();
			LevelManager.THIS.LoadLevel();
			openLevel = args.Number;
			//  currentTarget = targets[args.Number];
			GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
		}
	}

	void OnEnable ()
	{
		LevelsMap.LevelSelected += OnLevelClicked;
	}

	void OnDisable ()
	{
		LevelsMap.LevelSelected -= OnLevelClicked;

		//		if(RestLifeTimer>0){
		PlayerPrefs.SetFloat("RestLifeTimer", RestLifeTimer);
		//		}
		ZPlayerPrefs.SetInt("Lifes", lifes);
		PlayerPrefs.SetString("DateOfExit", DateTime.Now.ToString());
		PlayerPrefs.Save();
#if GOOGLE_MOBILE_ADS
		interstitial.OnAdLoaded -= HandleInterstitialLoaded;
		interstitial.OnAdFailedToLoad -= HandleInterstitialFailedToLoad;
#endif

	}


}
