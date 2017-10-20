using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GUIEvents : MonoBehaviour {

	void Start () {
		if (name == "FaceBook") {//1.3
//            if (PlayerPrefs.GetInt("Facebook_Logged") == 1) {
//                FaceBookLogin();
//            }
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
			settings.SetActive (true);
		else
			settings.SetActive (false);
		// GameObject.Find("CanvasGlobal").transform.Find("Settings").gameObject.SetActive(true);

	}

	public void Play () {
		SoundBase.Instance.PlaySound (SoundBase.Instance.click);

		transform.Find ("Loading").gameObject.SetActive (true);//1.4
		SceneManager.LoadScene ("game");
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
