using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Background : MonoBehaviour
{
    public Sprite[] pictures;

    // Use this for initialization
    void OnEnable ()
    {
        if (LevelManager.THIS != null)
        {
            int levelToLoad = LevelManager.Instance.currentLevel;
            if (LevelManager.THIS.questInfo != null)
            {
                levelToLoad = LevelManager.THIS.questInfo.actualLevel;
            }

            int bgIndex = (int)((float)levelToLoad / 20f - 0.01f);
            if (pictures.Length <= bgIndex)
            {
                Debug.LogError("Need New background Image");
                return;
            }

            GetComponent<Image>().sprite = pictures[bgIndex];
        }

    }
	

}
