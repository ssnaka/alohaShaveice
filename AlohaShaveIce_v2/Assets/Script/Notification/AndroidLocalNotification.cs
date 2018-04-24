using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class AndroidLocalNotification
{
    /// <summary>
    /// Inexact uses `set` method
    /// Exact uses `setExact` method
    /// ExactAndAllowWhileIdle uses `setAndAllowWhileIdle` method
    /// Documentation: https://developer.android.com/intl/ru/reference/android/app/AlarmManager.html
    /// </summary>
    public enum NotificationExecuteMode
    {
        Inexact = 0,
        Exact = 1,
        ExactAndAllowWhileIdle = 2
    }

#if UNITY_ANDROID && !UNITY_EDITOR
	private static string fullClassName = "com.android_notification.clada.androidnotification.androidnotificationlibrary.AndroidNotification";
    private static string mainActivityClassName = "com.unity3d.player.UnityPlayerNativeActivity";
#endif

	#if UNITY_5_6_OR_NEWER
	private static string bundleIdentifier { get { return Application.identifier; } }
	#else
	private static string bundleIdentifier { get { return Application.bundleIdentifier; } }
	#endif

    public static void SendNotification(int id, TimeSpan delay, string title, string message)
    {
        SendNotification(id, (int)delay.TotalSeconds, title, message, Color.white);
    }
    
    public static void SendNotification(int id, long delay, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "", NotificationExecuteMode executeMode = NotificationExecuteMode.Inexact)
    {
//		SetNotification(int id, long delayMs, String title, String message, String ticker, int sound, String soundName, int vibrate,
//			int lights, String largeIconResource, String smallIconResource, int bgColor, String bundle, String channel,
//			String unityClass)
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetNotification", id, delay * 1000L, title, message, message, sound ? 1 : 0, "", vibrate ? 1 : 0, 
				lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, bundleIdentifier, "default", 
				mainActivityClassName);
        }
#endif
    }

    public static void SendRepeatingNotification(int id, long delay, long timeout, string title, string message, Color32 bgColor, bool sound = true, bool vibrate = true, bool lights = true, string bigIcon = "")
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
        {
            pluginClass.CallStatic("SetRepeatingNotification", id, delay * 1000L, title, message, message, timeout * 1000, sound ? 1 : 0, vibrate ? 1 : 0, lights ? 1 : 0, bigIcon, "notify_icon_small", bgColor.r * 65536 + bgColor.g * 256 + bgColor.b, mainActivityClassName);
        }
#endif
    }

    public static void CancelNotification(int id)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null) {
            pluginClass.CallStatic("CancelNotification", id);
        }
#endif
    }

    public static void CancelAllNotifications()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass pluginClass = new AndroidJavaClass(fullClassName);
        if (pluginClass != null)
            pluginClass.CallStatic("ClearShowingNotifications");
#endif
    }
}
