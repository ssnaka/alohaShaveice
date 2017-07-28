using UnityEngine;
using System.Collections;

public class HandTutorial : MonoBehaviour {
	public TutorialManager tutorialManager;

	void OnEnable () {
		PrepareAnimateHand ();
	}

	void PrepareAnimateHand () {
		Vector3[] positions = tutorialManager.GetItemsPositions ();
		StartCoroutine (AnimateHand (positions));
	}

	IEnumerator AnimateHand (Vector3[] positions) {
		float speed = 3;
		int posNum = 0;

		for (int i = 0; i < positions.Length - 1; i++) {

			transform.position = positions [posNum];
			posNum++;
			Vector2 startPos = transform.position;
			Vector2 endPos = positions [posNum];
			float distance = Vector3.Distance (startPos, endPos);
			float fracJourney = 0;
			float startTime = Time.time;

			while (fracJourney < 1) {
				float distCovered = (Time.time - startTime) * speed;
				fracJourney = distCovered / distance;
				transform.position = Vector2.Lerp (startPos, endPos, fracJourney);
				yield return new WaitForFixedUpdate ();
			}
		}
		yield return new WaitForFixedUpdate ();
		PrepareAnimateHand ();
	}
}
