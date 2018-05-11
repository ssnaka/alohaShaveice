using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolkit.Localization;

public class MenuTutorialPanelScript : MonoBehaviour 
{
	[SerializeField]
	Image topCover;
	[SerializeField]
	Image leftCover;
	[SerializeField]
	Image rightCover;
	[SerializeField]
	Image bottomCover;

	[SerializeField]
	Image fingerImage;

	[SerializeField]
	Image characterIamge;
	[SerializeField]
	Image speechBubble;
	[SerializeField]
	Text descriptionText;
	[SerializeField]
	LocalizedTextBehaviour localizedTextBehaviour;

	[SerializeField]
	RectTransform rectTransform;

	ATutorial tutorialData;
	float mergin = 10.0f;

	Vector2 speechBubbleOriginalPosition;
	Vector2 characterImageOriginalPosition;
	void Awake ()
	{
		speechBubbleOriginalPosition = speechBubble.rectTransform.localPosition;
		characterImageOriginalPosition = characterIamge.rectTransform.localPosition;
	}

	public void SetUpTutorial (ATutorial _tutorialData, RectTransform _pivot, Canvas _tutorialCanvas)
	{
		tutorialData = _tutorialData;

		Canvas[] canvases = _pivot.transform.GetComponentsInParent<Canvas>();
		Canvas pivotParentCanvas = canvases[0];

		float scale = pivotParentCanvas.scaleFactor / _tutorialCanvas.scaleFactor;
		float pivotWidth = _pivot.rect.width * scale;
		float pivotHeight = _pivot.rect.height * scale;

		float pivotHalfX = pivotWidth / 2.0f + mergin;
		float pivotHalfY = pivotHeight / 2.0f + mergin;

		topCover.rectTransform.position = new Vector2(topCover.rectTransform.position.x, _pivot.position.y);
		topCover.rectTransform.anchoredPosition = new Vector2(topCover.rectTransform.anchoredPosition.x, topCover.rectTransform.anchoredPosition.y + pivotHalfY);

		bottomCover.rectTransform.position = new Vector2(bottomCover.rectTransform.position.x, _pivot.position.y);
		bottomCover.rectTransform.anchoredPosition = new Vector2(bottomCover.rectTransform.anchoredPosition.x, bottomCover.rectTransform.anchoredPosition.y - pivotHalfY);

		leftCover.rectTransform.position = new Vector2(_pivot.position.x, _pivot.position.y);
		leftCover.rectTransform.anchoredPosition = new Vector2(leftCover.rectTransform.anchoredPosition.x - pivotHalfX, leftCover.rectTransform.anchoredPosition.y);
		leftCover.rectTransform.sizeDelta = new Vector2(leftCover.rectTransform.rect.width, pivotHeight + (mergin * 2));

		rightCover.rectTransform.position = new Vector2(_pivot.position.x, _pivot.position.y);
		rightCover.rectTransform.anchoredPosition = new Vector2(rightCover.rectTransform.anchoredPosition.x + pivotHalfX, rightCover.rectTransform.anchoredPosition.y);
		rightCover.rectTransform.sizeDelta = new Vector2(rightCover.rectTransform.rect.width, pivotHeight + (mergin * 2));

		fingerImage.rectTransform.position = new Vector2(_pivot.position.x, _pivot.position.y);
		fingerImage.rectTransform.anchoredPosition = new Vector2(fingerImage.rectTransform.anchoredPosition.x, fingerImage.rectTransform.anchoredPosition.y + pivotHalfY);

		localizedTextBehaviour.LocalizedAsset = Resources.Load<LocalizedText>(_tutorialData.descriptionTextAsset);

		if (bottomCover.rectTransform.localPosition.y < speechBubbleOriginalPosition.y)
		{
//			speechBubble.rectTransform.localPosition = new Vector2(speechBubble.rectTransform.localPosition.x, speechBubbleOriginalPosition.y * -1.0f);
//			characterIamge.rectTransform.localPosition = new Vector2(characterIamge.rectTransform.localPosition.x, characterImageOriginalPosition.y * -1.0f);
			speechBubble.rectTransform.localPosition = new Vector2(speechBubble.rectTransform.localPosition.x, 200.0f);
			characterIamge.rectTransform.localPosition = new Vector2(characterIamge.rectTransform.localPosition.x, 200.0f);
		}

		gameObject.SetActive(true);
	}

	public void SetUpTutorialForLevel (ATutorial _tutorialData, Transform _pivot, Canvas _tutorialCanvas)
	{
		LevelManager.OnStartPlay += OnCloseButtonPressed;
		float aMergin = mergin * 2;
		tutorialData = _tutorialData;
		Vector3 pivotScreenPosition = Camera.main.WorldToScreenPoint(_pivot.position);
		Rect pivotRect = BoundsToScreenRect(_pivot.GetComponent<Renderer>().bounds);

		float pivotHalfX = pivotRect.width / 2.0f + aMergin;
		float pivotHalfY = pivotRect.height / 2.0f + aMergin;

		topCover.rectTransform.position = new Vector2(topCover.rectTransform.position.x, pivotScreenPosition.y);
		topCover.rectTransform.anchoredPosition = new Vector2(topCover.rectTransform.anchoredPosition.x, topCover.rectTransform.anchoredPosition.y + pivotHalfY);

		bottomCover.rectTransform.position = new Vector2(bottomCover.rectTransform.position.x, pivotScreenPosition.y);
		bottomCover.rectTransform.anchoredPosition = new Vector2(bottomCover.rectTransform.anchoredPosition.x, bottomCover.rectTransform.anchoredPosition.y - pivotHalfY);

		leftCover.rectTransform.position = new Vector2(pivotScreenPosition.x, pivotScreenPosition.y);
		leftCover.rectTransform.anchoredPosition = new Vector2(leftCover.rectTransform.anchoredPosition.x - pivotHalfX, leftCover.rectTransform.anchoredPosition.y);
		leftCover.rectTransform.sizeDelta = new Vector2(leftCover.rectTransform.rect.width, pivotRect.height + (aMergin * 2));

		rightCover.rectTransform.position = new Vector2(pivotScreenPosition.x, pivotScreenPosition.y);
		rightCover.rectTransform.anchoredPosition = new Vector2(rightCover.rectTransform.anchoredPosition.x + pivotHalfX, rightCover.rectTransform.anchoredPosition.y);
		rightCover.rectTransform.sizeDelta = new Vector2(rightCover.rectTransform.rect.width, pivotRect.height + (aMergin * 2));

		fingerImage.rectTransform.position = new Vector2(pivotScreenPosition.x, pivotScreenPosition.y);
		fingerImage.rectTransform.anchoredPosition = new Vector2(fingerImage.rectTransform.anchoredPosition.x, fingerImage.rectTransform.anchoredPosition.y + pivotHalfY);

		localizedTextBehaviour.LocalizedAsset = Resources.Load<LocalizedText>(_tutorialData.descriptionTextAsset);

		if (bottomCover.rectTransform.localPosition.y < speechBubbleOriginalPosition.y)
		{
//			speechBubble.rectTransform.localPosition = new Vector2(speechBubble.rectTransform.localPosition.x, speechBubbleOriginalPosition.y * -1.0f);
//			characterIamge.rectTransform.localPosition = new Vector2(characterIamge.rectTransform.localPosition.x, characterImageOriginalPosition.y * -1.0f);
			speechBubble.rectTransform.localPosition = new Vector2(speechBubble.rectTransform.localPosition.x, 200.0f);
			characterIamge.rectTransform.localPosition = new Vector2(characterIamge.rectTransform.localPosition.x, 200.0f);
		}

		gameObject.SetActive(true);
	}

	Rect BoundsToScreenRect (Bounds bounds)
	{
		// Get mesh origin and farthest extent (this works best with simple convex meshes)
		Vector3 origin = Camera.main.WorldToScreenPoint(new Vector3(bounds.min.x, bounds.max.y, 0f));
		Vector3 extent = Camera.main.WorldToScreenPoint(new Vector3(bounds.max.x, bounds.min.y, 0f));

		// Create rect in screen space and return - does not account for camera perspective
		return new Rect(origin.x, Screen.height - origin.y, extent.x - origin.x, origin.y - extent.y);
	}

	public void OnCloseButtonPressed ()
	{
		gameObject.SetActive(false);
	}

	public void OnDisable ()
	{
		LevelManager.OnStartPlay -= OnCloseButtonPressed;
		GameTutorialManager.Instance.SetLocalTutorialStatus(tutorialData.type);
	}
}
