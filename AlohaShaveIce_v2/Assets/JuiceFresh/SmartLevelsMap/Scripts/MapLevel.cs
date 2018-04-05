using UnityEngine;
using System.Collections.Generic;

public class MapLevel : MonoBehaviour {
	private Vector3 _originalScale;
	private bool _isScaled;
	public float OverScale = 1.05f;
	public float ClickScale = 0.95f;

	public int Number;
	public bool IsLocked;
	public Transform Lock;
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

	[SerializeField]
	GameObject timerImage;

	public void Awake () {
		_originalScale = transform.localScale;
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

	public void OnDisable () {
		if (LevelsMap.GetIsClickEnabled ())
			ResetScale ();
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
		Lock.gameObject.SetActive (isLocked);

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
					LIMIT limit = (LIMIT)int.Parse(sizes[0]);
					if (limit.Equals(LIMIT.TIME))
					{
						timerImage.SetActive(true);
					}
					break;
				}
			}
		}
	}

	public void UpdateStars (int starsCount) 
	{
		for (int i = 0 ; i < starsCount; i++)
		{
			CreateStar(i);
		}
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
			starObject = Instantiate<GameObject>(starPrefab, StarsHoster.transform);
			stars.Add(starObject.transform);
		}
		else
		{
			starObject = stars[_index].gameObject;
		}

		starObject.name = starPrefab.name + "_" + (_index + 1).ToString();
		starObject.transform.localScale = starScales[_index];
		starObject.transform.localPosition = starPositions[_index];
		starObject.transform.eulerAngles = starRotations[_index];
	}

	public void UpdateStarsType (StarsType starsType) {
		StarsHoster.gameObject.SetActive (starsType == StarsType.Separated);
//		SolidStarsHoster.gameObject.SetActive (starsType == StarsType.Solid);
	}
}
