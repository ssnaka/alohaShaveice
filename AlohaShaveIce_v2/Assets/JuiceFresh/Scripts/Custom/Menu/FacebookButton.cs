using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacebookButton : MonoBehaviour {

    [SerializeField]
    GameObject faceBoolLogoutGameObject;

	// Use this for initialization
	void Start () 
    {
        FacebookManager.Instance.onFacebookLogIn += OnFaceBookLogin;

        faceBoolLogoutGameObject.SetActive(false);
        if (FacebookManager.Instance.IsFaceboolLoggedIn()) 
        {
            faceBoolLogoutGameObject.SetActive(true);
        }
        else
        {
            faceBoolLogoutGameObject.SetActive(false);
        }
	}

    public void FaceBookLogin () {
        #if FACEBOOK
        if (!FacebookManager.Instance.IsFaceboolLoggedIn())
        {
            FacebookManager.Instance.CallFBLogin();
        }
        else
        {
            FacebookManager.Instance.CallFBLogout();
        }
        #endif
    }

    public void FaceBookLogout () { //1.3.3
        #if FACEBOOK
        FacebookManager.Instance.CallFBLogout ();

        #endif
    }

    void OnFaceBookLogin (bool _login)
    {
        faceBoolLogoutGameObject.SetActive(_login);
        LoadingCanvasScript.Instance.HideLoading();
    }

    void OnDisable ()
    {
        if (FacebookManager.Instance)
        {
            FacebookManager.Instance.onFacebookLogIn -= OnFaceBookLogin;
        }
    }

//    void OnEnable ()
//    {
//        FacebookManager.Instance.onFacebookLogIn += OnFaceBookLogin;
//    }
}
