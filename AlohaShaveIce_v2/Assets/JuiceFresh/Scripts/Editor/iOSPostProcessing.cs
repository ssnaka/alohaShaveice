using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor.iOS.Xcode;
using UnityEditor;
using UnityEditor.Callbacks;
using JetBrains.Annotations;

#if UNITY_IPHONE
public class iOSPostProcessing : MonoBehaviour 
{
	[PostProcessBuild(100)]
	public static void OnPostProcessBuild (BuildTarget target, string pathToBuildProject) 
	{	
		if (target.ToString () == "iOS" || target.ToString () == "iPhone") 
		{
			UpdatePlist(pathToBuildProject);
			UpdatePBX(pathToBuildProject);
		}
	}

	private static void UpdatePlist (string buildPath) 
	{
		string plistPath = Path.Combine (buildPath, "Info.plist");
		PlistDocument plist = new PlistDocument ();
		plist.ReadFromString(File.ReadAllText (plistPath));
		plist.root.SetString("NSCalendarsUsageDescription", "Advertising");
		File.WriteAllText(plistPath, plist.WriteToString());
	}

	static void UpdatePBX (string pathToBuildProject)
	{
//		string projPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
		string projPath = PBXProject.GetPBXProjectPath(pathToBuildProject);

		PBXProject proj = new PBXProject ();
		proj.ReadFromString (File.ReadAllText (projPath));
		string target = proj.TargetGuidByName("Unity-iPhone");

		var entitlementFileName = "alohashaveice.entitlements";
		var entitlementPath = Path.Combine(Application.dataPath + "/Entitlement", entitlementFileName);
		var unityTarget = PBXProject.GetUnityTargetName();
		var relativeDestination = unityTarget + "/" + entitlementFileName;
		FileUtil.CopyFileOrDirectory(entitlementPath, pathToBuildProject + "/" + relativeDestination);

		proj.AddFile(relativeDestination, entitlementFileName);
		proj.AddBuildProperty(target, "CODE_SIGN_ENTITLEMENTS", relativeDestination);

		proj.AddBuildProperty(target, "SystemCapabilities", "{com.apple.Push = {enabled = 1;};}");
//		Debug.LogError(projPath + " : " + target);
//		var dummy = CreateInstance<PostProcessBuildScript>();
//		var entitlementsFile = dummy.entitlementsFile;
//		DestroyImmediate(dummy);
//		if (entitlementsFile != null)
//		{
//			// Copy the entitlement file to the xcode project
//			var entitlementPath = AssetDatabase.GetAssetPath(entitlementsFile);
//			var entitlementFileName = Path.GetFileName(entitlementPath);
//			var unityTarget = PBXProject.GetUnityTargetName();
//			var relativeDestination = unityTarget + "/" + entitlementFileName;
//			FileUtil.CopyFileOrDirectory(entitlementPath, pathToBuiltProject + "/" + relativeDestination);
//			// Add the pbx configs to include the entitlements files on the project
//			proj.AddFile(relativeDestination, entitlementFileName);
//			proj.AddBuildProperty(target, "CODE_SIGN_ENTITLEMENTS", relativeDestination);
//			// Add push notifications as a capability on the target
//			proj.AddBuildProperty(target, "SystemCapabilities", "{com.apple.Push = {enabled = 1;};}");
//		}
//		// Save the changed configs to the file
		File.WriteAllText(projPath, proj.WriteToString());



//		proj.AddCapability(target, PBXCapabilityType.GameCenter);
//		proj.AddCapability (target, PBXCapabilityType.InAppPurchase);
//
//		File.WriteAllText(projPath, proj.ToString());
	}
}
#endif
