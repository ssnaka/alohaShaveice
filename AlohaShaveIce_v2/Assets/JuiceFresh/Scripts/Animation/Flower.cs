using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Flower : MonoBehaviour
{
	Item item;

	[SerializeField]
	float speed = 6.5f;

	ParticleSystem particleSystem;
	SpriteRenderer spriteRenderer;
	// Use this for initialization
	void Start ()
	{
		particleSystem = GetComponent<ParticleSystem>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		particleSystem.Stop();
	}

	// Update is called once per frame
	void Update ()
	{
		if (particleSystem.isPlaying)
		{
			transform.Rotate(Vector3.back * Time.deltaTime * 1000);
			transform.position = new Vector3(transform.position.x, transform.position.y, -15f);
		}
	}

	public void StartFly (Vector3 pos1, bool directFly = false, int _index = 0)
	{
		GetComponent<SpriteRenderer>().enabled = true;
		StartCoroutine(FlyCor(pos1, directFly, _index));
	}

	IEnumerator FlyCor (Vector3 pos1, bool directFly = false, int _index = 0)
	{
		Vector3 pos2 = Vector3.zero;
		yield return new WaitForFixedUpdate();

		transform.position = pos1;
		while (LevelManager.THIS.DragBlocked)
		{
			yield return new WaitForEndOfFrame();
		}
		List<Item> items = LevelManager.THIS.GetRandomItems(1);
		foreach (Item item1 in items)
		{
			item = item1;
			pos2 = item.transform.position;
		}
		if (item == null)
		{
			particleSystem.Stop();
			spriteRenderer.enabled = false;
			yield break;
		}
		Item _item = item;

		int minRandomRange = 3;
		int maxRandomRange = 5;
		if (_index == 0)
		{
			minRandomRange = 1;
			maxRandomRange = 4;
		}

		if (directFly)
		{
			minRandomRange = 1;
			maxRandomRange = 5;
		}

		//if flower count is greater than 1, it only will show square bomb or cross bomb.
		_item.nextType = (ItemsTypes)Random.Range(minRandomRange, maxRandomRange);

		float startTime = Time.time;
		Vector3 startPos = pos1;
		float distance = Vector3.Distance(pos1, pos2);
		float aSpeed = speed;
		if (directFly)
			aSpeed *= 5;
		float fracJourney = 0;
		particleSystem.gravityModifier = 0.1f;
		particleSystem.Play();

		//        iTween.MoveTo(gameObject, iTween.Hash("position", pos2, "time", 1, "oncomplete", "AnimCallBack"));
		while (fracJourney < 1)
		{
			if (_item.awaken && _item.gameObject != null)
			{
				_item.nextType = ItemsTypes.NONE;
				StartFly(transform.position, directFly, _index);
				yield break;
			}
			// aSpeed += 0.2f;
			float distCovered = (Time.time - startTime) * aSpeed;
			fracJourney = distCovered / distance;
			if (float.IsNaN(fracJourney))  //1.3
				fracJourney = 0;   //1.3
			transform.position = Vector3.Lerp(startPos, pos2, fracJourney);
			yield return new WaitForFixedUpdate();
		}

		particleSystem.gravityModifier = 0;

		AnimCallBack();
	}

	void AnimCallBack ()
	{
		particleSystem.Stop();
		spriteRenderer.enabled = false;
		item.ChangeType();
		LevelManager.THIS.DragBlocked = false;

	}
}
