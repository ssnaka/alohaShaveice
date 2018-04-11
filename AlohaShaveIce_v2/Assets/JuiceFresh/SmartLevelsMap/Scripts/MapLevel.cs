using UnityEngine;
using System.Collections.Generic;

public class MapLevel : MonoBehaviour {
	[SerializeField]
	GameObject renderGroup;

	private Vector3 _originalScale;
	private bool _isScaled;
	public float OverScale = 1.05f;
	public float ClickScale = 0.95f;

	public int Number;
	public bool IsLocked;
	Transform Lock;
	MapLevelNumber mapLevelNumber;
	public Transform PathPivot;
	public Object LevelScene;
	public string SceneName;

	public int StarsCount;
	[SerializeField]
	List<Vector3> starPositions;
	[SerializeField]
	List<Vector3> starRotations;
	[SerializeField]
	List<Vector3> starScales;
	public Transform StarsHoster;
	[SerializeField]
	GameObject starPrefab;
	List<Transform> stars = new List<Transform>();
	public Transform Star1;
	public Transform Star2;
	public Transform Star3;

	public Transform SolidStarsHoster;
	public Transform SolidStars0;
	public Transform SolidStars1;
	public Transform SolidStars2;
	public Transform SolidStars3;

	GameObject timerImage;
	[SerializeField]
	Vector3 timerImagePosition;
	LIMIT levelLimit;

	Camera aCamera;
	MapCamera mapCamera;

	public void Awake () {
		_originalScale = transform.localScale;
	}

	void MapCamera_OnCameraMove ()
	{
		if (LevelManager.Instance == null)
		{
			return;
		}

		Vector3 screenPoint = aCamera.WorldToViewportPoint(transform.position);
		bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1.1;
		if (onScreen)
		{	
			CreateLock();
			CreateNumber();
			CreateTimer();
			if (!levelShown)
			{
				if (!renderGroup.activeSelf)
				{
					renderGroup.SetActive(true);
				}
				levelShown = true;
				for (int i = 0 ; i < StarsCount; i++)
				{
					CreateStar(i);
				}
			}
		}
		else
		{
			if (renderGroup.activeSelf)
			{
				renderGroup.SetActive(false);
				CleanLevel();
			}
		}
	}

	#region Enable click

	public void OnMouseEnter () {
		if (LevelsMap.GetIsClickEnabled ())
			Scale (OverScale);
	}

	public void OnMouseDown () {
		if (LevelsMap.GetIsClickEnabled ())
			Scale (ClickScale);
	}

	public void OnMouseExit () {
		if (LevelsMap.GetIsClickEnabled ())
			ResetScale ();
	}

	private void Scale (float scaleValue) {
		transform.localScale = _originalScale * scaleValue;
		_isScaled = true;
	}

	void OnEnable ()
	{
		if (aCamera == null)
		{
			aCamera = Camera.main;
			mapCamera = Camera.main.GetComponent<MapCamera>();
		}
		mapCamera.OnCameraMove += MapCamera_OnCameraMove;
		MapCamera_OnCameraMove();
	}

	public void OnDisable () {
		mapCamera.OnCameraMove -= MapCamera_OnCameraMove;
		if (LevelsMap.GetIsClickEnabled ())
			ResetScale ();
		CleanLevel();
	}

	public void OnMouseUpAsButton () {
		if (LevelsMap.GetIsClickEnabled ()) {
			ResetScale ();
			LevelsMap.OnLevelSelected (Number);
		}
	}

	private void ResetScale () {
		if (_isScaled)
			transform.localScale = _originalScale;
	}

	#endregion

	public void UpdateState (int starsCount, bool isLocked) {
		StarsCount = starsCount;
		UpdateStars (starsCount);
		IsLocked = isLocked;
//		Lock.gameObject.SetActive (isLocked);

		if (!IsLocked)
		{
			string mapText = LevelManager.GetDataFromLocal(Number);


			string[] lines = mapText.Split(new string[] { "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
			foreach (string line in lines)
			{
				if (line.StartsWith("LIMIT"))
				{
					string blocksString = line.Replace("LIMIT", string.Empty).Trim();
					string[] sizes = blocksString.Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
					levelLimit = (LIMIT)int.Parse(sizes[0]);
//					if (levelLimit.Equals(LIMIT.TIME))
//					{
//						timerImage.SetActive(true);
//					}
					break;
				}
			}
		}
	}

	public void UpdateStars (int starsCount) 
	{
//		for (int i = 0 ; i < starsCount; i++)
//		{
//			CreateStar(i);
//		}
//		Star1.gameObject.SetActive (starsCount >= 1);
//		Star2.gameObject.SetActive (starsCount >= 2);
//		Star3.gameObject.SetActive (starsCount >= 3);

//		SolidStars0.gameObject.SetActive (starsCount == 0);
//		SolidStars1.gameObject.SetActive (starsCount == 1);
//		SolidStars2.gameObject.SetActive (starsCount == 2);
//		SolidStars3.gameObject.SetActive (starsCount == 3);
	}

	void CreateStar (int _index)
	{
		GameObject starObject = null;
		if (stars.Count <= _index)
		{
			starObject = LevelManager.Instance.GetLevelStarFromPool();
			stars.Add(starObject.transform);
		}
		else
		{
			starObject = stars[_index].gameObject;
		}

		starObject.transform.localScale = starScales[_index];
		starObject.transform.position = transform.position + starPositions[_index];
		starObject.transform.eulerAngles = starRotations[_index];
	}

	void CreateLock ()
	{
		if (!IsLocked)
		{
			levelShown = false;
			CleanLock();
			return;
		}

		if (IsLocked && Lock == null)
		{
			Lock = LevelManager.Instance.GetLevelLockFromPool().transform;
			Lock.localScale = Vector3.one;
			Lock.position = transform.position;
		}
	}

	void CreateNumber ()
	{
		if (IsLocked)
		{
			levelShown = false;
			CleanNumber();
			return;
		}

		if (!IsLocked && mapLevelNumber == null)
		{
			mapLevelNumber = LevelManager.Instance.GetLevelNumberFromPool();
			mapLevelNumber.SetLevel(Number);
			mapLevelNumber.transform.position = transform.position;
		}
	}

	void CreateTimer ()
	{
		if (levelLimit.Equals(LIMIT.TIME) && timerImage == null)
		{
			timerImage = LevelManager.Instance.GetLevelTimerFromPool();
			timerImage.transform.position = transform.position + timerImagePosition;
		}
	}

	public void UpdateStarsType (StarsType starsType) {
		StarsHoster.gameObject.SetActive (starsType == StarsType.Separated);
//		SolidStarsHoster.gameObject.SetActive (starsType == StarsType.Solid);
	}

	bool levelShown = false;
//	void Update ()
//	{
//		Vector3 screenPoint = aCamera.WorldToViewportPoint(transform.position);
//		bool onScreen = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1.1;
//		if (onScreen)
//		{	
//			CreateLock();
//			CreateNumber();
//			CreateTimer();
//			if (!levelShown)
//			{
//				if (!renderGroup.activeSelf)
//				{
//					renderGroup.SetActive(true);
//				}
//				levelShown = true;
//				for (int i = 0 ; i < StarsCount; i++)
//				{
//					CreateStar(i);
//				}
//			}
//		}
//		else
//		{
//			if (renderGroup.activeSelf)
//			{
//				renderGroup.SetActive(false);
//				CleanLevel();
//			}
//		}
//	}

	void CleanLevel ()
	{
		CleanStars();
		CleanLock();
		CleanNumber();
		CleanTimer();
		levelShown = false;
	}

	void CleanStars ()
	{
		for (int i = 0 ; i < stars.Count ; i++)
		{
			if (stars[i] == null)
			{
				continue;
			}
			GameObject star = stars[i].gameObject;
			star.SetActive(false);
		}
		stars.Clear();
	}

	void CleanLock ()
	{
		if (Lock != null)
		{
			Lock.gameObject.SetActive(false);
			Lock = null;
		}
	}

	void CleanNumber ()
	{
		if (mapLevelNumber != null)
		{
			mapLevelNumber.gameObject.SetActive(false);
			mapLevelNumber = null;
		}
	}

	void CleanTimer ()
	{
		if (timerImage != null)
		{
			timerImage.SetActive(false);
			timerImage = null;
		}
	}
}
