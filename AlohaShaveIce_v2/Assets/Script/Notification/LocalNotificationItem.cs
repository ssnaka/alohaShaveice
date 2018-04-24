using UnityEngine;
using System;
using System.Collections;

public class LocalNotificationItem
{
	public int id;
	public string title;
	public string message;
	public DateTime fireDate;
	public string largeImage = string.Empty;

	public LocalNotificationItem (int _id, string _title, string _message, DateTime _fireDate, string _largeImage = null)
	{
		id = _id;
		title = _title;
		message = _message;
		fireDate = _fireDate;
		largeImage = _largeImage;
	}
}
