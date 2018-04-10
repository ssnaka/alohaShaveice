using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBackgroundManager : MonoBehaviour {
	[SerializeField]
	Camera mainCamera;
	[SerializeField]
	GameObject mapBGPrefab;
	[SerializeField]
	Transform mapBGParent;
	[SerializeField]
	List<Sprite> mapBackgroundSpriteList;
	[SerializeField]
	List<float> mapBackgroundYPositions;
	Queue<MapBackgroundScript> mapBGPool = new Queue<MapBackgroundScript>();
	int maxBGCount = 3;

	bool isInitDone = false;

	int currentIndex = -1;
	float previousCameraYPos = 0.0f;
	// Use this for initialization
	void Start ()
	{
//		CreateMapBGPool();
		LevelsMap._instance.OnReset += LevelsMap__instance_OnReset;
	}

	void LevelsMap__instance_OnReset ()
	{
		CheckBG();
	}
		
	// Update is called once per frame
	void Update () 
	{
		if (isInitDone)
		{
			CheckBG();
		}
//		mainCamera.rect
	}

	void CheckBG ()
	{
		if (currentIndex < 0)
		{
			for (int i = 0 ; i < mapBackgroundYPositions.Count; i++)
			{
				Vector2 spriteYBounds = GetSpriteYBounds(mapBackgroundSpriteList[i].bounds.size.y, mapBackgroundYPositions[i]);

				if (mainCamera.transform.position.y < spriteYBounds.y && mainCamera.transform.position.y > spriteYBounds.x)
				{
					currentIndex = i;
					break;
				}
			}
		}
		else
		{
			int newIndex = 0;
			if (previousCameraYPos.Equals(mainCamera.transform.position.y))
			{
				return;
			}

			if (previousCameraYPos < mainCamera.transform.position.y)
			{
				newIndex = currentIndex + 1 >= mapBackgroundYPositions.Count ? currentIndex : currentIndex + 1;
			}
			else if (previousCameraYPos > mainCamera.transform.position.y)
			{
				newIndex = currentIndex - 1 <= 0 ? 0 : currentIndex - 1;
			}

			Vector2 spriteYBounds = GetSpriteYBounds(mapBackgroundSpriteList[newIndex].bounds.size.y, mapBackgroundYPositions[newIndex]);
			if (mainCamera.transform.position.y < spriteYBounds.y && mainCamera.transform.position.y > spriteYBounds.x)
			{
				if (currentIndex == newIndex)
				{
					previousCameraYPos = mainCamera.transform.position.y;
					return;
				}
				currentIndex = newIndex;
			}
			else
			{
				previousCameraYPos = mainCamera.transform.position.y;
				return;
			}
		}

		UpdateMapBG(currentIndex - 1);
		UpdateMapBG(currentIndex);
		UpdateMapBG(currentIndex + 1);
		previousCameraYPos = mainCamera.transform.position.y;
	}

	Vector2 GetSpriteYBounds (float spriteHight, float mapBGYPos)
	{
		float spriteHalfY = spriteHight * 0.5f;
		float upperBound = mapBGYPos + spriteHalfY;
		float lowerBound = mapBGYPos - spriteHalfY;
		return new Vector2(lowerBound, upperBound);
	}


	void UpdateMapBG (int _spriteIndex)
	{
		if (_spriteIndex < 0 || _spriteIndex >= mapBackgroundYPositions.Count)
		{
			return;
		}

		MapBackgroundScript script = null;
		if (mapBGPool.Count < maxBGCount)
		{
			GameObject go = Instantiate<GameObject>(mapBGPrefab, Vector3.one, Quaternion.identity, mapBGParent);
			script = go.GetComponent<MapBackgroundScript>();
		}
		else
		{
			script = mapBGPool.Dequeue();
		}
		mapBGPool.Enqueue(script);

		script.SetupSprite(mapBackgroundSpriteList[_spriteIndex], mapBackgroundYPositions[_spriteIndex], _spriteIndex);

		isInitDone = true;
	}
}
