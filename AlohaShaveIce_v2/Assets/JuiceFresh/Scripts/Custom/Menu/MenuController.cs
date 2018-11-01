using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    GameObject MenuPanelPrefab;
    public MenuPanelScript menuPanelScript;
    IPhoneXSafeArea iPhoneXSafeArea;

    // Use this for initialization
    void Start ()
    {
		
    }
	
    // Update is called once per frame
    void Update ()
    {
		
    }

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
}
