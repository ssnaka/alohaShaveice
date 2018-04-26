using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using GameToolkit.Localization;

public class Tutorial : MonoBehaviour {
//    public Sprite[] sprites;
	public GameObject[] tutorials;

	List<RectTransform> loadedTutorials = new List<RectTransform>();
    public Image image;
    int i;

	[SerializeField]
	LocalizedTextBehaviour welcomeText;
	[SerializeField]
	LocalizedTextBehaviour okText;

	[SerializeField]
	LocalizedText welcomeTextAsset;
	[SerializeField]
	LocalizedText playTextAsset;
	[SerializeField]
	LocalizedText okTextAsset;
	[SerializeField]
	LocalizedText skipTextAsset;

	void OnEnable ()
	{
//		Debug.LogError(Localization.Instance.CurrentLanguage);
//		Localization.Instance.CurrentLanguage = SystemLanguage.Korean;
		if (!InitScript.Instance.didTutorialShown)
		{
			welcomeText.LocalizedAsset = welcomeTextAsset;//.text = "WELINA";
			okText.LocalizedAsset = skipTextAsset;//.text = "Skip";
		}
		else
		{
			welcomeText.LocalizedAsset = playTextAsset;//.text = "HOW TO PLAY";
			okText.LocalizedAsset = okTextAsset;//.text = "OK";
		}
	}

	void Start ()
	{
		if (tutorials != null && tutorials.Length > 0)
		{
			foreach (GameObject tutorial in tutorials)
			{
				GameObject go = GameObject.Instantiate(tutorial);
				RectTransform loadedTutorial = go.GetComponent<RectTransform>();
				loadedTutorial.SetParent(image.rectTransform.parent);
				loadedTutorial.transform.localScale = Vector3.one;
				loadedTutorial.anchoredPosition = image.rectTransform.anchoredPosition;
				loadedTutorial.gameObject.SetActive(false);
				loadedTutorials.Add(loadedTutorial);
			}

			loadedTutorials[0].gameObject.SetActive(true);
		}
	}

    public void Next()
    {
		int previousIndex = i;
        i++;
		if (i >= loadedTutorials.Count)
		{
			i = 0;
		}
		SetTutorial(previousIndex);

//		i++;
//		i = Mathf.Clamp(i, 0, sprites.Length-1);
//		SetImage();
    }
    public void Back()
    {
		int previousIndex = i;
        i--;
		if (i < 0)
		{
			i = loadedTutorials.Count-1;
		}
		SetTutorial(previousIndex);

//		i--;
//		i = Mathf.Clamp(i, 0, sprites.Length-1);
//		SetImage();
    }

//    void SetImage()
//    {
//        image.sprite = sprites[i];
//        image.SetNativeSize();
//    }

	void SetTutorial (int _previousIndex)
	{
		loadedTutorials[_previousIndex].gameObject.SetActive(false);
		loadedTutorials[i].gameObject.SetActive(true);
	}
}
