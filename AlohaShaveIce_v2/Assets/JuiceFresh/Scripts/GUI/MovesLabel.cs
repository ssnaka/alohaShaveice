using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MovesLabel : MonoBehaviour {
    public Sprite[] sprites;
	// Use this for initialization
	void OnEnable () {
        if (LevelManager.THIS != null)
        {

            if (LevelManager.THIS.limitType == LIMIT.MOVES) GetComponent<Text>().text = "MOVES";
            else GetComponent<Text>().text = "TIME";
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
