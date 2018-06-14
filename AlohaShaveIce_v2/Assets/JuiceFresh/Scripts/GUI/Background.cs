using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Background : MonoBehaviour {
	public Sprite[] pictures;

	// Use this for initialization
	void OnEnable () {
		if (LevelManager.THIS != null)
		{
			int levelToLoad = LevelManager.Instance.currentLevel;
			if (LevelManager.THIS.questInfo != null)
			{
				levelToLoad = LevelManager.THIS.questInfo.actualLevel;
			}
			GetComponent<Image> ().sprite = pictures [(int)((float)levelToLoad / 20f - 0.01f)];
		}

	}
	

}
