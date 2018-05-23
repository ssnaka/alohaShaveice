using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolkit.Localization;

public class GDPRController : MonoBehaviour {
	[SerializeField]
	Button gdprButton;
	[SerializeField]
	RectTransform gdprMainViewRect;

	[SerializeField]
	LocalizedTextBehaviour gdprTitleTextBehaviour;
	[SerializeField]
	LocalizedTextBehaviour gdprDescriptionTextBehaviour;

	[SerializeField]
	RectTransform gdprMainButtonBG;
	[SerializeField]
	RectTransform gdprMainButtonContainer;
	[SerializeField]
	RectTransform gdprSubButtonContainer;

	[SerializeField]
	LocalizedText policyTextAsset;
	[SerializeField]
	LocalizedText agreeTextAsset;
	[SerializeField]
	LocalizedText disagreeTextAsset;

	public void OnGDRPButtonPressed ()
	{
		gdprMainViewRect.gameObject.SetActive(true);
		gdprButton.gameObject.SetActive(false);

		gdprMainButtonBG.gameObject.SetActive(true);
		gdprMainButtonContainer.gameObject.SetActive(true);
		gdprSubButtonContainer.gameObject.SetActive(false);
		gdprDescriptionTextBehaviour.LocalizedAsset = policyTextAsset;
	}

	public void OnGDPRAgreeButtonPressed ()
	{
		ZPlayerPrefs.SetInt("GDPRAgreement", System.Convert.ToInt16(true));
		gdprDescriptionTextBehaviour.LocalizedAsset = agreeTextAsset;
		gdprMainButtonBG.gameObject.SetActive(false);
		gdprMainButtonContainer.gameObject.SetActive(false);
		gdprSubButtonContainer.gameObject.SetActive(true);
	}

	public void OnGDPRDisagreeButtonPressed ()
	{
		ZPlayerPrefs.SetInt("GDPRAgreement", System.Convert.ToInt16(false));
		gdprDescriptionTextBehaviour.LocalizedAsset = disagreeTextAsset;
		gdprMainButtonBG.gameObject.SetActive(false);
		gdprMainButtonContainer.gameObject.SetActive(false);
		gdprSubButtonContainer.gameObject.SetActive(true);
	}

	public void OnGDPRLearnMoreButtonPressed ()
	{
		Application.OpenURL("https://www.appodeal.com/privacy-policy");
	}

	public void OnGDPRCloseButtonPressed ()
	{
		gdprMainViewRect.gameObject.SetActive(false);
		gdprButton.gameObject.SetActive(false);
	}

}
