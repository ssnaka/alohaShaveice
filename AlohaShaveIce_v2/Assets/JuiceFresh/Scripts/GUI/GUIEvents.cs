using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIEvents : MonoBehaviour {

	void Start () {
		ZPlayerPrefs.Initialize("TryYourBestToGuessPass", "saltIsnotGoingToBeEasy");
		if (name == "FaceBook") {//1.3
//            if (PlayerPrefs.GetInt("Facebook_Logged") == 1) {
//                FaceBookLogin();
//            }
		}

		else if (name == "Settings")
		{
			GameObject tutorialNotification = GameObject.Find("tutorialNotification");
			if (tutorialNotification != null)
			{
				int tutorialNotificationView = PlayerPrefs.GetInt("tutorialNotificationView", 0);
				if (tutorialNotificationView <= 0)
				{
					tutorialNotification.SetActive(true);
				}
				else
				{
					tutorialNotification.SetActive(false);
				}
			}
		}

	}

	void Update () {
		if (name == "FaceBook" || name == "Share" || name == "FaceBookLogout") {
			if (!LevelManager.THIS.FacebookEnable)
				gameObject.SetActive (false);
		}
	}

	public void Settings (GameObject settings) {
		SoundBase.Instance.PlaySound (SoundBase.Instance.click);
		if (!settings.activeSelf)
		{
			settings.SetActive (true);
			int tutorialNotificationView = PlayerPrefs.GetInt("tutorialNotificationView", 0);
			if (tutorialNotificationView <= 0)
			{
				GameObject tutorialNotification = GameObject.Find("tutorialNotification");
				if (tutorialNotification != null)
				{
					PlayerPrefs.SetInt("tutorialNotificationView", 1);
					PlayerPrefs.Save();
					tutorialNotification.SetActive(false);
				}
			}
		}
		else
		{
			settings.SetActive (false);
		}
		// GameObject.Find("CanvasGlobal").transform.Find("Settings").gameObject.SetActive(true);
	}

	public void Play () {
		SoundBase.Instance.PlaySound (SoundBase.Instance.click);
		LoadingCanvasScript.Instance.ShowLoading();
//		Transform loadingTransform = transform.Find ("Loading");
//		loadingTransform.gameObject.SetActive (true);//1.4
//		Transform loadingText = loadingTransform.Find("Text");
//		iTween.ScaleTo(loadingText.gameObject, iTween.Hash("scale", new Vector3(2.0f, 2.0f, 2.0f), "time", 2.0f, "easeType", "easeInOutExpo"));
//		SceneManager.LoadScene ("game");
		StartCoroutine(LoadScene());
	}

	IEnumerator LoadScene ()
	{
		AsyncOperation loadingOperation = SceneManager.LoadSceneAsync("game");
		while (!loadingOperation.isDone)
		{
			yield return null;
		}

		AsyncOperation unloadingOperation = SceneManager.UnloadSceneAsync("main");
		while (!unloadingOperation.isDone)
		{
			yield return null;
		}
//		LoadingCanvasScript.Instance.HideLoading();
//		MusicBase.Instance.GetComponent<AudioSource>().Stop();
	}

	public void Pause () {
		SoundBase.Instance.PlaySound (SoundBase.Instance.click);

		if (LevelManager.THIS.gameStatus == GameState.Playing)
			GameObject.Find ("CanvasGlobal").transform.Find ("MenuPause").gameObject.SetActive (true);

	}

	public void FaceBookLogin () {
#if FACEBOOK
		FacebookManager.THIS.CallFBLogin ();

		// GameSparksIntegration.THIS.CallFBLogin();
#endif
	}

	public void FaceBookLogout () { //1.3.3
		#if FACEBOOK
		FacebookManager.THIS.CallFBLogout ();

		#endif
	}


	public void FaceBookLoginWithPublishPerm () {
#if FACEBOOK

		FacebookManager.THIS.CallFBLoginForPublish ();
#endif
	}

	public void Share () {
#if FACEBOOK

		FacebookManager.THIS.Share ();
#endif
	}

	public void ApiRequest () {
#if FACEBOOK

		FacebookManager.THIS.ReadScores ();
#endif
	}

	public void ApiPost () {
#if FACEBOOK

		FacebookManager.THIS.SaveScores ();
#endif
	}


}
