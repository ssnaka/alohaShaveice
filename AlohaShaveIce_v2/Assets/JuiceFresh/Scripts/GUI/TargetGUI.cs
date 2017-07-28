using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TargetGUI : MonoBehaviour {
    public Sprite star;
    public Sprite bomb;
    public GameObject bigBack;
    public GameObject small;
    public GameObject check;
    public GameObject text;
    public GameObject textForMenu;
    public Image image;

    public void SetSprite(Sprite sprite) {
        image.sprite = sprite;
    }

    void OnEnable() {
        Reset();
    }

    public void Reset() {
        text.SetActive(true);
        check.SetActive(false);

    }

    public void Done() {
        text.SetActive(false);
        check.SetActive(true);
    }

    public void SetBack(bool show) {
        if (show) {
            bigBack.SetActive(true);
            small.SetActive(false);
            text.SetActive(false);
            textForMenu.SetActive(true);
        }
        else {
            bigBack.SetActive(false);
            small.SetActive(false);
            text.SetActive(true);
            textForMenu.SetActive(false);
        }
    }
}
