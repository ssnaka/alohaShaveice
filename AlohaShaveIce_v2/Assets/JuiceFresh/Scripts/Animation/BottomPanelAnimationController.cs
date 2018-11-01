using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelAnimationController : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform;

    [SerializeField]
    Animator animator;
    [SerializeField]
    Button openCloseButton;
    string openAnimationName = "Open";
    string closeAnimationName = "Close";

    bool isOpened;

    float speed = 12000.0f;

    Vector2 closePosition;
    Vector2 openPosition;

    // Use this for initialization
    void Start ()
    {
        closePosition = new Vector2(0.0f, -rectTransform.rect.height);
        openPosition = new Vector2(0.0f, 0.0f);
        rectTransform.anchoredPosition = closePosition;
    }
	
//    // Update is called once per frame
//    void Update ()
//    {
//    }

    public void OnButtonPressed ()
    {
        if (isOpened)
        {
            StartCoroutine(MenuAnimation(openPosition, closePosition));
        }
        else
        {
            StartCoroutine(MenuAnimation(closePosition, openPosition));
        }

        openCloseButton.enabled = false;
        isOpened = !isOpened;
    }

    IEnumerator MenuAnimation (Vector2 _fromPosition, Vector2 _toPosition)
    {
        float step = (speed / (_fromPosition - _toPosition).magnitude) * Time.deltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step;
            Vector2 anchoredPosition = Vector2.Lerp(_fromPosition, _toPosition, t);
            rectTransform.anchoredPosition = anchoredPosition;
            yield return new WaitForEndOfFrame();
        }

        rectTransform.anchoredPosition = _toPosition;
        SetButtonEnable();
    }

    public void SetButtonEnable ()
    {
        openCloseButton.enabled = true;
    }

}
