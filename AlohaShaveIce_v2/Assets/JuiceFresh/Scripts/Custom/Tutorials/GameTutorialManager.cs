using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

[Prefab("Custom/Tutorial/GameTutorialManager")]
public class GameTutorialManager : Singleton<GameTutorialManager>
{
	//config data
	TutorialData tutorialData;
	//Save data
	TutorialSaveData tutorialSaveData;

	MenuTutorialPanelScript menuTutorialPanelScript;

	void LoadTutorialData ()
	{
		if (tutorialData != null)
		{
			return;
		}

		TextAsset textAsset = Resources.Load<TextAsset>("Custom/Json/TutorialData");
		if (textAsset == null)
		{
			Debug.LogError("Error loading Tutorial Config Data");
		}

		string jsonString = textAsset.text;
		tutorialData = Newtonsoft.Json.JsonConvert.DeserializeObject<TutorialData>(jsonString);
	}


	public void ShowFirstTutorial ()
	{
		LoadTutorialData();

		string tutorialJson = PlayerPrefs.GetString("tutorialSaveData", string.Empty);
		if (tutorialJson.Equals(string.Empty))
		{
			tutorialSaveData = new TutorialSaveData();
			//Generate tutorial data.
			tutorialSaveData.tutorials = new List<SavedTutorial>();
			foreach(TutorialType tutorialType in Enum.GetValues(typeof(TutorialType)))
			{
				SavedTutorial aTutorial = new SavedTutorial();
				aTutorial.type = tutorialType;
				aTutorial.status = false;
				tutorialSaveData.tutorials.Add(aTutorial);
			}

			SaveLocalTutorialData(tutorialSaveData);
		}
		else
		{
			tutorialSaveData = Newtonsoft.Json.JsonConvert.DeserializeObject<TutorialSaveData>(tutorialJson);
			return;
		}

		GameObject.Find("CanvasGlobal").transform.Find("Tutorial").gameObject.SetActive(true);
	}

	ATutorial GetTutorialConfigData (TutorialType _type)
	{
		return tutorialData.tutorials.Find(item => item.type.Equals(_type));
	}

	void SetMenuTutorialView (string _path)
	{
		if (menuTutorialPanelScript != null)
		{
			return;
		}

		GameObject goAsset = Resources.Load<GameObject>(_path);
		GameObject go = Instantiate<GameObject>(goAsset, transform);
		go.transform.localPosition = Vector3.zero;
		go.transform.localScale = Vector3.one;
		menuTutorialPanelScript = go.GetComponent<MenuTutorialPanelScript>();
	}

	public void ShowMenuTutorial (TutorialType _type, RectTransform _pivotTransform)
	{
		
		TutorialType tutorialType = _type;
		bool tutorialShown = GetLocalTutorialStatus(tutorialType);
		if (tutorialShown || (menuTutorialPanelScript != null && menuTutorialPanelScript.gameObject.activeInHierarchy) || !_pivotTransform.gameObject.activeInHierarchy)
		{
			return;
		}

		ATutorial aTutorial = GetTutorialConfigData(tutorialType);
		if (aTutorial == null)
		{
			Debug.LogError("Tutorial cannot be found.");
			return;
		}

		SetMenuTutorialView(aTutorial.prefabName);
		menuTutorialPanelScript.SetUpTutorial(aTutorial, _pivotTransform, GetComponent<Canvas>());
	}

	public void CheckBoostShopTutorial ()
	{
		GameObject boostItemPanel = GameObject.Find("BoostItemsPanel");
		if (boostItemPanel != null && LevelsMap._instance.GetLastestReachedLevel() >= 4)
		{
			ShowMenuTutorial(TutorialType.Buy_Boosts, boostItemPanel.GetComponent<RectTransform>());
		}
	}

	public void SetUpTutorialForLevel (TutorialType _type, Transform _pivotTransform)
	{
		TutorialType tutorialType = _type;
		bool tutorialShown = GetLocalTutorialStatus(tutorialType);
		if (tutorialShown || (menuTutorialPanelScript != null && menuTutorialPanelScript.gameObject.activeInHierarchy) || !_pivotTransform.gameObject.activeInHierarchy)
		{
			return;
		}

		ATutorial aTutorial = GetTutorialConfigData(tutorialType);
		if (aTutorial == null)
		{
			Debug.LogError("Tutorial cannot be found.");
			return;
		}

		SetMenuTutorialView(aTutorial.prefabName);
		menuTutorialPanelScript.SetUpTutorialForLevel(aTutorial, _pivotTransform, GetComponent<Canvas>());
	}

	public void CloseTutorial ()
	{
		if (menuTutorialPanelScript != null && menuTutorialPanelScript.gameObject.activeInHierarchy)
		{
			menuTutorialPanelScript.gameObject.SetActive(false);
		}
	}

	#region data handler
	public void SetLocalTutorialStatus (TutorialType _type)
	{
		SavedTutorial aTutorial = tutorialSaveData.tutorials.Find(item => item.type.Equals(_type));
		if (aTutorial.status)
		{
			return;
		}

		aTutorial.status = true;

		SaveLocalTutorialData(tutorialSaveData);
		NetworkManager.dataManager.UpdateTutorial();
	}

	public bool GetLocalTutorialStatus (TutorialType _type)
	{
		SavedTutorial aTutorial = tutorialSaveData.tutorials.Find(item => item.type.Equals(_type));
		if (aTutorial == null)
		{
			aTutorial = new SavedTutorial();
			aTutorial.type = _type;
			aTutorial.status = false;
			tutorialSaveData.tutorials.Add(aTutorial);
			SaveLocalTutorialData(tutorialSaveData);
		}

		return aTutorial.status;
	}

	

	public void SaveLocalTutorialData (TutorialSaveData _tutorialSaveData)
	{
		string tutorialJson = Newtonsoft.Json.JsonConvert.SerializeObject(_tutorialSaveData);
		PlayerPrefs.SetString("tutorialSaveData", tutorialJson);
		PlayerPrefs.Save();
	}
	#endregion
}
