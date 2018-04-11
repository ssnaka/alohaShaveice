using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Counter_ : MonoBehaviour
{
	public int ingrTrackNumber;
	Text txt;
	private float lastTime;
	bool alert;
	public int totalCount;
	TargetGUI parentGUI;
	//	∞
	// Use this for initialization
	void Start ()
	{
		if (transform.parent.GetComponent<TargetGUI>() != null)
			parentGUI = transform.parent.GetComponent<TargetGUI>();
	}

	void OnEnable ()
	{
		if (txt == null)
		{
			txt = GetComponent<Text>();
		}

		lastTime = 0;
		alert = false;
		if (name == "TargetScore" && LevelManager.THIS != null)
		{
			if (LevelManager.THIS.target == Target.SCORE)
				txt.text = "" + LevelManager.THIS.GetScoresOfTargetStars();
		}

		if (name == "Score")
		{
			txt.text = "0";
			LevelManager.Instance.OnScoreUpdate += LevelManager_Instance_OnScoreUpdate;
		}
		else if (name == "CountStar")
		{
			LevelManager.Instance.OnStarUpdate += LevelManager_Instance_OnStarUpdate;
			;
			txt.text = "0/" + (int)LevelManager.THIS.starsTargetCount;
		}
		else if (name == "Lifes")
		{
			InitScript.Instance.OnLifeUpdate += InitScript_Instance_OnLifeUpdate;
			txt.text = "" + InitScript.Instance.GetLife();
		}
		else if (name == "Gems")
		{
			InitScript.Instance.OnGemUpdate += InitScript_Instance_OnGemUpdate;
			txt.text = "" + InitScript.Gems;
		}
		else if (name == "TargetBlocks")
		{
			LevelManager.Instance.OnTargetBlockUpdate += LevelManager_Instance_OnTargetBlockUpdate;
		}
		else if (name == "TargetCages")
		{
			LevelManager.Instance.OnTargetCageUpdate += LevelManager_Instance_OnTargetCageUpdate;

		}
		else if (name == "TargetBombs")
		{
			LevelManager.Instance.OnTargetBombUpdate += LevelManager_Instance_OnTargetBombUpdate;
		}
		else if (name == "CountIngr")
		{
			LevelManager.Instance.OnTargetIngredientUpdate += LevelManager_Instance_OnTargetIngredientUpdate;;
		}
		else if (name == "Limit")
		{
			LevelManager.Instance.OnLimitUpdate += LevelManager_Instance_OnLimitUpdate;
		}
		else if (name == "LabelKeepPlay")
		{
			if (LevelManager.THIS.limitType == LIMIT.MOVES)
				txt.text = "GET + " + LevelManager.THIS.ExtraFailedMoves + " moves";
			else
				txt.text = "GET + " + LevelManager.THIS.ExtraFailedSecs + " secs";
		}
		else if (name == "BestScore")
		{
			txt.text = "Best score:" + PlayerPrefs.GetInt("Score" + PlayerPrefs.GetInt("OpenLevel"));
		}
		else if (name == "Level")
		{
			txt.text = "LEVEL " + PlayerPrefs.GetInt("OpenLevel");
		}
		else if (name == "TargetDescription1")
		{
			if (LevelManager.THIS.target == Target.SCORE)
				txt.text = LevelManager.THIS.targetDiscriptions[6].Replace("%n", "" + LevelManager.THIS.GetScoresOfTargetStars()).Replace("%s", "" + (int)LevelManager.THIS.starsTargetCount);
			else if (LevelManager.THIS.target == Target.BLOCKS)
				txt.text = LevelManager.THIS.targetDiscriptions[1];
			else if (LevelManager.THIS.target == Target.COLLECT)
				txt.text = LevelManager.THIS.targetDiscriptions[2];
			else if (LevelManager.THIS.target == Target.ITEMS)
				txt.text = LevelManager.THIS.targetDiscriptions[3];
			else if (LevelManager.THIS.target == Target.CAGES)
				txt.text = LevelManager.THIS.targetDiscriptions[4];
			else if (LevelManager.THIS.target == Target.BOMBS)
				txt.text = LevelManager.THIS.targetDiscriptions[5];
		}
	}

	void LevelManager_Instance_OnLimitUpdate (int _limit)
	{
		if (LevelManager.Instance.limitType == LIMIT.MOVES)
		{
			txt.text = "" + _limit;
			txt.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-460, -42, 0);
			//txt.transform.localScale = Vector3.one;
			if (_limit <= 5)
			{
				txt.color = Color.red;
				//txt.GetComponent<Outline>().effectColor = new Color(214 / 255, 0, 196 / 255);
				if (!alert)
				{
					alert = true;
					SoundBase.Instance.PlaySound(SoundBase.Instance.alert);
				}

			}
			else
			{
				alert = false;
				txt.color = new Color(214f / 255f, 0, 196f / 255f);
				//txt.GetComponent<Outline>().effectColor = new Color(148f / 255f, 61f / 255f, 95f / 255f);
			}

		}
		else
		{
			int minutes = Mathf.FloorToInt(_limit / 60F);
			int seconds = Mathf.FloorToInt(_limit - minutes * 60);
			txt.text = "" + string.Format("{0:00}:{1:00}", minutes, seconds);
			txt.transform.localScale = Vector3.one * 0.35f;
			txt.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-445, -42, 0);
			if (_limit <= 30 && LevelManager.THIS.gameStatus == GameState.Playing)
			{
				txt.color = Color.red;
				//txt.GetComponent<Outline>().effectColor = Color.white;
				if (lastTime + 30f < Time.time)
				{
					lastTime = Time.time;
					SoundBase.Instance.PlaySound(SoundBase.Instance.timeOut);
				}

			}
			else
			{
				txt.color = new Color(214f / 255f, 0, 196f / 255f);
				//txt.GetComponent<Outline>().effectColor = new Color(148f / 255f, 61f / 255f, 95f / 255f);
			}

		}
	}

	void LevelManager_Instance_OnTargetIngredientUpdate (int _index)
	{
		if (ingrTrackNumber != _index)
		{
			return;
		}
		txt.text = "" + (totalCount - LevelManager.THIS.ingrTarget[_index].count) + "/" + totalCount;
		if (LevelManager.THIS.ingrTarget[_index].count == 0)
			parentGUI.Done();
	}

	void LevelManager_Instance_OnTargetBombUpdate (int _count)
	{
		txt.text = "" + (_count) + "/" + totalCount;
		if (LevelManager.THIS.TargetBombs >= totalCount)
			parentGUI.Done();
	}

	void LevelManager_Instance_OnTargetCageUpdate (int _count)
	{
		txt.text = "" + (totalCount - _count) + "/" + totalCount;
		if (LevelManager.THIS.TargetCages == 0)
			parentGUI.Done();
	}

	void LevelManager_Instance_OnTargetBlockUpdate (int _blockCount)
	{
		txt.text = "" + (totalCount - _blockCount) + "/" + totalCount;
		if (LevelManager.THIS.targetBlocks == 0)
			parentGUI.Done();
		
	}

	void InitScript_Instance_OnGemUpdate (int _gemCount)
	{
		txt.text = "" + _gemCount;
	}

	void InitScript_Instance_OnLifeUpdate (int _lifeCount)
	{
		txt.text = "" + _lifeCount;
	}

	void LevelManager_Instance_OnStarUpdate (int _starCount)
	{
		txt.text = "" + _starCount + "/" + (int)LevelManager.THIS.starsTargetCount;
		if (_starCount == (int)LevelManager.THIS.starsTargetCount)
			parentGUI.Done();
	}

	void LevelManager_Instance_OnScoreUpdate (int _newScore)
	{
		txt.text = "" + LevelManager.Score;
	}

	public void SetUpTotalCount (int _totalCount)
	{
		totalCount = _totalCount;
		if (name == "TargetBlocks")
		{
			txt.text = "" + (totalCount - LevelManager.THIS.targetBlocks) + "/" + totalCount;
		}
		else if (name == "TargetCages")
		{
			txt.text = "" + (totalCount - LevelManager.THIS.TargetCages) + "/" + totalCount;
		}
		else if (name == "TargetBombs")
		{
			txt.text = "" + (LevelManager.THIS.TargetBombs) + "/" + totalCount;
		}
		else if (name == "CountIngr")
		{
			txt.text = "" + (totalCount - LevelManager.THIS.ingrTarget[ingrTrackNumber].count) + "/" + totalCount;
		}
		else if (name == "CountIngrForMenu")
		{
			txt.text = "" + totalCount;
			if (LevelManager.THIS.target == Target.BOMBS)
			{
				txt.text = "" + LevelManager.THIS.bombsCollect; //1.4.5
			}
		}
	}

	public void SetupIngredientTargetIndex (int _index)
	{
		ingrTrackNumber = _index;
	}

	// Update is called once per frame
//	void Update ()
//	{
//		// if (name == "TargetIngr2") {
//		// 	txt.text = "" + LevelManager.THIS.ingrCountTarget [1];
//		// }
//
//	}

	void OnDisable ()
	{
		LevelManager.Instance.OnScoreUpdate -= LevelManager_Instance_OnScoreUpdate;
		LevelManager.Instance.OnStarUpdate -= LevelManager_Instance_OnStarUpdate;
		InitScript.Instance.OnLifeUpdate -= InitScript_Instance_OnLifeUpdate;
		InitScript.Instance.OnGemUpdate -= InitScript_Instance_OnGemUpdate;
		LevelManager.Instance.OnTargetBlockUpdate -= LevelManager_Instance_OnTargetBlockUpdate;
		LevelManager.Instance.OnTargetCageUpdate -= LevelManager_Instance_OnTargetCageUpdate;
		LevelManager.Instance.OnTargetBombUpdate -= LevelManager_Instance_OnTargetBombUpdate;
		LevelManager.Instance.OnTargetIngredientUpdate -= LevelManager_Instance_OnTargetIngredientUpdate;;
	}
}
