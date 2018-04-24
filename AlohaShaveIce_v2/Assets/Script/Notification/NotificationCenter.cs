using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[Prefab("NotificationCenter")]
public class NotificationCenter : Singleton<NotificationCenter>
{
	public void Init ()
	{
		RegisterForLocalNotification();
	}

	/// <summary>
	/// Registers for local notification.
	/// This function should be called in the beginning of the game, 
	/// so the game let iOS know it'll send local notification.
	/// </summary>
	public void RegisterForLocalNotification ()
	{
		// don't need to register for Android!.
#if !UNITY_EDITOR
#if UNITY_IOS
		UnityEngine.iOS.NotificationServices.RegisterForNotifications(UnityEngine.iOS.NotificationType.Alert | 
			UnityEngine.iOS.NotificationType.Badge | 
			UnityEngine.iOS.NotificationType.Sound);
#endif
#endif
	}

	/// <summary>
	/// Registers the local notifications.
	/// </summary>
	/// <param name="_items">Items.</param>
	public void RegisterLocalNotifications (List<LocalNotificationItem> _items)
	{
		foreach (LocalNotificationItem item in _items)
		{
			RegisterLocalNotification(item);
		}
	}

	public void RegisterLocalNotification (LocalNotificationItem _item)
	{
		RegisterLocalNotification(_item.id, _item.title, _item.message, _item.fireDate, _item.largeImage);
	}

	void RegisterLocalNotification (int _id, string _title, string _message, DateTime _fireDate, string _largeImage)
	{
#if !UNITY_EDITOR
#if UNITY_ANDROID
		System.TimeSpan sp = _fireDate.Subtract(DateTime.Now);
		AndroidLocalNotification.SendNotification(_id, (int)sp.TotalSeconds, _title, _message, Color.white, true, true, true, _largeImage);
#elif UNITY_IOS
		UnityEngine.iOS.LocalNotification nl = new UnityEngine.iOS.LocalNotification();
		nl.alertAction = _title;
		nl.alertBody = _message;
		nl.soundName = UnityEngine.iOS.LocalNotification.defaultSoundName;
		//TODO: Ki, increase badgeNumber as new notification arrives.
		nl.applicationIconBadgeNumber = 1;
		System.DateTime fireDate = _fireDate;
		nl.fireDate = fireDate;
		nl.userInfo["userData_Id"] = _id;
		UnityEngine.iOS.NotificationServices.ScheduleLocalNotification(nl);
#endif
#endif

	}


	public void CancelLocalNotification (int _id)
	{
#if !UNITY_EDITOR
#if UNITY_ANDROID
		AndroidLocalNotification.CancelNotification(_id);
#elif UNITY_IOS
		UnityEngine.iOS.LocalNotification[] lns = UnityEngine.iOS.NotificationServices.scheduledLocalNotifications;
		foreach (UnityEngine.iOS.LocalNotification ln in lns)
		{
			int registeredId = 0;
			try
			{
				registeredId = Convert.ToInt32(ln.userInfo["userData_Id"]);
			}
			catch (System.Exception ex) 
			{ 
				Debug.LogError(ex.Message); 
				continue;
			}

			if (registeredId == _id)
			{
				UnityEngine.iOS.NotificationServices.CancelLocalNotification(ln);
			}
		}
#endif
#endif
	}

	public void CancelAllLocalNotification ()
	{
#if !UNITY_EDITOR
#if UNITY_ANDROID
		AndroidLocalNotification.CancelAllNotifications();
#elif UNITY_IOS
		UnityEngine.iOS.NotificationServices.ClearLocalNotifications();
		UnityEngine.iOS.NotificationServices.CancelAllLocalNotifications();
		ResetAppBadgeIcon();
#endif
#endif
	}

	/// <summary>
	/// Resets the app badge icon.
	/// Only for iOS
	/// </summary>
	public void ResetAppBadgeIcon ()
	{
#if !UNITY_EDITOR
#if UNITY_IOS
		UnityEngine.iOS.LocalNotification setCountNotif = new UnityEngine.iOS.LocalNotification();
		//TODO: Ki, Reset badge number. It is not working now
		setCountNotif.applicationIconBadgeNumber = -1;
		setCountNotif.hasAction = false;
		UnityEngine.iOS.NotificationServices.PresentLocalNotificationNow(setCountNotif);
#endif
#endif
	}

	//		public void RegisterPushNotification ()
	//		{
	//			UnityEngine.iOS.
	//		}
}
