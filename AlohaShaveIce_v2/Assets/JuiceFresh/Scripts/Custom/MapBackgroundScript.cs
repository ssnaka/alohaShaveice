using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapBackgroundScript : MonoBehaviour {
	[SerializeField]
	SpriteRenderer sRenderer;

	public int index;

	public void SetupSprite (Sprite _sprite, float _yPosition, int _newIndex)
	{
		index = _newIndex;
		sRenderer.sprite = _sprite;
		transform.position = new Vector3(0.0f, _yPosition, 0.0f);
	}
}
