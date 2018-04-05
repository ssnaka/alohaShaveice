using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public enum GratzType
{
	None,
	GratzSmall,
	GratzMedium,
	GratzLarge,
	LevelEnd
}

public class GratzWord : MonoBehaviour 
{
	[SerializeField]
	Image gratzImage;
	[SerializeField]
	List<Sprite> gratzSprite;

	[SerializeField]
	Animator animator;
	[SerializeField]
	AnimationClip normalFloatingWord;
	[SerializeField]
	AnimationClip levelEndFloatingWord;

	public void SetupGartz (GratzType _type)
	{
		int index = 0;
		AudioClip audioClip = null;
		string animationTriggerName = normalFloatingWord.name;
		switch(_type)
		{
		case GratzType.GratzSmall:
			index = 0;
			break;
		case GratzType.GratzMedium:
			audioClip = SoundBase.Instance.ono;
			index = 1;
			break;
		case GratzType.GratzLarge:
			audioClip = SoundBase.Instance.cheehoo;
			index = 2;
			break;
		case GratzType.LevelEnd:
			animationTriggerName = levelEndFloatingWord.name;
			index = 3;
			break;
		}

		gratzImage.overrideSprite = gratzSprite[index];
		transform.localPosition = Vector3.zero;
		gameObject.SetActive(true);
		animator.SetTrigger(animationTriggerName);
		if (audioClip != null)
		{
			SoundBase.Instance.PlaySound(audioClip);
		}
	}

	public void OnGratzAnimationEnd ()
	{
		animator.SetTrigger("none");
		gameObject.SetActive(false);
	}
}
