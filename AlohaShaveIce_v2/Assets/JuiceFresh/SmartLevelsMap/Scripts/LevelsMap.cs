using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;

public class LevelsMap : MonoBehaviour
{
	public static LevelsMap _instance;
	private static IMapProgressManager _mapProgressManager = new PlayerPrefsMapProgressManager();

	public bool IsGenerated;

	public MapLevel MapLevelPrefab;
	public Transform CharacterPrefab;
	public int Count = 10;

	public WaypointsMover WaypointsMover;
	public MapLevel CharacterLevel;
	public TranslationType TranslationType;

	public bool StarsEnabled;
	public StarsType StarsType;

	public bool ScrollingEnabled;
	public MapCamera MapCamera;

	public bool IsClickEnabled;
	public bool IsConfirmationEnabled;

	public event Action OnReset;

	GameObject dailyQuestButton;

    public int LatestReachedLevel;

	public void Awake ()
	{
		_instance = this;
	}

	public void OnDestroy ()
	{
		_instance = null;
	}

	public void OnEnable ()
	{
		if (IsGenerated)
		{
			Reset();
		}
	}

	List<MapLevel> MapLevels = new List<MapLevel>();

	public List<MapLevel> GetMapLevels ()
	{
		if (MapLevels.Count == 0)//1.4.4
			MapLevels = FindObjectsOfType<MapLevel>().OrderBy(ml => ml.Number).ToList();
		
		return MapLevels;
	}

	public void Reset ()
	{
		UpdateMapLevels();
		PlaceCharacterToLastUnlockedLevel();
		SetCameraToCharacter();

		if (OnReset != null)
		{
			OnReset();
		}
	}

	private void UpdateMapLevels ()
	{
		foreach (MapLevel mapLevel in GetMapLevels())
		{
			mapLevel.UpdateState(
				_mapProgressManager.LoadLevelStarsCount(mapLevel.Number),
				IsLevelLocked(mapLevel.Number));
		}
	}

	private void PlaceCharacterToLastUnlockedLevel ()
	{
		int lastUnlockedNumber = GetMapLevels().Where(l => !l.IsLocked).Select(l => l.Number).Max();

        LatestReachedLevel = lastUnlockedNumber;

		if (dailyQuestButton == null)
		{
			dailyQuestButton = GameObject.Find("DailyQuest");//.SetActive(false);
		}

		dailyQuestButton.SetActive(false);
		if (lastUnlockedNumber > 6)
		{
			dailyQuestButton.SetActive(true);
//			if (GameTutorialManager.Instance.ShowMenuTutorial(TutorialType.DailyQuest_Try, dailyQuestButton.GetComponent<RectTransform>()))
//			{
//				//					DailyRewardManager.Instance.EnableReward(true);
//				//					CloseMenu();
//			}
		}

		if (WaypointsMover == null || CharacterLevel == null || CharacterLevel.Number == lastUnlockedNumber || CharacterLevel.Number <= lastUnlockedNumber - 2)
		{
			TeleportToLevelInternal(lastUnlockedNumber, true);
			return;
		}
		_instance.WalkToLevelInternal(lastUnlockedNumber);
	}

	public int GetLastestReachedLevel ()
	{//1.3.3
		if (LevelManager.THIS.gameStatus == GameState.Map)//1.4.4
			return GetMapLevels().Where(l => !l.IsLocked).Select(l => l.Number).Max();
		else
			return LevelManager.THIS.currentLevel;
	}

	public void SetCameraToCharacter ()
	{
        if (MapCamera == null)
        {
            MapCamera = FindObjectOfType<MapCamera>();
        }

        if (MapCamera != null)
            MapCamera.SetPosition(WaypointsMover.transform.position);
	}

	#region Events

	public static event EventHandler<LevelReachedEventArgs> LevelSelected;
	public static event EventHandler<LevelReachedEventArgs> LevelReached;

	#endregion

	#region Static API

	public static void CompleteLevel (int number)
	{
		CompleteLevelInternal(number, 1);
	}

	public static void CompleteLevel (int number, int starsCount)
	{
		CompleteLevelInternal(number, starsCount);
	}

	internal static void OnLevelSelected (int number)
	{
		if (LevelSelected != null && !IsLevelLocked(number))  //need to fix in the map plugin
            LevelSelected(_instance, new LevelReachedEventArgs(number));

		if (!_instance.IsConfirmationEnabled)
			GoToLevel(number);
	}

	public static void GoToLevel (int number)
	{
		switch (_instance.TranslationType)
		{
		case TranslationType.Teleportation:
			_instance.TeleportToLevelInternal(number, false);
			break;
		case TranslationType.Walk:
			_instance.WalkToLevelInternal(number);
			break;
		}
	}

	public static bool IsLevelLocked (int number)
	{
		return number > 1 && _mapProgressManager.LoadLevelStarsCount(number - 1) == 0;
	}

	public static void OverrideMapProgressManager (IMapProgressManager mapProgressManager)
	{
		_mapProgressManager = mapProgressManager;
	}

	public static void ClearAllProgress ()
	{
		_instance.ClearAllProgressInternal();
	}

	public static bool IsStarsEnabled ()
	{
		return _instance.StarsEnabled;
	}

	public static bool GetIsClickEnabled ()
	{
		int touchId = -1;
		#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
		touchId = 0;
		#endif

		return _instance.IsClickEnabled && UnityEngine.EventSystems.EventSystem.current != null && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touchId);
	}

	public static bool GetIsConfirmationEnabled ()
	{
		return _instance.IsConfirmationEnabled;
	}

	#endregion

	private static void CompleteLevelInternal (int number, int starsCount)
	{
		if (IsLevelLocked(number))
		{
			Debug.Log(string.Format("Can't complete locked level {0}.", number));
		}
		else if (starsCount < 1 || starsCount > 3)
		{
			Debug.Log(string.Format("Can't complete level {0}. Invalid stars count {1}.", number, starsCount));
		}
		else
		{
			int curStarsCount = _mapProgressManager.LoadLevelStarsCount(number);
			int maxStarsCount = Mathf.Max(curStarsCount, starsCount);
			_mapProgressManager.SaveLevelStarsCount(number, maxStarsCount);

			if (_instance != null)
				_instance.UpdateMapLevels();
		}
	}

	private void TeleportToLevelInternal (int number, bool isQuietly)
	{
		MapLevel mapLevel = GetLevel(number);
		if (mapLevel.IsLocked)
		{
			Debug.Log(string.Format("Can't jump to locked level number {0}.", number));
		}
		else
		{
			WaypointsMover.transform.position = mapLevel.PathPivot.transform.position;   //need to fix in the map plugin
			CharacterLevel = mapLevel;
			if (!isQuietly)
				RaiseLevelReached(number);
		}
	}

	private void WalkToLevelInternal (int number)
	{
		MapLevel mapLevel = GetLevel(number);
		if (mapLevel.IsLocked)
		{
			Debug.Log(string.Format("Can't go to locked level number {0}.", number));
		}
		else
		{
			WaypointsMover.Move(CharacterLevel.PathPivot, mapLevel.PathPivot,
				() =>
				{
					RaiseLevelReached(number);
					CharacterLevel = mapLevel;
					StartCoroutine(OpenMenuPlayAfterCharacterMove());
				});
		}
	}

	IEnumerator OpenMenuPlayAfterCharacterMove ()
	{
		yield return new WaitForSeconds(0.3f);
		if (LevelManager.Instance.questInfo == null)
		{
			if (DailyRewardManager.Instance.openChestBoxCanvas == null || (DailyRewardManager.Instance.openChestBoxCanvas != null && !DailyRewardManager.Instance.openChestBoxCanvas.activeSelf))
			{
				if (!DailyQuestManager.Instance.panel.gameObject.activeSelf)
				{
					GameObject.Find("CanvasGlobal").transform.Find("MenuPlay").gameObject.SetActive(true);
				}
			}
		}
	}

	private void RaiseLevelReached (int number)
	{
		MapLevel mapLevel = GetLevel(number);
		if (!string.IsNullOrEmpty(mapLevel.SceneName))
			Application.LoadLevel(mapLevel.SceneName);

		if (LevelReached != null)
			LevelReached(this, new LevelReachedEventArgs(number));
	}

	public MapLevel GetLevel (int number)
	{
		return GetMapLevels().SingleOrDefault(ml => ml.Number == number);
	}

	private void ClearAllProgressInternal ()
	{
		foreach (MapLevel mapLevel in GetMapLevels())
			_mapProgressManager.ClearLevelProgress(mapLevel.Number);
		Reset();
	}

	public void SetStarsEnabled (bool bEnabled)
	{
		StarsEnabled = bEnabled;
		int starsCount = 0;
		foreach (MapLevel mapLevel in GetMapLevels())
		{
			mapLevel.UpdateStars(starsCount);
			starsCount = (starsCount + 1) % 4;
			mapLevel.StarsHoster.gameObject.SetActive(bEnabled);
			mapLevel.SolidStarsHoster.gameObject.SetActive(bEnabled);
		}
	}

	public void SetStarsType (StarsType starsType)
	{
		StarsType = starsType;
		foreach (MapLevel mapLevel in GetMapLevels())
			mapLevel.UpdateStarsType(starsType);
	}

}
