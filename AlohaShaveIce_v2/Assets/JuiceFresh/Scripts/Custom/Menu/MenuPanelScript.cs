using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UnityEngine.UI;

public enum MenuItemType
{
    chest,
    bank,
    boosts,
//    life,
    settings
}

public class MenuPanelScript : MonoBehaviour
{
    [SerializeField]
    RectTransform rectTransform;

    [Header("Open/Close")]
    [SerializeField]
    Button openCloseButton;
    public bool isOpened { get; private set; }
    float speed = 12000.0f;
    Vector2 closePosition;
    Vector2 openPosition;

    public MenuItemType currentMenuItemType { get; private set; }
    [Header("ScrollView")]
    [SerializeField]
    RectTransform middlePanel;
    [SerializeField]
    HorizontalScrollSnap horizontalScrollSnap;
    [SerializeField]
    RectTransform bodyContainer;
    [SerializeField]
    List<GameObject> bodyPanelPrefabs;
    List<MenuBodyPanelScript> bodyPanelScripts = new List<MenuBodyPanelScript>();

    [Header("MenuButton")]
    [SerializeField]
    RectTransform buttonCoverRect;
    [SerializeField]
    List<RectTransform> menuButtons;

    delegate void OnMoveAnimationDone();

    int currentPage;

    Coroutine moveCoroutine;

    // Use this for initialization
    void Start ()
    {
        closePosition = new Vector2(0.0f, -rectTransform.rect.height);
        openPosition = new Vector2(0.0f, 0.0f);
        rectTransform.anchoredPosition = closePosition;

//        middlePanel.sizeDelta = new Vector2(middlePanel.rect.width * bodyPanelPrefabs.Count, middlePanel.sizeDelta.y);
        for (int i = 0; i < bodyPanelPrefabs.Count; i++)
        {
            GameObject prefab = bodyPanelPrefabs[i];
            GameObject bodyPanelGameObject = Instantiate<GameObject>(prefab);
            bodyPanelGameObject.transform.SetParent(bodyContainer);
            bodyPanelGameObject.transform.localScale = Vector3.one;
            bodyPanelGameObject.transform.localPosition = new Vector3(bodyPanelGameObject.transform.localPosition.x, bodyPanelGameObject.transform.localPosition.y, 0.0f);
            MenuBodyPanelScript script = bodyPanelGameObject.GetComponent<MenuBodyPanelScript>();
            bodyPanelScripts.Add(script);
            horizontalScrollSnap.AddChild(bodyPanelGameObject);
        }

//        horizontalScrollSnap.OnSelectionChangeEndEvent.AddListener(OnPageChangedEvent);
        horizontalScrollSnap.GetComponent<ScrollRect>().onValueChanged.AddListener(MoveMenuCoverWhileSwipe);
    }
	
    public void OnOpenCloseButtonPressed ()
    {
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
        {
            return;
        }

        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }

        if (isOpened)
        {
            // Close menu
            if (LevelManager.THIS.gameStatus == GameState.Map)
            {
                InitScript.Instance.menuController.ChangeCanvasOrder(false);
            }
            else if (LevelManager.THIS.gameStatus == GameState.Playing)
            {
                InitScript.Instance.menuController.EnableLifeGemGo(false);
            }
            StartCoroutine(MoveAnimation(rectTransform, openPosition, closePosition, speed, delegate {
                SetButtonEnable();
                if (LevelManager.THIS.gameStatus == GameState.Playing)
                {
                    InitScript.Instance.menuController.EnableMainMenu(false, true);
                }
            }));
        }
        else
        {
            // Open menu
            StartCoroutine(MoveAnimation(rectTransform, closePosition, openPosition, speed, delegate {
                SetButtonEnable();
                if (LevelManager.THIS.gameStatus == GameState.Playing)
                {
                    InitScript.Instance.menuController.EnableLifeGemGo(true);
                }
            }));
        }

        openCloseButton.enabled = false;
        isOpened = !isOpened;
    }

    IEnumerator MoveAnimation (RectTransform _rectTransform, Vector2 _fromPosition, Vector2 _toPosition, float _speed, OnMoveAnimationDone _onMoveAnimationDone = null)
    {
        float step = (_speed / (_fromPosition - _toPosition).magnitude) * Time.deltaTime;
        float t = 0;
        while (t <= 1.0f)
        {
            t += step;
            Vector2 anchoredPosition = Vector2.Lerp(_fromPosition, _toPosition, t);
            _rectTransform.anchoredPosition = anchoredPosition;
            yield return new WaitForEndOfFrame();
        }

        _rectTransform.anchoredPosition = _toPosition;
        if (_onMoveAnimationDone != null)
        {
            _onMoveAnimationDone();
        }
    }

    public void SetButtonEnable ()
    {
        openCloseButton.enabled = true;
    }

    public void OnMenuButtonPressed (int _index)
    {
        ChangePage(_index);
        OnPageChangedEvent(_index);
    }

    public void OnPageChangedEvent (int _page)
    {
        if (!gameObject.activeInHierarchy || !gameObject.activeSelf)
        {
            return;
        }
        float defaultDistanceBetweenButton = Vector2.Distance(menuButtons[0].anchoredPosition, menuButtons[1].anchoredPosition);
        Vector2 newAncchoredPosition = new Vector2(menuButtons[_page].anchoredPosition.x, 0.0f);
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
        }
        moveCoroutine = StartCoroutine(MoveAnimation(buttonCoverRect, buttonCoverRect.anchoredPosition, newAncchoredPosition, 5.0f * (defaultDistanceBetweenButton * (Mathf.Abs(currentPage - _page)))));
        currentPage = _page;
    }

    public void ChangePage (int _index)
    {
        horizontalScrollSnap.ChangePage(_index);
    }

    public void MoveMenuCoverWhileSwipe (Vector2 _position)
    {
        float newX = (middlePanel.rect.width * _position.x) - (buttonCoverRect.sizeDelta.x * _position.x);
        if (newX <= 0.0f)
        {
            newX = 0.0f;
        }
        else if (newX >= middlePanel.rect.width - buttonCoverRect.sizeDelta.x)
        {
            newX = middlePanel.rect.width - buttonCoverRect.sizeDelta.x;
        }

        buttonCoverRect.anchoredPosition = new Vector2(newX, buttonCoverRect.anchoredPosition.y);
    }

    public MenuBodyPanelScript GetBodyPanelScript (MenuItemType _menuType)
    {
        return bodyPanelScripts.Find(item => item.menuItemType.Equals(_menuType));
    }
}
