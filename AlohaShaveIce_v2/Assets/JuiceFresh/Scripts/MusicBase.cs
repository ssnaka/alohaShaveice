using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[Prefab("Custom/MusicBase")]
public class MusicBase : Singleton<MusicBase> {
	
	[SerializeField]
	public List<AudioClip> musicList;
	[SerializeField]
	List<AudioSource> bgmSources;

	AudioSource fadeOutAudioSource = null;
	const float bgmFadeDuration = 1.5f;

	int masterVolume = 1;

	int currentPlayingIndex = 0;
    
	// Use this for initialization
    void Awake()
    {
		masterVolume = PlayerPrefs.GetInt("Music", 1);
    }

	public void PlayCurrentBGM ()
	{
		if (masterVolume == 0)
		{
			return;
		}

		FadeBGMIn(bgmSources[currentPlayingIndex], 1.0f);
	}

	public void StopCurrentBGM ()
	{
		if (masterVolume == 0)
		{
			return;
		}

		FadeBGMOut(bgmSources[currentPlayingIndex], 0.0f);
	}

	public void SetVolume (float _volume)
	{
		masterVolume = (int)_volume;
	}

	public void PlayBGM (string _soundName, bool _shouldStartFromBeginning = false, bool _loop = true)
	{
		AudioClip newClip = musicList.Find(item => item.name.Equals(_soundName));
		if (newClip == null)
		{
			return;
		}

		FadeBGMOut(bgmSources[currentPlayingIndex]);
		if (string.IsNullOrEmpty(_soundName))
		{
			return;
		}

		currentPlayingIndex = (++currentPlayingIndex) < bgmSources.Count ? currentPlayingIndex : 0;

		AudioSource nextBGM = bgmSources[currentPlayingIndex];
		nextBGM.clip = newClip;
		nextBGM.loop = _loop;
		if (_shouldStartFromBeginning)
		{
			nextBGM.time = 0.0f;
		}
		FadeBGMIn(nextBGM, 1.0f);
	}

	public void PlayRandomBGM ()
	{
		int index = Random.Range(0, musicList.Count);
		PlayBGM(musicList[index].name, true, true);
	}

	delegate void OnSoundLoadDone(AudioSource _audioSource);
	void FadeBGMOut (AudioSource _audioSource, float _delay = 0.0f)
	{
		float toVolume = 0.0f;

		if (_audioSource == null)
		{
			return;
		}

		if (fadeOutAudioSource != null)
		{
			OnFadeDone();
			_delay += (bgmFadeDuration / 2.0f);
		}

		fadeOutAudioSource = _audioSource;
		StartCoroutine(FadeBGM(fadeOutAudioSource, toVolume, _delay, bgmFadeDuration));
	}

	void FadeBGMIn (AudioSource _audioSource, float _toVolume, float _delay = 0.0f)
	{
		_audioSource.gameObject.SetActive(true);
		if (_audioSource == null)
		{
			return;
		}

		_audioSource.gameObject.SetActive(true);
		_audioSource.volume = 0.0f;
		_audioSource.Play();

		if (masterVolume == 0)
		{
			_toVolume = (float)masterVolume;
		}
		StartCoroutine(FadeBGM(_audioSource, _toVolume, _delay, bgmFadeDuration));
	}

	IEnumerator FadeBGM (AudioSource _audioSource, float _fadeToVolume, float _delay, float _duration)
	{
		yield return new WaitForSeconds(_delay);

		float startVolume = _audioSource.volume;
		float elapsed = 0f;

		while (elapsed <= _duration)
		{
			float t = elapsed / _duration;

			float volume = Mathf.Lerp(startVolume, _fadeToVolume, t);
			_audioSource.volume = volume;

			elapsed += Time.deltaTime;
			yield return 0;
		}

		_audioSource.volume = _fadeToVolume;
		OnFadeDone();
	}

	void OnFadeDone ()
	{
		if (fadeOutAudioSource != null)
		{
			fadeOutAudioSource.time = fadeOutAudioSource.time;
			fadeOutAudioSource.Pause();
			fadeOutAudioSource.gameObject.SetActive(false);
			fadeOutAudioSource = null;
		}
	}
}
