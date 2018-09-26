using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyButtonScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
//	// Update is called once per frame
//	void Update () {
//		
//	}

    public void OnPrivacyButtonPressed ()
    {
        Application.OpenURL("https://docs.google.com/document/d/1t5vePkLeGI3mGN3H3p7oWshcogkDOBa2rrUy4qP-3O0/edit#heading=h.9gl0i71zlh3w");
    }

}
