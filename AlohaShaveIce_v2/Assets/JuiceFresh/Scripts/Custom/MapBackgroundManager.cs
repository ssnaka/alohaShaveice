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
//	[SerializeField]
//	List<float> mapBackgroundYPositions;
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

		mainCamera.GetComponent<MapCamera>().SetCameraBounds(mapBackgroundSpriteList);
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
			float bgYPos = 0.0f;
			for (int i = 0 ; i < mapBackgroundSpriteList.Count; i++)
			{
				Vector2 spriteYBounds = GetSpriteYBounds(mapBackgroundSpriteList[i].bounds.size.y, bgYPos);
				bgYPos += (i == 0 ? 0.0f : mapBackgroundSpriteList[i - 1].bounds.size.y / 2.0f) + (mapBackgroundSpriteList[i].bounds.size.y / 2.0f);
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
				newIndex = currentIndex + 1 >= mapBackgroundSpriteList.Count ? currentIndex : currentIndex + 1;
			}
			else if (previousCameraYPos > mainCamera.transform.position.y)
			{
				newIndex = currentIndex - 1 <= 0 ? 0 : currentIndex - 1;
			}

			float bgYPos = calculateYPos(1, newIndex);
			Vector2 spriteYBounds = GetSpriteYBounds(mapBackgroundSpriteList[newIndex].bounds.size.y, bgYPos);
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
		if (_spriteIndex < 0 || _spriteIndex >= mapBackgroundSpriteList.Count)
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

		float bgYPos = calculateYPos(1, _spriteIndex);
		script.SetupSprite(mapBackgroundSpriteList[_spriteIndex], bgYPos, _spriteIndex);

		isInitDone = true;
	}

	float calculateYPos (int _startIndex, int _endIndex)
	{
		float result = 0.0f;
		for (int i = _startIndex ; i <= _endIndex; i++)
		{
			result += (i == 0 ? 0.0f : mapBackgroundSpriteList[i - 1].bounds.size.y / 2.0f) + (mapBackgroundSpriteList[i].bounds.size.y / 2.0f);
		}

		return result;
	}
}
