using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameToolkit.Localization;

public enum SystemMessageTitleType
{
	None,
	Error
}

public enum SystemMessageMessageType
{
	None,
	VideoLoadError
}

public enum SystemMessageOKType
{
	None,
	OK,
	Retry
}

public enum SystemMessageCancelType
{
	None,
	Cancle,
	Close
}

[Prefab("Custom/SystemMessageCanvas")]
public class SystemMessageCanvas : Singleton<SystemMessageCanvas> {

	[SerializeField]
	LocalizedTextBehaviour titleText;
	[SerializeField]
	LocalizedTextBehaviour messageText;
	[SerializeField]
	Button okButton;
	[SerializeField]
	LocalizedTextBehaviour okText;
	[SerializeField]
	Button cancelButton;
	[SerializeField]
	LocalizedTextBehaviour cancelText;

	public delegate void SystemMessageCallback(bool _isConfirm);
	SystemMessageCallback systemMessageCallback;

	// Use this for initialization
	void Start () {
		
	}

	public void SetupSystemMessage (SystemMessageTitleType _title, SystemMessageMessageType _message, SystemMessageOKType _ok, SystemMessageCancelType _cancel, SystemMessageCallback _callback)
	{
		LocalizedText localizedTitle = null;
		LocalizedText localizedMessage = null;
		LocalizedText localizedOk = null;
		LocalizedText localizedCancel = null;
		switch (_title)
		{
			case SystemMessageTitleType.Error:
			localizedTitle = GetLocalizedText("Aloha_LocalizedText_Error");
			break;
			default:
			break;
		}

		switch (_message)
		{
			case SystemMessageMessageType.VideoLoadError:
			localizedMessage = GetLocalizedText("Aloha_LocalizedText_VideoError");
			break;
			default:
			break;
		}
		switch (_ok)
		{
			case SystemMessageOKType.OK:
			localizedOk = GetLocalizedText("Aloha_LocalizedText_OK");
			break;
			case SystemMessageOKType.Retry:
			localizedOk = GetLocalizedText("Aloha_LocalizedText_Retry");
			break;
			default:
			okButton.gameObject.SetActive(false);
			break;
		}

		switch (_cancel)
		{
			case SystemMessageCancelType.Cancle:
			localizedCancel = GetLocalizedText("Aloha_LocalizedText_Cancel");
			break;
			case SystemMessageCancelType.Close:
			localizedCancel = GetLocalizedText("Aloha_LocalizedText_Close");
			break;
			default:
			cancelButton.gameObject.SetActive(false);
			break;
		}

		titleText.LocalizedAsset = localizedTitle;
		messageText.LocalizedAsset = localizedMessage;
		okText.LocalizedAsset = localizedOk;
		cancelText.LocalizedAsset = localizedCancel;
		systemMessageCallback = _callback;
		gameObject.SetActive(true);
	}

	LocalizedText GetLocalizedText (string _name)
	{
		return Resources.Load<LocalizedText>("Localization/" + _name);
	}

	public void OnButtonPressed (bool _isOK)
	{
		if (systemMessageCallback != null)
		{
			systemMessageCallback(_isOK);
		}
		gameObject.SetActive(false);
	}


//	// Update is called once per frame
//	void Update () {
//		
//	}
}
