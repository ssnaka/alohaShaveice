/*
 * Made by Kamen Dimitrov, http://www.studio-generative.com
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class NativeReviewRequest {

	public static void RequestReview() {
		#if UNITY_IOS && !UNITY_EDITOR
		requestReview();
		#endif
	}

	#if UNITY_IOS && !UNITY_EDITOR
	[DllImport ("__Internal")] private static extern void requestReview();
	#endif
}
