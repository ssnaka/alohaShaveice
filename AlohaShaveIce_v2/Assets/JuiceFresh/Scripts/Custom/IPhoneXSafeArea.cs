using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPhoneXSafeArea : MonoBehaviour {
	[SerializeField]
	RectTransform topPanel;
	[SerializeField]
	public RectTransform bottoPanel;

	void OnEnable ()
	{
		float screenRatio = (float)Screen.height / (float)Screen.width;
        screenRatio = (float)System.Math.Round(screenRatio, 2);
        if (screenRatio == 2.17f)
		{
            if (topPanel != null)
            {
			    topPanel.anchoredPosition = new Vector2(topPanel.anchoredPosition.x, -120.0f);
            }
            if (bottoPanel != null)
            {
                bottoPanel.anchoredPosition = new Vector2(bottoPanel.anchoredPosition.x, 68.0f);
            }
		}
	}
}
