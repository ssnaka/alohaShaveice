using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SettingAnimation : MonoBehaviour
{
    public AudioMixer mainMixer;

    // Use this for initialization
    void OnEnable()
    {
        transform.localScale = new Vector3(1, 0, 1);
        iTween.ScaleTo(gameObject, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easeType", "easeInOutExpo"));
    }

    public void Info()
    {

    }

}
