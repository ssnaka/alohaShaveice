using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using GameToolkit.Localization;

public class SquareBlocks
{
	public SquareTypes block;
	public SquareTypes obstacle;

}

public enum GameState
{
	Map,
	PrepareGame,
	PrepareBoosts,
	Playing,
	Highscore,
	GameOver,
	Pause,
	PreWinAnimations,
	Win,
	WaitForPopup,
	WaitAfterClose,
	BlockedGame,
	Tutorial,
	PreTutorial,
	WaitForPotion,
	PreFailed,
	PreFailedBomb,
	RegenLevel,
	BoostShop
}


public class LevelManager : MonoBehaviour
{
	public event Action<int> OnScoreUpdate;
	public event Action<int> OnStarUpdate;
	public event Action<int> OnTargetBlockUpdate;
	public event Action<int> OnTargetCageUpdate;
	public event Action<int> OnTargetBombUpdate;
	public event Action<int> OnTargetIngredientUpdate;
	public event Action<int> OnLimitUpdate;

	//inctance of LevelManager for direct references
	public static LevelManager THIS;
	//inctance of LevelManager for direct references
	public static LevelManager Instance;
	//prefab of item
	public GameObject itemPrefab;
	//prefab of square
	public GameObject squarePrefab;
	//sprite of square
	public Sprite squareSprite;
	//second sprite of square
	public Sprite squareSprite1;
	//outline border for squares
	public Sprite outline1;
	//outline border for squares
	public Sprite outline2;
	//outline border for squares
	public Sprite outline3;
	//prefab of block, for more info check enum SquareTypes
	public GameObject blockPrefab;
	//prefab of wire, for more info check enum SquareTypes
	public GameObject wireBlockPrefab;
	//prefab of solid block, for more info check enum SquareTypes
	public GameObject solidBlockPrefab;
	//prefab of undestroyable block, for more info check enum SquareTypes
	public GameObject undesroyableBlockPrefab;
	//prefab of growing block, for more info check enum SquareTypes
	public GameObject thrivingBlockPrefab;
	//life shop scene object
	public LifeShop lifeShop;
	//Gamefield scene object
	public Transform GameField;
	//enabling iapps flag
	public bool enableInApps;
	//max rows of gamefield
	public int maxRows = 10;
	//max cols of gamefield
	public int maxCols = 8;
	//right square size for level generation
	public float squareWidth = 1.2f;
	//right square size for level generation
	public float squareHeight = 1.2f;
	//position of the first square on the game field
	public Vector2 firstSquarePosition;
	//array for holding squares data of the game field
	public Square[] squaresArray;
	//array of combined items
	List<List<Item>> combinedItems = new List<List<Item>>();
	// latest touched item
	public Item lastDraggedItem;
	//array for items prepeared to destory
	public List<Item> destroyAnyway = new List<Item>();
	//prefab for popuping scores
	public GameObject popupScore;
	//amount of scores for item
	public int scoreForItem = 10;
	//amount of scores for block
	public int scoreForBlock = 100;
	//amount of scores for wire block
	public int scoreForWireBlock = 100;
	//amount of scores for solid block
	public int scoreForSolidBlock = 100;
	//amount of scores for growing block
	public int scoreForThrivingBlock = 100;
	//type of game limit (moves or time)
	public LIMIT limitType;
	//value of rest limit (moves or time)
	public int Limit = 30;
	//deprecated
	public int TargetScore = 1000;
	//current level number
	public int currentLevel = 1;
	//cost of continue playing after fail
	public int FailedCost;
	//extra moves that you get to continue game after fail
	public int ExtraFailedMoves = 5;
	//extra seconds that you get to continue game after fail
	public int ExtraFailedSecs = 30;
	// array of iapps products
	public List<GemProduct> gemsProducts = new List<GemProduct>();
	// product IDs
	public string[] InAppIDs;
	// google licnse key
	public string GoogleLicenseKey;
	//line object for effect
	public Line line;
	//is any growing blocks destroyed in that turn
	public bool thrivingBlockDestroyed;
	//inner using variable
	List<List<Item>> newCombines;
	// is touch blocks?
	private bool dragBlocked;
	// amount of boosts
	public int BoostColorfullBomb;
	//amount of boosts
	public int BoostPackage;
	//amount of boosts
	public int BoostStriped;
	//inner using variable
	public BoostIcon emptyBoostIcon;
	// deprecated
	public BoostIcon AvctivatedBoostView;
	//currently active boost
	public BoostIcon activatedBoost;


	// field of getting and setting currently activated boost
	public BoostIcon ActivatedBoost
	{
		get
		{
			if (activatedBoost == null)
			{
				//BoostIcon bi = new BoostIcon();
				//bi.type = BoostType.None;
				return emptyBoostIcon;
			}
			else
				return activatedBoost;
		}
		set
		{

			if (value == null)
			{
				if (activatedBoost != null && gameStatus == GameState.Playing)
					InitScript.Instance.SpendBoost(activatedBoost.type);
				UnLockBoosts();
			}
			//        if (activatedBoost != null) return;
			activatedBoost = value;

			if (value != null)
			{
				LockBoosts();
			}

			if (activatedBoost != null)
			{
				if (activatedBoost.type == BoostType.ExtraMoves || activatedBoost.type == BoostType.ExtraTime)
				{
					if (LevelManager.Instance.limitType == LIMIT.MOVES)
						LevelManager.THIS.Limit += 5;
					else
						LevelManager.THIS.Limit += 30;

					ActivatedBoost = null;
				}
			}
		}
	}

	//level data from the file
	SquareBlocks[] levelSquaresFile = new SquareBlocks[81];
	//amount of blocks for collecting
	public int targetBlocks;

	[Header("ObjectPool")]
	[SerializeField]
	public Transform objectPoolParent;
	public GameObject levelStar;
	//pool of level stars
	List<GameObject> starPool = new List<GameObject>();
	public GameObject levelLockPrefab;
	//pool of level lock
	List<GameObject> levelLockPool = new List<GameObject>();
	public GameObject levelNumberPrefab;
	//pool of level number
	List<MapLevelNumber> levelNumberPool = new List<MapLevelNumber>();
	public GameObject levelTimerPrefab;
	//pool of level timer
	List<GameObject> levelTimerPool = new List<GameObject>();
	//pool of explosion effects for items
    public List<GameObject> itemExplPool = new List<GameObject>(5);
	//pool of flowers
    public List<GameObject> flowersPool = new List<GameObject>(5);
    //pool of item appearing
    public List<GameObject> appearingEffectPool = new List<GameObject>(5);


	//global Score amount on current level
	public static int Score;
	// stars amount on current level
	public int stars;
	//deprecated
	private int linePoint;
	// amount of scores is necessary for reaching first star
	public int star1;
	// amount of scores is necessary for reaching second star
	public int star2;
	// amount of scores is necessary for reaching third star
	public int star3;
	//editor option to show popup scores
	public bool showPopupScores;
	//inner using
	int nextExtraItems;
	//prefab of row explosion effect
	public GameObject stripesEffect;
	//UI star object
	public GameObject star1Anim;
	//UI star object
	public GameObject star2Anim;
	//UI star object
	public GameObject star3Anim;
	//snow particle prefab
	public GameObject snowParticle;
	//array of colors for popup scores
	public Color[] scoresColors;
	//array of outline colors for popup scores
	public Color[] scoresColorsOutline;
	//editor variable for limitation of colors
	public int colorLimit;
	//necessary amount of collectable items
	public int[] ingrCountTarget = new int[4];
	//necessary collectable items
	public List<CollectedIngredients> ingrTarget = new List<CollectedIngredients>();
	//necessary collectable items
	CollectItems[] collectItems = new CollectItems[6];
	//sprites of collectable items
	public Sprite[] ingrediendSprites;
	//editor values of description tasks
	public string[] targetDiscriptions;
	public List<LocalizedText> targetDiscriptionAssets;
	//UI object
	public GameObject ingrObject;
	//UI object
	public GameObject blocksObject;
	//UI object
	public GameObject scoreTargetObject;
	//UI object
	public GameObject cageTargetObject;
	//UI object
	public GameObject bombTargetObject;
	//inner using
	private bool matchesGot;
	//inner using
	bool ingredientFly;

	//UI objects
	[SerializeField]
	GameObject gratzWordPrefabs;
	GratzWord gratzWord;

	//UI object
	public GameObject Level;
	//scene object
	public GameObject LevelsMapGO;

	public BoostIcon[] InGameBoosts;
	public int passLevelCounter;
	List<ItemsTypes> gatheredTypes = new List<ItemsTypes>();
	List<Vector3> startPosFlowers = new List<Vector3>();
	public List<GameObject> friendsAvatars = new List<GameObject>();

	public Target target;

	public int TargetBlocks
	{
		get
		{
			return targetBlocks;
		}
		set
		{
			if (targetBlocks < 0)
				targetBlocks = 0;
			targetBlocks = value;

			if (OnTargetBlockUpdate != null)
			{
				OnTargetBlockUpdate(targetBlocks);
			}

		}
	}

	public int TargetCages;
	public int TargetBombs;

	public bool DragBlocked
	{
		get
		{
			return dragBlocked;
		}
		set
		{
			if (value)
			{
				List<Item> items = GetItems();
				foreach (Item item in items)
				{
					//if (item != null)
					//    item.anim.SetBool("stop", true);
				}
			}
			else
			{
				//  StartCoroutine( StartIdleCor());
			}
			dragBlocked = value;
		}
	}

	int cageHP;

	private GameState GameStatus;
	public bool itemsHided;
	public int moveID;
	public int lastRandColor;
	public bool onlyFalling;
	public bool levelLoaded;
	public Hashtable countedSquares;
	public Sprite doubleBlock;
	public Sprite doubleSolidBlock;
	public bool FacebookEnable;
	public bool PlayFab;
	private int selectedColor;
	private bool stopSliding;
	private float offset;
	public GameObject flower;
	float extraItemEvery = 6;
	public int bombsCollect;
	//1.4.5
	public int bombTimer;
	public int NumIngredients = 4;

    public int hintLevelMax = 3;
	#region EVENTS

	public delegate void GameStateEvents ();
	public delegate void LevelCompleteEvent (bool _isWin, int _limitLeftOver, bool _extraLifeUsed);
	public delegate void QuestStartEvent (int _level);
	public delegate void QuestEndEvent (int _level, bool _isWin, int _limitLeftOver, bool _extraLifeUsed);

//	public static event GameStateEvents OnAppEnd;
	public static event GameStateEvents OnMapState;

	public static event GameStateEvents OnLevelLoaded;
	public static event GameStateEvents OnMenuPlay;
	public static event GameStateEvents OnMenuComplete;
	public static event GameStateEvents OnStartPlay;
//	public static event GameStateEvents OnWin;
//	public static event GameStateEvents OnLose;
	public static event GameStateEvents OnPowerUpUsed;
	public static event GameStateEvents OnEnterLevel;
	public static event LevelCompleteEvent OnLevelComplete;

	public static event QuestStartEvent OnQuestLevelStart;
	public static event QuestEndEvent OnQuestLevelEnd;
//	public static event GameStateEvents OnVideoAdShown;
//	public static event GameStateEvents OnFreeChestOpen;
//	public static event GameStateEvents OnDailyChestOpen;
//	public static event GameStateEvents OnPremiumChestOpen;


	public GameState gameStatus
	{
		get
		{
			return GameStatus;
		}
		set
		{
			GameStatus = value;

			if (value == GameState.PrepareGame)
			{
//				MusicBase.Instance.PlayBGM("game_music", true, true);
				MusicBase.Instance.PlayRandomBGM();
//				MusicBase.Instance.GetComponent<AudioSource>().Stop();
//				MusicBase.Instance.GetComponent<AudioSource>().loop = true;
//				MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[1];
//				MusicBase.Instance.GetComponent<AudioSource>().Play();

				PrepareGame();
				avatarManager.EnableAvatar(false);
			}
			else if (value == GameState.WaitForPopup)
			{

				InitLevel();
				OnLevelLoaded();

			}
			else if (value == GameState.PreFailedBomb)
			{

			}
			else if (value == GameState.PreFailed)
			{
				GameObject.Find("CanvasGlobal").transform.Find("PreFailed").gameObject.SetActive(true);

			}
			else if (value == GameState.Map)
			{
				avatarManager.EnableAvatar(true);
				if (questInfo != null && (questSaveData != null && !questSaveData.type.Equals(DailyQuestType.NextLevel)))
				{
//					MusicBase.Instance.PlayBGM("game_music", true, true);
					MusicBase.Instance.PlayRandomBGM();
					EnableMap(true);
					return;
				}

				if (PlayerPrefs.GetInt("OpenLevelTest") <= 0)
				{
//					MusicBase.Instance.PlayBGM("game_music", true, true);
					MusicBase.Instance.PlayRandomBGM();
//					MusicBase.Instance.GetComponent<AudioSource>().Stop();
//					MusicBase.Instance.GetComponent<AudioSource>().loop = true;
//					MusicBase.Instance.GetComponent<AudioSource>().clip = MusicBase.Instance.music[1];
//					MusicBase.Instance.GetComponent<AudioSource>().Play();
					EnableMap(true);
					OnMapState();
				}
				else
				{
					LevelManager.THIS.gameStatus = GameState.PrepareGame;
					PlayerPrefs.SetInt("OpenLevelTest", 0);
					PlayerPrefs.Save();
				}
#if UNITY_ANDROID || UNITY_IOS || UNITY_WINRT
				if (passLevelCounter > 0 && InitScript.Instance.ShowRateEvery > 0)
				{
					if (passLevelCounter % InitScript.Instance.ShowRateEvery == 0 && InitScript.Instance.ShowRateEvery > 0 && PlayerPrefs.GetInt("Rated", 0) == 0)
						InitScript.Instance.ShowRate();
				}
#endif

			}
			else if (value == GameState.Pause)
			{
				Time.timeScale = 0;

			}
			else if (value == GameState.PrepareBoosts)
			{
				SetPreBoosts();

			}
			else if (value == GameState.Playing)
			{
				Time.timeScale = 1;

				switch (LevelManager.THIS.target)
				{
					case Target.BLOCKS:
						for (int col = 0; col < maxCols; col++)
						{
							for (int row = maxRows - 1; row >= 0; row--)
							{
								Square square = GetSquare(col, row);
								if (square.type == SquareTypes.BLOCK)
								{
									GameTutorialManager.Instance.SetUpTutorialForLevel(TutorialType.Level_Target_Block, square.transform);
									break;
								}
							}
						}
						break;
					case Target.BOMBS:
						// Handled in IntiBombs()
						break;
					case Target.CAGES:
						for (int col = 0; col < maxCols; col++)
						{
							for (int row = maxRows - 1; row >= 0; row--)
							{
								Square square = GetSquare(col, row);
								if (square.type == SquareTypes.WIREBLOCK)
								{
									GameTutorialManager.Instance.SetUpTutorialForLevel(TutorialType.Level_Target_Bubble, square.transform);
									break;
								}
							}
						}
						break;
					case Target.COLLECT:
						for (int col = 0; col < maxCols; col++)
						{
							for (int row = maxRows - 1; row >= 0; row--)
							{
								Square square = GetSquare(col, row);
								Item item = square.item;
								if (item != null && item.currentType == ItemsTypes.INGREDIENT)
								{
									GameTutorialManager.Instance.SetUpTutorialForLevel(TutorialType.Level_Target_Ingredient, square.transform);
									break;
								}
							}
						}
						break;
					case Target.ITEMS:
						for (int col = 0; col < maxCols; col++)
						{
							for (int row = maxRows - 1; row >= 0; row--)
							{
								Square square = GetSquare(col, row);
								Item item = square.item;
								if (item != null && item.color == (int)collectItems[0] - 1)
								{
									GameTutorialManager.Instance.SetUpTutorialForLevel(TutorialType.Level_Target_Item, square.transform);
									break;
								}
							}
						}
						break;
					default:
						break;
				}

				CheckUndestroyableBlockToturial();

				StartCoroutine(TipsManager.THIS.CheckPossibleCombines());
			}
			else if (value == GameState.GameOver)
			{
				MusicBase.Instance.StopCurrentBGM();
//				MusicBase.Instance.GetComponent<AudioSource>().Stop();
				SoundBase.Instance.PlaySound(SoundBase.Instance.gameOver[0]);
				GameObject.Find("CanvasGlobal").transform.Find("MenuFailed").gameObject.SetActive(true);
				if (questInfo == null || (questSaveData != null && questSaveData.type.Equals(DailyQuestType.NextLevel)))
				{
					OnLevelComplete(false, limitLeftOver, extraLifeUsed);
				}
//				OnLose();
			}
			else if (value == GameState.PreWinAnimations)
			{
				MusicBase.Instance.StopCurrentBGM();
//				MusicBase.Instance.GetComponent<AudioSource>().Stop();
				StartCoroutine(PreWinAnimationsCor());
			}
			else if (value == GameState.Win)
			{
				passLevelCounter++;
				GameObject.Find("CanvasGlobal").transform.Find("MenuComplete").gameObject.SetActive(true);
				if (questInfo == null || (questSaveData != null && questSaveData.type.Equals(DailyQuestType.NextLevel)))
				{
					OnMenuComplete();
					OnLevelComplete(true, limitLeftOver, extraLifeUsed);
					if (questSaveData != null && questSaveData.type.Equals(DailyQuestType.NextLevel))
					{
						DailyQuestInfo _questInfo = LevelManager.Instance.questSaveData.dailyQuestInfos.Find(Item => Item.actualLevel.Equals(currentLevel));
						DailyQuestManager.Instance.UpdateQuestInfo(_questInfo);
					}
				}
				else
				{
					DailyQuestManager.Instance.UpdateQuestInfo(questInfo);
				}
//				OnWin();
			}
			InitScript.Instance.CheckAdsEvents(value);


		}
	}

	[Header("Background")]
	[SerializeField]
	Vector3 backgroundCenters;

	int totalLimit;
	int limitLeftOver;
	bool extraLifeUsed;

	public void MenuPlayEvent ()
	{
		OnMenuPlay();
	}

	Camera mCamera;
    MapCamera mapCamera;

	public bool isPlayingQuest;
	public int questLevel;
	public DailyQuestInfo questInfo;
	public DailyQuestSaveData questSaveData
	{
		get 
		{
			return DailyQuestManager.Instance.saveData;
		}
	}

    [SerializeField]
    GameObject appearingEffectPrefab;

    [SerializeField]
    Button skipButton;
	#endregion

	void OnEnable ()
	{
		if (mCamera == null)
		{
			mCamera = GetComponent<Camera>();
            mapCamera = GetComponent<MapCamera>();
		}
	}
	void LockBoosts ()
	{
		foreach (BoostIcon item in InGameBoosts)
		{
			if (item != ActivatedBoost)
				item.LockBoost();
		}
	}

	public void UnLockBoosts ()
	{
		foreach (BoostIcon item in InGameBoosts)
		{
			item.UnLockBoost();
		}
	}


	public LevelInfo LoadLevel ()
	{
		currentLevel = PlayerPrefs.GetInt("OpenLevel");// TargetHolder.level;
		if (currentLevel == 0)
			currentLevel = 1;
		LevelInfo result = LoadDataFromLocal(currentLevel);
//		NumIngredients = ingrTarget.Count;

		return result;
	}

	void SetupGameCamera ()
	{
		float aspect = (float)Screen.height / (float)Screen.width;
		mCamera.orthographicSize = 10.05f;
		aspect = (float)Math.Round(aspect, 2);
        if (aspect >= 1.9f && aspect < 2.06f)
            mCamera.orthographicSize = 11.5f;                  //18:9
        else if (aspect == 2.06f)
            mCamera.orthographicSize = 11.5f;                  //2960:1440 S8
        else if (aspect == 2.17f)
            mCamera.orthographicSize = 12.26f;                  //iphone x
        mapCamera.SetPosition(new Vector2(0, mCamera.transform.position.y));
	}


	public void EnableMap (bool enable)
	{
		if (enable)
		{
            LevelsMapGO.SetActive(true);
			float aspect = (float)Screen.height / (float)Screen.width;
			mCamera.orthographicSize = 10.25f;
			aspect = (float)Math.Round(aspect, 2);
            if (aspect == 1.6f)
                mCamera.orthographicSize = 12.2f;               //16:10
            else if (aspect == 1.78f)
                mCamera.orthographicSize = 13.6f;               //16:9
            else if (aspect == 1.5f)
                mCamera.orthographicSize = 11.2f;               //3:2
            else if (aspect == 1.33f)
                mCamera.orthographicSize = 10.25f;              //4:3
            else if (aspect == 1.67f)
                mCamera.orthographicSize = 12.5f;               //5:3
            else if (aspect >= 1.9f && aspect < 2.06f)
                mCamera.orthographicSize = 15.0f;               //18:9
            else if (aspect == 2.06f)
                mCamera.orthographicSize = 15.75f;              //2960:1440 S8   //1.4.7
            else if (aspect == 2.17f)
                mCamera.orthographicSize = 16.5f;               //iphone x    //1.4.7
            //else if (aspect == 1.25f)
            //    GetComponent<Camera>().orthographicSize = 4.9f;                  //5:4
            mapCamera.SetPosition(new Vector2(0, mCamera.transform.position.y));
		}
		else
		{
			InitScript.DateOfExit = DateTime.Now.ToString();
			SetupGameCamera();
			GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = false;
			GameObject.Find("CanvasGlobal").GetComponent<GraphicRaycaster>().enabled = true;
			Level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = false;
			Level.transform.Find("Canvas").GetComponent<GraphicRaycaster>().enabled = true;

		}
        mapCamera.enabled = enable;
		//1.4.4
		if (questInfo == null || (questSaveData != null && questSaveData.type.Equals(DailyQuestType.NextLevel)))
		{
            //		LevelsMapGO.SetActive (!enable);
            //		LevelsMapGO.SetActive (enable);
            LevelsMapGO.GetComponent<LevelsMap>().Reset();
		}
        else
        {
            LevelsMapGO.GetComponent<LevelsMap>().SetCameraToCharacter();
        }

        foreach (Transform tr in LevelsMapGO.transform)
		{
            if (tr.name != "AvatarManager" && tr.name != "Character" && tr.name != "CanvasMap")
            {
				tr.gameObject.SetActive(enable);
            }

			if (tr.name == "Character")
			{
				tr.GetComponent<SpriteRenderer>().enabled = enable;
				tr.transform.GetChild(0).gameObject.SetActive(enable);
			}

            InitScript.Instance.menuController.EnableMapMenu(enable);
		}
		Level.SetActive(!enable);

		if (enable)
			GameField.gameObject.SetActive(false);

		if (!enable)
			mCamera.transform.position = new Vector3(0, 0, -10);
		foreach (Transform item in GameField.transform)
		{
			Destroy(item.gameObject);
		}
	}

	AvatarManager avatarManager;

    void Awake ()
    {
        FacebookManager.Instance.Init();
        NetworkManager.Instance.Init();
    }

	// Use this for initialization
	void Start ()
	{
		avatarManager = GameObject.Find("AvatarManager").GetComponent<AvatarManager>();
		ingrCountTarget = new int[NumIngredients]; //necessary amount of collectable items
		//ingrTarget = InitScript.Instance.collectedIngredients.ToArray();  //necessary collectable items
		//collectItems = new CollectItems[NumIngredients];   //necessary collectable items

		if (Level.gameObject.activeSelf)
			Level.gameObject.SetActive(false);

        FacebookEnable = FacebookManager.Instance.FacebookEnable;//1.3.2
        if (FacebookEnable && FacebookManager.Instance.IsFaceboolLoggedIn())
        {
            NetworkManager.friendsManager.GetFriends();
            NetworkManager.dataManager.GetTutorial();
            NetworkManager.dataManager.GetPlayerLevel();
            NetworkManager.dataManager.GetBoosterData();
            NetworkManager.currencyManager.GetBalance();
        }
        else
        {
            LoadingCanvasScript.Instance.HideLoading();
        }


#if UNITY_INAPPS

		gameObject.AddComponent<UnityInAppsIntegration>();
		enableInApps = true;//1.3
#else
		enableInApps = false;
#endif

		THIS = this;
		Instance = this;
		if (!LevelManager.THIS.enableInApps)
			GameObject.Find("Gems").gameObject.SetActive(false);

		gameStatus = GameState.Map;

//		for (int i = 0; i < 30; i++)
//		{
//			GameObject go = Instantiate(levelStar, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
//			go.name = levelStar.name + "_" + i;
//			go.SetActive(false);
//			starPool.Add(go);
//		}
//
//		for (int i = 0; i < 10; i++)
//		{
//			GameObject go = Instantiate(levelLockPrefab, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
//			go.name = levelLockPrefab.name + "_" + i;
//			go.SetActive(false);
//			levelLockPool.Add(go);
//		}
//
//		for (int i = 0; i < 10; i++)
//		{
//			GameObject go = Instantiate(levelNumberPrefab, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
//			go.name = levelNumberPrefab.name + "_" + i;
//			go.SetActive(false);
//			MapLevelNumber mapLevelNumber = go.GetComponent<MapLevelNumber>();
//			levelNumberPool.Add(mapLevelNumber);
//		}
//
//		for (int i = 0; i < 3; i++)
//		{
//			GameObject go = Instantiate(levelTimerPrefab, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
//			go.name = levelTimerPrefab.name + "_" + i;
//			go.SetActive(false);
//			levelTimerPool.Add(go);
//		}

        for (int i = 0; i < itemExplPool.Count; i++)
		{
			itemExplPool[i] = Instantiate(Resources.Load("Prefabs/Effects/ItemExplNew"), transform.position, Quaternion.identity, objectPoolParent) as GameObject;
//			itemExplPool[i].GetComponent<SpriteRenderer>().enabled = false;
            itemExplPool[i].SetActive(false);
		}
        for (int i = 0; i < flowersPool.Count; i++)
		{
			flowersPool[i] = Instantiate(flower, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
//			flowersPool[i].GetComponent<SpriteRenderer>().enabled = false;
            flowersPool[i].SetActive(false);
		}
        for (int i = 0; i < appearingEffectPool.Count; i++)
        {
            appearingEffectPool[i] = Instantiate(appearingEffectPrefab, transform.position, Quaternion.identity, LevelManager.Instance.objectPoolParent) as GameObject;
            appearingEffectPool[i].SetActive(false);
        }

		passLevelCounter = 0;

#if UNITY_INAPPS
		if (GetComponent<UnityInAppsIntegration>() == null)
			gameObject.AddComponent<UnityInAppsIntegration>();
		
#endif

	}

	void InitLevel ()
	{
		//        itemPrefab = Resources.Load("Prefabs/Item " + currentLevel)as GameObject;
		GenerateLevel();
		GenerateOutline();
		GenerateNewItems(false);
		nextExtraItems = 0;
		bombTimers.Clear();//1.3
		//        ReGenLevel();
		RestartTimer();
		InitTargets();

		GameField.gameObject.SetActive(true);
	}

	public void RestartTimer ()
	{
		if (limitType == LIMIT.TIME)
		{
			StopCoroutine(TimeTick());
			StartCoroutine(TimeTick());
		}
	}

	List<GameObject> listIngredientsGUIObjects = new List<GameObject>();

	void InitTargets ()
	{
		blocksObject.SetActive(false);
		ingrObject.SetActive(false);
		scoreTargetObject.SetActive(false);
		cageTargetObject.SetActive(false);
		bombTargetObject.SetActive(false);
		GameObject ingrPrefab = Resources.Load("Prefabs/CollectGUIObj") as GameObject;
		foreach (GameObject item in listIngredientsGUIObjects)
		{
			Destroy(item);
		}
		listIngredientsGUIObjects.Clear();


		if (target != Target.COLLECT && target != Target.ITEMS)
			ingrObject.SetActive(false);
		else if (target == Target.COLLECT)
		{
			blocksObject.SetActive(false);
			CreateCollectableTarget(ingrObject, target, false);
		}
		else if (target == Target.ITEMS)
		{
			blocksObject.SetActive(false);
			CreateCollectableTarget(ingrObject, target, false);
		}
		if (targetBlocks > 0 && target == Target.BLOCKS)
		{
			blocksObject.SetActive(true);

			blocksObject.GetComponent<TargetGUI>().text.GetComponent<Counter_>().SetUpTotalCount(targetBlocks);
			//CreateCollectableTarget(ingrObject, target, false);
		}
		else if (LevelManager.THIS.target == Target.CAGES)
		{
			cageTargetObject.SetActive(true);
			cageTargetObject.GetComponent<TargetGUI>().text.GetComponent<Counter_>().SetUpTotalCount(TargetCages);
			//CreateCollectableTarget(ingrObject, target, false);
		}
		else if (LevelManager.THIS.target == Target.BOMBS)
		{
			StartCoroutine(InitBombs());
			bombTargetObject.SetActive(true);
			bombTargetObject.GetComponent<TargetGUI>().text.GetComponent<Counter_>().SetUpTotalCount(bombsCollect);
			//CreateCollectableTarget(ingrObject, target, false);
		}
		else if (target == Target.SCORE)
		{
			ingrObject.SetActive(false);
			blocksObject.SetActive(false);
			scoreTargetObject.SetActive(true);
			cageTargetObject.SetActive(false);
			bombTargetObject.SetActive(false);
		}

	}

	public void CreateCollectableTarget (GameObject parentTransform, Target tar, bool ForDialog = true)
	{
		tar = target;
		// if (tar != Target.COLLECT && tar != Target.ITEMS && tar != Target.BOMBS && tar != Target.CAGES ) return;
		GameObject ingrPrefab = Resources.Load("Prefabs/CollectGUIObj") as GameObject;

		parentTransform.SetActive(true);
		RectTransform containerRect = parentTransform.GetComponent<RectTransform>();
		int Sprites_Length = (Resources.Load("Prefabs/Item") as GameObject).GetComponent<Item>().items.Length;
		Sprite[] spr = new Sprite[Sprites_Length];
		for (int i = 0; i < Sprites_Length; i++)
		{
			spr[i] = (Resources.Load("Prefabs/Item") as GameObject).GetComponent<Item>().items[i];
		}
		int num = NumIngredients;
		List<object> collectionItems = new List<object>();
		if (tar == Target.ITEMS)
		{
			for (int i = 0; i < num; i++)
			{
				collectionItems.Add(collectItems[i]);
			}
			Sprite[] sprOld = spr;
			int ii = 0;
			for (int i = 0; i < collectItems.Length; i++)
			{
				if (collectItems[i] != CollectItems.None)
				{
					spr[ii] = sprOld[(int)collectItems[i] - 1];
					ii++;
				}
			}
		}
		else if (tar == Target.COLLECT)
		{
			spr = ingrediendSprites;
			for (int i = 0; i < num; i++)
				collectionItems.Add(ingrTarget[i]);
		}
		else if (tar == Target.BLOCKS)
		{
			num = 1;
			spr = new Sprite[] { blockPrefab.GetComponent<SpriteRenderer>().sprite };
			for (int i = 0; i < num; i++)
				collectionItems.Add(Ingredients.Ingredient1);
			ingrTarget.Add(new CollectedIngredients());

			ingrTarget[0].count = TargetBlocks;
		}
		else if (tar == Target.CAGES)
		{
			num = 1;
			spr = new Sprite[] { wireBlockPrefab.GetComponent<SpriteRenderer>().sprite };
			for (int i = 0; i < num; i++)
				collectionItems.Add(Ingredients.Ingredient1);
			ingrTarget.Add(new CollectedIngredients());

			ingrTarget[0].count = TargetCages;
		}
		else if (tar == Target.BOMBS)
		{
			num = 1;
			spr = new Sprite[] { ingrPrefab.GetComponent<TargetGUI>().bomb };
			for (int i = 0; i < num; i++)
				collectionItems.Add(Ingredients.Ingredient1);
			ingrTarget.Add(new CollectedIngredients());
			ingrTarget[0].count = 1;
		}
		else if (tar == Target.SCORE)
		{
			num = 1;
			spr = new Sprite[] { ingrPrefab.GetComponent<TargetGUI>().star };
			for (int i = 0; i < num; i++)
				collectionItems.Add(Ingredients.Ingredient1);
			ingrTarget.Add(new CollectedIngredients());

			ingrTarget[0].count = 1;
		}
		int f = 0;
		for (int i = 0; i < num; i++)
		{
			if (collectionItems[i] != (object)0 && ingrTarget[i].count > 0)
			{
				f++;
			}
		}
		float offset = 110;
		if (ForDialog)
			offset = 200;

		// if collection items count is greater than 4, closer and smaller.
		if (num > 4)
		{
			offset = 140;
		}

		containerRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, (f - 1) * offset + ingrPrefab.transform.GetComponent<RectTransform>().rect.width / 2 * f - ingrPrefab.transform.GetComponent<RectTransform>().rect.width / 2 * (f - 2));
		int j = 0;
		//print(ingrCountTarget[0]);
		for (int i = 0; i < num; i++)
		{
			if (collectionItems[i] != (object)0 && ingrTarget[i].count > 0)
			{
				GameObject ingr = Instantiate(ingrPrefab) as GameObject;
				ingr.name = "Ingr" + i;
				ingr.GetComponent<TargetGUI>().SetBack(ForDialog);
				listIngredientsGUIObjects.Add(ingr);
				if (tar != Target.COLLECT)
					ingr.transform.Find("Image").GetComponent<Image>().sprite = spr[j];
				ingr.transform.Find("CountIngr").GetComponent<Counter_>().SetupIngredientTargetIndex(i);
				ingr.transform.Find("CountIngr").GetComponent<Counter_>().SetUpTotalCount(ingrTarget[i].count);
				ingr.transform.Find("CountIngrForMenu").GetComponent<Counter_>().SetUpTotalCount(ingrTarget[i].count);
				if (tar == Target.SCORE)
					ingr.transform.Find("CountIngrForMenu").GetComponent<Counter_>().SetUpTotalCount((int)LevelManager.THIS.starsTargetCount);
				else if (tar == Target.BLOCKS)
					ingr.transform.Find("CountIngr").name = "TargetBlocks";
				else if (tar == Target.CAGES)
					ingr.transform.Find("CountIngr").name = "TargetCages";
				else if (tar == Target.BOMBS)
					ingr.transform.Find("CountIngr").name = "TargetBombs";
				if (tar == Target.COLLECT)
				{
					ingr.GetComponent<TargetGUI>().SetSprite(ingrTarget[i].sprite);
				}
				ingr.transform.SetParent(parentTransform.transform);
				ingr.transform.localScale = Vector3.one;
				// if collection items count is greater than 4, closer and smaller.
				if (num > 4)
				{
					ingr.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);	
				}
				int heightPos = 0;
				//if (f > 2 && (j == 0 || j == f - 1) && !ForDialog)
				//    heightPos += 8;

				ingr.transform.GetComponent<RectTransform>().anchoredPosition = new Vector2(j * offset - containerRect.rect.width / 2 + ingr.transform.GetComponent<RectTransform>().rect.width / 2, heightPos);
				ingr.transform.localPosition = new Vector3(ingr.transform.localPosition.x, ingr.transform.localPosition.y, 0.0f);

				j++;
			}
		}
		//containerRect.anchoredPosition = new Vector2(0, 0);

	}

	IEnumerator InitBombs ()
	{
		if (Limit.Equals(totalLimit) && !target.Equals(Target.BOMBS))
		{
			yield break;
		}

		yield return new WaitUntil(() => !TipsManager.THIS.gotTip); //1.3
		yield return new WaitForSeconds(1);
		int bombsOnField = 0;
		List<Item> items = GetItems();
		foreach (Item item in items)
		{
			if (item.currentType == ItemsTypes.BOMB)
				bombsOnField++;
		}

		List<Item> itemsRand = GetRandomItems(bombsCollect - bombsOnField - LevelManager.THIS.TargetBombs);//1.3
		int i = 0;

		foreach (Item item in itemsRand)
		{
			item.nextType = ItemsTypes.BOMB;

			if (bombTimers.Count > 0)//1.3
				item.bombTimer = bombTimers[i];
			i++;
			item.ChangeType();
		}
			
		GameTutorialManager.Instance.SetUpTutorialForLevel(TutorialType.Level_Target_Bomb, itemsRand[0].square.transform);
	}

	public void RechargeBombs ()
	{//1.3
		StartCoroutine(InitBombs());
	}

	void PrepareGame ()
	{
		InitScript.Instance.SpendLife(1);

		extraLifeUsed = false;
		limitLeftOver = 0;

		ActivatedBoost = null;
		Score = 0;
		stars = 0;
		moveID = 0;
		selectedColor = -1;  //1.3
		highlightedItems = new List<Item>();
		if (ProgressBarScript.Instance)
			ProgressBarScript.Instance.ResetBar();


		blocksObject.SetActive(false);
		ingrObject.SetActive(false);
		scoreTargetObject.SetActive(false);
		cageTargetObject.SetActive(false);
		bombTargetObject.SetActive(false);

		star1Anim.SetActive(false);
		star2Anim.SetActive(false);
		star3Anim.SetActive(false);
		ingrTarget = new List<CollectedIngredients>();
		if (target != Target.COLLECT)
			ingrTarget.Add(new CollectedIngredients());
		//ingrTarget = InitScript.Instance.collectedIngredients.ToArray();  //necessary collectable items

		for (int i = 0; i < collectItems.Length; i++)
		{
			collectItems[i] = CollectItems.None;
			//ingrTarget[i] = Ingredients.None;
			//ingrCountTarget[i] = 0;
		}


		TargetBlocks = 0;
		TargetCages = 0;
		TargetBombs = 0;
		EnableMap(false);


		GameField.transform.position = Vector3.zero;
		firstSquarePosition = GameField.transform.position;

		squaresArray = new Square[maxCols * maxRows];
		if (questInfo == null || (questSaveData != null && questSaveData.type.Equals(DailyQuestType.NextLevel)))
		{
			LoadLevel();
		}
		else
		{
			LoadDataFromLocal(questInfo.actualLevel);
		}

		//float getSize = maxCols - 9;
		//if (getSize < maxRows - 9)
		//    getSize = maxRows - 9;
		//if (getSize > 0)
		//    camera.orthographicSize = 6.5f + getSize * 0.5f;
		Level.transform.Find("Canvas/PrePlay").gameObject.SetActive(true);//1.3.3
		if (limitType == LIMIT.MOVES)
		{
			InGameBoosts[0].gameObject.SetActive(true);
			InGameBoosts[1].gameObject.SetActive(false);
		}
		else
		{
			InGameBoosts[0].gameObject.SetActive(false);
			InGameBoosts[1].gameObject.SetActive(true);

		}

		OnEnterLevel();
	}

	public void CheckCollectedTarget (GameObject _item)
	{
		for (int i = 0; i < NumIngredients; i++)
		{
			if (ingrTarget[i].count > 0)
			{
				if (_item.GetComponent<Item>() != null)
				{
					if (_item.GetComponent<Item>().currentType == ItemsTypes.NONE)
					{
						if (_item.GetComponent<Item>().color == (int)collectItems[i] - 1)
						{
							GameObject item = new GameObject();
							item.transform.position = _item.transform.position;
							item.transform.localScale = Vector3.one / 2f;
							SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
							spr.sprite = _item.GetComponent<Item>().items[_item.GetComponent<Item>().color];
							spr.sortingLayerName = "UI";
							spr.sortingOrder = 1;

							StartCoroutine(StartAnimateIngredient(item, i));
						}
					}
					else if (_item.GetComponent<Item>().currentType == ItemsTypes.INGREDIENT)
					{
						if (ingrTarget[i].count > 0)
						{

							if (_item.GetComponent<Item>().color == i + 1000)
							{
								GameObject item = new GameObject();
								item.transform.position = _item.transform.position;
								item.transform.localScale = Vector3.one / 2f;
								SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
								spr.sprite = _item.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite;
								spr.sortingLayerName = "UI";
								spr.sortingOrder = 1;

								StartCoroutine(StartAnimateIngredient(item, i));
							}
						}
					}

				}
			}
		}
		if (targetBlocks > 0)
		{
			if (_item.GetComponent<Square>() != null)
			{
				GameObject item = new GameObject();
				item.transform.position = _item.transform.position;
				item.transform.localScale = Vector3.one / 2f;
				SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
				spr.sprite = _item.GetComponent<SpriteRenderer>().sprite;
				spr.sortingLayerName = "UI";
				spr.sortingOrder = 1;

				StartCoroutine(StartAnimateIngredient(item, 0));

			}
		}
		if (target == Target.BOMBS)
		{
			if (_item.GetComponent<Item>().currentType == ItemsTypes.BOMB)
			{

				GameObject item = new GameObject();
				item.transform.position = _item.transform.position;
				// item.transform.localScale = Vector3.one / 2f;
				SpriteRenderer spr = item.AddComponent<SpriteRenderer>();
				spr.sprite = _item.GetComponent<Item>().sprRenderer.sprite;
				spr.sortingLayerName = "UI";
				spr.sortingOrder = 1;
				item.transform.localScale /= 4f;

				StartCoroutine(StartAnimateIngredient(item, 0));

			}
		}
	}

	public GameObject GetLevelStarFromPool ()
	{
		for (int i = 0; i < starPool.Count; i++)
		{
			if (!starPool[i].activeSelf)
			{
				starPool[i].SetActive(true);
				return starPool[i];
			}
		}

		GameObject go = Instantiate(levelStar, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
		go.name = levelStar.name + "_" + starPool.Count;
		starPool.Add(go);
		return go;
	}

	public GameObject GetLevelLockFromPool ()
	{
		for (int i = 0; i < levelLockPool.Count; i++)
		{
			if (!levelLockPool[i].activeSelf)
			{
				levelLockPool[i].SetActive(true);
				return levelLockPool[i];
			}
		}

		GameObject go = Instantiate(levelLockPrefab, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
		go.name = levelLockPrefab.name + "_" + levelLockPool.Count;
		levelLockPool.Add(go);
		return go;
	}

	public MapLevelNumber GetLevelNumberFromPool ()
	{
		for (int i = 0; i < levelNumberPool.Count; i++)
		{
			if (!levelNumberPool[i].gameObject.activeSelf)
			{
				levelNumberPool[i].gameObject.SetActive(true);
				return levelNumberPool[i];
			}
		}

		GameObject go = Instantiate(levelNumberPrefab, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
		go.name = levelNumberPrefab.name + "_" + levelNumberPool.Count;
		MapLevelNumber mapLevelNumber = go.GetComponent<MapLevelNumber>();
		levelNumberPool.Add(mapLevelNumber);
		return mapLevelNumber;
	}

	public GameObject GetLevelTimerFromPool ()
	{
		for (int i = 0; i < levelTimerPool.Count; i++)
		{
			if (!levelTimerPool[i].activeSelf)
			{
				levelTimerPool[i].SetActive(true);
				return levelTimerPool[i];
			}
		}

		GameObject go = Instantiate(levelTimerPrefab, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
		go.name = levelTimerPrefab.name + "_" + levelTimerPool.Count;
		levelTimerPool.Add(go);
		return go;
	}

	public GameObject GetExplFromPool ()
	{
        for (int i = 0; i < itemExplPool.Count; i++)
		{
            if (!itemExplPool[i].activeSelf)//.GetComponent<SpriteRenderer>().enabled)
			{
				itemExplPool[i].SetActive(true);
//				itemExplPool[i].GetComponent<SpriteRenderer>().enabled = true;
				StartCoroutine(HideDelayed(itemExplPool[i]));
				return itemExplPool[i];
			}
		}

        GameObject newItemExpl = Instantiate(Resources.Load("Prefabs/Effects/ItemExplNew"), transform.position, Quaternion.identity, objectPoolParent) as GameObject;
        StartCoroutine(HideDelayed(newItemExpl));
        itemExplPool.Add(newItemExpl);

        return newItemExpl;
	}

	public bool CheckFlowerStillFly ()
	{ //check if any flower still not reachec his target
        for (int i = 0; i < flowersPool.Count; i++)
		{
            if (flowersPool[i].activeSelf)//.GetComponent<SpriteRenderer>().enabled)
			{
				return true;
			}
		}

		return false;
	}

	public GameObject GetFlowerFromPool ()
	{
        for (int i = 0; i < flowersPool.Count; i++)
		{
            if (!flowersPool[i].activeSelf)//.GetComponent<SpriteRenderer>().enabled)
			{
                flowersPool[i].SetActive(true);
//                flowersPool[i].GetComponent<SpriteRenderer>().enabled = true;
//				StartCoroutine(HideDelayed(flowersPool[i]));
				return flowersPool[i];
			}

		}

        GameObject newFlower = Instantiate(flower, transform.position, Quaternion.identity, objectPoolParent) as GameObject;
        flowersPool.Add(newFlower);
        return newFlower;
	}

    public GameObject GetAppearingEffectFromPool ()
    {
        for (int i = 0; i < appearingEffectPool.Count; i++)
        {
            if (!appearingEffectPool[i].activeSelf)
            {
                appearingEffectPool[i].SetActive(true);
                return appearingEffectPool[i];
            }

        }

        GameObject newAppearingEffect = Instantiate(appearingEffectPrefab, transform.position, Quaternion.identity, LevelManager.Instance.objectPoolParent) as GameObject;
        appearingEffectPool.Add(newAppearingEffect);
        return newAppearingEffect;
    }

	IEnumerator HideDelayed (GameObject gm)
	{
		yield return new WaitForSeconds(0.5f);
		if (gm.GetComponent<Animator>())
		{
			gm.GetComponent<Animator>().SetTrigger("stop");
			gm.GetComponent<Animator>().SetInteger("color", 10);
		}
//		gm.GetComponent<SpriteRenderer>().enabled = false;
		gm.SetActive(false);
	}

	public int GetActualIngredients ()
	{
		int count = 0;
		if (target == Target.COLLECT)
		{
			for (int i = 0; i < ingrTarget.Count; i++)
			{
				//if (ingrTarget[i] > 0)
				count++;
			}
		}
		else if (target == Target.ITEMS)
		{
			for (int i = 0; i < collectItems.Length; i++)
			{
				//if (collectItems[i] > 0)
				count++;
			}
		}
		return count;
	}

	public int GetRestIngredients ()
	{
		int count = 0;
		for (int i = 0; i < ingrTarget.Count; i++)
		{
			count += LevelManager.THIS.ingrTarget[i].count;
		}
		return count;
	}

	IEnumerator StartAnimateIngredient (GameObject item, int i)
	{
		ingredientFly = true;
		GameObject[] ingr = new GameObject[GetActualIngredients()];
		if (target == Target.COLLECT || target == Target.ITEMS)
		{
			for (int j = 0; j < NumIngredients; j++)
			{
				if (ingrObject.transform.Find("Ingr" + j) != null)
				{
					ingr[j] = ingrObject.transform.Find("Ingr" + j).gameObject;
				}
			}
		}
		else if (target == Target.BLOCKS || target == Target.BOMBS)
			ingr = new GameObject[1];
		if (target == Target.BLOCKS)
		{
			ingr[0] = blocksObject.transform.gameObject;

		}
		else if (target == Target.BOMBS)
		{
			ingr[0] = bombTargetObject.transform.gameObject;

		}

		AnimationCurve curveX = new AnimationCurve(new Keyframe(0, item.transform.position.x), new Keyframe(0.4f, mCamera.ScreenToWorldPoint(ingr[i].transform.position).x));
		AnimationCurve curveY = new AnimationCurve(new Keyframe(0, item.transform.position.y), new Keyframe(0.5f, mCamera.ScreenToWorldPoint(ingr[i].transform.position).y));
		curveY.AddKey(0.2f, item.transform.position.y + UnityEngine.Random.Range(-2, 0.5f));
		float startTime = Time.time;
		Vector3 startPos = item.transform.position;
		float speed = UnityEngine.Random.Range(0.4f, 0.6f);
		float distCovered = 0;
		if (ingrTarget.Count > 0)
		{
			if (ingrTarget[i].count > 0)
			{
				ingrTarget[i].count--;
				if (OnTargetIngredientUpdate != null)
				{
					OnTargetIngredientUpdate(i);
				}
			}
		}
		while (distCovered < 0.5f)
		{
			distCovered = (Time.time - startTime) * speed;
			item.transform.position = new Vector3(curveX.Evaluate(distCovered), curveY.Evaluate(distCovered), 0);
			item.transform.Rotate(Vector3.back, Time.deltaTime * 1000);
			yield return new WaitForFixedUpdate();
		}
		//     SoundBase.Instance.audio.PlayOneShot(SoundBase.Instance.getStarIngr);
		if (target == Target.BOMBS)
		{
			TargetBombs++;
			if (OnTargetBombUpdate != null)
			{
				OnTargetBombUpdate(TargetBombs);
			}
		}
		Destroy(item);
		if (gameStatus == GameState.Playing)
			CheckWinLose();
		ingredientFly = false;
	}

	public void CheckWinLose ()
	{
		if (Limit <= 0)
		{
			bool lose = false;
			AddLimit(-Limit);
			Limit = 0;
//			if (OnLimitUpdate != null)
//			{
//				OnLimitUpdate(Limit);
//			}

			if (LevelManager.THIS.target == Target.BLOCKS && LevelManager.THIS.TargetBlocks > 0)
			{
				lose = true;
			}
			else if (LevelManager.THIS.target == Target.CAGES && LevelManager.THIS.TargetCages > 0)
			{
				lose = true;
			}
			else if (LevelManager.THIS.target == Target.COLLECT || LevelManager.THIS.target == Target.ITEMS)
			{
				if (GetRestIngredients() > 0)
				{
					lose = true;
				}
			}
			else if (LevelManager.THIS.target == Target.SCORE && LevelManager.Score < GetScoresOfTargetStars())
			{
				lose = true;
			}
			if (LevelManager.Score < LevelManager.THIS.star1 && LevelManager.THIS.target != Target.SCORE)
			{
				lose = true;

			}
			if (lose)
				gameStatus = GameState.GameOver;
			else if (LevelManager.Score >= LevelManager.THIS.star1 && (LevelManager.THIS.target == Target.BOMBS) && LevelManager.THIS.TargetBombs >= bombsCollect)
			{
				gameStatus = GameState.PreWinAnimations;

			}
			else if (LevelManager.Score >= LevelManager.THIS.star1 && LevelManager.THIS.target == Target.BLOCKS && LevelManager.THIS.TargetBlocks <= 0)
			{
				gameStatus = GameState.PreWinAnimations;

			}
			else if (LevelManager.Score >= LevelManager.THIS.star1 && LevelManager.THIS.target == Target.CAGES && LevelManager.THIS.TargetCages <= 0)
			{
				gameStatus = GameState.PreWinAnimations;

			}
			else if (LevelManager.Score >= LevelManager.THIS.star1 && (LevelManager.THIS.target == Target.COLLECT || LevelManager.THIS.target == Target.ITEMS) && GetRestIngredients() <= 0)
			{
				gameStatus = GameState.PreWinAnimations;

			}
			else if (LevelManager.THIS.target == Target.SCORE && LevelManager.Score >= GetScoresOfTargetStars())
			{
				gameStatus = GameState.PreWinAnimations;
			}


		}
		else
		{
			bool win = false;

			if (LevelManager.THIS.target == Target.BLOCKS && LevelManager.THIS.TargetBlocks <= 0)
			{
				win = true;
			}
			if (LevelManager.THIS.target == Target.CAGES && LevelManager.THIS.TargetCages <= 0)
			{
				win = true;
			}
			if (LevelManager.THIS.target == Target.BOMBS && LevelManager.THIS.TargetBombs >= bombsCollect)
			{
				win = true;
			}
			else if (LevelManager.THIS.target == Target.COLLECT || LevelManager.THIS.target == Target.ITEMS)
			{
				win = true;
				if (GetRestIngredients() > 0)
				{
					win = false;
				}
			}
			if (LevelManager.THIS.target == Target.SCORE && LevelManager.Score >= GetScoresOfTargetStars())
			{
				win = true;
			}

			if (LevelManager.Score < LevelManager.THIS.star1 && LevelManager.THIS.target != Target.SCORE)
			{
				win = false;

			}
			if (win)
				gameStatus = GameState.PreWinAnimations;

		}
	}

	public int GetScoresOfTargetStars ()
	{
		return (int)this.GetType().GetField("star" + (int)starsTargetCount).GetValue(this); //get value of appropriate field (star1, star2 or star3)
	}

    public void OnSkipButtonPressed ()
    {
        skipButton.gameObject.SetActive(false);
        Time.timeScale = 100.0f;
    }

	IEnumerator PreWinAnimationsCor ()
	{
        skipButton.gameObject.SetActive(true);
        Time.timeScale = 1.0f;
        if (!InitScript.Instance.losingLifeEveryGame)
			InitScript.Instance.AddLife(1);
		SoundBase.Instance.PlaySound(SoundBase.Instance.complete[1]);
		GameObject.Find("Level/Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(true);//1.4.5
		yield return new WaitForSeconds(3);
		GameObject.Find("Level/Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(false);//1.4.5
		Vector3 pos1 = mCamera.ScreenToWorldPoint(GameObject.Find("Limit").transform.position);

		yield return new WaitForSeconds(1);

//		int countFlowers = limitType == LIMIT.MOVES ? Mathf.Clamp(Limit, 0, 8) : 3;
//		List<Item> items = GetRandomItems(limitType == LIMIT.MOVES ? Mathf.Clamp(Limit, 0, 8) : 3);
//		for (int i = 1; i <= countFlowers; i++)
//		{
//			if (limitType == LIMIT.MOVES)
//				Limit--;
//			GameObject flowerParticle = GetFlowerFromPool();
//			flowerParticle.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
//			flowerParticle.GetComponent<Flower>().StartFly(pos1, true);
//
//			//            item.nextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
//			//            item.ChangeType();
//			yield return new WaitForSeconds(0.3f);
//		}
//		Limit = 0;
//
//		while (CheckFlowerStillFly())
//			yield return new WaitForSeconds(0.3f);

//		List<Item> allExtraItems = GetAllExtraItems();
		while (GetAllExtraItems().Count > 0)
		{
			Item item = GetAllExtraItems()[0];
			item.DestroyItem(false, "", false, true);
			dragBlocked = true;
			yield return new WaitForSeconds(0.1f);
			FindMatches();
			yield return new WaitForSeconds(0.5f);
//			allExtraItems.RemoveAt(0);
			//           GenerateNewItems();
			while (dragBlocked)
				yield return new WaitForFixedUpdate();
		}

		yield return new WaitForSeconds(0.5f);

		if (gratzWord == null)
		{
			gratzWord = Instantiate(gratzWordPrefabs, new Vector3(1000.0f, 0.0f, 0.0f), Quaternion.identity, Level.transform.Find("Canvas").transform).GetComponent<GratzWord>();
			gratzWord.transform.localScale = Vector3.one;
		}
		gratzWord.SetupGartz(GratzType.LevelEnd);
		yield return new WaitForSeconds(0.5f);

//		int countFlowers = limitType == LIMIT.MOVES ? Mathf.Clamp(Limit, 0, 8) : 3;
//		List<Item> items = GetRandomItems(limitType == LIMIT.MOVES ? Mathf.Clamp(Limit, 0, 8) : 3);
		int countFlowers = limitType == LIMIT.MOVES ? Mathf.Clamp(Limit, 0, 10) : Mathf.Clamp(Limit, 3, Mathf.Min(Limit/6, 10));
		List<Item> items = GetRandomItems(countFlowers);//limitType == LIMIT.MOVES ? Mathf.Clamp(Limit, 0, 10) : Mathf.Clamp(Limit, 3, Mathf.Min(Limit/6, 10)));
		limitLeftOver = Limit;
		while (countFlowers > 0)
//		for (int i = 1; i <= countFlowers; i++)
		{
			if (limitType == LIMIT.MOVES)
			{
				AddLimit(-1);
//				Limit--;
			}

//			if (OnLimitUpdate != null)
//			{
//				OnLimitUpdate(Limit);
//			}

			GameObject flowerParticle = GetFlowerFromPool();
			flowerParticle.GetComponent<SpriteRenderer>().sortingLayerName = "UI";
			flowerParticle.GetComponent<Flower>().StartFly(pos1, true);

			//            item.nextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
			//            item.ChangeType();
			countFlowers--;
			yield return new WaitForSeconds(0.3f);
		}

		AddLimit(-Limit);
//		Limit = 0;
//		if (OnLimitUpdate != null)
//		{
//			OnLimitUpdate(Limit);
//		}

		while (CheckFlowerStillFly())
			yield return new WaitForSeconds(0.3f);

//		yield return new WaitForSeconds(0.3f);
//		allExtraItems = GetAllExtraItems();
		while (GetAllExtraItems().Count > 0)
		{
			Item item = GetAllExtraItems()[0];
			item.DestroyItem(false, "", false, true);
			dragBlocked = true;
			yield return new WaitForSeconds(0.1f);
			FindMatches();
			yield return new WaitForSeconds(1f);
//			allExtraItems.RemoveAt(0);
			//           GenerateNewItems();
			while (dragBlocked)
				yield return new WaitForFixedUpdate();
		}

		yield return new WaitForSeconds(1f);


		while (dragBlocked)
			yield return new WaitForSeconds(0.2f);

		//        GameObject.Find("Canvas").transform.Find("CompleteLabel").gameObject.SetActive(false);
		//        SoundBase.Instance.PlaySound(SoundBase.Instance.complete[0]);

		//        GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(true);
		//        yield return new WaitForSeconds(3);
		//        GameObject.Find("Canvas").transform.Find("PreCompleteBanner").gameObject.SetActive(false);

        previousStars = PlayerPrefs.GetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), 0);
		if (questInfo == null || (questSaveData != null && questSaveData.type.Equals(DailyQuestType.NextLevel)))
		{
            if (previousStars < stars)
				PlayerPrefs.SetInt(string.Format("Level.{0:000}.StarsCount", currentLevel), stars);
			if (Score > PlayerPrefs.GetInt("Score" + currentLevel))
			{
				PlayerPrefs.SetInt("Score" + currentLevel, Score);
			}
            LevelsMapGO.SetActive(false);//1.4.4
            //		LevelsMapGO.SetActive(true);//1.4.4
#if PLAYFAB || GAMESPARKS
			NetworkManager.dataManager.SetPlayerScore(currentLevel, Score);
			NetworkManager.dataManager.SetPlayerLevel(currentLevel + 1);
			NetworkManager.dataManager.SetStars();
#endif
		}

		gameStatus = GameState.Win;
        Time.timeScale = 1.0f;
        skipButton.gameObject.SetActive(false);
	}

    public int previousStars = 0;

	void DestroyGatheredExtraItems (Item item)
	{
        if (item.currentType.Equals(ItemsTypes.INGREDIENT))
        {
            return;
        }
		//ClearHighlight(true);
		if (gatheredTypes.Count > 1)
		{
			item.DestroyHorizontal();
			item.DestroyVertical();
		}

		foreach (ItemsTypes itemType in gatheredTypes)
		{
			if (itemType == ItemsTypes.HORIZONTAL_STRIPPED)
				item.DestroyHorizontal();
            else if (itemType == ItemsTypes.VERTICAL_STRIPPED)
				item.DestroyVertical();
		}
	}

	public BoostIcon waitingBoost;

	void Update ()
	{                                               // Debug keys events for editor   
		//if (Application.isEditor)
		//    SetupGameCamera();
		if (gameStatus == GameState.Playing)
		{
			//  AvctivatedBoostView = ActivatedBoost;
			if (Input.GetKeyDown(KeyCode.Space))
			{
				NoMatches();
			}
			if (Input.GetKeyDown(KeyCode.W))
			{            //Instant win
				gameStatus = GameState.PreWinAnimations;
			}
			if (Input.GetKeyDown(KeyCode.L))
			{            //last move 
				Limit = 1;
			}
		}
		if (Input.GetKeyUp(KeyCode.Escape))
		{    //Back button for Android
			if (LevelManager.THIS.gameStatus == GameState.Playing)
				GameObject.Find("CanvasGlobal").transform.Find("MenuPause").gameObject.SetActive(true);
			else if (LevelManager.THIS.gameStatus == GameState.Map)
				Application.Quit();
		}

		if (LevelManager.THIS.gameStatus == GameState.Playing)
		{
			Item itemSelected = null;
			if (Input.GetMouseButton(0))
			{        //touch detected
				if (LoadingCanvasScript.Instance.IsOn())
					return;
				OnStartPlay();
				Collider2D hit = Physics2D.OverlapPoint(mCamera.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Item"));

				if (hit != null)
				{
					itemSelected = hit.gameObject.GetComponent<Item>();

					if (itemSelected.currentType != ItemsTypes.INGREDIENT)
					{
						if (LevelManager.THIS.ActivatedBoost.type == BoostType.Bomb && itemSelected.currentType != ItemsTypes.INGREDIENT)
						{       //boost action events   BOMB
						}
						else if (LevelManager.THIS.ActivatedBoost.type == BoostType.Shovel && itemSelected.currentType != ItemsTypes.INGREDIENT)
						{  //boost action events   SHOVEL
						}
						else if (LevelManager.THIS.ActivatedBoost.type == BoostType.Energy && itemSelected.currentType != ItemsTypes.INGREDIENT)
						{ //boost action events   ENERGY
						}
						else if (selectedColor == -1 || selectedColor == itemSelected.color)
						{                //selection items by touch
							if (selectedColor >= 0)
							{
								if (itemSelected.currentType == ItemsTypes.SQUARE_BOMB || itemSelected.currentType == ItemsTypes.CROSS_BOMB)
								{
									return;
								}
							}

                            int i = 0;
                            line.SetVertexCount(destroyAnyway.Count);       //draw line effect for selected items
                            foreach (Item item in destroyAnyway)
                            {
                                if (item != null)
                                {
                                    line.AddPoint(item.transform.position, i);
                                    i++;
                                }
                                //Drawing.DrawLine(destroyAnyway[i-1].transform.position, item.transform.position );
                                //line.SetPosition(i, item.transform.position);
                                //line.SetPosition(i, item.transform.position+Vector3.one*0.01f);
                                //i++;
                            }

                            if (currentLevel <= hintLevelMax && LevelsMap._instance.GetLastestReachedLevel() <= hintLevelMax)
                            {
                                if (destroyAnyway.Count > 0 && destroyAnyway[0].currentType != ItemsTypes.SQUARE_BOMB && destroyAnyway[0].currentType != ItemsTypes.CROSS_BOMB)
                                {
                                    for (int col = 0; col < maxCols; col++)
                                    {
                                        for (int row = maxRows - 1; row >= 0; row--)
                                        {
                                            Square square = GetSquare(col, row);
                                            if (square != null)
                                            {
                                                if (!square.IsNone())
                                                {
                                                    if (square.item != null)
                                                    {
                                                        square.item.AddInactiveBlocker(itemSelected.color);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

							if (extraCageAddItem < 0)
								extraCageAddItem = 0;
							selectedColor = itemSelected.color;
							//if (destroyAnyway.Count > 0)
							//{
							//    if (destroyAnyway[destroyAnyway.Count - 1] == item) stopSliding = false;
							//}
							if (!LevelManager.THIS.DragBlocked && LevelManager.THIS.gameStatus == GameState.Playing && !stopSliding)
							{
								if (destroyAnyway.Count > 1)
								{
									Vector2 pos1 = new Vector2(destroyAnyway[destroyAnyway.Count - 1].square.col, destroyAnyway[destroyAnyway.Count - 1].square.row);
									Vector2 pos2 = new Vector2(itemSelected.square.col, itemSelected.square.row);
									offset = Vector2.Distance(pos1, pos2);
								}
								if (destroyAnyway.IndexOf(itemSelected) < 0 && offset < 2)
								{         //add item to selection
									if (destroyAnyway.Count > 0)
									{
										Vector2 pos1 = new Vector2(destroyAnyway[destroyAnyway.Count - 1].square.col, destroyAnyway[destroyAnyway.Count - 1].square.row);
										Vector2 pos2 = new Vector2(itemSelected.square.col, itemSelected.square.row);
										offset = Vector2.Distance(pos1, pos2);

										if (offset >= 2)
										{
											offset = 0;
											return;
										}
									}
									destroyAnyway.Add(itemSelected);
									int selectingSoundNum = Mathf.Clamp(destroyAnyway.Count - 1, 0, 9);
									SoundBase.Instance.PlaySound(SoundBase.Instance.selecting[selectingSoundNum]);
									if ((destroyAnyway.Count % (extraItemEvery + extraCageAddItem) == 0) && itemSelected.square.cageHP <= 0)
										itemSelected.SetLight();
									else if ((destroyAnyway.Count % (extraItemEvery + extraCageAddItem) == 0) && itemSelected.square.cageHP > 0)
										extraCageAddItem += 1;
									if (itemSelected.currentType == ItemsTypes.HORIZONTAL_STRIPPED)
										gatheredTypes.Add(itemSelected.currentType);
									else if (itemSelected.currentType == ItemsTypes.VERTICAL_STRIPPED)
										gatheredTypes.Add(itemSelected.currentType);
									HighlightManager.SelectItem(itemSelected);
									//CheckHighlightExtraItem(item);
									itemSelected.square.SetActiveCage(true);
									itemSelected.AwakeItem();


								}
								else if (destroyAnyway.IndexOf(itemSelected) > -1)
								{                  //remove item from selection (step back by finger)
									if ((destroyAnyway.Count % (extraItemEvery + extraCageAddItem) == 0) && itemSelected.square.cageHP > 0)
										extraCageAddItem -= 1;

									if (destroyAnyway.Count > 1)
									{
										if (destroyAnyway[destroyAnyway.Count - 2] == itemSelected)
										{
											if (destroyAnyway[destroyAnyway.Count - 1].currentType == ItemsTypes.HORIZONTAL_STRIPPED && gatheredTypes.Count > 0)
											{
												gatheredTypes.Remove(gatheredTypes[gatheredTypes.Count - 1]);
											}
											else if (destroyAnyway[destroyAnyway.Count - 1].currentType == ItemsTypes.VERTICAL_STRIPPED && gatheredTypes.Count > 0)
											{
												gatheredTypes.Remove(gatheredTypes[gatheredTypes.Count - 1]);
											}

											destroyAnyway[destroyAnyway.Count - 1].SleepItem();



											destroyAnyway[destroyAnyway.Count - 1].square.SetActiveCage(false);
											HighlightManager.DeselectItem(destroyAnyway[destroyAnyway.Count - 1], itemSelected);
											destroyAnyway.Remove(destroyAnyway[destroyAnyway.Count - 1]);
											//CheckHighlightExtraItem(destroyAnyway[destroyAnyway.Count - 1]);
											//highlightedItems.Clear();

										}
									}
								}

								if (itemSelected.currentType == ItemsTypes.SQUARE_BOMB || itemSelected.currentType == ItemsTypes.CROSS_BOMB)
								{
									stopSliding = true;
								}
							}
						}

						//else if(selectedColor > -1 || selectedColor != item.color)
						//{
						//    stopSliding = true;
						//}
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (LoadingCanvasScript.Instance.IsOn())
					return;

                for (int col = 0; col < maxCols; col++)
                {
                    for (int row = maxRows - 1; row >= 0; row--)
                    {
                        Square aSquare = GetSquare(col, row);
                        if (aSquare != null)
                        {
                            if (!aSquare.IsNone())
                            {
                                if (aSquare.item != null)
                                {
                                    aSquare.item.RemoveInactiveBlocker();
                                }
                            }
                        }
                    }
                }

				Collider2D hit = Physics2D.OverlapPoint(mCamera.ScreenToWorldPoint(Input.mousePosition), 1 << LayerMask.NameToLayer("Default"));
				if (hit != null)
				{
					Square square = hit.gameObject.GetComponent<Square>();
					Item item = square.item;
					bool isIngredient = false;

					if (item)
					{ //1.3
						if (item.currentType == ItemsTypes.INGREDIENT)
						{
							isIngredient = true;
						}

						if (!isIngredient && !DragBlocked)
						{
//							bool shouldIgnitePower = false;
							if (LevelManager.THIS.ActivatedBoost.type == BoostType.Bomb || (item.currentType == ItemsTypes.SQUARE_BOMB && destroyAnyway.Count == 1 && destroyAnyway.Contains(item)))// && gatheredTypes.Count == 0))
							{
								if (LevelManager.THIS.ActivatedBoost.type == BoostType.Bomb)
								{
									OnPowerUpUsed();
								}
								LevelManager.THIS.ActivatedBoost.type = BoostType.Bomb;
								SoundBase.Instance.PlaySound(SoundBase.Instance.boostBomb);
								LevelManager.THIS.DragBlocked = true;
								GameObject obj = null;
								if (item.currentType == ItemsTypes.SQUARE_BOMB)
								{
									GameTutorialManager.Instance.CloseTutorial();
									obj = Instantiate(Resources.Load("Prefabs/Effects/item_bomb"), square.transform.position, square.transform.rotation) as GameObject;
								}
								else
								{
									obj = Instantiate(Resources.Load("Prefabs/Effects/bomb"), square.transform.position, square.transform.rotation) as GameObject;
								}
								obj.GetComponent<SpriteRenderer>().sortingOrder = 5;
								BoostAnimation boostAnimation = obj.GetComponent<BoostAnimation>();
								boostAnimation.square = square;
								waitingBoost = LevelManager.THIS.ActivatedBoost;
								LevelManager.THIS.ActivatedBoost = null;
							}
							else if (LevelManager.THIS.ActivatedBoost.type == BoostType.Shovel)
							{
								//SoundBase.Instance.PlaySound(SoundBase.Instance.boostBomb);
								OnPowerUpUsed();
								LevelManager.THIS.DragBlocked = true;
								GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/shovel"), square.transform.position, square.transform.rotation) as GameObject;
								obj.GetComponent<SpriteRenderer>().sortingOrder = 5;
								BoostAnimation boostAnimation = obj.GetComponent<BoostAnimation>();
								boostAnimation.square = square;
//								boostAnimation.EnableSquareAndItemCollider(false);
								waitingBoost = LevelManager.THIS.ActivatedBoost;
								LevelManager.THIS.ActivatedBoost = null;
							}
							else if (LevelManager.THIS.ActivatedBoost.type == BoostType.Energy || (item.currentType == ItemsTypes.CROSS_BOMB && destroyAnyway.Count == 1 && destroyAnyway.Contains(item)))
							{
								if (LevelManager.THIS.ActivatedBoost.type == BoostType.Energy)
								{
									OnPowerUpUsed();
								}

								LevelManager.THIS.ActivatedBoost.type = BoostType.Energy;
								SoundBase.Instance.PlaySound(SoundBase.Instance.boostBomb);
								LevelManager.THIS.DragBlocked = true;
								GameObject obj = Instantiate(Resources.Load("Prefabs/Effects/energy"), square.transform.position, square.transform.rotation) as GameObject;
								obj.GetComponent<SpriteRenderer>().sortingOrder = 5;
								BoostAnimation boostAnimation = obj.GetComponent<BoostAnimation>();
								boostAnimation.square = square;
								waitingBoost = LevelManager.THIS.ActivatedBoost;
								LevelManager.THIS.ActivatedBoost = null;
							}
						}
					}
				}

				selectedColor = -1;
				stopSliding = false;
				offset = 0;
				if (destroyAnyway.Count >= 3)
				{
					LevelManager.THIS.DragBlocked = true;
					FindMatches();
					if (LevelManager.Instance.limitType == LIMIT.MOVES)
					{
						AddLimit(-1);
//						LevelManager.THIS.Limit--;
					}

//					if (OnLimitUpdate != null)
//					{
//						OnLimitUpdate(Limit);
//					}

					LevelManager.THIS.moveID++;
				}
				else
				{
                    if (currentLevel <= hintLevelMax && LevelsMap._instance.GetLastestReachedLevel() <= hintLevelMax)
                    {
                        for (int col = 0; col < maxCols; col++)
                        {
                            for (int row = maxRows - 1; row >= 0; row--)
                            {
                                Square aSquare = GetSquare(col, row);
                                if (aSquare != null)
                                {
                                    if (!aSquare.IsNone())
                                    {
                                        if (aSquare.item != null)
                                        {
                                            aSquare.item.SetSpriteRendererSortingOrder(2);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    line.ResetLineSorting();

					foreach (Item item in destroyAnyway)
					{
						if (item.currentType == ItemsTypes.SQUARE_BOMB || item.currentType == ItemsTypes.CROSS_BOMB)
						{
							if (LevelManager.Instance.limitType == LIMIT.MOVES)
							{
								AddLimit(-1);
//								LevelManager.THIS.Limit--;
//
//								if (OnLimitUpdate != null)
//								{
//									OnLimitUpdate(Limit);
//								}
							}
						}
						item.SleepItem();
						item.square.SetActiveCage(false);
					}
					destroyAnyway.Clear();
					gatheredTypes.Clear();
					//ClearHighlight();
				}
				HighlightManager.StopAndClearAll();
				itemSelected = null;
				//Collider2D hit = Physics2D.OverlapPoint (Camera.main.ScreenToWorldPoint (Input.mousePosition));
				//if (hit != null) {
				//	Item item = hit.gameObject.GetComponent<Item> ();
				//	item.dragThis = false;
				//	item.switchDirection = Vector3.zero;
				//}

			}
		}
	}

	IEnumerator TimeTick ()
	{
		while (true)
		{
			if (gameStatus == GameState.Playing)
			{
				if (LevelManager.Instance.limitType == LIMIT.TIME)
				{
					AddLimit(-1);
//					LevelManager.THIS.Limit--;
//					if (OnLimitUpdate != null)
//					{
//						OnLimitUpdate(Limit);
//					}
					CheckWinLose();
				}
			}
			if (gameStatus == GameState.Map || LevelManager.THIS.Limit <= 0 || gameStatus == GameState.GameOver)
				yield break;

			yield return new WaitForSeconds(1);
		}
	}

	private void GenerateLevel ()
	{
		bool chessColor = false;
		float screenRatio = (float)Screen.height / (float)Screen.width;

		float sqWidth = 1.6f;
		GameField.localScale = Vector3.one;

		if (screenRatio >= 2.0f || maxCols == 8)
		{
			sqWidth = 1.27f;
			GameField.localScale = new Vector3(0.8f, 0.8f, 0.8f);
		}

		float halfSquare = sqWidth / 2;
		Vector3 fieldPos = new Vector3(-maxCols * sqWidth / 2 + halfSquare, maxRows / 1.4f, -10);

        for (int row = 0; row < maxRows; row++)
		{
			if (maxCols % 2 == 0)
				chessColor = !chessColor;
			for (int col = 0; col < maxCols; col++)
			{
				CreateSquare(col, row, chessColor);
				chessColor = !chessColor;
			}

		}

		AnimateField(fieldPos);
	}

	void AnimateField (Vector3 pos)
	{

		float yOffset = 0;
		if (target == Target.COLLECT)
		{
			yOffset = 0.3f;
		}

		if (maxRows >= 9)
		{
			yOffset = -1.2f;
		}

		Animation anim = GameField.GetComponent<Animation>();
		AnimationClip clip = new AnimationClip();
		AnimationCurve curveX = new AnimationCurve(new Keyframe(0, pos.x + 15), new Keyframe(0.7f, pos.x - 0.2f), new Keyframe(0.8f, pos.x));
		AnimationCurve curveY = new AnimationCurve(new Keyframe(0, pos.y + yOffset), new Keyframe(1, pos.y + yOffset));
#if UNITY_5
		clip.legacy = true;
#endif
		clip.SetCurve("", typeof(Transform), "localPosition.x", curveX);
		clip.SetCurve("", typeof(Transform), "localPosition.y", curveY);
		clip.AddEvent(new AnimationEvent() { time = 1, functionName = "EndAnimGamField" });
		anim.AddClip(clip, "appear");
		anim.Play("appear");

		GameField.transform.position = new Vector2(pos.x + 15, pos.y + yOffset);
	}

	void CreateSquare (int col, int row, bool chessColor = false)
	{
		GameObject square = null;
		square = Instantiate(squarePrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;

		if (chessColor)
		{
			square.GetComponent<SpriteRenderer>().sprite = squareSprite1;
		}
		square.transform.SetParent(GameField);
		square.transform.localScale = Vector3.one;
		square.transform.localPosition = firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight);
		squaresArray[row * maxCols + col] = square.GetComponent<Square>();
		square.GetComponent<Square>().row = row;
		square.GetComponent<Square>().col = col;
		square.GetComponent<Square>().type = SquareTypes.EMPTY;
		if (levelSquaresFile[row * maxCols + col].block == SquareTypes.EMPTY)
		{
			CreateObstacles(col, row, square, SquareTypes.NONE);
		}
		else if (levelSquaresFile[row * maxCols + col].block == SquareTypes.NONE)
		{
			square.GetComponent<SpriteRenderer>().enabled = false;
			square.GetComponent<Square>().type = SquareTypes.NONE;

		}
		else if (levelSquaresFile[row * maxCols + col].block == SquareTypes.BLOCK)
		{
			GameObject block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.01f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.BLOCK;
			block.GetComponent<Square>().type = SquareTypes.BLOCK;

			// TargetBlocks++;
			CreateObstacles(col, row, square, SquareTypes.NONE);
		}
		else if (levelSquaresFile[row * maxCols + col].block == SquareTypes.DOUBLEBLOCK)
		{
			GameObject block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.01f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.BLOCK;
			block.GetComponent<Square>().type = SquareTypes.BLOCK;

			//  TargetBlocks++;
			block = Instantiate(blockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.01f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.BLOCK;
			block.GetComponent<Square>().type = SquareTypes.BLOCK;

			block.GetComponent<SpriteRenderer>().sprite = doubleBlock;
			block.GetComponent<SpriteRenderer>().sortingOrder = 1;
			//  TargetBlocks++;
			CreateObstacles(col, row, square, SquareTypes.NONE);
		}

	}

	void GenerateOutline ()
	{
		int row = 0;
		int col = 0;
		for (row = 0; row < maxRows; row++)
		{ //down
			SetOutline(col, row, 0);
		}
		row = maxRows - 1;
		for (col = 0; col < maxCols; col++)
		{ //right
			SetOutline(col, row, 90);
		}
		col = maxCols - 1;
		for (row = maxRows - 1; row >= 0; row--)
		{ //up
			SetOutline(col, row, 180);
		}
		row = 0;
		for (col = maxCols - 1; col >= 0; col--)
		{ //left
			SetOutline(col, row, 270);
		}
		col = 0;
		for (row = 1; row < maxRows - 1; row++)
		{
			for (col = 1; col < maxCols - 1; col++)
			{
				//  if (GetSquare(col, row).type == SquareTypes.NONE)
				SetOutline(col, row, 0);
			}
		}
	}


	void SetOutline (int col, int row, float zRot)
	{
		Square square = GetSquare(col, row, true);
		if (square.type != SquareTypes.NONE)
		{
			if (row == 0 || col == 0 || col == maxCols - 1 || row == maxRows - 1)
			{
				GameObject outline = CreateOutline(square);
				SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
				outline.transform.localRotation = Quaternion.Euler(0, 0, zRot);
				if (zRot == 0)
					outline.transform.localPosition = Vector3.zero + Vector3.left * 0.83f;
				if (zRot == 90)
					outline.transform.localPosition = Vector3.zero + Vector3.down * 0.83f;
				if (zRot == 180)
					outline.transform.localPosition = Vector3.zero + Vector3.right * 0.83f;
				if (zRot == 270)
					outline.transform.localPosition = Vector3.zero + Vector3.up * 0.83f;
				if (row == 0 && col == 0)
				{   //top left
					spr.sprite = outline3;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
					outline.transform.localPosition = Vector3.zero + Vector3.left * 0.01f + Vector3.up * 0.01f;
				}
				if (row == 0 && col == maxCols - 1)
				{   //top right
					spr.sprite = outline3;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
					outline.transform.localPosition = Vector3.zero + Vector3.right * 0.01f + Vector3.up * 0.01f;
				}
				if (row == maxRows - 1 && col == 0)
				{   //bottom left
					spr.sprite = outline3;
					outline.transform.localRotation = Quaternion.Euler(0, 0, -90);
					outline.transform.localPosition = Vector3.zero + Vector3.left * 0.01f + Vector3.down * 0.01f;
				}
				if (row == maxRows - 1 && col == maxCols - 1)
				{   //bottom right
					spr.sprite = outline3;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
					outline.transform.localPosition = Vector3.zero + Vector3.right * 0.01f + Vector3.down * 0.01f;
				}
			}
			else
			{
				//top left
				if (GetSquare(col - 1, row - 1, true).type == SquareTypes.NONE && GetSquare(col, row - 1, true).type == SquareTypes.NONE && GetSquare(col - 1, row, true).type == SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					spr.sprite = outline3;
					outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.up * 0.015f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
				}
				//top right
				if (GetSquare(col + 1, row - 1, true).type == SquareTypes.NONE && GetSquare(col, row - 1, true).type == SquareTypes.NONE && GetSquare(col + 1, row, true).type == SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					spr.sprite = outline3;
					outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.up * 0.015f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
				}
				//bottom left
				if (GetSquare(col - 1, row + 1, true).type == SquareTypes.NONE && GetSquare(col, row + 1, true).type == SquareTypes.NONE && GetSquare(col - 1, row, true).type == SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					spr.sprite = outline3;
					outline.transform.localPosition = Vector3.zero + Vector3.left * 0.015f + Vector3.down * 0.015f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
				}
				//bottom right
				if (GetSquare(col + 1, row + 1, true).type == SquareTypes.NONE && GetSquare(col, row + 1, true).type == SquareTypes.NONE && GetSquare(col + 1, row, true).type == SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					spr.sprite = outline3;
					outline.transform.localPosition = Vector3.zero + Vector3.right * 0.015f + Vector3.down * 0.015f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
				}


			}
		}
		else
		{
			bool corner = false;
			if (GetSquare(col - 1, row, true).type != SquareTypes.NONE && GetSquare(col, row - 1, true).type != SquareTypes.NONE)
			{
				GameObject outline = CreateOutline(square);
				SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
				spr.sprite = outline2;
				outline.transform.localPosition = Vector3.zero;
				outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
				corner = true;
			}
			if (GetSquare(col + 1, row, true).type != SquareTypes.NONE && GetSquare(col, row + 1, true).type != SquareTypes.NONE)
			{
				GameObject outline = CreateOutline(square);
				SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
				spr.sprite = outline2;
				outline.transform.localPosition = Vector3.zero;
				outline.transform.localRotation = Quaternion.Euler(0, 0, 180);
				corner = true;
			}
			if (GetSquare(col + 1, row, true).type != SquareTypes.NONE && GetSquare(col, row - 1, true).type != SquareTypes.NONE)
			{
				GameObject outline = CreateOutline(square);
				SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
				spr.sprite = outline2;
				outline.transform.localPosition = Vector3.zero;
				outline.transform.localRotation = Quaternion.Euler(0, 0, 270);
				corner = true;
			}
			if (GetSquare(col - 1, row, true).type != SquareTypes.NONE && GetSquare(col, row + 1, true).type != SquareTypes.NONE)
			{
				GameObject outline = CreateOutline(square);
				SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
				spr.sprite = outline2;
				outline.transform.localPosition = Vector3.zero;
				outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
				corner = true;
			}

			if (!corner)
			{
				if (GetSquare(col, row - 1, true).type != SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					outline.transform.localPosition = Vector3.zero + Vector3.up * 0.79f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
				}
				if (GetSquare(col, row + 1, true).type != SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					outline.transform.localPosition = Vector3.zero + Vector3.down * 0.79f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 90);
				}
				if (GetSquare(col - 1, row, true).type != SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					outline.transform.localPosition = Vector3.zero + Vector3.left * 0.79f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
				}
				if (GetSquare(col + 1, row, true).type != SquareTypes.NONE)
				{
					GameObject outline = CreateOutline(square);
					SpriteRenderer spr = outline.GetComponent<SpriteRenderer>();
					outline.transform.localPosition = Vector3.zero + Vector3.right * 0.79f;
					outline.transform.localRotation = Quaternion.Euler(0, 0, 0);
				}
			}
		}
		//Vector3 pos = GameField.transform.TransformPoint((Vector3)firstSquarePosition + new Vector3(col * squareWidth - squareWidth / 2, -row * squareHeight, 10));
		//line.SetVertexCount(linePoint + 1);
		//line.SetPosition(linePoint++, pos);

	}

	GameObject CreateOutline (Square square)
	{
		GameObject outline = new GameObject();
		outline.name = "outline";
		outline.transform.SetParent(square.transform);
		outline.transform.localPosition = Vector3.zero;
		outline.transform.localScale = Vector3.one * 2;
		SpriteRenderer spr = outline.AddComponent<SpriteRenderer>();
		spr.sprite = outline1;
		spr.sortingOrder = 1;
		return outline;
	}

	void CreateObstacles (int col, int row, GameObject square, SquareTypes type)
	{
		if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.WIREBLOCK && type == SquareTypes.NONE) || type == SquareTypes.WIREBLOCK)
		{
			GameObject block = Instantiate(wireBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.WIREBLOCK;
			block.GetComponent<SpriteRenderer>().sortingOrder = 3;
			block.GetComponent<Square>().type = SquareTypes.WIREBLOCK;
			square.GetComponent<Square>().SetCage(cageHP);
			//   TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.SOLIDBLOCK && type == SquareTypes.NONE) || type == SquareTypes.SOLIDBLOCK)
		{
			GameObject block = Instantiate(solidBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			block.GetComponent<SpriteRenderer>().sortingOrder = 3;
			square.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;
			block.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;

			//  TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.DOUBLESOLIDBLOCK && type == SquareTypes.NONE) || type == SquareTypes.DOUBLESOLIDBLOCK)
		{
			GameObject block = Instantiate(solidBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			block.GetComponent<SpriteRenderer>().sortingOrder = 3;
			square.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;
			block.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;

			block = Instantiate(solidBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			block.GetComponent<SpriteRenderer>().sprite = doubleSolidBlock;
			block.GetComponent<SpriteRenderer>().sortingOrder = 4;
			square.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;
			block.GetComponent<Square>().type = SquareTypes.SOLIDBLOCK;

			//  TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.UNDESTROYABLE && type == SquareTypes.NONE) || type == SquareTypes.UNDESTROYABLE)
		{
			GameObject block = Instantiate(undesroyableBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.UNDESTROYABLE;
			block.GetComponent<Square>().type = SquareTypes.UNDESTROYABLE;

			//  TargetBlocks++;
		}
		else if ((levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.THRIVING && type == SquareTypes.NONE) || type == SquareTypes.THRIVING)
		{
			GameObject block = Instantiate(thrivingBlockPrefab, firstSquarePosition + new Vector2(col * squareWidth, -row * squareHeight), Quaternion.identity) as GameObject;
			block.transform.SetParent(square.transform);
			block.transform.localPosition = new Vector3(0, 0, -0.5f);
			block.transform.localScale = new Vector3(1.0f, 1.0f, block.transform.localScale.z);
			block.GetComponent<SpriteRenderer>().sortingOrder = 3;
			if (square.GetComponent<Square>().item != null)
				Destroy(square.GetComponent<Square>().item.gameObject);
			square.GetComponent<Square>().block.Add(block);
			square.GetComponent<Square>().type = SquareTypes.THRIVING;
			block.GetComponent<Square>().type = SquareTypes.THRIVING;

			//   TargetBlocks++;
		}

	}

	void GenerateNewItems (bool falling = true)
	{
        Dictionary<int, int> colRowDict = new Dictionary<int, int>();
        for (int col = 0; col < maxCols; col++)
        {
            int maxRow = 0;
            for (int row = maxRows - 1; row >= 0; row--)
            {
                if (GetSquare(col, row) != null)
                {
                    if (!GetSquare(col, row).IsNone() && GetSquare(col, row).CanGoInto() && GetSquare(col, row).item == null)
                    {
                        if ((GetSquare(col, row).item == null && !GetSquare(col, row).IsHaveSolidAbove()) || !falling)
                        {
                            if (maxRow < row)
                            {
                                maxRow = row;
                            }
                            colRowDict[col] = maxRow;
                        }
                    }
                }
            }
        }
		for (int col = 0; col < maxCols; col++)
		{
			for (int row = maxRows - 1; row >= 0; row--)
			{
				if (GetSquare(col, row) != null)
				{
					if (!GetSquare(col, row).IsNone() && GetSquare(col, row).CanGoInto() && GetSquare(col, row).item == null)
					{
						if ((GetSquare(col, row).item == null && !GetSquare(col, row).IsHaveSolidAbove()) || !falling)
						{
                            GetSquare(col, row).GenItem(falling, colRowDict[col]);
						}
					}
				}
			}
		}

	}

	public void NoMatches ()
	{
		StartCoroutine(NoMatchesCor());
	}

	IEnumerator NoMatchesCor ()
	{
		if (gameStatus == GameState.Playing)
		{
			SoundBase.Instance.PlaySound(SoundBase.Instance.noMatch);

			GameObject noMoreMatches = GameObject.Find("Level/Canvas").transform.Find("NoMoreMatches").gameObject;//1.4.5
			noMoreMatches.SetActive(true);
			noMoreMatches.GetComponent<Animator>().Play("floating_word");
			gameStatus = GameState.RegenLevel;
			yield return new WaitForSeconds(1);
			ReGenLevel();
		}
	}

	List<int> bombTimers = new List<int>();
	//1.3

	public void ReGenLevel ()
	{
		itemsHided = false;
		DragBlocked = true;
		if (gameStatus != GameState.Playing && gameStatus != GameState.RegenLevel)
			DestroyItems();
		else if (gameStatus == GameState.RegenLevel)
			DestroyItems(true);
		GenerateNewItems(false);

		StartCoroutine(InitBombs());

		DragBlocked = false;
		gameStatus = GameState.Playing;
		OnLevelLoaded();
		// StartCoroutine(RegenMatches());
	}

	IEnumerator RegenMatches (bool onlyFalling = false)
	{
		if (gameStatus == GameState.RegenLevel)
		{
			//while (!itemsHided)
			//{
			yield return new WaitForSeconds(0.5f);
			//}
		}
		if (!onlyFalling)
			GenerateNewItems(false);
		else
			LevelManager.THIS.onlyFalling = true;
		//   yield return new WaitForSeconds(1f);
		yield return new WaitForFixedUpdate();

		List<List<Item>> combs = GetMatches();
		//while (!matchesGot)
		//{
		//    yield return new WaitForFixedUpdate();

		//}
		//combs = newCombines;
		//matchesGot = false;
		do
		{
			foreach (List<Item> comb in combs)
			{
				int colorOffset = 0;
				foreach (Item item in comb)
				{
					item.GenColor(item.color + colorOffset);
					colorOffset++;
				}
			}
			combs = GetMatches();
			//while (!matchesGot)
			//{
			//    yield return new WaitForFixedUpdate();

			//}
			//combs = newCombines;
			//matchesGot = false;

			//     yield return new WaitForFixedUpdate();
		}
		while (combs.Count > 0);
		yield return new WaitForFixedUpdate();
		SetPreBoosts();
		if (!onlyFalling)
			DragBlocked = false;
		LevelManager.THIS.onlyFalling = false;

		if (LevelManager.THIS.target == Target.BOMBS)
		{
			StartCoroutine(InitBombs());
		}
		if (gameStatus == GameState.RegenLevel)
			gameStatus = GameState.Playing;
		//StartCoroutine(CheckFallingAtStart());

	}

	void SetPreBoosts ()
	{   //activate boosts from map
		bool NoBoosts = true;
		if (BoostColorfullBomb > 0)
		{
			InitScript.Instance.SpendBoost(BoostType.Colorful_bomb);
			GameObject colorMix = Instantiate(Resources.Load("Prefabs/Effects/colorful_mix")) as GameObject;
			colorMix.transform.position = Vector3.zero + Vector3.up * -5f;
			BoostColorfullBomb = 0;
			NoBoosts = false;
		}
		if (BoostStriped > 0)
		{
			InitScript.Instance.SpendBoost(BoostType.Stripes);
			foreach (Item item in GetRandomItems(BoostStriped))
			{
				item.nextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
				item.ChangeType();
			}
			BoostStriped = 0;
			NoBoosts = false;
			gameStatus = GameState.Playing;
		}
		if (NoBoosts)
			gameStatus = GameState.Playing;
	}



	public List<Item> GetIngredients (int i = -1)
	{
		List<Item> list = new List<Item>();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (i > -1)
			{
				if (item.GetComponent<Item>().currentType == ItemsTypes.INGREDIENT && item.GetComponent<Item>().color == 1000 + i)
				{
					list.Add(item.GetComponent<Item>());
				}
			}
			else
			{
				if (item.GetComponent<Item>().currentType == ItemsTypes.INGREDIENT)
				{
					list.Add(item.GetComponent<Item>());
				}
			}
		}

		return list;
	}


	public void DestroyItems (bool withoutEffects = false)
	{

		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item != null)
			{
				if (item.GetComponent<Item>().currentType != ItemsTypes.INGREDIENT)
				{
					if (!withoutEffects)
						item.GetComponent<Item>().DestroyItem();
					else
						item.GetComponent<Item>().SmoothDestroy();
				}
			}
		}

	}

	public IEnumerator FindMatchDelay ()
	{
		yield return new WaitForSeconds(0.2f);
		LevelManager.THIS.FindMatches();

	}

	public void FindMatches ()
	{
		StartCoroutine(FallingDown());
	}

	public List<List<Item>> GetMatches (FindSeparating separating = FindSeparating.NONE, int matches = 3)
	{
		newCombines = new List<List<Item>>();
		//       List<Item> countedSquares = new List<Item>();
		countedSquares = new Hashtable();
		countedSquares.Clear();
		for (int col = 0; col < maxCols; col++)
		{
			for (int row = 0; row < maxRows; row++)
			{
				if (GetSquare(col, row) != null)
				{
					if (!countedSquares.ContainsValue(GetSquare(col, row).item))
					{
						List<Item> newCombine = GetSquare(col, row).FindMatchesAround(separating, matches, countedSquares);
						if (newCombine.Count >= matches)
							newCombines.Add(newCombine);
					}
				}
			}
		}
		//print("global " + countedSquares.Count);
		//  Debug.Break();
		return newCombines;
	}

	IEnumerator GetMatchesCor (FindSeparating separating = FindSeparating.NONE, int matches = 3, bool Smooth = true)
	{
		Hashtable countedSquares = new Hashtable();
		for (int col = 0; col < maxCols; col++)
		{
			//if (Smooth)
			//                    yield return new WaitForFixedUpdate();
			for (int row = 0; row < maxRows; row++)
			{

				if (GetSquare(col, row) != null)
				{
					if (!countedSquares.ContainsValue(GetSquare(col, row).item))
					{
						List<Item> newCombine = GetSquare(col, row).FindMatchesAround(separating, matches, countedSquares);
						if (newCombine.Count >= matches)
							newCombines.Add(newCombine);
					}
				}
			}
		}
		matchesGot = true;
		yield return new WaitForFixedUpdate();

	}

	IEnumerator CheckFallingAtStart ()
	{
		yield return new WaitForSeconds(0.5f);
		while (!IsAllItemsFallDown())
		{
			yield return new WaitForSeconds(0.1f);
		}
		FindMatches();
	}

	public bool CheckExtraPackage (List<List<Item>> rowItems)
	{
		foreach (List<Item> items in rowItems)
		{
			foreach (Item item in items)
			{
				if (item.square.FindMatchesAround(FindSeparating.VERTICAL).Count > 2)
				{
					if (LevelManager.THIS.lastDraggedItem == null)
						LevelManager.THIS.lastDraggedItem = item;
					return true;
				}
			}
		}
		return false;
	}



	IEnumerator FallingDown ()
	{
		bool throwflower = false;
		extraCageAddItem = 0;
		bool nearEmptySquareDetected = false;
		int combo = 0;
		// AI.THIS.allowShowTip = false;
		List<Item> it = GetItems();
		for (int i = 0; i < it.Count; i++)
		{
			Item item = it[i];
			if (item != null)
			{
				//AI.THIS.StopAllCoroutines();
				item.anim.StopPlayback();
			}
		}
		while (true)
		{

			//find matches
			yield return new WaitForSeconds(0.1f);

			combinedItems.Clear();
			//combinedItems = GetMatches();
			//StartCoroutine(GetMatchesCor());
			//while (!matchesGot)
			//    yield return new WaitForFixedUpdate();
			//combinedItems = newCombines;
			//matchesGot = false;

			//if (LevelManager.THIS.CheckExtraPackage(GetMatches(FindSeparating.HORIZONTAL)))
			//{
			//    LevelManager.THIS.lastDraggedItem.nextType = ItemsTypes.PACKAGE;
			//}
			//if (combinedItems.Count > 0)
			//    combo++;
			combo = destroyAnyway.Count;
			foreach (List<Item> desrtoyItems in combinedItems)
			{
				if (lastDraggedItem == null)
				{
					if (desrtoyItems.Count == 4)
					{
						if (lastDraggedItem == null)
							lastDraggedItem = desrtoyItems[UnityEngine.Random.Range(0, desrtoyItems.Count)];
						lastDraggedItem.nextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
						//lastDraggedItem.ChangeType();
					}
					if (desrtoyItems.Count >= 5)
					{
						if (lastDraggedItem == null)
							lastDraggedItem = desrtoyItems[UnityEngine.Random.Range(0, desrtoyItems.Count)];
						lastDraggedItem.nextType = ItemsTypes.CHOCOBOMB;
						//lastDraggedItem.ChangeType();
					}

				}
			}

			if (destroyAnyway.Count >= extraItemEvery)
			{
				LevelManager.THIS.nextExtraItems = destroyAnyway.Count / (int)extraItemEvery;
			}
			int destroyArrayCount = destroyAnyway.Count;
			int iCounter = 0;
			int cc = 0;
			foreach (Item item in destroyAnyway)
			{
                line.ResetLineSorting(iCounter);
				iCounter++;
				//  if(item.sprRenderer.enabled)
				if (item.nextType == ItemsTypes.NONE)
				{
					if (item.square.IsCageGoingToBroke())
					{
						if (iCounter == destroyArrayCount)
						{
							DestroyGatheredExtraItems(item);
						}
						if (iCounter % extraItemEvery == 0)
						{
                            if (item == null)
                            {
                                continue;
                            }
							startPosFlowers.Add(item.transform.position);
							List<Item> items = GetRandomItems(1);
							foreach (Item item1 in items)
							{
								LevelManager.THIS.DragBlocked = true;
								throwflower = true;
//                                item1.nextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
								GameObject flowerParticle = GetFlowerFromPool();
								flowerParticle.GetComponent<Flower>().StartFly(item.transform.position, false, cc);
								cc++;
							}
						}
						yield return new WaitForSeconds(0.03f);
						item.DestroyItem(true, "", true);  //destroy items safely
					}
					else
					{
						if (iCounter == destroyArrayCount)
						{
							DestroyGatheredExtraItems(item);
						}

						item.SleepItem();
					}
				}
			}

            if (currentLevel <= hintLevelMax && LevelsMap._instance.GetLastestReachedLevel() <= hintLevelMax)
            {
                for (int col = 0; col < maxCols; col++)
                {
                    for (int row = maxRows - 1; row >= 0; row--)
                    {
                        Square aSquare = GetSquare(col, row);
                        if (aSquare != null)
                        {
                            if (!aSquare.IsNone())
                            {
                                if (aSquare.item != null)
                                {
                                    aSquare.item.SetSpriteRendererSortingOrder(2);
                                }
                            }
                        }
                    }
                }
            }
			//          if (destroyAnyway.Count > 0) PopupScore(scoreForItem * destroyAnyway.Count, destroyAnyway[(int)destroyAnyway.Count / 2].transform.position);
			destroyAnyway.Clear();


			if (lastDraggedItem != null)
			{

				//if (LevelManager.THIS.CheckExtraPackage(GetMatches(FindSeparating.HORIZONTAL)))
				//{
				//    LevelManager.THIS.lastDraggedItem.nextType = ItemsTypes.PACKAGE;
				//}
				if (lastDraggedItem.nextType != ItemsTypes.NONE)
				{
					//lastDraggedItem.ChangeType();
					yield return new WaitForSeconds(0.5f);

				}
				lastDraggedItem = null;
			}

			while (!IsAllDestoyFinished())
			{
				yield return new WaitForSeconds(0.1f);
			}

			//falling down
			for (int i = 0; i < 20; i++)
			{   //just for testing
				for (int col = 0; col < maxCols; col++)
				{
					for (int row = maxRows - 1; row >= 0; row--)
					{   //need to enumerate rows from bottom to top
						if (GetSquare(col, row) != null)
							GetSquare(col, row).FallOut();
					}
				}
				// yield return new WaitForFixedUpdate();
			}
			if (!nearEmptySquareDetected)
			{
				yield return new WaitForSeconds(0.1f);
			}

			CheckIngredient();
			for (int col = 0; col < maxCols; col++)
			{
				for (int row = maxRows - 1; row >= 0; row--)
				{
					if (GetSquare(col, row) != null)
					{
						if (!GetSquare(col, row).IsNone())
						{
							if (GetSquare(col, row).item != null)
							{
								GetSquare(col, row).item.StartFalling();
								//if (row == maxRows - 1 && GetSquare(col, row).item.currentType == ItemsTypes.INGREDIENT)
								//{
								//    destroyAnyway.Add(GetSquare(col, row).item);
								//}
							}
						}
					}
				}
			}
			yield return new WaitForSeconds(0.1f);
			GenerateNewItems();
			// StartCoroutine(RegenMatches(true));
            yield return new WaitForEndOfFrame();//new WaitForSeconds(0.1f);
			while (!IsAllItemsFallDown())
			{
                yield return new WaitForSeconds(0.1f);
			}

			//detect near empty squares to fall into
			nearEmptySquareDetected = false;

			for (int col = 0; col < maxCols; col++)
			{
				for (int row = maxRows - 1; row >= 0; row--)
				{
					if (GetSquare(col, row) != null)
					{
						if (!GetSquare(col, row).IsNone())
						{
							if (GetSquare(col, row).item != null)
							{
								if (GetSquare(col, row).item.GetNearEmptySquares())
									nearEmptySquareDetected = true;

							}
						}
					}
					// if (nearEmptySquareDetected) break;
				}
				//   if (nearEmptySquareDetected) break;
			}
			//StartCoroutine(GetMatchesCor());
			//while (!matchesGot)
			//    yield return new WaitForFixedUpdate();
			//matchesGot = false;
			//CheckIngredient();
			while (!IsAllItemsFallDown())
			{//1.3.2
                yield return new WaitForSeconds(0.1f);
			}

			if (destroyAnyway.Count > 0)
				nearEmptySquareDetected = true;
			if (!nearEmptySquareDetected)
				break;
		}

		List<Item> item_ = GetItems();
		for (int i = 0; i < it.Count; i++)
		{
			if (item_.Count > i)
			{
				Item item1 = item_[i];
				if (item1 != null)
				{
					if (item1 != item1.square.item)
					{
						Destroy(item1.gameObject);
					}
				}
			}
		}

		//thrive thriving blocks
		if (!thrivingBlockDestroyed)
		{
			bool thrivingBlockSelected = false;
			for (int col = 0; col < maxCols; col++)
			{
				if (thrivingBlockSelected)
					break;
				for (int row = maxRows - 1; row >= 0; row--)
				{
					if (thrivingBlockSelected)
						break;
					if (GetSquare(col, row) != null)
					{
						if (GetSquare(col, row).type == SquareTypes.THRIVING)
						{
							List<Square> sqList = GetSquaresAround(GetSquare(col, row));

							foreach (Square sq in sqList)
							{
								if (sq.CanFallInto() && UnityEngine.Random.Range(0, 5) == 0 && sq.type == SquareTypes.EMPTY)
								{
									if (sq.item != null)
									{//1.3
										if (sq.item.currentType == ItemsTypes.NONE)
										{//1.3
											//GetSquare(col, row).GenThriveBlock(sq);
											CreateObstacles(sq.col, sq.row, sq.gameObject, SquareTypes.THRIVING);

											thrivingBlockSelected = true;
											break;
										}
									}
								}
							}
						}
					}
				}
			}
		}
		thrivingBlockDestroyed = false;

		if (gameStatus == GameState.Playing && !ingredientFly)
			LevelManager.THIS.CheckWinLose();

		if (gratzWord == null)
		{
			gratzWord = Instantiate(gratzWordPrefabs, new Vector3(1000.0f, 0.0f, 0.0f), Quaternion.identity, Level.transform.Find("Canvas").transform).GetComponent<GratzWord>();
			gratzWord.transform.localScale = Vector3.one;
		}

		if (combo > 11 && gameStatus == GameState.Playing)
		{
			gratzWord.SetupGartz(GratzType.GratzLarge);
//			gratzWords[2].SetActive(true);
		}
		else if (combo > 8 && gameStatus == GameState.Playing)
		{
			gratzWord.SetupGartz(GratzType.GratzMedium);
//			gratzWords[1].SetActive(true);
		}
		else if (combo > 5 && gameStatus == GameState.Playing)
		{
			gratzWord.SetupGartz(GratzType.GratzSmall);
//			gratzWords[0].SetActive(true);
		}

		combo = 0;

		//if (nextExtraItems > 0)
		//{
		//    List<Item> items = GetRandomItems(nextExtraItems);
		//    int cc = 0;
		//    foreach (Item item in items)
		//    {
		//        item.nextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
		//        GameObject flowerParticle = GetFlowerFromPool();
		//        flowerParticle.GetComponent<Flower>().StartFly(startPosFlowers[cc], item.transform);
		//        item.ChangeType();
		//        cc++;
		//    }
		//}
		nextExtraItems = 0;

		gatheredTypes.Clear();
		startPosFlowers.Clear();
		DragBlocked = false;

		yield return new WaitForEndOfFrame();
		//if (throwflower)
		//    DragBlocked = true;
		GameField.BroadcastMessage("BombTick");

		List<Item> itemss = GetItems();
		bombTimers.Clear();
		foreach (Item item in itemss)
		{
			if (item.currentType == ItemsTypes.BOMB)
				bombTimers.Add(item.bombTimer);
		}

//		if (gameStatus == GameState.Playing)
//			StartCoroutine(TipsManager.THIS.CheckPossibleCombines());

	}

	public List<Item> highlightedItems;
	public bool testByPlay;
	public CollectStars starsTargetCount;
	public int extraCageAddItem;

	public void CheckHighlightExtraItem (Item item)
	{
		//  if (gatheredTypes.Count >=1)
		ClearHighlight(true);
		foreach (Item _item in destroyAnyway)
		{
			if (_item.currentType == ItemsTypes.HORIZONTAL_STRIPPED || _item.currentType == ItemsTypes.VERTICAL_STRIPPED)
				highlightedItems.Add(_item);

		}

		if (gatheredTypes.Count > 1)
		{
			item.SetHighlight(ItemsTypes.HORIZONTAL_STRIPPED);
			item.SetHighlight(ItemsTypes.VERTICAL_STRIPPED);
		}
		foreach (ItemsTypes itemType in gatheredTypes)
		{

			if (itemType == ItemsTypes.HORIZONTAL_STRIPPED)
				item.SetHighlight(ItemsTypes.HORIZONTAL_STRIPPED);
			else
				item.SetHighlight(ItemsTypes.VERTICAL_STRIPPED);
		}

	}

	public void ClearHighlight (bool boost = false)
	{

		if (!boost)
			return;
		highlightedItems.Clear();
		for (int col = 0; col < maxCols; col++)
		{
			for (int row = 0; row < maxRows; row++)
			{
				if (GetSquare(col, row) != null)
				{
					GetSquare(col, row).SetActiveCage(false);
					GetSquare(col, row).HighLight(false);
				}
			}
		}
	}

	public void DestroyDoubleBomb (int col)
	{
		StartCoroutine(DestroyDoubleBombCor(col));
		StartCoroutine(DestroyDoubleBombCorBack(col));
	}

	IEnumerator DestroyDoubleBombCor (int col)
	{
		for (int i = col; i < maxCols; i++)
		{
			List<Item> list = GetColumn(i);
			foreach (Item item in list)
			{
				if (item != null)
					item.DestroyItem(true, "", true);
			}
			yield return new WaitForSeconds(0.3f);
			//GenerateNewItems();
			//yield return new WaitForSeconds(0.3f);
		}
		if (col <= maxCols - col - 1)
			FindMatches();
	}

	IEnumerator DestroyDoubleBombCorBack (int col)
	{
		for (int i = col - 1; i >= 0; i--)
		{
			List<Item> list = GetColumn(i);
			foreach (Item item in list)
			{
				if (item != null)
					item.DestroyItem(true, "", true);
			}
			yield return new WaitForSeconds(0.3f);
			//GenerateNewItems();
			//yield return new WaitForSeconds(0.3f);
		}
		if (col > maxCols - col - 1)
			FindMatches();
	}


	public Square GetSquare (int col, int row, bool safe = false)
	{
		if (!safe)
		{
			if (row >= maxRows || col >= maxCols)
				return null;
			return squaresArray[row * maxCols + col];
		}
		else
		{
			row = Mathf.Clamp(row, 0, maxRows - 1);
			col = Mathf.Clamp(col, 0, maxCols - 1);
			return squaresArray[row * maxCols + col];
		}
	}

	bool IsAllDestoyFinished ()
	{
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			Item itemComponent = item.GetComponent<Item>();
			if (itemComponent == null)
			{
				return false;
			}
			if (itemComponent.destroying && !itemComponent.animationFinished)
				return false;
		}
		return true;
	}


	bool IsAllItemsFallDown ()
	{
		if (gameStatus == GameState.PreWinAnimations)
			return true;
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			Item itemComponent = item.GetComponent<Item>();
			if (itemComponent == null)
			{
				return false;
			}
			if (itemComponent.falling)
				return false;
		}
		return true;
	}

	public Vector2 GetPosition (Item item)
	{
		return GetPosition(item.square);
	}

	public Vector2 GetPosition (Square square)
	{
		return new Vector2(square.col, square.row);
	}

	public List<Item> GetRow (int row)
	{
		List<Item> itemsList = new List<Item>();
		for (int col = 0; col < maxCols; col++)
		{
			itemsList.Add(GetSquare(col, row, true).item);
		}
		return itemsList;
	}

	public List<Square> GetRowSquare (int row)
	{
		List<Square> itemsList = new List<Square>();
		for (int col = 0; col < maxCols; col++)
		{
			itemsList.Add(GetSquare(col, row, true));
		}
		return itemsList;
	}


	public List<Item> GetColumn (int col)
	{
		List<Item> itemsList = new List<Item>();
		for (int row = 0; row < maxRows; row++)
		{
			itemsList.Add(GetSquare(col, row, true).item);
		}
		return itemsList;
	}

	public List<Square> GetColumnSquare (int col)
	{
		List<Square> itemsList = new List<Square>();
		for (int row = 0; row < maxRows; row++)
		{
			itemsList.Add(GetSquare(col, row, true));
		}
		return itemsList;
	}


	public List<Square> GetColumnSquaresObstacles (int col)
	{
		List<Square> itemsList = new List<Square>();
		for (int row = 0; row < maxRows; row++)
		{
			if (GetSquare(col, row, true).IsHaveDestroybleObstacle())
				itemsList.Add(GetSquare(col, row, true));
			if (GetSquare(col, row, true).block.Count > 0)
				itemsList.Add(GetSquare(col, row, true));

		}
		return itemsList;
	}

	public List<Square> GetRowSquaresObstacles (int row)
	{
		List<Square> itemsList = new List<Square>();
		for (int col = 0; col < maxCols; col++)
		{
			if (GetSquare(col, row, true).IsHaveDestroybleObstacle())
				itemsList.Add(GetSquare(col, row, true));
			if (GetSquare(col, row, true).block.Count > 0)
				itemsList.Add(GetSquare(col, row, true));
		}
		return itemsList;
	}

	public List<Item> GetRandomItems (int count)
	{
//		Debug.LogError(":::: " + count);
		List<Item> list = new List<Item>();
		List<Item> list2 = new List<Item>();
		if (count <= 0)
			return list2;
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		if (items.Length < count)
			count = items.Length;
		
		foreach (GameObject item in items)
		{
			Item itemScript = item.GetComponent<Item>();
			if (!itemScript.destroying && itemScript.currentType == ItemsTypes.NONE
				&& itemScript.nextType == ItemsTypes.NONE
				&& itemScript.square.type != SquareTypes.WIREBLOCK)
			{
				list.Add(itemScript);
			}
		}

		while (list2.Count < count && list.Count > 0)
		{
			Item newItem = list[UnityEngine.Random.Range(0, list.Count)];
			if (newItem != null && list2.IndexOf(newItem) < 0)
			{
				list2.Add(newItem);
			}
		}

		return list2;
	}

	List<Item> GetAllExtraItems ()
	{
		List<Item> list = new List<Item>();
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			Item itemScript = item.GetComponent<Item>();
			if (itemScript.currentType != ItemsTypes.NONE)
			{
				list.Add(itemScript);
			}
			if (gameStatus == GameState.PreWinAnimations)
			{
				if (itemScript.nextType != ItemsTypes.NONE)
				{
					itemScript.currentType = itemScript.nextType;
					itemScript.nextType = ItemsTypes.NONE;
					list.Add(itemScript);
				}
			}
		}

		return list;
	}


	public List<Item> GetItemsAround (Square square)
	{
		int col = square.col;
		int row = square.row;
		List<Item> itemsList = new List<Item>();
		for (int r = row - 1; r <= row + 1; r++)
		{
			for (int c = col - 1; c <= col + 1; c++)
			{
				itemsList.Add(GetSquare(c, r, true).item);
			}
		}
		return itemsList;
	}

	public List<Square> GetSquaresAround (Square square)
	{
		int col = square.col;
		int row = square.row;
		List<Square> itemsList = new List<Square>();
		for (int r = row - 1; r <= row + 1; r++)
		{
			for (int c = col - 1; c <= col + 1; c++)
			{
				itemsList.Add(GetSquare(c, r, true));
			}
		}
		return itemsList;
	}

	public List<Item> GetItemsCross (Square square, List<Item> exceptList = null, int COLOR = -1)
	{
		if (exceptList == null)
			exceptList = new List<Item>();
		int c = square.col;
		int r = square.row;
		List<Item> itemsList = new List<Item>();
		Item item = null;
		item = GetSquare(c - 1, r, true).item;
		if (exceptList.IndexOf(item) <= -1)
		{
			if (item.color == COLOR || COLOR == -1)
				itemsList.Add(item);
		}
		item = GetSquare(c + 1, r, true).item;
		if (exceptList.IndexOf(item) <= -1)
		{
			if (item.color == COLOR || COLOR == -1)
				itemsList.Add(item);
		}
		item = GetSquare(c, r - 1, true).item;
		if (exceptList.IndexOf(item) <= -1)
		{
			if (item.color == COLOR || COLOR == -1)
				itemsList.Add(item);
		}
		item = GetSquare(c, r + 1, true).item;
		if (exceptList.IndexOf(item) <= -1)
		{
			if (item.color == COLOR || COLOR == -1)
				itemsList.Add(item);
		}

		return itemsList;
	}

	public List<Item> GetItems ()
	{
		List<Item> itemsList = new List<Item>();
		for (int row = 0; row < maxRows; row++)
		{
			for (int col = 0; col < maxCols; col++)
			{
				if (GetSquare(col, row) != null)
				{
					if (GetSquare(col, row).item != null)
					{
						itemsList.Add(GetSquare(col, row, true).item);
					}
				}
			}
		}
		return itemsList;
	}

	public List<Square> GetSquares ()
	{
		List<Square> itemsList = new List<Square>();
		for (int row = 0; row < maxRows; row++)
		{
			for (int col = 0; col < maxCols; col++)
			{
				if (GetSquare(col, row) != null)
				{
					itemsList.Add(GetSquare(col, row));
				}
			}
		}
		return itemsList;
	}


	public void SetTypeByColor (int p, ItemsTypes nextType)
	{
		GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
		foreach (GameObject item in items)
		{
			if (item.GetComponent<Item>().color == p)
			{
				if (nextType == ItemsTypes.HORIZONTAL_STRIPPED || nextType == ItemsTypes.VERTICAL_STRIPPED)
					item.GetComponent<Item>().nextType = (ItemsTypes)UnityEngine.Random.Range(1, 3);
				else
					item.GetComponent<Item>().nextType = nextType;
				
				Debug.LogError("-- " + item.GetComponent<Item>().nextType);
				item.GetComponent<Item>().ChangeType();
				if (nextType == ItemsTypes.NONE)
					destroyAnyway.Add(item.GetComponent<Item>());
			}
		}
	}

	public void SetColorToRandomItems ()
	{
		StartCoroutine(SetColorToRandomItemscCor());

	}

	IEnumerator SetColorToRandomItemscCor ()
	{
		int p = UnityEngine.Random.Range(0, colorLimit);
		List<Item> items = GetRandomItems((GameObject.FindGameObjectsWithTag("Item").Length) / 3);
		foreach (Item item in items)
		{

			yield return new WaitForSeconds(0.01f);
			item.SetColor(p);
			item.anim.SetTrigger("appear");

		}

	}

	public void CheckIngredient ()
	{
		int row = maxRows;
		List<Square> sqList = GetBottomRow();
		foreach (Square sq in sqList)
		{
			if (sq.item != null)
			{
				if (sq.item.currentType == ItemsTypes.INGREDIENT)
				{
					destroyAnyway.Add(sq.item);
				}
			}
		}
	}

	public List<Square> GetBottomRow ()
	{
		List<Square> itemsList = new List<Square>();
		int listCounter = 0;
		for (int col = 0; col < maxCols; col++)
		{
			for (int row = maxRows - 1; row >= 0; row--)
			{
				Square square = GetSquare(col, row, true);
				if (square.type != SquareTypes.NONE)
				{
					itemsList.Add(square);
					listCounter++;
					break;
				}
			}
		}
		return itemsList;
	}

	IEnumerator StartIdleCor ()
	{
		for (int col = 0; col < maxCols; col++)
		{
			for (int row = 0; row < maxRows; row++)
			{
				// GetSquare(col, row, true).item.anim.SetBool("stop", false);
				if (GetSquare(col, row, true).item != null)
					GetSquare(col, row, true).item.StartIdleAnim();
			}

		}

		yield return new WaitForFixedUpdate();
	}

	public void StrippedShow (GameObject obj, bool horrizontal)
	{
		GameObject effect = Instantiate(stripesEffect, obj.transform.position, Quaternion.identity) as GameObject;
		if (!horrizontal)
			effect.transform.Rotate(Vector3.back, 90);
		Destroy(effect, 1);
	}

	public void PopupScore (int value, Vector3 pos, int color)
	{
		Score += value;
		if (OnScoreUpdate != null)
		{
			OnScoreUpdate(Score);
		}

		UpdateBar();
		CheckStars();
		if (showPopupScores)
		{
			Transform parent = GameObject.Find("CanvasScore").transform;
			GameObject poptxt = Instantiate(popupScore, pos, Quaternion.identity) as GameObject;
			poptxt.transform.GetComponentInChildren<Text>().text = "" + value;
			if (color <= scoresColors.Length - 1)
			{
				poptxt.transform.GetComponentInChildren<Text>().color = scoresColors[color];
				poptxt.transform.GetComponentInChildren<Outline>().effectColor = scoresColorsOutline[color];
			}
			poptxt.transform.SetParent(parent);
			//   poptxt.transform.position += Vector3.right * 1;
			poptxt.transform.localScale = Vector3.one / 1.5f;
			Destroy(poptxt, 1.4f);
		}
	}

	void UpdateBar ()
	{
		ProgressBarScript.Instance.UpdateDisplay((float)Score * 100f / ((float)star1 / ((star1 * 100f / star3)) * 100f) / 100f);

	}

	void CheckStars ()
	{
		if (Score >= star1 && stars <= 0)
		{
			stars = 1;
		}
		if (Score >= star2 && stars <= 1)
		{
			stars = 2;
		}
		if (Score >= star3 && stars <= 2)
		{
			stars = 3;
		}

		bool shouldUpdateStar = false;
		if (Score >= star1)
		{
			if (!star1Anim.activeSelf)
			{
				SoundBase.Instance.PlaySound(SoundBase.Instance.getStarIngr);
				shouldUpdateStar = true;
				star1Anim.SetActive(true);
			}
		}
		if (Score >= star2)
		{
			if (!star2Anim.activeSelf)
			{
				SoundBase.Instance.PlaySound(SoundBase.Instance.getStarIngr);
				star2Anim.SetActive(true);
				shouldUpdateStar = true;
			}
		}
		if (Score >= star3)
		{
			if (!star3Anim.activeSelf)
			{
				SoundBase.Instance.PlaySound(SoundBase.Instance.getStarIngr);
				star3Anim.SetActive(true);
				shouldUpdateStar = true;
			}
		}

		if (shouldUpdateStar && OnStarUpdate != null)
		{
			OnStarUpdate(stars);
		}
	}

	public LevelInfo LoadDataFromLocal (int currentLevel)
	{
		levelLoaded = false;
		string level = GetDataFromLocal(currentLevel);
		LevelInfo result = ProcessGameDataFromString(level);
		NumIngredients = ingrTarget.Count;
		return result;
	}

	public static string GetDataFromLocal (int currentLevel)
	{
		string result = null;
		//Read data from text file
		TextAsset mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
		if (mapText != null)
		{
//			mapText = Resources.Load("Levels/" + currentLevel) as TextAsset;
			result = mapText.text;
		}

		return result;
	}

	LevelInfo ProcessGameDataFromString (string mapText)
	{
		string[] lines = mapText.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
		ingrTarget = new List<CollectedIngredients>();
		int mapLine = 0;
		foreach (string line in lines)
		{
			//check if line is game mode line
			if (line.StartsWith("MODE"))
			{
				//Replace GM to get mode number, 
				string modeString = line.Replace("MODE", string.Empty).Trim();
				//then parse it to interger
				target = (Target)int.Parse(modeString);
				//Assign game mode
			}
			else if (line.StartsWith("SIZE "))
			{
				string blocksString = line.Replace("SIZE", string.Empty).Trim();
				string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				maxCols = int.Parse(sizes[0]);
				maxRows = int.Parse(sizes[1]);
				squaresArray = new Square[maxCols * maxRows];
				levelSquaresFile = new SquareBlocks[maxRows * maxCols];
				for (int i = 0; i < levelSquaresFile.Length; i++)
				{

					SquareBlocks sqBlocks = new SquareBlocks();
					sqBlocks.block = SquareTypes.EMPTY;
					sqBlocks.obstacle = SquareTypes.NONE;

					levelSquaresFile[i] = sqBlocks;
				}
			}
			else if (line.StartsWith("LIMIT"))
			{
				string blocksString = line.Replace("LIMIT", string.Empty).Trim();
				string[] sizes = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				limitType = (LIMIT)int.Parse(sizes[0]);
				Limit = 0;
				totalLimit = int.Parse(sizes[1]);
				AddLimit(int.Parse(sizes[1]));
//				Limit = int.Parse(sizes[1]);
//				if (OnLimitUpdate != null)
//				{
//					OnLimitUpdate(Limit);
//				}
			}
			else if (line.StartsWith("COLOR LIMIT "))
			{
				string blocksString = line.Replace("COLOR LIMIT", string.Empty).Trim();
				colorLimit = int.Parse(blocksString);
			}

			//check third line to get missions
			else if (line.StartsWith("STARS"))
			{
				string blocksString = line.Replace("STARS", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				star1 = int.Parse(blocksNumbers[0]);
				star2 = int.Parse(blocksNumbers[1]);
				star3 = int.Parse(blocksNumbers[2]);
			}
			else if (line.StartsWith("COLLECT COUNT "))
			{
				string blocksString = line.Replace("COLLECT COUNT", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < blocksNumbers.Length; i++)
				{
					if (InitScript.Instance.collectedIngredients.Count <= i && target == Target.COLLECT)
						break;
					//ingrCountTarget[i] = int.Parse(blocksNumbers[i]);
					if (target == Target.COLLECT)
						ingrTarget.Add(InitScript.Instance.collectedIngredients[i]);
					else
						ingrTarget.Add(new CollectedIngredients());
					ingrTarget[ingrTarget.Count - 1].count = int.Parse(blocksNumbers[i]);
				}
			}
			else if (line.StartsWith("COLLECT ITEMS "))
			{
				string blocksString = line.Replace("COLLECT ITEMS", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < blocksNumbers.Length; i++)
				{
					if (target == Target.COLLECT)
					{
						if (ingrTarget.Count > i)
						{

							CollectedIngredients ingFromList = InitScript.Instance.collectedIngredients[int.Parse(blocksNumbers[i])];
							ingrTarget[i].check = true;
							ingrTarget[i].name = ingFromList.name;
							ingrTarget[i].sprite = ingFromList.sprite;
						}
					}
					else if (target == Target.ITEMS)
					{
						collectItems[i] = (CollectItems)int.Parse(blocksNumbers[i]) + 1;
					}


				}
			}
			else if (line.StartsWith("CAGE "))
			{
				string blocksString = line.Replace("CAGE ", string.Empty).Trim();
				cageHP = int.Parse(blocksString);
			}
			else if (line.StartsWith("BOMBS "))
			{
				Debug.Log("load bomb");
				string blocksString = line.Replace("BOMBS ", string.Empty).Trim();
				string[] blocksNumbers = blocksString.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
				bombsCollect = int.Parse(blocksNumbers[0]);
				bombTimer = int.Parse(blocksNumbers[1]);
			}
			else if (line.StartsWith("GETSTARS "))
			{
				string blocksString = line.Replace("GETSTARS ", string.Empty).Trim();
				starsTargetCount = (CollectStars)int.Parse(blocksString);
			}
			else
			{ //Maps
				//Split lines again to get map numbers
				string[] st = line.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
				for (int i = 0; i < st.Length; i++)
				{
					levelSquaresFile[mapLine * maxCols + i].block = (SquareTypes)int.Parse(st[i][0].ToString());
					levelSquaresFile[mapLine * maxCols + i].obstacle = (SquareTypes)int.Parse(st[i][1].ToString());
				}
				mapLine++;
			}
		}
		TargetBlocks = 0;
		for (int row = 0; row < maxRows; row++)
		{
			for (int col = 0; col < maxCols; col++)
			{
				if (levelSquaresFile[row * maxCols + col].block == SquareTypes.BLOCK)
					TargetBlocks++;
				else if (levelSquaresFile[row * maxCols + col].block == SquareTypes.DOUBLEBLOCK)
					TargetBlocks += 2;
			}
		}
		TargetCages = 0;
		for (int row = 0; row < maxRows; row++)
		{
			for (int col = 0; col < maxCols; col++)
			{
				if (levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.WIREBLOCK)
					TargetCages++;
			}
		}

		//print(TargetBlocks);
		levelLoaded = true;

		LevelInfo result = new LevelInfo();
		result.target = target;
		result.limitType = limitType;
		return result;
	}

	public void DecreaseTargetCage ()
	{
		TargetCages--;
		if (OnTargetCageUpdate != null)
		{
			OnTargetCageUpdate(TargetCages);
		}
	}

	public void AddLimit (int _adds)
	{
		Limit += _adds;
		if (OnLimitUpdate != null)
		{
			OnLimitUpdate(Limit);
		}
	}

	public void ExtraLifeUsed ()
	{
		extraLifeUsed = true;
	}

	void CheckUndestroyableBlockToturial ()
	{
		int undestroyableBlockRow = -1;
		int undestroyableBlockCol = -1;
		for (int row = 0; row < maxRows; row++)
		{
			for (int col = 0; col < maxCols; col++)
			{
				if (levelSquaresFile[row * maxCols + col].obstacle == SquareTypes.UNDESTROYABLE)
				{
					undestroyableBlockRow = row;
					undestroyableBlockCol = col;
					break;
				}
			}
		}

		if (undestroyableBlockRow >= 0 && undestroyableBlockCol >= 0)
		{
			Square square = GetSquare(undestroyableBlockCol, undestroyableBlockRow);
			GameTutorialManager.Instance.SetUpTutorialForLevel(TutorialType.UndestroyableBlock, square.transform);
		}
	}
}

[System.Serializable]
public class GemProduct
{
	public int count;
	public float price;
}