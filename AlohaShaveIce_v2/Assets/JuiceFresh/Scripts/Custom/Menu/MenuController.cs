using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    Canvas canvas;
    [SerializeField]
    GraphicRaycaster graphicRaycaster;
    [SerializeField]
    GameObject questGO;
    [SerializeField]
    GameObject rewardGO;
    [SerializeField]
    GameObject tutorialGO;
    [SerializeField]
    GameObject settingsGO;
    [SerializeField]
    GameObject LifeGemGO;

    [SerializeField]
    GameObject MenuPanelPrefab;
    public MenuPanelScript menuPanelScript;
    IPhoneXSafeArea iPhoneXSafeArea;


//    // Use this for initialization
//    void Start ()
//    {
//		
//    }
//	
//    // Update is called once per frame
//    void Update ()
//    {
//		
//    }

    void OnEnable ()
    {
        if (iPhoneXSafeArea == null)
        {
            iPhoneXSafeArea = GetComponent<IPhoneXSafeArea>();
        }

        if (menuPanelScript == null)
        {
            GameObject menuPanelObject = Instantiate(MenuPanelPrefab, transform);
            menuPanelObject.transform.localScale = Vector3.one;
//            menuPanelObject.transform.localPosition = Vector3.zero;
            menuPanelScript = menuPanelObject.GetComponent<MenuPanelScript>();
            menuPanelObject.transform.SetSiblingIndex(menuPanelObject.transform.GetSiblingIndex()-1);
        }

        iPhoneXSafeArea.bottoPanel = menuPanelScript.GetComponent<RectTransform>();
    }

    public void EnableMapMenu (bool _enable)
    {
        EnableQuest(_enable);
//        questGO.SetActive(_enable);
        rewardGO.SetActive(_enable);
        tutorialGO.SetActive(_enable);
        settingsGO.SetActive(_enable);
        EnableLifeGemGo(_enable);
        EnableMainMenu(_enable, false);
    }

    public void EnableLifeGemGo (bool _enable)
    {
        LifeGemGO.SetActive(_enable);
    }

    public void EnableMainMenu (bool _enable, bool _shouldReorder)
    {
        graphicRaycaster.enabled = _enable;
        if (_shouldReorder)
        {
            ChangeCanvasOrder(_enable);
        }

        menuPanelScript.gameObject.SetActive(_enable);
//        LifeGemGO.SetActive(_enable);
    }

    public void ChangeCanvasOrder (bool _isOpening)
    {
        canvas.sortingOrder = 0;
        if (_isOpening)
        {
            canvas.sortingOrder = 10;
        }
    }

    public void EnableQuest (bool _enable)
    {
        int LatestReachedLevel = LevelsMap._instance.LatestReachedLevel;
        questGO.SetActive(false);
        if (LatestReachedLevel > 6 && _enable)
        {
            questGO.SetActive(true);
        }
    }
}
