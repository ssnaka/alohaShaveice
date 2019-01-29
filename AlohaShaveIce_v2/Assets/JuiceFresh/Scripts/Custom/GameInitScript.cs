using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameToolkit.Localization;

public class GameInitScript : MonoBehaviour 
{
    void Awake ()
    {
        FacebookManager.Instance.Init();
        NetworkManager.Instance.Init();
    }
	// Use this for initialization
	void Start () 
	{
		Localization.Instance.CurrentLanguage = Application.systemLanguage; //SystemLanguage.Korean;
		MusicBase.Instance.PlayBGM("alohaShaveIce_music_03paipai", true, true);
		NotificationCenter.Instance.Init();
	}
}
