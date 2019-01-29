using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsBodyPanelScript : MenuBodyPanelScript
{
    [SerializeField]
    GameObject soundOffGameObject;
    [SerializeField]
    GameObject musicOffGameObject;

    // Use this for initialization
    void Start ()
    {
        if (PlayerPrefs.GetInt("Sound") == 0)
            soundOffGameObject.SetActive(true);
        else
            soundOffGameObject.SetActive(false);

        if (PlayerPrefs.GetInt("Music") == 0)
            musicOffGameObject.SetActive(true);
        else
            musicOffGameObject.SetActive(false);
    }

    public void Info()
    {
        GameObject.Find("CanvasGlobal").transform.Find("Tutorial").gameObject.SetActive(true);
    }

    public void SoundOff (GameObject Off)
    {
        if (!Off.activeSelf)
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 0;
            InitScript.sound = false;

            Off.SetActive(true);
        }
        else
        {
            SoundBase.Instance.GetComponent<AudioSource>().volume = 1;
            InitScript.sound = true;

            Off.SetActive(false);

        }
        PlayerPrefs.SetInt("Sound", (int)SoundBase.Instance.GetComponent<AudioSource>().volume);
        PlayerPrefs.Save();

    }

    public void MusicOff (GameObject Off)
    {
        float volume = 0.0f;
        if (!Off.activeSelf)
        {
            //          GameObject.Find("Music").GetComponent<AudioSource>().volume = 0;
            MusicBase.Instance.StopCurrentBGM();
            MusicBase.Instance.SetVolume(volume);
            InitScript.music = false;
            Off.SetActive(true);
        }
        else
        {
            volume = 1.0f;
            MusicBase.Instance.SetVolume(volume);
            MusicBase.Instance.PlayCurrentBGM();
            //          GameObject.Find("Music").GetComponent<AudioSource>().volume = 1;
            InitScript.music = true;
            Off.SetActive(false);
        }

        PlayerPrefs.SetInt("Music", (int)volume);
        PlayerPrefs.Save();
    }
}
