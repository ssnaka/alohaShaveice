using UnityEngine;
using System.Collections;

public class BombExplosion : MonoBehaviour {

	// Use this for initialization
	void Start () {
	   iTween.ScaleTo(gameObject, Vector3.one * 20, 1);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
