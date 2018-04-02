using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Prefab("Custom/LoadingCanvas")]
public class LoadingCanvasScript : Singleton<LoadingCanvasScript> {
	
	[SerializeField]
	Text loadingText;

	public void ShowLoading (string _text = "")
	{
		loadingText.text= _text.Equals(string.Empty) ? loadingText.text : _text;
		gameObject.SetActive(true);
	}

	public void HideLoading ()
	{
		gameObject.SetActive(false);	
	}
}
