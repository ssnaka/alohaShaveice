using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BottomPanelAnimationController : MonoBehaviour
{
    [SerializeField]
    Animator animator;
    [SerializeField]
    Button openCloseButton;
    string openAnimationName = "Open";
    string closeAnimationName = "Close";

    bool isOpened;
    // Use this for initialization
    void Start ()
    {
		
    }
	
//    // Update is called once per frame
//    void Update ()
//    {
//		
//    }

    public void OnButtonPressed ()
    {
        if (isOpened)
        {
            animator.SetTrigger(closeAnimationName);
        }
        else
        {
            animator.SetTrigger(openAnimationName);
        }

        openCloseButton.enabled = false;
        isOpened = !isOpened;
    }

    public void SetButtonEnable ()
    {
        openCloseButton.enabled = true;
    }

}
