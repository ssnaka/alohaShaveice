using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPhoneXSafeArea : MonoBehaviour {
	[SerializeField]
	RectTransform topPanel;
	[SerializeField]
	RectTransform bottomanel;

	void OnEnable ()
	{
		float screenRatio = (float)Screen.height / (float)Screen.width;

		if (screenRatio >= 2.0f)
		{
			topPanel.anchoredPosition = new Vector2(topPanel.anchoredPosition.x, -120.0f);
			bottomanel.anchoredPosition = new Vector2(bottomanel.anchoredPosition.x, 68.0f);
		}
	}
}
