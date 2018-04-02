#if PLAYFAB || GAMESPARKS
using UnityEngine;
using System.Collections;

#if PLAYFAB
using PlayFab.ClientModels;
using PlayFab;
#endif
using System.Collections.Generic;


public class NetworkDataManager
{
	IDataManager dataManager;
	public	static int LatestReachedLevel = 0;
	public static int LevelScoreCurrentRecord = 0;

	public NetworkDataManager ()
	{
#if PLAYFAB
		dataManager = new PlayFabDataManager ();
#elif GAMESPARKS
		dataManager = new GamesparksDataManager();
#endif
		NetworkManager.OnLoginEvent += GetPlayerLevel;
		LevelManager.OnEnterGame += GetPlayerScore;
		NetworkManager.OnLogoutEvent += Logout;
		NetworkManager.OnLoginEvent += GetBoosterData;	
	}

	public void Logout ()
	{
		dataManager.Logout();
		NetworkManager.OnLoginEvent -= GetPlayerLevel;
		LevelManager.OnEnterGame -= GetPlayerScore;
		NetworkManager.OnLoginEvent -= GetBoosterData;
		NetworkManager.OnLogoutEvent -= Logout;
	}


	#region SCORE

	//	public static void SetPlayerScoreTotal () {//1.3.3
	//		int latestLevel = LevelsMap._instance.GetLastestReachedLevel ();
	//		for (int i = 1; i <= latestLevel; i++) {
	//			SetPlayerScore (i, PlayerPrefs.GetInt ("Score" + i, 0));//TODO sync Level_
	//		}
	//	}

	public  void SetPlayerScore (int level, int score)
	{
		if (!NetworkManager.THIS.IsLoggedIn)
			return;

		if (score <= LevelScoreCurrentRecord)
			return;
		
		dataManager.SetPlayerScore(level, score);
	}

	public void GetPlayerScore ()
	{
		if (!NetworkManager.THIS.IsLoggedIn)
			return;
		
		dataManager.GetPlayerScore((value) =>
		{
			NetworkDataManager.LevelScoreCurrentRecord = value;
			PlayerPrefs.SetInt("Score" + LevelManager.THIS.currentLevel, NetworkDataManager.LevelScoreCurrentRecord);
			PlayerPrefs.Save();
		});
	}

	#endregion


	#region LEVEL

	public  void SetPlayerLevel (int level)
	{
		if (!NetworkManager.THIS.IsLoggedIn)
			return;

		if (level <= LatestReachedLevel)
			return;
		
		dataManager.SetPlayerLevel(level);
	}

	public void GetPlayerLevel ()
	{
		if (!NetworkManager.THIS.IsLoggedIn)
			return;

		dataManager.GetPlayerLevel((value) =>
		{
			NetworkDataManager.LatestReachedLevel = value;
			GetStars();
		});
		if (NetworkDataManager.LatestReachedLevel <= 0)
			NetworkManager.dataManager.SetPlayerLevel(1);
	}

	#endregion

	#region STARS

	public  void SetStars ()
	{
		int level = LevelManager.THIS.currentLevel;
		int stars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", level));
		dataManager.SetStars(stars, level);
	}

	public void GetStars ()
	{
		if (!NetworkManager.THIS.IsLoggedIn)
			return;


		if (LevelsMap._instance.GetLastestReachedLevel() > LatestReachedLevel)
		{
			Debug.Log("reached higher level than synced");
			SyncAllData();
			return;
		}

		dataManager.GetStars((dic) =>
		{
			foreach (var item in dic)
			{
				PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", int.Parse(item.Key.Replace("StarsLevel_", ""))), item.Value);
			}
			PlayerPrefs.Save();
			LevelsMap._instance.Reset();

		});
	}

	#endregion

	#region BOOSTS

	public void SetBoosterData ()
	{
		Dictionary<string, string> dic = new Dictionary<string, string>() {
			{ "Boost_" + (int)BoostType.ExtraMoves, "" + ZPlayerPrefs.GetInt("" + BoostType.ExtraMoves) },
			{ "Boost_" + (int)BoostType.Stripes, "" + ZPlayerPrefs.GetInt("" + BoostType.Stripes) },
			{ "Boost_" + (int)BoostType.ExtraTime, "" + ZPlayerPrefs.GetInt("" + BoostType.ExtraTime) },
			{ "Boost_" + (int)BoostType.Bomb, "" + ZPlayerPrefs.GetInt("" + BoostType.Bomb) },
			{ "Boost_" + (int)BoostType.Colorful_bomb, "" + ZPlayerPrefs.GetInt("" + BoostType.Colorful_bomb) },
			{ "Boost_" + (int)BoostType.Shovel, "" + ZPlayerPrefs.GetInt("" + BoostType.Shovel) },
			{ "Boost_" + (int)BoostType.Energy, "" + ZPlayerPrefs.GetInt("" + BoostType.Energy) }
		};

		dataManager.SetBoosterData(dic);
	}

	public  void GetBoosterData ()
	{
		if (!NetworkManager.THIS.IsLoggedIn)
			return;

		dataManager.GetBoosterData((dic) =>
		{
			foreach (var item in dic)
			{
				BoostType boostType = (BoostType)int.Parse(item.Key.Replace("Boost_", ""));
				ZPlayerPrefs.SetInt("" + boostType, item.Value);
				Messenger.Broadcast<BoostType, int>("BoostValueChanged", boostType, item.Value);
			}
			ZPlayerPrefs.Save();
		});
	}


	#endregion

	public	void SetTotalStars ()
	{
		dataManager.SetTotalStars();
	}

	public void SyncAllData ()
	{
		SetTotalStars();
		SetPlayerLevel(LevelsMap._instance.GetLastestReachedLevel());
		//		SetPlayerScoreTotal ();
		Debug.LogError("SyncAllData");
	}

}

#endif