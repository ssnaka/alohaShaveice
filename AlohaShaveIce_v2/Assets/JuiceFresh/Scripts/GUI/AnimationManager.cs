using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;
using System.Collections.Generic;

#if UNITY_ADS
using UnityEngine.Advertisements;
#endif

public class LevelInfo 
{
	public Target target;
	public LIMIT limitType;
}


public class AnimationManager : MonoBehaviour
{
	public bool PlayOnEnable = true;
	bool WaitForPickupFriends;
	public GameObject fireworkPrefab;

	bool WaitForAksFriends;
	System.Collections.Generic.Dictionary<string, string> parameters;

	Button videoButton;
	void OnEnable()
	{
		if (PlayOnEnable) {
			SoundBase.Instance.PlaySound(SoundBase.Instance.swish[0]);

			//if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
			//    GetComponent<SequencePlayer>().Play();
		}
		if (name == "MenuPlay") {
//			LoadLevel(PlayerPrefs.GetInt("OpenLevel"));
			LevelInfo levelInfo = LevelManager.THIS.LoadLevel();
			LevelManager.THIS.CreateCollectableTarget(transform.Find("Image/TargetIngr/TargetIngr").gameObject, target);

			Transform timerImage = transform.Find("Image/TimerImage");
			if (timerImage != null)
			{
				if (levelInfo.limitType.Equals(LIMIT.TIME))
				{
					timerImage.gameObject.SetActive(true);
				}
				else
				{
					timerImage.gameObject.SetActive(false);
				}
			}

			// for (int i = 1; i <= 3; i++)
			// {
			//     transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
			// }
			// int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", PlayerPrefs.GetInt("OpenLevel")), 0);
			// if (stars > 0)
			// {
			//     for (int i = 1; i <= stars; i++)
			//     {
			//         transform.Find("Image").Find("Star" + i).gameObject.SetActive(true);
			//     }

			// }
			// else
			// {
			//     for (int i = 1; i <= 3; i++)
			//     {
			//         transform.Find("Image").Find("Star" + i).gameObject.SetActive(false);
			//     }

			// }
		}

		if (name == "PrePlay") {
			// GameObject
		}
		if (name == "PreFailed") {
			SoundBase.Instance.PlaySound(SoundBase.Instance.gameOver[0]);
			transform.Find("Video").gameObject.SetActive(false);
			transform.Find("Buy").GetComponent<Button>().interactable = true;

			GetComponent<Animation>().Play();
		}

		if (name == "Settings" || name == "MenuPause") {
			if (PlayerPrefs.GetInt("Sound") == 0)
				transform.Find("Image/Sound/SoundOff").gameObject.SetActive(true);
			else
				transform.Find("Image/Sound/SoundOff").gameObject.SetActive(false);

			if (PlayerPrefs.GetInt("Music") == 0)
				transform.Find("Image/Music/MusicOff").gameObject.SetActive(true);
			else
				transform.Find("Image/Music/MusicOff").gameObject.SetActive(false);
		}

		if (name == "GemsShop") {
			for (int i = 1; i <= 4; i++) {
				transform.Find("Image/Pack" + i + "/Count").GetComponent<Text>().text = "" + LevelManager.THIS.gemsProducts[i - 1].count;
				transform.Find("Image/Pack" + i + "/Buy/Price").GetComponent<Text>().text = "" + LevelManager.THIS.gemsProducts[i - 1].price;
			}
		}
		if (name == "MenuComplete") {
			for (int i = 1; i <= 3; i++) {
				transform.Find("Image/Star" + i + "/Star").gameObject.SetActive(false);
			}
		}

		if (name == "MenuFailed")
		{
			string spriteName = "Texture/see_ads_button_moves";
			if (LevelManager.THIS.limitType == LIMIT.TIME)
			{
				spriteName = "Texture/see_ads_button_sec";
			}

			Sprite sprite = Resources.Load<Sprite>(spriteName);
			transform.Find("Image/Video").GetComponent<Image>().overrideSprite = sprite;
		}

		if (transform.Find("Image/Video") != null) {

#if UNITY_ADS
			InitScript.Instance.rewardedVideoZone = "rewardedVideo";

			if (!InitScript.Instance.enableUnityAds || !InitScript.Instance.GetRewardedUnityAdsReady ())
				transform.Find ("Image/Video").gameObject.SetActive (false);
#else
#if !APPODEAL_ADS
			transform.Find("Image/Video").gameObject.SetActive(false);
#endif
#endif
		}

		if (name != "Lifes" && name != "Gems" && name != "Settings")
		{
			if (name != "gratzWord1" && name != "gratzWord2" && name != "gratzWord3" && name != "PrePlay")
			{
				#if APPODEAL_ADS
				InitScript.Instance.EnableBannerAds(true);
				#endif
			}


			Button[] allButtonsArray = transform.GetComponentsInChildren<Button>();
			if (videoButton == null)
			{
				if (allButtonsArray != null && allButtonsArray.Length > 0)
				{
					List<Button> allButtons = new List<Button>(allButtonsArray);
					videoButton = allButtons.Find(item => item.name.Equals("Video"));
				}
			}

			if (videoButton != null)
			{
				GameObject watchAdsTextObj = GameObject.Find("watchAds_text");
				if (watchAdsTextObj != null)
				{
					watchAds_text = watchAdsTextObj.GetComponent<Text>();
				}
				
				if (!InitScript.Instance.CanVideoBePlayed())
				{
					string dateString = PlayerPrefs.GetString("NextVideoResetTime", "");
					nextVideoResetTime = DateTime.Parse(dateString);

					if (!string.IsNullOrEmpty(dateString))
					{
						videoButton.enabled = false;
						shouldDisplayRemainingTimeForVideo = true;
					}
//					videoButton.gameObject.SetActive(false);
				}
				else
				{
					if (watchAds_text != null)
					{
						watchAds_text.text = "Watch Ads for Boosts";
					}
					videoButton.enabled = true;
					videoButton.gameObject.SetActive(true);
					shouldDisplayRemainingTimeForVideo = false;
				}
			}
		}
	}

	bool shouldDisplayRemainingTimeForVideo = false;
	DateTime nextVideoResetTime;
	Text watchAds_text;

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape)) {
			if (name == "MenuPlay" || name == "Settings" || name == "BoostInfo" || name == "GemsShop" || name == "LiveShop" || name == "BoostShop" || name == "Reward")
				CloseMenu();
		}

		if (shouldDisplayRemainingTimeForVideo)
		{
			if (nextVideoResetTime.CompareTo(DateTime.Now) >= 0)
			{
				TimeSpan timeLeft = nextVideoResetTime.Subtract(DateTime.Now);
				if (watchAds_text != null)
				{
					watchAds_text.text = "Video available in " +  timeLeft.Hours + " : " + timeLeft.Minutes + " : " + timeLeft.Seconds;
				}
			}
			else
			{
				if (watchAds_text != null)
				{
					watchAds_text.text = "Watch Ads for Boosts";
				}
				videoButton.enabled = true;
				videoButton.gameObject.SetActive(true);
				shouldDisplayRemainingTimeForVideo = false;
			}
		}

		if (shouldPlaySwishSound)
		{
			shouldPlaySwishSound = false;
			SoundBase.Instance.PlaySound(SoundBase.Instance.swish[1]);
		}
	}

	public void ShowAds()
	{
		if (name == "GemsShop")
			InitScript.Instance.currentReward = RewardedAdsType.GetGems;
		else if (name == "LiveShop")
			InitScript.Instance.currentReward = RewardedAdsType.GetLifes;
		else if (name == "MenuFailed")
			InitScript.Instance.currentReward = RewardedAdsType.GetGoOn;
		else if (name == "BoostShop")
		{	
			BoostShop shop = GetComponent<BoostShop>();
			switch(shop.boostType)
			{
			case BoostType.Stripes:
				InitScript.Instance.currentReward = RewardedAdsType.Stripes;
				break;
			case BoostType.Colorful_bomb:
				InitScript.Instance.currentReward = RewardedAdsType.Colorful_bomb;
				break;
			case BoostType.ExtraMoves:
				InitScript.Instance.currentReward = RewardedAdsType.ExtraMoves;
				break;
			case BoostType.ExtraTime:
				InitScript.Instance.currentReward = RewardedAdsType.ExtraTime;
				break;
			case BoostType.Bomb:
				InitScript.Instance.currentReward = RewardedAdsType.Bomb;
				break;
			case BoostType.Energy:
				InitScript.Instance.currentReward = RewardedAdsType.Energy;
				break;
			case BoostType.Shovel:
				InitScript.Instance.currentReward = RewardedAdsType.Shovel;
				break;
			}
		}
		InitScript.Instance.ShowRewardedAds();
		if (name != "MenuFailed")
			CloseMenu();
	}

	public void GoRate()
	{
#if UNITY_ANDROID
		Application.OpenURL(InitScript.Instance.RateURL);
#elif UNITY_IOS
//		Application.OpenURL (InitScript.Instance.RateURLIOS);
		NativeReviewRequest.RequestReview();
#endif
		PlayerPrefs.SetInt("Rated", 1);
		PlayerPrefs.Save();
	}

	void OnDisable()
	{
		if (transform.Find("Image/Video") != null) {
			if (!name.Equals("LiveShop") && !name.Equals("GemsShop"))
			{
				transform.Find("Image/Video").gameObject.SetActive(true);
			}
		}

		//if( PlayOnEnable )
		//{
		//    if( !GetComponent<SequencePlayer>().sequenceArray[0].isPlaying )
		//        GetComponent<SequencePlayer>().sequenceArray[0].Play
		//}
		#if APPODEAL_ADS
		InitScript.Instance.EnableBannerAds(false);
		#endif
	}

	public void OnFinished()
	{
		if (name == "MenuComplete") {
			StartCoroutine(MenuComplete());
			StartCoroutine(MenuCompleteScoring());
		}
		if (name == "Reward") {
			StartCoroutine(FireworkParticles());
		}
		if (name == "MenuPlay") {
			//            InitScript.Instance.currentTarget = InitScript.Instance.targets[PlayerPrefs.GetInt( "OpenLevel" )];
			transform.Find("Image/Boost1").GetComponent<BoostIcon>().InitBoost();
			transform.Find("Image/Boost2").GetComponent<BoostIcon>().InitBoost();
			// transform.Find("Image/Boost3").GetComponent<BoostIcon>().InitBoost();

		}
		if (name == "MenuPause") {
			if (LevelManager.THIS.gameStatus == GameState.Playing)
				LevelManager.THIS.gameStatus = GameState.Pause;
		}

		if (name == "MenuFailed") {
			if (LevelManager.Score < LevelManager.THIS.star1) {
				TargetCheck(false, 2);
			} else {
				TargetCheck(true, 2);
			}
			if (LevelManager.THIS.target == Target.BLOCKS) {
				if (LevelManager.THIS.TargetBlocks > 0) {
					TargetCheck(false, 1);
				} else {
					TargetCheck(true, 1);
				}
			} else if (LevelManager.THIS.target == Target.COLLECT) {
				if (LevelManager.THIS.ingrTarget[0].count > 0 || LevelManager.THIS.ingrTarget[1].count > 0) {
					TargetCheck(false, 1);
				} else {
					TargetCheck(true, 1);
				}
			} else if (LevelManager.THIS.target == Target.SCORE) {
				if (LevelManager.Score < LevelManager.THIS.GetScoresOfTargetStars()) {
					TargetCheck(false, 1);
				} else {
					TargetCheck(true, 1);
				}
			}


		}
		if (name == "PrePlay") {
			CloseMenu();
			LevelManager.THIS.gameStatus = GameState.WaitForPopup;

		}
		if (name == "MenuFailed") {
			if (LevelManager.THIS.Limit <= 0) {
				if (LevelManager.THIS.gameStatus != GameState.GameOver)//1.3.3
					LevelManager.THIS.gameStatus = GameState.GameOver;
			}
			//transform.Find("Image/Video").gameObject.SetActive(false);

			//    CloseMenu();

		}

		if (name.Contains("gratzWord"))
			gameObject.SetActive(false);
		if (name == "NoMoreMatches")
			gameObject.SetActive(false);
		if (name == "CompleteLabel")
			gameObject.SetActive(false);

	}

	void TargetCheck(bool check, int n = 1)
	{
		//Transform TargetCheck = transform.Find("Image/TargetCheck" + n);
		//Transform TargetUnCheck = transform.Find("Image/TargetUnCheck" + n);
		//TargetCheck.gameObject.SetActive(check);
		//TargetUnCheck.gameObject.SetActive(!check);
	}

	public void WaitForGiveUp()
	{
		if (name == "MenuFailed") {
			GetComponent<Animation>()["bannerFailed"].speed = 0;
#if UNITY_ADS

			if (InitScript.Instance.enableUnityAds) {

				if (InitScript.Instance.GetRewardedUnityAdsReady ()) {
					transform.Find ("Image/Video").gameObject.SetActive (true);
				}
			}
#endif
		}
	}

	IEnumerator FireworkParticles()
	{
		GameObject firework = Instantiate(fireworkPrefab, UnityEngine.Random.insideUnitCircle * 10, Quaternion.identity) as GameObject;
		GameObject firework1 = Instantiate(fireworkPrefab, UnityEngine.Random.insideUnitCircle * 10, Quaternion.identity) as GameObject;
		for (int i = 0; i < 10; i++) {
			yield return new WaitForSeconds(0.5f);
			firework.transform.position = UnityEngine.Random.insideUnitCircle * 10;
			firework.GetComponent<ParticleSystem>().Stop();
			firework.GetComponent<ParticleSystem>().Play();
			yield return new WaitForSeconds(0.5f);
			firework1.transform.position = UnityEngine.Random.insideUnitCircle * 10;
			firework1.GetComponent<ParticleSystem>().Stop();
			firework1.GetComponent<ParticleSystem>().Play();

		}
		Destroy(firework, 1);
		Destroy(firework1, 1);
	}

	IEnumerator MenuComplete()
	{
		for (int i = 1; i <= LevelManager.Instance.stars; i++) {
			//  SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoringStar );
			transform.Find("Image/Star" + i + "/Star").gameObject.SetActive(true);
			SoundBase.Instance.PlaySound(SoundBase.Instance.star[i - 1]);
			yield return new WaitForSeconds(0.5f);
		}
		StartCoroutine(FireworkParticles());

	}

	IEnumerator MenuCompleteScoring()
	{
		Text scores = transform.Find("Image").Find("Score").GetComponent<Text>();
		for (int i = 0; i <= LevelManager.Score; i += 500) {
			scores.text = "" + i;
			// SoundBase.Instance.audio.PlayOneShot( SoundBase.Instance.scoring );
			yield return new WaitForSeconds(0.00001f);
		}
		scores.text = "" + LevelManager.Score;
	}

	public void Info()
	{
		GameObject.Find("CanvasGlobal").transform.Find("Tutorial").gameObject.SetActive(true);
		CloseMenu();
	}



	public void PlaySoundButton()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);

	}

	public IEnumerator Close()
	{
		yield return new WaitForSeconds(0.5f);
	}

	bool shouldPlaySwishSound = false;
	public void CloseMenu()
	{
		
		if (gameObject.name == "MenuPreGameOver") {
			ShowGameOver();
		}
		if (gameObject.name == "MenuComplete") {
			LevelManager.THIS.gameStatus = GameState.Map;
			//1.4.5
			if (PlayerPrefs.GetInt("OpenLevel") + 1 <= LevelsMap._instance.GetMapLevels().Count) {
				PlayerPrefs.SetInt("OpenLevel", LevelManager.THIS.currentLevel + 1);
				GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
			} else {
				GameObject g = Instantiate(Resources.Load("Prefabs/Congratulations"), Vector2.zero, Quaternion.identity) as GameObject;
				g.transform.SetParent(GameObject.Find("CanvasGlobal").transform);
				g.transform.localScale = Vector3.one;
				g.transform.localPosition = Vector3.zero;
			}
		}
		if (gameObject.name == "MenuFailed") {
			if (!keepGaming)
				LevelManager.THIS.gameStatus = GameState.Map;
			keepGaming = false;
		}
		if (gameObject.name == "Tutorial") {
			//LevelManager.Instance.gameStatus = GameState.WaitForPopup;
		}

		if (Application.loadedLevelName == "game") {
			if (LevelManager.Instance.gameStatus == GameState.Pause) {
				LevelManager.Instance.gameStatus = GameState.WaitAfterClose;

			}
		}

		if (name == "MenuPlay") {
			Transform tr = transform.Find("Image/TargetIngr/TargetIngr");
			foreach (Transform item in tr) {
				Destroy(item.gameObject);
			}
		}

		shouldPlaySwishSound = true;
//		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
//		SoundBase.Instance.PlaySound(SoundBase.Instance.swish[1]);

		gameObject.SetActive(false);
	}

	public void SwishSound()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.swish[1]);

	}

	public void ShowInfo()
	{
		GameObject.Find("CanvasGlobal").transform.Find("BoostInfo").gameObject.SetActive(true);

	}

	public void Play()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		if (gameObject.name == "MenuPreGameOver") {
			if (InitScript.Gems >= 12) {
				InitScript.Instance.SpendGems(12);
				//                LevelData.LimitAmount += 12;
				LevelManager.Instance.gameStatus = GameState.WaitAfterClose;
				gameObject.SetActive(false);

			} else {
				BuyGems();
			}
		} else if (gameObject.name == "MenuFailed") {
			LevelManager.Instance.gameStatus = GameState.Map;
		} else if (gameObject.name == "MenuPlay") {
			if (InitScript.lifes > 0) {
				//InitScript.Instance.SpendLife(1);
				LevelManager.THIS.gameStatus = GameState.PrepareGame;
				CloseMenu();
				//Application.LoadLevel( "game" );
			} else {
				BuyLifeShop();
			}

		} else if (gameObject.name == "MenuPause") {
			CloseMenu();
			LevelManager.Instance.gameStatus = GameState.Playing;
		}
	}

	public void PlayTutorial()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		LevelManager.Instance.gameStatus = GameState.Playing;
		//    mainscript.Instance.dropDownTime = Time.time + 0.5f;
		//        CloseMenu();
	}

	public void BackToMap()
	{
		Time.timeScale = 1;
		LevelManager.THIS.gameStatus = GameState.GameOver;
		CloseMenu();
	}

	public void Next()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		CloseMenu();
	}

	public void Again()
	{
		if (InitScript.lifes > 0) {

			SoundBase.Instance.PlaySound(SoundBase.Instance.click);
			LevelManager.THIS.gameStatus = GameState.PrepareGame;
			keepGaming = true;
			CloseMenu();
		} else {
			BuyLifeShop();
		}
	}


	public void BuyGems()
	{

		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
	}

	public void Buy(GameObject pack)
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		if (pack.name == "Pack1") {
			InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
			InitScript.Instance.PurchaseSucceded ();
			CloseMenu ();
			return;
#endif
#if UNITY_INAPPS
			UnityInAppsIntegration.THIS.BuyProductID (LevelManager.THIS.InAppIDs [0]);
#elif AMAZON
			AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [0]);

#endif

		}

		if (pack.name == "Pack2") {
			InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
			InitScript.Instance.PurchaseSucceded ();
			CloseMenu ();
			return;
#endif
#if UNITY_INAPPS
			UnityInAppsIntegration.THIS.BuyProductID (LevelManager.THIS.InAppIDs [1]);
#elif AMAZON
			AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [1]);

#endif


		}
		if (pack.name == "Pack3") {
			InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
			InitScript.Instance.PurchaseSucceded ();
			CloseMenu ();
			return;
#endif
#if UNITY_INAPPS
			UnityInAppsIntegration.THIS.BuyProductID (LevelManager.THIS.InAppIDs [2]);
#elif AMAZON
			AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [2]);

#endif


		}
		if (pack.name == "Pack4") {
			InitScript.waitedPurchaseGems = int.Parse(pack.transform.Find("Count").GetComponent<Text>().text.Replace("x ", ""));
#if UNITY_WEBPLAYER || UNITY_WEBGL
			InitScript.Instance.PurchaseSucceded ();
			CloseMenu ();
			return;
#endif
#if UNITY_INAPPS
			UnityInAppsIntegration.THIS.BuyProductID (LevelManager.THIS.InAppIDs [3]);
#elif AMAZON
			AmazonInapps.THIS.Purchase (LevelManager.THIS.InAppIDs [3]);

#endif


		}
		CloseMenu();

	}

	public void BuyLifeShop()
	{

		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		if (InitScript.lifes < InitScript.Instance.CapOfLife)
			GameObject.Find("CanvasGlobal").transform.Find("LiveShop").gameObject.SetActive(true);

	}

	public void BuyLife(GameObject button)
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		if (InitScript.Gems >= int.Parse(button.transform.Find("Price").GetComponent<Text>().text)) {
			InitScript.Instance.SpendGems(int.Parse(button.transform.Find("Price").GetComponent<Text>().text));
			InitScript.Instance.RestoreLifes();
			CloseMenu();
		} else {
			GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
		}

	}

	public void BuyFailed(GameObject button)
	{
		//if (GetComponent<Animation>()["bannerFailed"].speed == 0)
		//{
		if (InitScript.Gems >= LevelManager.THIS.FailedCost) {
			InitScript.Instance.SpendGems(LevelManager.THIS.FailedCost);
			//button.GetComponent<Button>().interactable = false;
			GoOnFailed();
		} else {
			GameObject.Find("CanvasGlobal").transform.Find("GemsShop").gameObject.SetActive(true);
		}
		//}
	}

	public void GoOnFailed()
	{
		if (LevelManager.THIS.limitType == LIMIT.MOVES)
			LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedMoves;
		else {
			LevelManager.THIS.Limit += LevelManager.THIS.ExtraFailedSecs;
		}

		if (LevelManager.THIS.target == Target.BOMBS)//1.3
			LevelManager.THIS.RechargeBombs();
		//GetComponent<Animation>()["bannerFailed"].speed = 1;
		keepGaming = true;
//		MusicBase.Instance.GetComponent<AudioSource>().Play();
		CloseMenu();

		LevelManager.THIS.gameStatus = GameState.Playing;
		LevelManager.THIS.RestartTimer();

	}

	public void GiveUp()
	{
		GetComponent<Animation>()["bannerFailed"].speed = 1;
	}

	void ShowGameOver()
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.gameOver[1]);

		GameObject.Find("Canvas").transform.Find("MenuGameOver").gameObject.SetActive(true);
		gameObject.SetActive(false);

	}

	#region boosts

	public void BuyBoost(BoostType boostType, int price, int count)
	{
		SoundBase.Instance.PlaySound(SoundBase.Instance.click);
		if (InitScript.Gems >= price) {
			SoundBase.Instance.PlaySound(SoundBase.Instance.cash);
			InitScript.Instance.SpendGems(price);
			InitScript.Instance.BuyBoost(boostType, price, count);
			//InitScript.Instance.SpendBoost(boostType);
			CloseMenu();
		} else {
			BuyGems();
		}
	}

	#endregion

	public void SoundOff(GameObject Off)
	{
		if (!Off.activeSelf) {
			SoundBase.Instance.GetComponent<AudioSource>().volume = 0;
			InitScript.sound = false;

			Off.SetActive(true);
		} else {
			SoundBase.Instance.GetComponent<AudioSource>().volume = 1;
			InitScript.sound = true;

			Off.SetActive(false);

		}
		PlayerPrefs.SetInt("Sound", (int)SoundBase.Instance.GetComponent<AudioSource>().volume);
		PlayerPrefs.Save();

	}

	public void MusicOff(GameObject Off)
	{
		float volume = 0.0f;
		if (!Off.activeSelf) {
//			GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
			MusicBase.Instance.StopCurrentBGM();
			MusicBase.Instance.SetVolume(volume);
			InitScript.music = false;
			Off.SetActive(true);
		} else {
			volume = 1.0f;
			MusicBase.Instance.SetVolume(volume);
			MusicBase.Instance.PlayCurrentBGM();
//			GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
			InitScript.music = true;
			Off.SetActive(false);
		}

		PlayerPrefs.SetInt("Music", (int)volume);
		PlayerPrefs.Save();

	}

	Target target;
	LIMIT limitType;
	int[] ingrCountTarget;
	Ingredients[] ingrTarget;
	CollectItems[] collectItems;
	private bool keepGaming;

	void LoadLevel(int n)
	{
		TextAsset map = Resources.Load("Levels/" + n) as TextAsset;
		if (map != null) {
			string mapText = map.text;
			string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
			ingrTarget = new Ingredients[LevelManager.THIS.NumIngredients];
			ingrCountTarget = new int[LevelManager.THIS.NumIngredients];
			collectItems = new CollectItems[LevelManager.THIS.NumIngredients];
			int mapLine = 0;
			foreach (string line in lines) {
				//check if line is game mode line
				if (line.StartsWith("MODE")) {
					//Replace GM to get mode number, 
					string modeString = line.Replace("MODE", string.Empty).Trim();
					//then parse it to interger
					target = (Target)int.Parse(modeString);
					//Assign game mode
				} else if (line.StartsWith("LIMIT")) {
					string blocksString = line.Replace("LIMIT", string.Empty).Trim();
					string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
					limitType = (LIMIT)int.Parse(sizes[0]);
				} else if (line.StartsWith("COLLECT COUNT ")) {
					string blocksString = line.Replace("COLLECT COUNT", string.Empty).Trim();
					string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < blocksNumbers.Length; i++) {
						ingrCountTarget[i] = int.Parse(blocksNumbers[i]);

					}
				} else if (line.StartsWith("COLLECT ITEMS ")) {
					string blocksString = line.Replace("COLLECT ITEMS", string.Empty).Trim();
					string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
					for (int i = 0; i < blocksNumbers.Length; i++) {
						if (target == Target.COLLECT)
							ingrTarget[i] = (Ingredients)int.Parse(blocksNumbers[i]);
						else if (target == Target.ITEMS)
							collectItems[i] = (CollectItems)int.Parse(blocksNumbers[i]);


					}
				}

			}
		}

	}

}
